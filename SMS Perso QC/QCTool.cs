using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Data.OleDb;
using AHDev.Card;
using AHDev.License;

namespace SMS_Perso_QC
{
    public partial class QCTool : Form
    {
        #region Variable

        // Koneksi Database
        private OleDbConnection kon;
        private OleDbCommand cmd;
        private OleDbDataReader rdb;
        private OleDbDataAdapter dap;
        private DataSet dst;
        // System Writer
        private StreamWriter sw = null;
        private FileStream fst = null;
        // Koneksi Card 
        private CardBase m_iCard = null;
        private APDUParam m_apduParam = null;
        private APDUCommand m_apduCmd = null;
        // License
        private KeyManager km;
        private LicenseInfo lic;
        private KeyValuesClass kv;
        // App Config
        private string log_tgl = DateTime.Now.ToString("dd/MM/yy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        private string log_admin = "Admin";
        private string log_config = @"D:\Tool QC\IP_QC_Tool_v1.1\";
        // Variable
        private string dIccid, dImsi, ciccid, iccid, rev, productID, productKey, License, folderlog, folderdb,
            filename, datalog, result, rdata, dconf, produk, qty, mesin, shift, hari, j_shift, cfg, log_jam, proses, log_dir, f_db;
        private string[] aiccid, aimsi, ldata, adata, aproduk, aqty;
        private byte bCls, bIns, bP1, bP2, bLe;
        private byte[] atrValue, bData;
        private int nproses = 1;
        private int statinpt = 0;
        private int value;
        private bool logopen = false;
        private bool dbopen = false;

        // Hari Tanggal Jam Info
        private void tmr_tgl_Tick(object sender, EventArgs e)
        {
            // Setup Hari Tgl Jam
            txt_tgl.Text = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            lbl_tgl.Text = DateTime.Now.ToString("dddd, dd-MM-yyyy     HH:mm:ss");
        }

        private void auto_shift()
        {
            hari = DateTime.Now.ToString("dddd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            j_shift = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);            

            // Data shift
            DateTime dt_shift = DateTime.Parse(j_shift);
            DateTime j_shift1 = DateTime.Parse("07:00:00");
            DateTime j_shift2 = DateTime.Parse("15:00:00");
            DateTime j_shift2s = DateTime.Parse("12:00:00");
            DateTime j_shift3 = DateTime.Parse("23:00:00");
            DateTime j_shift3s = DateTime.Parse("17:00:00");

            // Insert value shift
            if (hari == "Saturday")
            {
                if (dt_shift > j_shift1 && dt_shift <= j_shift2s)
                {
                    ch_s1.Checked = true;
                    ch_s2.Checked = false;
                    ch_s3.Checked = false;
                }
                else if (dt_shift > j_shift2s && dt_shift <= j_shift3s)
                {
                    ch_s1.Checked = false;
                    ch_s2.Checked = true;
                    ch_s3.Checked = false;
                }
                else
                {
                    ch_s1.Checked = false;
                    ch_s2.Checked = false;
                    ch_s3.Checked = true;
                }
            }
            else
            {
                if (dt_shift > j_shift1 && dt_shift <= j_shift2)
                {
                    ch_s1.Checked = true;
                    ch_s2.Checked = false;
                    ch_s3.Checked = false;
                }
                else if (dt_shift > j_shift2 && dt_shift <= j_shift3)
                {
                    ch_s1.Checked = false;
                    ch_s2.Checked = true;
                    ch_s3.Checked = false;
                }
                else
                {
                    ch_s1.Checked = false;
                    ch_s2.Checked = false;
                    ch_s3.Checked = true;
                }
            }
        }

        #endregion        

        #region Keypress Action

        private void txt_brcd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) 
                e.Handled = true;
            else if (e.KeyChar == (char) Keys.Return)
            {
                txt_brcd.Enabled = false;
                lbl_match.Text = "";
                txt_qr.Focus();
            }
        }

        private void txt_qr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
            else if (e.KeyChar == (char) Keys.Return)
            {
                string barcode = txt_brcd.Text;
                string qrcode = txt_qr.Text;

                if (string.Equals(barcode, qrcode))
                {
                    lbl_match.Text = "MATCH";
                    lbl_match.ForeColor = Color.Lime;
                    txt_brcd.Text = "";
                    txt_qr.Text = "";
                    txt_brcd.Enabled = true;
                    txt_brcd.Focus();
                }
                else
                {
                    lbl_match.Text = "NOT MATCH";
                    lbl_match.ForeColor = Color.Red;
                    txt_qr.Text = "";
                }
            }
        }

