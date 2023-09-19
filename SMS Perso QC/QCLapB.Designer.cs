namespace SMS_Perso_QC
{
    partial class QCLapB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QCLapB));
            this.Report_B = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.IPQCReportB1 = new SMS_Perso_QC.IPQCReportB();
            this.SuspendLayout();
            // 
            // Report_B
            // 
            this.Report_B.ActiveViewIndex = 0;
            this.Report_B.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Report_B.Cursor = System.Windows.Forms.Cursors.Default;
            this.Report_B.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Report_B.Location = new System.Drawing.Point(0, 0);
            this.Report_B.Name = "Report_B";
            this.Report_B.ReportSource = this.IPQCReportB1;
            this.Report_B.ShowCloseButton = false;
            this.Report_B.ShowGotoPageButton = false;
            this.Report_B.ShowGroupTreeButton = false;
            this.Report_B.ShowLogo = false;
            this.Report_B.ShowParameterPanelButton = false;
            this.Report_B.Size = new System.Drawing.Size(848, 465);
            this.Report_B.TabIndex = 0;
            this.Report_B.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // QCLapB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 465);
            this.Controls.Add(this.Report_B);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QCLapB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SMS Perso QC Report PIOTEC B";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.QCLapB_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private IPQCReportB IPQCReportB1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer Report_B;
    }
}