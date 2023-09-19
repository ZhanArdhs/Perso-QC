using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace SMS_Perso_QC
{
    public partial class QCLapA_2 : Form
    {
        public QCLapA_2()
        {
            InitializeComponent();
        }

        private void QCLapA_2_Load(object sender, EventArgs e)
        {
            Report_A2.RefreshReport();
        }
    }
}