        private void txt_qc_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar))
                e.Handled = true;
            else if (e.KeyChar == (char)Keys.Return)
                if (txt_qc.Text != "")
                    txt_job.Focus();
                else
                    MessageBox.Show("Nama QC Patrol kosong, silahkan isi terlebih dahulu", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txt_job_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsNumber(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '_' && e.KeyChar != (char)Keys.Space)
                e.Handled = true;
            else if (e.KeyChar == (char)Keys.Return)
            {
                string[] ajob = txt_job.Text.Split('_');
                if (ajob.Length == 2)
                {
                    if (txt_job.Text != "")
                        txt_pn.Focus();
                    else
                        MessageBox.Show("Nama JOP kosong, silahkan isi terlebih dahulu", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } 
                else
                {
                    var mbox = MessageBox.Show("Format nama JOP tidak sesuai silahkan isi dengan format sebagai berikut, IF QTY_PRODUK","SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (mbox == DialogResult.OK)
                    {
                        txt_job.Text = "";
                        txt_job.Focus();
                    }
                }
            }
        }

        private void txt_pn_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
            if (!char.IsControl(e.KeyChar) && !char.IsNumber(e.KeyChar))
                e.Handled = true;
            else if (e.KeyChar == (char)Keys.Return)
                if (txt_pn.Text != "")
                    txt_po.Focus();
                else
                    MessageBox.Show("Nomer PN kosong, silahkan isi terlebih dahulu", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txt_po_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsNumber(e.KeyChar))
                e.Handled = true;
            else if (e.KeyChar == (char)Keys.Return)
                if (txt_po.Text != "")
                    txt_inner.Focus();
                else
                    MessageBox.Show("Nomer PO kosong, silahkan isi terlebih dahulu", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txt_inner_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsNumber(e.KeyChar))
                e.Handled = true;
        }

        private void cb_remake_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_remake.Checked)
            {
                btn_psrpt.Enabled = false;
                btn_rpt.Enabled = false;
            }
            else
            {
                if (cb_msn1.Checked || cb_msn2.Checked)
                {
                    btn_psrpt.Enabled = true;
                    btn_rpt.Enabled = true;
                }
                else
                {
                    btn_psrpt.Enabled = false;
                    btn_rpt.Enabled = false;
                }
            }
        }   

        private void cb_msn1_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_remake.Checked)
            {
                if (cb_msn1.Checked)
                    cb_msn2.Checked = false;
                groupBox1.Enabled = true;
                groupBox3.Enabled = true;
                btn_cek.Enabled = true;
                btn_cek.BackColor = Color.Lime;
                btn_psrpt.Enabled = false;
                btn_rpt.Enabled = false;
                txt_viccid.Focus();
            } 
            else
            {
                if (cb_msn1.Checked)
                    cb_msn2.Checked = false;
                groupBox1.Enabled = true;
                groupBox3.Enabled = true;
                btn_cek.Enabled = true;
                btn_psrpt.Enabled = true;
                btn_rpt.Enabled = true;
                btn_cek.BackColor = Color.Lime;
                
                txt_viccid.Focus();
            }
        }

        private void cb_msn2_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_remake.Checked)
            {
                if (cb_msn2.Checked)
                    cb_msn1.Checked = false;
                groupBox1.Enabled = true;
                groupBox3.Enabled = true;
                btn_cek.Enabled = true;
                btn_cek.BackColor = Color.Lime;
                btn_psrpt.Enabled = false;
                btn_rpt.Enabled = false;
                txt_viccid.Focus();
            }
            else
            {
                if (cb_msn2.Checked)
                    cb_msn1.Checked = false;
                groupBox1.Enabled = true;
                groupBox3.Enabled = true;
                btn_cek.Enabled = true;
                btn_cek.BackColor = Color.Lime;
                btn_psrpt.Enabled = true;
                btn_rpt.Enabled = true;
                txt_viccid.Focus();
            }
        }

        private void ch_s1_CheckedChanged(object sender, EventArgs e)
        {
            if (ch_s1.Checked)
            {
                ch_s2.Checked = false;
                ch_s3.Checked = false;
            }
        }

        private void ch_s2_CheckedChanged(object sender, EventArgs e)
        {
            if (ch_s2.Checked)
            {
                ch_s1.Checked = false;
                ch_s3.Checked = false;
            }
        }

        private void ch_s3_CheckedChanged(object sender, EventArgs e)
        {
            if (ch_s3.Checked)
            {
                ch_s1.Checked = false;
                ch_s2.Checked = false;
            }
        }

        private void txt_viccid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
            lbl_result.Text = "";
        } 

        #endregion

        #region Setup Loader Data

        private void LoadLoader()
        {
            // setup Text
            lbl_match.Text = "";
            txt_qc.Text = "";
            txt_job.Text = "";
            txt_pn.Text = "";
            txt_po.Text = "";
            txt_inner.Text = "";
            txt_tgl.Text = "";
            txt_atr.Text = "";
            txt_iccid.Text = "";
            txt_imsi.Text = "";
            txt_viccid.Text = "";
            txt_vimsi.Text = "";
            lbl_result.Text = "";
            lbl_pid.Text = "";
            lbl_pkey.Text = "";
            txt_log.Text = "";
            txt_db.Text = "";

            // setup Button
            btn_reload.Enabled = true;
            btn_cek.Enabled = false;
            btn_cek.BackColor = Color.Red;
            btn_psrpt.Enabled = false;
            btn_update.Enabled = false;
            btn_rpt.Enabled = false;
            btn_log.Enabled = false;
            btn_db.Enabled = false;

            // setup Check
            cb_reader.Enabled = true;     
            cb_remake.Checked = false;
            cb_msn1.Checked = false;
            cb_msn2.Checked = false;
            ch_s1.Checked = false;
            ch_s2.Checked = false;
            ch_s3.Checked = false;

            // setup Group Box    
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;            

            // setup focus 
            txt_qc.Focus();   
        }

        #endregion

        #region Setup Reader

        private void SelectICard()
        {
            try
            {
                if (m_iCard != null)
                    m_iCard.Disconnect(DISCONNECT.Unpower);

                m_iCard = new CardNative();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetupReaderList()
        {
            try
            {
                string[] sListReaders = m_iCard.ListReaders();
                cb_reader.Items.Clear();                

                if (sListReaders != null)
                {
                    for (int nI = 0; nI < sListReaders.Length; nI++)
                        cb_reader.Items.Add(sListReaders[nI]);

                    cb_reader.SelectedIndex = 0;
                    btn_reload.Enabled = false;
                    groupBox2.Enabled = true;
                    btn_db.Enabled = true;
                    btn_log.Enabled = true;
                }
                else
                    MessageBox.Show("No Reader is Connected to the Computer", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Card Method

        private void card_conection()
        {
            try
            {
                m_iCard.Connect(cb_reader.SelectedItem.ToString(), SHARE.Shared, PROTOCOL.T0orT1);
            
                try
                {
                    // Get ATR from card
                    atrValue = m_iCard.GetAttribute(SCARD_ATTR_VALUE.ATR_STRING);
                    txt_atr.Text = ByteArrayToString(atrValue);
                }
                catch (Exception)
                {
                    txt_atr.Text = "Unable to read ATR";
                }
                
                btn_reload.Enabled = false;
            }
            catch (Exception)
            {
                btn_reload.Enabled = true;
                //MessageBox.Show(ex.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void card_disconnect()
        {
            try
            {
                m_iCard.Disconnect(DISCONNECT.Unpower);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);               
            }
        }

        #endregion

        #region Byte To String & Build Param   

        static private string ByteArrayToString(byte[] data)
        {
            StringBuilder sDataOut;

            if (data != null)
            {
                sDataOut = new StringBuilder(data.Length * 2);
                for (int nI = 0; nI < data.Length; nI++)
                    sDataOut.AppendFormat("{0:X02} ", data[nI]);
            }
            else
                sDataOut = new StringBuilder();

            return sDataOut.ToString();
        }

        private APDUParam buildParam(byte[] bPrm)
        {
            m_apduParam = new APDUParam();
            m_apduParam.P1 = bPrm[0];
            m_apduParam.P2 = bPrm[1];
            m_apduParam.Le = bPrm[2];

            return m_apduParam;
        }

        #endregion

        #region Access Data Card

        private void select_mf()
        {
            bCls = 160;
            bIns = 164;
            bP1 = 0;
            bP2 = 0;
            bData = new byte[2] { (byte)63, (byte)0 };
            bLe = 0;
            byte[] bPrm = new byte[3] { bP1, bP2, bLe };
            try
            {
                buildParam(bPrm);
                m_apduCmd = new APDUCommand(bCls, bIns, bP1, bP2, bData, bLe);
                m_apduCmd.Update(m_apduParam);
                APDUResponse apduResp = m_iCard.Transmit(m_apduCmd);
            }
            catch (SmartCardException)
            {
                //MessageBox.Show(exSC.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }   

        private void select_iccid()
        {
            bCls = 160;
            bIns = 164;
            bP1 = 0;
            bP2 = 0;
            bData = new byte[2] {(byte)47, (byte)226};
            bLe = 0;
            byte[] bPrm = new byte[3] { bP1, bP2, bLe };
            try
            {
                buildParam(bPrm);
                m_apduCmd = new APDUCommand(bCls, bIns, bP1, bP2, bData, bLe);
                m_apduCmd.Update(m_apduParam);
                APDUResponse apduResp = m_iCard.Transmit(m_apduCmd);
            }
            catch (SmartCardException)
            {
                //MessageBox.Show(exSC.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }       
        
        private void read_iccid()
        {
            bCls = 160;
            bIns = 176;
            bP1 = 0;
            bP2 = 0;
            bData = null;
            bLe = 10;
            byte[] bPrm = new byte[3] { bP1, bP2, bLe };
            try
            {
                buildParam(bPrm);
                m_apduCmd = new APDUCommand(bCls, bIns, bP1, bP2, bData, bLe);
                m_apduCmd.Update(m_apduParam);
                APDUResponse apduResp = m_iCard.Transmit(m_apduCmd);
                if (apduResp.Data != null)
                {
                    StringBuilder sDataOut = new StringBuilder(apduResp.Data.Length * 2);
                    for (int nI = 0; nI < apduResp.Data.Length; nI++)
                    {
                        sDataOut.AppendFormat("{0:X02} ", apduResp.Data[nI]);
                    }

                    dIccid = sDataOut.ToString();
                    aiccid = dIccid.Split(' ');
                    rev = "";

                    for (int i = 0; i < aiccid.Length; i++)
                    {
                        string str = aiccid[i];

                        int a = str.Length - 1;
                        while (a >= 0)
                        {
                            rev = rev + str[a];
                            a--;
                        }
                    }

                    txt_iccid.Text = rev;
                }
                else
                    txt_iccid.Text = "Unable to read ICCID";
            }
            catch (SmartCardException)
            {
                //MessageBox.Show(exSC.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void select_df()
        {
            bCls = 160;
            bIns = 164;
            bP1 = 0;
            bP2 = 0;
            bData = new byte[2] { (byte)127, (byte)32 };
            bLe = 0;
            byte[] bPrm = new byte[3] { bP1, bP2, bLe };
            try
            {
                buildParam(bPrm);
                m_apduCmd = new APDUCommand(bCls, bIns, bP1, bP2, bData, bLe);
                m_apduCmd.Update(m_apduParam);
                APDUResponse apduResp = m_iCard.Transmit(m_apduCmd);
            }
            catch (SmartCardException)
            {
                //MessageBox.Show(exSC.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void select_imsi()
        {
            bCls = 160;
            bIns = 164;
            bP1 = 0;
            bP2 = 0;
            bData = new byte[2] {(byte)111, (byte)7};
            bLe = 0;
            byte[] bPrm = new byte[3] { bP1, bP2, bLe };
            try
            {
                buildParam(bPrm);
                m_apduCmd = new APDUCommand(bCls, bIns, bP1, bP2, bData, bLe);
                m_apduCmd.Update(m_apduParam);
                APDUResponse apduResp = m_iCard.Transmit(m_apduCmd);
            }
            catch (SmartCardException)
            {
                //MessageBox.Show(exSC.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void read_imsi()
        {
            bCls = 160;
            bIns = 176;
            bP1 = 0;
            bP2 = 0;
            bData = (byte[])null;
            bLe = 9;
            byte[] bPrm = new byte[3] { bP1, bP2, bLe };
            try
            {
                buildParam(bPrm);
                m_apduCmd = new APDUCommand(bCls, bIns, bP1, bP2, bData, bLe);
                m_apduCmd.Update(m_apduParam);
                APDUResponse apduResp = m_iCard.Transmit(m_apduCmd);
                if (apduResp.Data != null)
                {
                    StringBuilder sDataOut = new StringBuilder(apduResp.Data.Length * 2);
                    for (int nI = 0; nI < apduResp.Data.Length; nI++)
                    {
                        sDataOut.AppendFormat("{0:X02} ", apduResp.Data[nI]);
                    }

                    dImsi = sDataOut.ToString();
                    aimsi = dImsi.Split(' ');
                    rev = "";

                    for (int i = 0; i < aimsi.Length; i++)
                    {
                        string str = aimsi[i];

                        int a = str.Length - 1;
                        while (a >= 0)
                        {
                            rev = rev + str[a];
                            a--;
                        }
                    }                    

                    txt_imsi.Text = rev.Substring(3);
                }
                else
                    txt_imsi.Text = "Unable to read IMSI";            
            }
            catch (SmartCardException)
            {
                //MessageBox.Show(exSC.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region button action

        private void btn_reload_Click(object sender, EventArgs e)
        {
            SetupReaderList();
        }

        private void btn_log_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderlog = folderBrowserDialog1.SelectedPath;
                txt_log.Text = folderlog + @"\";
                if (!logopen)
                {
                    openFileDialog1.InitialDirectory = folderlog;
                    openFileDialog1.FileName = null;
                }
            }
        }

        private void btn_db_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderdb = folderBrowserDialog2.SelectedPath;
                txt_db.Text = folderdb;
                if (!dbopen)
                {
                    openFileDialog2.InitialDirectory = folderdb;
                    openFileDialog2.FileName = null;
                }
            }
        }

        private void btn_cek_Click(object sender, EventArgs e)
        {
            txt_atr.Text = "";
            txt_iccid.Text = "";
            txt_imsi.Text = "";
            lbl_result.Text = "";

            card_conection();
            select_mf();
            select_iccid();
            read_iccid();
            select_df();
            select_imsi();
            read_imsi();
            card_disconnect();

            tmr_card.Enabled = true;

            card_conection();
            select_mf();
            select_iccid();
            read_iccid();
            select_df();
            select_imsi();
            read_imsi();
            card_disconnect();

            tmr_card.Enabled = false;
            lbl_result.Text = "";
            txt_vimsi.Text = "";

            if (txt_qc.Text == "" || txt_job.Text == "" || txt_pn.Text == "" || txt_po.Text == "")
                MessageBox.Show("Silahkan lengkapi data terlebih dahulu untuk melanjutkan", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (txt_atr.Text != "")
                {
                    ciccid = txt_viccid.Text;
                    iccid = txt_iccid.Text;

                    if (iccid.Contains(ciccid))
                    {
                        if (txt_imsi.Text.Contains("Unable to read IMSI"))
                        {
                            txt_vimsi.Text = "Unable to read IMSI";
                            txt_viccid.Focus();
                            lbl_result.ForeColor = Color.Red;
                            lbl_result.Text = "DATA NOT MATCH";
                        }
                        else
                        {
                            lbl_result.ForeColor = Color.Lime;
                            lbl_result.Text = "DATA MATCH";
                            txt_vimsi.Text = txt_imsi.Text;
                            txt_viccid.Focus();
                        }
                    }
                    else
                    {
                        txt_vimsi.Text = "Unable to read IMSI";
                        txt_viccid.Focus();
                        lbl_result.ForeColor = Color.Red;
                        lbl_result.Text = "DATA NOT MATCH";
                    }

                    save_log();

                    if (cb_msn1.Checked == true)
                        mesin = "PIOTEC 1";
                    else
                        mesin = "PIOTEC 2";

                    if (ch_s1.Checked == true)
                        shift = "1";
                    else if (ch_s2.Checked == true)
                        shift = "2";
                    else
                        shift = "3";

                    if (lbl_result.Text == "DATA MATCH")
                        result = "PASS";
                    else
                        result = "FAIL";

                    if (cb_remake.Checked == false)
                        save_db(mesin, shift, result);
                }
                else
                    MessageBox.Show("Simcard Not Detected in Reader", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_psrpt_Click(object sender, EventArgs e)
        {
            if (txt_qc.Text == "")
                MessageBox.Show("Silahkan lengkapi data terlebih dahulu untuk melanjutkan", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                var mbox = MessageBox.Show("Apakah anda yakin akan memproses laporan ini ?, Pastikan No PN dan No Inner sudah sessuai", "SMS Perso QC Tool", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (mbox == DialogResult.Yes)
                {
                    cek_qc();
                }
            }
        }

        private void btn_rpt_Click(object sender, EventArgs e)
        {
            if (txt_qc.Text == "")
                MessageBox.Show("Silahkan lengkapi data terlebih dahulu untuk melanjutkan", "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (cb_msn1.Checked == true)
                {
                    if (txt_db.Text == "")
                    {
                        using (QCLapA qla = new QCLapA())
                            qla.ShowDialog();
                    }
                    else if (txt_db.Text == @"C:\Database\APP1")
                    {
                        using (QCLapA qla = new QCLapA())
                            qla.ShowDialog();
                    }
                    else if (txt_db.Text == @"C:\Database\APP2")
                    {
                        using (QCLapA_2 qla2 = new QCLapA_2())
                            qla2.ShowDialog();
                    }
                }                    
                else
                {
                    if (txt_db.Text == "")
                    {
                        using (QCLapB qlb = new QCLapB())
                            qlb.ShowDialog();
                    }
                    else if (txt_db.Text == @"C:\Database\APP1")
                    {
                        using (QCLapB qlb = new QCLapB())
                            qlb.ShowDialog();
                    }
                    else if (txt_db.Text == @"C:\Database\APP2")
                    {
                        using (QCLapB_2 qlb2 = new QCLapB_2())
                            qlb2.ShowDialog();
                    }
                }
            }           
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using(QCHelp qch = new QCHelp())
                qch.ShowDialog();
        }

        private void dg_ipqc_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            proses = dg_ipqc.Rows[e.RowIndex].Cells[2].Value.ToString();
            mesin = dg_ipqc.Rows[e.RowIndex].Cells[5].Value.ToString();
            if (mesin == "PIOTEC 1")
            {
                cb_msn1.Checked = true;
                cb_msn2.Checked = false;
            }
            else
            {
                cb_msn1.Checked = false;
                cb_msn2.Checked = true;
            }

            txt_pn.Text = dg_ipqc.Rows[e.RowIndex].Cells[3].Value.ToString();
            txt_inner.Text  = dg_ipqc.Rows[e.RowIndex].Cells[4].Value.ToString();

            btn_update.Enabled = true;
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            mesin = "";
            if (cb_msn1.Checked)
                mesin = "PIOTEC 1";
            else if (cb_msn2.Checked)
                mesin = "PIOTEC 2";

            if (txt_db.Text == "")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP1")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP2")
                f_db = "C:\\Database\\APP2\\db_ipqc.mdb;";

            kon = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + f_db + "Persist Security Info=True;Jet OLEDB:Database Password=Arha150701");
            cmd = kon.CreateCommand();
            cmd.Connection = kon;
            kon.Open();
            cmd.CommandText = "UPDATE tbl_report SET PN = '" + txt_pn.Text + "', INO = '" + txt_inner.Text + "', MESIN = '" + mesin + "' WHERE PROSES = '" + proses + "'";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT MIN(ICCID) AS S_ICCID, MAX(ICCID) AS E_ICCID, PROSES, MIN(PN) AS PN, MIN(INO) AS INO, MIN(MESIN) AS MESIN FROM tbl_report WHERE QC = '" + txt_qc.Text + "' AND MESIN = '" + mesin + "' AND SHIFT = '" + shift + "' GROUP BY PROSES";
            dst = new DataSet();
            dap = new OleDbDataAdapter(cmd);
            dap.Fill(dst, "tbl_report");
            dg_ipqc.DataSource = dst;
            dg_ipqc.DataMember = "tbl_report";
            dg_ipqc.Columns[2].Visible = false;
            dg_ipqc.Columns[3].Visible = false;
            dg_ipqc.Columns[4].Visible = false;
            dg_ipqc.Columns[5].Visible = false;
            foreach (DataGridViewColumn col in dg_ipqc.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dg_ipqc.Columns[0].Width = 175;
            dg_ipqc.Columns[0].HeaderText = "START ICCID";
            dg_ipqc.Columns[0].DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dg_ipqc.Columns[1].Width = 175;
            dg_ipqc.Columns[1].HeaderText = "END ICCID";
            dg_ipqc.Columns[1].DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dg_ipqc.Refresh();
            kon.Close();

            btn_update.Enabled = false;
        }

        #endregion

        #region Create LOG File

        private void save_log()
        {
            if (txt_log.Text == "")
                log_dir = @"D:\IPQC\LOG\";
            else
                log_dir = txt_log.Text;

            if (txt_vimsi.Text.Contains("51010"))
                cfg = "1 - 19 digits.conf";
            else
                cfg = "7 - 18 digits.conf";

            log_jam = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            if (cb_remake.Checked)
                filename = txt_job.Text + "_PN" + txt_pn.Text + "_Remake";
            else
                filename = txt_job.Text + "_PN" + txt_pn.Text;

            datalog = log_dir + filename + "_" + txt_qc.Text + ".txt";            

            if (lbl_result.Text == "DATA MATCH")
                result = "PASS";
            else
                result = "FAIL";

            if (File.Exists(datalog))
            {
                rdata = File.ReadAllText(datalog);
                ldata = rdata.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                adata = ldata[ldata.Length - 2].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                dconf = adata[3].Substring(13);

                if (dconf == log_config + cfg && statinpt != 0)
                {
                    try
                    {
                        // create filestream
                        fst = new FileStream(datalog, FileMode.Append, FileAccess.Write);
                        // create streamwriter from filestream
                        using (sw = new StreamWriter(fst, Encoding.UTF8))
                        {
                            // continue write to text file
                            if (txt_viccid.Text.Length == 12)
                                sw.WriteLine(txt_iccid.Text + "\t\t" + txt_viccid.Text + "        \t\t" + txt_imsi.Text + "     \t\t" + "               \t\t" + result);
                            else
                                sw.WriteLine(txt_iccid.Text + "\t\t" + txt_viccid.Text + " \t\t" + txt_imsi.Text + "     \t\t" + "               \t\t" + result);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    try
                    {
                        // create filestream
                        fst = new FileStream(datalog, FileMode.Append, FileAccess.Write);
                        // create streamwriter from filestream
                        using (sw = new StreamWriter(fst, Encoding.UTF8))
                        {
                            // write new text file
                            sw.WriteLine("");
                            sw.WriteLine("Operator Name: " + log_admin);
                            sw.WriteLine("Date: " + log_tgl);
                            sw.WriteLine("Time: " + log_jam);
                            sw.WriteLine("Config File: " + log_config + cfg);
                            sw.WriteLine("Job ID: " + filename);
                            sw.WriteLine("");
                            sw.WriteLine("ICCID IN CARD\t\t\tICCID USER INPUT\t\tIMSI IN CARD\t\t\tIMSI USER INPUT\t\tRESULTS");
                            if (txt_viccid.Text.Length == 12)
                                sw.WriteLine(txt_iccid.Text + "\t\t" + txt_viccid.Text + "        \t\t" + txt_imsi.Text + "     \t\t" + "               \t\t" + result);
                            else
                                sw.WriteLine(txt_iccid.Text + "\t\t" + txt_viccid.Text + " \t\t" + txt_imsi.Text + "     \t\t" + "               \t\t" + result);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                try
                {
                    // create filestream
                    fst = new FileStream(datalog, FileMode.CreateNew);
                    // create streamwriter from filestream
                    using (sw = new StreamWriter(fst, Encoding.UTF8))
                    {
                        // write to text file
                        sw.WriteLine("Operator Name: " + log_admin);
                        sw.WriteLine("Date: " + log_tgl);
                        sw.WriteLine("Time: " + log_jam);
                        sw.WriteLine("Config File: " + log_config + cfg);
                        sw.WriteLine("Job ID: " + filename);
                        sw.WriteLine("");
                        sw.WriteLine("ICCID IN CARD\t\t\tICCID USER INPUT\t\tIMSI IN CARD\t\t\tIMSI USER INPUT\t\tRESULTS");
                        if (txt_viccid.Text.Length == 12)
                            sw.WriteLine(txt_iccid.Text + "\t\t" + txt_viccid.Text + "        \t\t" + txt_imsi.Text + "     \t\t" + "               \t\t" + result);
                        else
                            sw.WriteLine(txt_iccid.Text + "\t\t" + txt_viccid.Text + " \t\t" + txt_imsi.Text + "     \t\t" + "               \t\t" + result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SMS Perso QC Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            statinpt = statinpt + 1;   
        }

        #endregion

        #region Database Function

        private void save_db(string mesin, string shift, string result)
        {
            aproduk = txt_job.Text.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            aqty = aproduk[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            produk = aproduk[aproduk.Length - 1];
            qty = aqty[1];            

            string jam = DateTime.Now.ToString("HH:mm", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            string nopn = txt_pn.Text;

            if (txt_db.Text == "")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP1")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP2")
                f_db = "C:\\Database\\APP2\\db_ipqc.mdb;";

            // Koneksi
            kon = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + f_db + "Persist Security Info=True;Jet OLEDB:Database Password=Arha150701");
            cmd = kon.CreateCommand();
            cmd.Connection = kon;
            kon.Open();
            cmd.CommandText = "INSERT INTO tmp_report VALUES ('" + txt_job.Text + "','" + produk + "','" + txt_po.Text + "','" + qty + "','" + mesin + "','" + txt_tgl.Text + "','" + shift + "','" + txt_qc.Text + "','" + jam + "','" + nopn + "','" + txt_viccid.Text + "','" + result + "','" + txt_inner.Text + "')";
            cmd.ExecuteNonQuery();
            kon.Close();
        }

        private void cek_qc()
        {
            string qcname = txt_qc.Text;

            if (cb_msn1.Checked == true)
                mesin = "PIOTEC 1";
            else
                mesin = "PIOTEC 2";

            if (ch_s1.Checked == true)
                shift = "1";
            else if (ch_s2.Checked == true)
                shift = "2";
            else
                shift = "3";

            if (txt_db.Text == "")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP1")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP2")
                f_db = "C:\\Database\\APP2\\db_ipqc.mdb;";

            // Koneksi
            kon = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + f_db + "Persist Security Info=True;Jet OLEDB:Database Password=Arha150701");
            cmd = kon.CreateCommand();
            cmd.Connection = kon;
            kon.Open();
            cmd.CommandText = "SELECT MIN(QC) AS QC FROM tbl_report";
            rdb = cmd.ExecuteReader();
            rdb.Read();
            string qc_cek = rdb[0].ToString();
            rdb.Close();
            kon.Close();

            if (qc_cek == "")
            {
                move_db();
                open_db(qcname, mesin, shift);                
            }
            else
            {
                if (qc_cek != txt_qc.Text)
                {
                    kon.Open();
                    cmd.CommandText = "DELETE FROM tbl_report";
                    cmd.ExecuteNonQuery();
                    kon.Close();
                    move_db();
                    open_db(qcname, mesin, shift);
                }
                else
                {
                    move_db();
                    open_db(qcname, mesin, shift);
                }                
            }           
            
        }

        private void move_db()
        {
            if (txt_db.Text == "")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP1")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP2")
                f_db = "C:\\Database\\APP2\\db_ipqc.mdb;";

            // Koneksi
            kon = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + f_db + "Persist Security Info=True;Jet OLEDB:Database Password=Arha150701");
            cmd = kon.CreateCommand();
            cmd.Connection = kon;
            kon.Open();
            
            cmd.CommandText = "SELECT TOP 1 PROSES FROM tbl_report ORDER BY PROSES DESC";
            using (rdb = cmd.ExecuteReader())
            {
                while (rdb.Read())
                {
                    nproses = Convert.ToInt32(rdb[0].ToString().Substring(3)) + 1;
                }
                rdb.Close();
            }

            string npros = "PR-" + nproses.ToString("000000");

            cmd.CommandText = "INSERT INTO tbl_report (JOP,PRODUK,PO,QTY,MESIN,TGL,SHIFT,QC,JAM,PN,ICCID,RESULT,INO,PROSES) SELECT JOP,PRODUK,PO,QTY,MESIN,TGL,SHIFT,QC,JAM,PN,ICCID,RESULT,INO,'" + npros + "' FROM tmp_report";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO his_report (JOP,PRODUK,PO,QTY,MESIN,TGL,SHIFT,QC,JAM,PN,ICCID,RESULT,INO,PROSES) SELECT JOP,PRODUK,PO,QTY,MESIN,TGL,SHIFT,QC,JAM,PN,ICCID,RESULT,INO,'" + npros + "' FROM tmp_report";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM tmp_report";
            cmd.ExecuteNonQuery();

            kon.Close();
        }

        private void open_db(string qcname, string mesin, string shift)
        {
            if (txt_db.Text == "")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP1")
                f_db = "C:\\Database\\APP1\\db_ipqc.mdb;";
            else if (txt_db.Text == @"C:\Database\APP2")
                f_db = "C:\\Database\\APP2\\db_ipqc.mdb;";

            // Koneksi
            kon = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + f_db + "Persist Security Info=True;Jet OLEDB:Database Password=Arha150701");
            cmd = kon.CreateCommand();
            cmd.Connection = kon;
            kon.Open();
            cmd.CommandText = "SELECT MIN(ICCID) AS S_ICCID, MAX(ICCID) AS E_ICCID, PROSES, MIN(PN) AS PN, MIN(INO) AS INO, MIN(MESIN) AS MESIN FROM tbl_report WHERE QC = '" + qcname + "' AND MESIN = '" + mesin + "' AND SHIFT = '" + shift + "' GROUP BY PROSES";
            dst = new DataSet();
            dap = new OleDbDataAdapter(cmd);            
            dap.Fill(dst, "tbl_report");
            dg_ipqc.DataSource = dst;
            dg_ipqc.DataMember = "tbl_report";
            dg_ipqc.Columns[2].Visible = false;
            dg_ipqc.Columns[5].Visible = false;
            foreach (DataGridViewColumn col in dg_ipqc.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dg_ipqc.Columns[0].Width = 175;
            dg_ipqc.Columns[0].HeaderText = "START ICCID";
            dg_ipqc.Columns[0].DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dg_ipqc.Columns[1].Width = 175;
            dg_ipqc.Columns[1].HeaderText = "END ICCID";
            dg_ipqc.Columns[1].DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dg_ipqc.Columns[2].Width = 70;
            dg_ipqc.Columns[2].HeaderText = "PN";
            dg_ipqc.Columns[2].DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dg_ipqc.Columns[2].Width = 100;
            dg_ipqc.Columns[2].HeaderText = "INNER";
            dg_ipqc.Columns[2].DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dg_ipqc.Refresh();           
            kon.Close();
        }

        #endregion

        #region Windows Form        

        public QCTool()
        {
            InitializeComponent();            
        }

        private void QCTool_Load(object sender, EventArgs e)
        {
            activation();
        }

        private void QCTool_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(1);
            m_iCard.Disconnect(DISCONNECT.Unpower);
            m_iCard.StopCardEvents();
            tmr_tgl.Stop();
        }        
        
        #endregion

        #region Activation Code

        private void activation()
        {
            productID = ComputerInfo.GetComputerId();
            km = new KeyManager(productID);
            lic = new LicenseInfo();
            value = km.LoadSuretyFile(string.Format(@"{0}\Key.lic", Application.StartupPath), ref lic);
            productKey = lic.ProductKey;
            License = "";

            if (km.ValidKey(ref productKey))
            {
                LoadLoader();
                SelectICard();
                SetupReaderList();
                tmr_tgl.Start();
                auto_shift();

                kv = new KeyValuesClass();
                if (km.DisassembleKey(productKey, ref kv))
                {
                    if (kv.Type == LicenseType.TRIAL)
                        License = string.Format("{0} days", (kv.Expiration - DateTime.Now.Date).Days);
                    else
                        License = "Ultimate";

                    lbl_pid.Text = "Application ID : " + productID;
                    lbl_pkey.Text = "Application Key : " + productKey;

                    string[] sdlcs = License.Split(' ');
                    int dlcs = Int32.Parse(sdlcs[0]);

                    if (dlcs <= 0)
                    {
                        var mbox = MessageBox.Show("SMS Perso QC Tool application license has expired", "Activation Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (mbox == DialogResult.OK)
                        {
                            using (QCReg qr = new QCReg())
                                qr.ShowDialog();
                        }
                    }
                }
            }
            else
            {
                using (QCReg qr = new QCReg())
                    qr.ShowDialog();
            }
        }

        #endregion                            
       
    }
}
