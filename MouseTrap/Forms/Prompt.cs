using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MouseTrap.Models;
using MouseTrap.Properties;


namespace MouseTrap {
    public class Prompt {
        public static ScreenConfig ChooseScreenDialog(ScreenConfigCollection screens, ScreenConfig exclude)
        {
            var resultId = -1;
            do {
                var f = new Form {
                    Width = 500,
                    Height = 200,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowOnly,
                    StartPosition = FormStartPosition.CenterScreen,
                    Text = "Choose target screen",
                    Icon = Resources.AppIcon
                };

                var container = new FlowLayoutPanel {
                    Location = Point.Empty,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10),
                    WrapContents = true
                };

                foreach (var screen in screens) {
                    var button = new Button {
                        Text = screen.ScreenNum,
                        Width = 50,
                        Height = 50,
                        Enabled = screen != exclude
                    };
                    button.Click += (sender, e) => {
                        resultId = screen.ScreenId;
                        f.Close();
                    };
                    container.Controls.Add(button);
                }

                f.Controls.Add(container);

                f.ShowDialog();
            } while (resultId == -1);

            return screens.Single(_ => _.ScreenId == resultId);
        }
    }
}
