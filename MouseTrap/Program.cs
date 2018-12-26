using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MouseTrap.Properties;


namespace MouseTrap {
    public class Program {
        public static void Main(string[] args)
        {
            //Application.Run(new ConfigFrom());

            //while (true) {
            //    Debug.WriteLine(Cursor.Position);
            //}

            var worker = new TrayWorker(new Program().Run);
            worker.TrayIcon = new NotifyIcon {
                Icon = Resources.AppIcon,
                Text = nameof(MouseTrap),
                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Info", (s, e) => new ConfigFrom().Show()) {DefaultItem = true},
                    new MenuItem("Exit", (s, e) => worker.Exit()),
                }),
                Visible = true
            };
            worker.Start();
        }

        private void Run()
        {
            var screens = Screen.AllScreens;

            var d1 = screens.First(x => x.Primary);
            var d2 = screens.First(x => x != d1);

            const int space = 10;
            var d1HotSpace = new Rectangle(d1.Bounds.X + d1.Bounds.Width - space, d1.Bounds.Y + 324, space, d1.Bounds.Height - 324);
            var d2HotSpace = new Rectangle(d2.Bounds.X, d2.Bounds.Y, space, d2.Bounds.Height);

            //Console.WriteLine(d1.Bounds);
            //Console.WriteLine(d2.Bounds);
            //Console.WriteLine(d1HotSpace);
            //Console.WriteLine(d2HotSpace);

            var activeTrap = 0;

            while (true) {
                if (d1.Bounds.Contains(Cursor.Position)) {
                    if (activeTrap != 1) {
                        Mouse.MouseTrap(d1.Bounds);
                        activeTrap = 1;
                    }
                }
                else if (d2.Bounds.Contains(Cursor.Position)) {
                    if (activeTrap != 2) {
                        Mouse.MouseTrap(d2.Bounds);
                        activeTrap = 2;
                    }
                }

                var direction = GetDirection(Cursor.Position.X);


                // ==>
                if (direction == Direction.ToRight && d1HotSpace.Contains(Cursor.Position)) {
                    var percent = (Cursor.Position.Y - d1HotSpace.Y) / (float) d1HotSpace.Height;
                    if (percent < 0) continue;

                    var newposy = (int) (d2HotSpace.Height * percent) + d2HotSpace.Y;
                    Mouse.MouseTrapClear();
                    activeTrap = 0;
                    Mouse.MouseMove((d2HotSpace.X + d2HotSpace.Width + 1), newposy);
                }

                // <==
                if (direction == Direction.ToLeft && d2HotSpace.Contains(Cursor.Position)) {
                    var percent = (Cursor.Position.Y - d2HotSpace.Y) / (float) d2HotSpace.Height;
                    if (percent < 0) continue;

                    var newposy = (int) (d1HotSpace.Height * percent) + d1HotSpace.Y;
                    Mouse.MouseTrapClear();
                    activeTrap = 0;
                    Mouse.MouseMove((d1HotSpace.X - 1), newposy);
                }

                Thread.Sleep(1);
            }
        }

        private int _posOldx;

        private Direction GetDirection(int posx)
        {
            if (_posOldx < posx) {
                _posOldx = posx;

                return Direction.ToRight;
            }

            if (_posOldx > posx) {
                _posOldx = posx;

                return Direction.ToLeft;
            }

            return Direction.None;
        }
    }

    internal enum Direction {
        None,
        ToLeft,
        ToRight
    }
}
