
namespace MouseTrap.Forms
{
    partial class DiagnosticForm
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
            this.ConsoleBox = new System.Windows.Forms.TextBox();
            this.InfoText = new System.Windows.Forms.TextBox();
            this.BtnStartDiagnostic = new System.Windows.Forms.Button();
            this.BtnCopy = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabRealtimeLog = new System.Windows.Forms.TabPage();
            this.TabInfos = new System.Windows.Forms.TabPage();
            this.InfosBox = new System.Windows.Forms.TextBox();
            this.TabLogFileViewer = new System.Windows.Forms.TabPage();
            this.LogfileBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TabRealtimeLog.SuspendLayout();
            this.TabInfos.SuspendLayout();
            this.TabLogFileViewer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConsoleBox
            // 
            this.ConsoleBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(22)))), ((int)(((byte)(16)))));
            this.ConsoleBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConsoleBox.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ConsoleBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(125)))), ((int)(((byte)(117)))));
            this.ConsoleBox.Location = new System.Drawing.Point(3, 3);
            this.ConsoleBox.Multiline = true;
            this.ConsoleBox.Name = "ConsoleBox";
            this.ConsoleBox.ReadOnly = true;
            this.ConsoleBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ConsoleBox.Size = new System.Drawing.Size(1619, 783);
            this.ConsoleBox.TabIndex = 0;
            // 
            // InfoText
            // 
            this.InfoText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InfoText.Enabled = false;
            this.InfoText.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.InfoText.Location = new System.Drawing.Point(3, 3);
            this.InfoText.Multiline = true;
            this.InfoText.Name = "InfoText";
            this.InfoText.ReadOnly = true;
            this.InfoText.Size = new System.Drawing.Size(494, 168);
            this.InfoText.TabIndex = 2;
            this.InfoText.TabStop = false;
            this.InfoText.Text = "To collects diagnostic data do the following:\r\n - Click \"Start Diagnostic\"\r\n - Th" +
    "en try to reproduce the problem\r\n - Click \"Stop Diagnostic\"\r\n - Click \"Copy To C" +
    "lipboard\" \r\n   (it will copy all data)";
            // 
            // BtnStartDiagnostic
            // 
            this.BtnStartDiagnostic.Dock = System.Windows.Forms.DockStyle.Top;
            this.BtnStartDiagnostic.Location = new System.Drawing.Point(503, 3);
            this.BtnStartDiagnostic.Name = "BtnStartDiagnostic";
            this.BtnStartDiagnostic.Size = new System.Drawing.Size(194, 50);
            this.BtnStartDiagnostic.TabIndex = 3;
            this.BtnStartDiagnostic.Text = "Start Diagnostic";
            this.BtnStartDiagnostic.UseVisualStyleBackColor = true;
            // 
            // BtnCopy
            // 
            this.BtnCopy.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BtnCopy.Location = new System.Drawing.Point(1436, 131);
            this.BtnCopy.Name = "BtnCopy";
            this.BtnCopy.Size = new System.Drawing.Size(194, 40);
            this.BtnCopy.TabIndex = 4;
            this.BtnCopy.Text = "Copy To Clipboard";
            this.BtnCopy.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 500F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.InfoText, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtnStartDiagnostic, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.BtnCopy, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1633, 174);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1639, 1013);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabRealtimeLog);
            this.tabControl1.Controls.Add(this.TabInfos);
            this.tabControl1.Controls.Add(this.TabLogFileViewer);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 183);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1633, 827);
            this.tabControl1.TabIndex = 5;
            // 
            // TabRealtimeLog
            // 
            this.TabRealtimeLog.Controls.Add(this.ConsoleBox);
            this.TabRealtimeLog.Location = new System.Drawing.Point(4, 34);
            this.TabRealtimeLog.Name = "TabRealtimeLog";
            this.TabRealtimeLog.Padding = new System.Windows.Forms.Padding(3);
            this.TabRealtimeLog.Size = new System.Drawing.Size(1625, 789);
            this.TabRealtimeLog.TabIndex = 0;
            this.TabRealtimeLog.Text = "Realtime Log";
            this.TabRealtimeLog.UseVisualStyleBackColor = true;
            // 
            // TabInfos
            // 
            this.TabInfos.Controls.Add(this.InfosBox);
            this.TabInfos.Location = new System.Drawing.Point(4, 34);
            this.TabInfos.Name = "TabInfos";
            this.TabInfos.Padding = new System.Windows.Forms.Padding(3);
            this.TabInfos.Size = new System.Drawing.Size(1625, 789);
            this.TabInfos.TabIndex = 1;
            this.TabInfos.Text = "Program and System Configuration";
            this.TabInfos.UseVisualStyleBackColor = true;
            // 
            // InfosBox
            // 
            this.InfosBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(22)))), ((int)(((byte)(16)))));
            this.InfosBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InfosBox.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.InfosBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(125)))), ((int)(((byte)(117)))));
            this.InfosBox.Location = new System.Drawing.Point(3, 3);
            this.InfosBox.Multiline = true;
            this.InfosBox.Name = "InfosBox";
            this.InfosBox.ReadOnly = true;
            this.InfosBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.InfosBox.Size = new System.Drawing.Size(1619, 783);
            this.InfosBox.TabIndex = 0;
            // 
            // TabLogFileViewer
            // 
            this.TabLogFileViewer.Controls.Add(this.LogfileBox);
            this.TabLogFileViewer.Location = new System.Drawing.Point(4, 34);
            this.TabLogFileViewer.Name = "TabLogFileViewer";
            this.TabLogFileViewer.Padding = new System.Windows.Forms.Padding(3);
            this.TabLogFileViewer.Size = new System.Drawing.Size(1625, 789);
            this.TabLogFileViewer.TabIndex = 2;
            this.TabLogFileViewer.Text = "Log File Viewer";
            this.TabLogFileViewer.UseVisualStyleBackColor = true;
            // 
            // LogfileBox
            // 
            this.LogfileBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(22)))), ((int)(((byte)(16)))));
            this.LogfileBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogfileBox.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LogfileBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(125)))), ((int)(((byte)(117)))));
            this.LogfileBox.Location = new System.Drawing.Point(3, 3);
            this.LogfileBox.Multiline = true;
            this.LogfileBox.Name = "LogfileBox";
            this.LogfileBox.ReadOnly = true;
            this.LogfileBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogfileBox.Size = new System.Drawing.Size(1619, 783);
            this.LogfileBox.TabIndex = 0;
            // 
            // DiagnosticForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1647, 1021);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "DiagnosticForm";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Text = "Diagnostic";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.TabRealtimeLog.ResumeLayout(false);
            this.TabRealtimeLog.PerformLayout();
            this.TabInfos.ResumeLayout(false);
            this.TabInfos.PerformLayout();
            this.TabLogFileViewer.ResumeLayout(false);
            this.TabLogFileViewer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ConsoleBox;
        private System.Windows.Forms.TextBox InfoText;
        private System.Windows.Forms.Button BtnStartDiagnostic;
        private System.Windows.Forms.Button BtnCopy;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabRealtimeLog;
        private System.Windows.Forms.TabPage TabInfos;
        private System.Windows.Forms.TextBox InfosBox;
        private System.Windows.Forms.TabPage TabLogFileViewer;
        private System.Windows.Forms.TextBox LogfileBox;
    }
}