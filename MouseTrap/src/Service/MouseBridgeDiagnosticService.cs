using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MouseTrap.Models;
using MouseTrap.Native;


namespace MouseTrap.Service {
    public class MouseBridgeDiagnosticService : IService {
        private readonly ScreenConfigCollection _screens;
        private Action<string> _log;

        public MouseBridgeDiagnosticService(ScreenConfigCollection screens, Action<string> log)
        {
            _screens = screens;

            var sw = new Stopwatch();
            sw.Start();
            _log = msg => {
                log($"{sw.Elapsed,-15:g} [{DirectionArrow(_direction)}]  Cursor({_position.X,4}, {_position.Y,4})  {msg}");
            };

            static char DirectionArrow(Direction d)
            {
                return d switch {
                    Direction.ToTop => '↑',
                    Direction.ToBottom => '↓',
                    Direction.ToLeft => '←',
                    Direction.ToRight => '→',
                    Direction.ToTop | Direction.ToLeft => '↖',
                    Direction.ToTop | Direction.ToRight => '↗',
                    Direction.ToBottom | Direction.ToLeft => '↙',
                    Direction.ToBottom | Direction.ToRight => '↘',
                    _ => ' '
                };
            }
        }


        public void OnStart()
        {
        }

        private int _errorCount = 0;

        public void Run(CancellationToken token)
        {
            _log("run");
            try {
                Loop(token);
            }
            catch (Win32Exception e) {
                if (token.IsCancellationRequested) {
                    return;
                }

                _log($"\r\n{e}\r\n");

                _errorCount++;
                if (_errorCount < 5) {
                    _log("rerun");
                    Run(token);
                }
                else {
                    // set a noop log func to prevent death locks!
                    _log = _ => {
                    };
                    throw;
                }
            }

            if (token.IsCancellationRequested) {
                // set a noop log func to prevent death locks!
                _log = _ => {
                };
            }
        }

        public void OnExit()
        {
            MouseTrapClear();
        }


        private void Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested) {
                var position = GetPosition();

                var current = _screens.FirstOrDefault(_ => _.Bounds.Contains(position));
                if (current != null && current.HasBridges) {
                    MouseTrap(current);

                    var direction = GetDirection(position);

                    // ==>
                    var hotspace = current.RightHotSpace;
                    if (direction.HasFlag(Direction.ToRight) && hotspace.Contains(position)) {
                        var targetScreen = _screens.FirstOrDefault(_ => _.ScreenId == current.RightBridge.TargetScreenId);
                        if (targetScreen != null) {
                            var target = targetScreen.LeftHotSpace;
                            if (target != Rectangle.Empty) {
                                MouseTrapClear();

                                var newY = MapY(position.Y, ref hotspace, ref target);
                                MouseMove(targetScreen, (target.X + target.Width + 1), newY);
                            }
                        }
                    }

                    // <==
                    hotspace = current.LeftHotSpace;
                    if (direction.HasFlag(Direction.ToLeft) && hotspace.Contains(position)) {
                        var targetScreen = _screens.FirstOrDefault(_ => _.ScreenId == current.LeftBridge.TargetScreenId);
                        if (targetScreen != null) {
                            var target = targetScreen.RightHotSpace;
                            if (target != Rectangle.Empty) {
                                MouseTrapClear();

                                var newY = MapY(position.Y, ref hotspace, ref target);
                                MouseMove(targetScreen, (target.X - 1), newY);
                            }
                        }
                    }


                    // ^
                    hotspace = current.TopHotSpace;
                    if (direction.HasFlag(Direction.ToTop) && hotspace.Contains(position)) {
                        var targetScreen = _screens.FirstOrDefault(_ => _.ScreenId == current.TopBridge.TargetScreenId);
                        if (targetScreen != null) {
                            var target = targetScreen.BottomHotSpace;
                            if (target != Rectangle.Empty) {
                                MouseTrapClear();

                                var newX = MapX(position.X, ref hotspace, ref target);
                                MouseMove(targetScreen, newX, (target.Y - 1));
                            }
                        }
                    }

                    // v
                    hotspace = current.BottomHotSpace;
                    if (direction.HasFlag(Direction.ToBottom) && hotspace.Contains(position)) {
                        var targetScreen = _screens.FirstOrDefault(_ => _.ScreenId == current.BottomBridge.TargetScreenId);
                        if (targetScreen != null) {
                            var target = targetScreen.TopHotSpace;
                            if (target != Rectangle.Empty) {
                                MouseTrapClear();

                                var newX = MapX(position.X, ref hotspace, ref target);
                                MouseMove(targetScreen, newX, (target.Y + target.Height + 1));
                            }
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }

        private Point _position;

        private Point GetPosition()
        {
            return _position = Cursor.Position;
        }


        private int _posOldx;
        private int _posOldy;
        private Direction _direction;

        private Direction GetDirection(Point pos)
        {
            var ret = Direction.None;
            if (_posOldx < pos.X) {
                _posOldx = pos.X;

                ret |= Direction.ToRight;
            }

            if (_posOldx > pos.X) {
                _posOldx = pos.X;

                ret |= Direction.ToLeft;
            }

            if (_posOldy < pos.Y) {
                _posOldy = pos.Y;

                ret |= Direction.ToBottom;
            }

            if (_posOldy > pos.Y) {
                _posOldy = pos.Y;

                ret |= Direction.ToTop;
            }

            return _direction = ret;
        }

        private static int MapY(int y, ref Rectangle src, ref Rectangle dst)
        {
            var percent = (y - src.Y) / (float) src.Height;
            var newY = (int) (dst.Height * percent) + dst.Y;
            return newY;
        }

        private static int MapX(int x, ref Rectangle src, ref Rectangle dst)
        {
            var percent = (x - src.X) / (float) src.Width;
            var newX = (int) (dst.Width * percent) + dst.X;
            return newX;
        }

        private int _activeTrap = -1;


        private void MouseTrap(ScreenConfig config)
        {
            if (_activeTrap != config.ScreenId || Mouse.Clip != config.Bounds) {
                _log($"MouseTrap({config.Bounds})");
                Mouse.Clip = config.Bounds;
                _activeTrap = config.ScreenId;
            }
        }

        private void MouseTrapClear()
        {
            if (_activeTrap != -1) {
                _log($"MouseTrapClear({_activeTrap})");
                Mouse.Clip = Rectangle.Empty;
                _activeTrap = -1;
            }
        }

        private void MouseMove(ScreenConfig targetScreen, int x, int y)
        {
            _log($"Move To Screen -> {targetScreen.Bounds}");
            _log($"    SwitchToInputDesktop");

            Mouse.SwitchToInputDesktop();

            // first move to center of screen, because windows has some problems :(
            var cx = targetScreen.Bounds.X + (targetScreen.Bounds.Width / 2);
            var cy = targetScreen.Bounds.Y + (targetScreen.Bounds.Height / 2);
            _log($"    MoveToCenter   ({cx,4}, {cy,4})");
            Mouse.MoveCursor(cx, cy);

            var pos = GetPosition();
            if (pos.X != cx || pos.Y != cy) {
                _log($"    Move Failed ! ({pos.X,4}, {pos.Y,4})");
            }

            _log($"    MouseMove      ({x,4}, {y,4})");
            Mouse.MoveCursor(x, y);

            pos = GetPosition();
            if (pos.X != x || pos.Y != y) {
                _log($"    Move Failed ! ({pos.X,4}, {pos.Y,4})");
            }
        }
    }
}
