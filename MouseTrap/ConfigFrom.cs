using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseTrap.Forms;


// ReSharper disable LocalizableElement
namespace MouseTrap {
    public partial class ConfigFrom : Form {
        public ConfigFrom()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
        }

        private void ConfigFrom_Load(object sender, EventArgs e)
        {
        }

        private void MouseTrackTimer_Tick(object sender, EventArgs e)
        {
            CursorPosition.Text = $"{Cursor.Position.X}x{Cursor.Position.Y}";
        }

        private void BtnShowBriges_Click(object sender, EventArgs e)
        {
            var screens = Screen.AllScreens;

            var d1 = screens.First(x => x.Primary);
            var d2 = screens.First(x => x != d1);

            const int space = 10;
            var d1HotSpace = new Rectangle(d1.Bounds.X + d1.Bounds.Width - space, d1.Bounds.Y + 324, space, d1.Bounds.Height - 324);
            var d2HotSpace = new Rectangle(d2.Bounds.X, d2.Bounds.Y, space, d2.Bounds.Height);

            //var d1Bar = CreateBar(d1HotSpace, Color.Green);
            //var d2Bar = CreateBar(d2HotSpace, Color.Red);

            //d1Bar.Show(this);
            //d2Bar.Show(this);

            ShowForms(d1, d2);
        }

        private void ShowForms(params Screen[] screens)
        {
            var forms = new List<Form>();
            foreach (var screen in screens) {
                var form = new ScreenConfigForm(screen);
                form.KeyDown += (sender, args) => {
                    if (args.KeyCode == Keys.Escape) {
                        forms.ForEach(_ => _.Close());
                    }
                };
                form.Show(this);
                forms.Add(form);
            }
        }
    }
}
