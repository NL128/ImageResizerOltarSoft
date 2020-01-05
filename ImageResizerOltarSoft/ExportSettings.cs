using OltarSoftJson.Service.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageResizerOltarSoft
{
    public partial class ExportSettings : Form
    { 
        public delegate void delUpdateUi(OtherSettings other);
        string _fileName = @"OtherSettings.json";
        public ExportSettings()
        {
            InitializeComponent();
            if (!File.Exists(Path.GetFullPath(_fileName)))
            {
                File.Create(Path.GetFullPath(_fileName));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
          string getImageName=   textBox_ImageNames.Text;
            if (!string.IsNullOrEmpty(getImageName))
            {
                try
                {
                    SaveChanges(getImageName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please check settings again, incorrect fields ", "General Notification", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveChanges(string getName)
        {
            try
            {
                OtherSettings settings = new OtherSettings();
                settings.SetImageName(getName);
                var result = settings.SaveChanges();
                if (result != null)
                {
                    CallDelegateToUpdate(result);
                    MessageBox.Show("Saved successfully : " + result.GetImageName(), "Updated Name ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CallDelegateToUpdate(OtherSettings other)
        {
            delUpdateUi delUpdateUi = new delUpdateUi(UpdateUI);
            textBox_ImageNames.BeginInvoke(delUpdateUi, other);
        }
        private void UpdateUI(OtherSettings other)
        {
            textBox_ImageNames.Text = other.GetImageName();
        }
    }
}
