namespace SMS_Perso_QC
{
    partial class QCLapA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QCLapA));
            this.Report_A = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.IPQCReport1 = new SMS_Perso_QC.IPQCReport();
            this.SuspendLayout();
            // 
            // Report_A
            // 
            this.Report_A.ActiveViewIndex = 0;
            this.Report_A.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Report_A.Cursor = System.Windows.Forms.Cursors.Default;
            this.Report_A.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Report_A.Location = new System.Drawing.Point(0, 0);
            this.Report_A.Name = "Report_A";
            this.Report_A.ReportSource = this.IPQCReport1;
            this.Report_A.ShowCloseButton = false;
            this.Report_A.ShowGotoPageButton = false;
            this.Report_A.ShowGroupTreeButton = false;
            this.Report_A.ShowLogo = false;
            this.Report_A.ShowParameterPanelButton = false;
            this.Report_A.Size = new System.Drawing.Size(854, 488);
            this.Report_A.TabIndex = 0;
            this.Report_A.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // QCLapA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 488);
            this.Controls.Add(this.Report_A);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QCLapA";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SMS Perso QC  Report PIOTEC A";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.QCLapA_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private IPQCReport IPQCReport1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer Report_A;


    }
}