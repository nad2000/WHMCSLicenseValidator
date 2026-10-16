using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shozam
{
    public partial class WHMCSLicenseTestForm : Form
    {
        public WHMCSLicenseTestForm()
        {
            InitializeComponent();
        }

        private WHMCSLicense license;

        private void remoteButton_Click(object sender, EventArgs e)
        {
            License l = this.license.License;
            l.Name = this.nameTextBox.Text;
            l.Email = this.emailTextBox.Text;
            l.LicenseKey = this.licenseKeyTextBox.Text;
            this.msgTextBox.Text = this.license.ValidateRemotely();
        }

        private void WHMCSLicenseTestForm_Load(object sender, EventArgs e)
        {
            // Instantiate the licence
            this.license = new WHMCSLicense(new FileLicenseStorage());
            // this.license = new WHMCSLicense(new RegistryLicenseStorage());
            License l = this.license.License;
            this.nameTextBox.Text = l.Name;
            this.emailTextBox.Text = l.Email;
            this.licenseKeyTextBox.Text = l.LicenseKey;

        }

        private void localButton_Click(object sender, EventArgs e)
        {
            License l = this.license.License;
            l.Name = this.nameTextBox.Text;
            l.Email = this.emailTextBox.Text;
            l.LicenseKey = this.licenseKeyTextBox.Text;
            this.msgTextBox.Text = this.license.Validate();
        }

    }
}
