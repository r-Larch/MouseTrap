namespace MouseTrap
{
    partial class ConfigFrom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFrom));
            this.ScreensView = new MouseTrap.ScreensView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ScreensView
            // 
            this.ScreensView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScreensView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScreensView.Location = new System.Drawing.Point(0, 0);
            this.ScreensView.Margin = new System.Windows.Forms.Padding(4);
            this.ScreensView.Name = "ScreensView";
            this.ScreensView.Padding = new System.Windows.Forms.Padding(4);
            this.ScreensView.Size = new System.Drawing.Size(1756, 981);
            this.ScreensView.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ScreensView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1756, 981);
            this.panel1.TabIndex = 1;
            // 
            // ConfigFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1756, 981);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Screen infos";
            this.Load += new System.EventHandler(this.ConfigFrom_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ScreensView ScreensView;
        private System.Windows.Forms.Panel panel1;
    }
}