﻿using System;
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
    public partial class QCLapA : Form
    {
        public QCLapA()
        {
            InitializeComponent();
        }

        private void QCLapA_Load(object sender, EventArgs e)
        {
            Report_A.RefreshReport();
        }      
    }
}
