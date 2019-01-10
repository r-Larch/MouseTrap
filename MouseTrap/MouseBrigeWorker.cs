using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MouseTrap.Models;
using MouseTrap.Native;


namespace MouseTrap {
    public class MouseBrigeWorker : IWorker {
        private readonly ScreenConfigCollection _screens;

        public MouseBrigeWorker()
        {
            _screens = ScreenConfigCollection.Load();
        }

        public MouseBrigeWorker(ScreenConfigCollection screens)
        {
            _screens = screens;
        }

        public void Run()
        {
            try {
                Loop();
            }
            catch (Exception e) {
                var msg = "";
                do {
                    msg += $"{e.GetType().FullName}: {e.Message}\r\n{e.StackTrace}";
                    e = e.InnerException;
                } while (e != null);

                // log
                EventLog.WriteEntry(nameof(MouseTrap), msg, EventLogEntryType.Error);

                // rerun
                Run();
            }
        }

        private void Loop()
        {
            while (true) {
                var current = _screens.FirstOrDefault(_ => _.Screen.Bounds.Contains(Cursor.Position));
                if (current != null && current.HasBridges) {
                    MouseTrap(current);

                    var direction = GetDirection(Cursor.Position);

                    // ==>
                    var hotspace = current.RightHotSpace;
                    if (direction.HasFlag(Direction.ToRight) && hotspace.Contains(Cursor.Position)) {
                        var targetScreen = _screens.First(_ => _.ScreenId == current.RightBridge.TargetScreenId);
                        var target = targetScreen.LeftHotSpace;
                        if (target != Rectangle.Empty) {
                            MouseTrapClear();

                            var newY = MapY(Cursor.Position.Y, ref hotspace, ref target);
                            MouseMove(targetScreen, (target.X + target.Width + 1), newY);
                        }
                    }

                    // <==
                    hotspace = current.LeftHotSpace;
                    if (direction.HasFlag(Direction.ToLeft) && hotspace.Contains(Cursor.Position)) {
                        var targetScreen = _screens.First(_ => _.ScreenId == current.LeftBridge.TargetScreenId);
                        var target = targetScreen.RightHotSpace;
                        if (target != Rectangle.Empty) {
                            MouseTrapClear();

                            var newY = MapY(Cursor.Position.Y, ref hotspace, ref target);
                            MouseMove(targetScreen, (target.X - 1), newY);
                        }
                    }


                    // ^
                    hotspace = current.TopHotSpace;
                    if (direction.HasFlag(Direction.ToTop) && hotspace.Contains(Cursor.Position)) {
                        var targetScreen = _screens.First(_ => _.ScreenId == current.TopBridge.TargetScreenId);
                        var target = targetScreen.BottomHotSpace;
                        if (target != Rectangle.Empty) {
                            MouseTrapClear();

                            var newX = MapX(Cursor.Position.X, ref hotspace, ref target);
                            MouseMove(targetScreen, newX, (target.Y - 1));
                        }
                    }

                    // v
                    hotspace = current.BottomHotSpace;
                    if (direction.HasFlag(Direction.ToBottom) && hotspace.Contains(Cursor.Position)) {
                        var targetScreen = _screens.First(_ => _.ScreenId == current.BottomBridge.TargetScreenId);
                        var target = targetScreen.TopHotSpace;
                        if (target != Rectangle.Empty) {
                            MouseTrapClear();

                            var newX = MapX(Cursor.Position.X, ref hotspace, ref target);
                            MouseMove(targetScreen, newX, (target.Y + target.Height + 1));
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }

        private int _posOldx;
        private int _posOldy;

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

            return ret;
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
            if (_activeTrap != config.ScreenId || Mouse.Clip != config.Screen.Bounds) {
                Mouse.Clip = config.Screen.Bounds;
                _activeTrap = config.ScreenId;
            }
        }

        private void MouseTrapClear()
        {
            if (_activeTrap != -1) {
                Mouse.Clip = Rectangle.Empty;
                _activeTrap = -1;
            }
        }

        private static void MouseMove(ScreenConfig targetScreen, int x, int y)
        {
            Mouse.SwitchToInputDesktop();

            // first move to center of screen, because windows has some problems :(
            Mouse.MoveCursor(targetScreen.Screen.Bounds.X + (targetScreen.Screen.Bounds.Width / 2), targetScreen.Screen.Bounds.Y + (targetScreen.Screen.Bounds.Height / 2));
            Mouse.MoveCursor(x, y);

            //var pos = Cursor.Position;
            //if (pos.X != x || pos.Y != y) {
            //    Console.WriteLine($"wrong pos: {x}, {y} -> {pos.X}, {pos.Y}");
            //}
        }
    }


    public interface IWorker {
        void Run();
    }


    [Flags]
    internal enum Direction {
        None = 0x00,
        ToLeft = 0x01,
        ToRight = 0x02,
        ToTop = 0x04,
        ToBottom = 0x08,
    }
}
