using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace MouseTrap {
    internal class Mouse {
        public static void MouseTrap(Rectangle rect)
        {
            //RECT rectangle = rect;
            //ClipCursor(ref rectangle);
            Cursor.Clip = rect;
        }

        public static void MouseTrapClear()
        {
            //ClipCursor(IntPtr.Zero);
            Cursor.Clip = Rectangle.Empty;
        }

        public static void MouseMove(int x, int y)
        {
            Cursor.Position = new Point(x, y);
        }

        #region Win32

        //[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "ClipCursor")]
        //private static extern int ClipCursor(ref RECT lpRect);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "ClipCursor")]
        //private static extern int ClipCursor(IntPtr lpRect);

        //// ReSharper disable once InconsistentNaming
        //private struct RECT {
        //    public int Left;
        //    public int Top;
        //    public int Right;
        //    public int Bottom;

        //    public static implicit operator Rectangle(RECT rect)
        //    {
        //        return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
        //    }

        //    public static implicit operator RECT(Rectangle rect)
        //    {
        //        return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
        //    }

        //    public RECT(int left, int top, int right, int bottom)
        //    {
        //        Left = left;
        //        Top = top;
        //        Right = right;
        //        Bottom = bottom;
        //    }
        //}

        #endregion
    }
}
