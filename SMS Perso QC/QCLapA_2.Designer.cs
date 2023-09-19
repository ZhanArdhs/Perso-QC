namespace SMS_Perso_QC
{
    partial class QCLapA_2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QCLapA_2));
            this.Report_A2 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.IPQCReport21 = new SMS_Perso_QC.IPQCReport2();
            this.SuspendLayout();
            // 
            // Report_A2
            // 
            this.Report_A2.ActiveViewIndex = 0;
            this.Report_A2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Report_A2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Report_A2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Report_A2.Location = new System.Drawing.Point(0, 0);
            this.Report_A2.Name = "Report_A2";
            this.Report_A2.ReportSource = this.IPQCReport21;
            this.Report_A2.ShowCloseButton = false;
            this.Report_A2.ShowGroupTreeButton = false;
            this.Report_A2.ShowLogo = false;
            this.Report_A2.ShowParameterPanelButton = false;
            this.Report_A2.ShowTextSearchButton = false;
            this.Report_A2.Size = new System.Drawing.Size(755, 455);
            this.Report_A2.TabIndex = 0;
            this.Report_A2.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // QCLapA_2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 455);
            this.Controls.Add(this.Report_A2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QCLapA_2";
            this.Text = "SMS Perso QC  Report PIOTEC A";
            this.Load += new System.EventHandler(this.QCLapA_2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer Report_A2;
        private IPQCReport2 IPQCReport21;
    }
}