namespace MouseTrap.Forms
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ScreensView = new MouseTrap.ScreensView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.InfoText = new System.Windows.Forms.TextBox();
            this.EnableAutoStart = new System.Windows.Forms.CheckBox();
            this.BtnConfigure = new System.Windows.Forms.Button();
            this.CursorPosition = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MouseTrackTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.ScreensView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80.53008F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.46993F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1756, 981);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // ScreensView
            // 
            this.ScreensView.CurrentScale = 0.2013889F;
            this.ScreensView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScreensView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScreensView.Location = new System.Drawing.Point(4, 4);
            this.ScreensView.Margin = new System.Windows.Forms.Padding(4);
            this.ScreensView.Name = "ScreensView";
            this.ScreensView.Padding = new System.Windows.Forms.Padding(4);
            this.ScreensView.Size = new System.Drawing.Size(1748, 781);
            this.ScreensView.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.InfoText);
            this.panel1.Controls.Add(this.EnableAutoStart);
            this.panel1.Controls.Add(this.BtnConfigure);
            this.panel1.Controls.Add(this.CursorPosition);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 792);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1750, 186);
            this.panel1.TabIndex = 2;
            // 
            // InfoText
            // 
            this.InfoText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.InfoText.BackColor = System.Drawing.SystemColors.Control;
            this.InfoText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.InfoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoText.ForeColor = System.Drawing.Color.DarkRed;
            this.InfoText.Location = new System.Drawing.Point(1444, 14);
            this.InfoText.Multiline = true;
            this.InfoText.Name = "InfoText";
            this.InfoText.ReadOnly = true;
            this.InfoText.Size = new System.Drawing.Size(296, 99);
            this.InfoText.TabIndex = 4;
            this.InfoText.Text = "You have not configured any mouse bridges yet! Click \"Configure Screen Bridges\" t" +
    "o configure some.";
            // 
            // EnableAutoStart
            // 
            this.EnableAutoStart.AutoSize = true;
            this.EnableAutoStart.Location = new System.Drawing.Point(13, 59);
            this.EnableAutoStart.Name = "EnableAutoStart";
            this.EnableAutoStart.Size = new System.Drawing.Size(394, 24);
            this.EnableAutoStart.TabIndex = 3;
            this.EnableAutoStart.Text = "Start MouseTrap automatically on Windows startup";
            this.EnableAutoStart.UseVisualStyleBackColor = true;
            // 
            // BtnConfigure
            // 
            this.BtnConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnConfigure.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnConfigure.Location = new System.Drawing.Point(1444, 119);
            this.BtnConfigure.Name = "BtnConfigure";
            this.BtnConfigure.Size = new System.Drawing.Size(297, 58);
            this.BtnConfigure.TabIndex = 2;
            this.BtnConfigure.Text = "Configure Screen Bridges";
            this.BtnConfigure.UseVisualStyleBackColor = true;
            // 
            // CursorPosition
            // 
            this.CursorPosition.AutoSize = true;
            this.CursorPosition.Location = new System.Drawing.Point(227, 21);
            this.CursorPosition.Name = "CursorPosition";
            this.CursorPosition.Size = new System.Drawing.Size(34, 20);
            this.CursorPosition.TabIndex = 1;
            this.CursorPosition.Text = "0x0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Mouse Position:";
            // 
            // MouseTrackTimer
            // 
            this.MouseTrackTimer.Enabled = true;
            this.MouseTrackTimer.Interval = 1;
            this.MouseTrackTimer.Tick += new System.EventHandler(this.MouseTrackTimer_Tick);
            // 
            // ConfigFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1756, 981);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConfigFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MouseTrap Settings";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ScreensView ScreensView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label CursorPosition;
        private System.Windows.Forms.Timer MouseTrackTimer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtnConfigure;
        private System.Windows.Forms.CheckBox EnableAutoStart;
        private System.Windows.Forms.TextBox InfoText;
    }
}