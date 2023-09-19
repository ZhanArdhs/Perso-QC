namespace SMS_Perso_QC
{
    partial class QCLapB_2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QCLapB_2));
            this.Report_B2 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.IPQCReportB21 = new SMS_Perso_QC.IPQCReportB2();
            this.SuspendLayout();
            // 
            // Report_B2
            // 
            this.Report_B2.ActiveViewIndex = 0;
            this.Report_B2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Report_B2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Report_B2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Report_B2.Location = new System.Drawing.Point(0, 0);
            this.Report_B2.Name = "Report_B2";
            this.Report_B2.ReportSource = this.IPQCReportB21;
            this.Report_B2.ShowCloseButton = false;
            this.Report_B2.ShowGotoPageButton = false;
            this.Report_B2.ShowGroupTreeButton = false;
            this.Report_B2.ShowLogo = false;
            this.Report_B2.ShowParameterPanelButton = false;
            this.Report_B2.Size = new System.Drawing.Size(766, 439);
            this.Report_B2.TabIndex = 0;
            this.Report_B2.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // QCLapB_2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 439);
            this.Controls.Add(this.Report_B2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QCLapB_2";
            this.Text = "SMS Perso QC Report PIOTEC B";
            this.Load += new System.EventHandler(this.QCLapB_2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer Report_B2;
        private IPQCReportB2 IPQCReportB21;
    }
}