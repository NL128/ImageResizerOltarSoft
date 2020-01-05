using ImageResizerOltarSoft.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using OltarSoftJson.Service;
namespace ImageResizerOltarSoft
{
    public partial class Resolutions : Form
    {
        private delegate void delEnableOptions();
     private   string _fileName = @"ResolutionExt.json";
        
        Resolution currentSelection2 = null;
        int element = -1;
        public Resolutions()
        {
            InitializeComponent();
            if (!File.Exists(Path.GetFullPath(_fileName)))
            {
                File.Create(Path.GetFullPath(_fileName));
            }
        }

        private void Resolutions_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
      

           
            
            LoadToGrid(_fileName);
        }
        private void LoadToGrid(string fileName)
        {
            try { 
            List<Resolution> result = JsonWriteLoad.LoadJson(_fileName).OrderBy(x => x.Width).ToList();

            dataGridView1.DataSource = result;

            label4.Text = "Total Resolutions : " + result.Count();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

   

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int w = -1;
                int h = -1;

                int.TryParse(textBox1.Text, out w);
                int.TryParse(textBox2.Text, out h);
                if (w > 20 && h > 20)
                {

                    List<Resolution> result = JsonWriteLoad.LoadJson(_fileName);

                    Resolution r = new Resolution();
                    r.Width = w;
                    r.Height = h;
                    if (result.FindIndex(x => x.Width == r.Width && x.Height == r.Height) < 0)
                    {


                        result.Add(r);
                        JsonWriteLoad.WriteJson(_fileName, result);
                        LoadToGrid(_fileName);
                    }
                    else
                    {
                        MessageBox.Show("Resolution Already Exists!");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter Valid Resolution!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            DeleteResolution();
        }
        private void DeleteResolution()
        {
            try
            {
                if (currentSelection2 != null)
                {
                    List<Resolution> result = JsonWriteLoad.LoadJson(_fileName);



                    int first = currentSelection2.Width;
                    int last = currentSelection2.Height;
                    Resolution resolution = new Resolution();
                    resolution.Width = first;
                    resolution.Height = last;
                    int index = result.FindIndex(x => x.Width == currentSelection2.Width && x.Height == currentSelection2.Height);
                    result.RemoveAt(index);
                    JsonWriteLoad.WriteJson(_fileName, result);

                    LoadToGrid(_fileName);
                    element = -1;
                    currentSelection2 = null;
                    if (currentSelection2 == null)
                    {
                        button2.Enabled = false;
                        button3.Enabled = false;
                    }
                    StringBuilder str = new StringBuilder();
                    str.Append("Total Resolutions : ");
                    str.Append(dataGridView1.Rows.Count);
                    label4.Text = str.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            EditContent();
        }
        private void EditContent()
        {
            try
            {
                if (currentSelection2 != null)
                {
                    List<Resolution> result = JsonWriteLoad.LoadJson(_fileName);



                    int first = currentSelection2.Width;
                    int last = currentSelection2.Height;
                    int index = result.FindIndex(x => x.Width == currentSelection2.Width && x.Height == currentSelection2.Height);
                    int.TryParse(textBox1.Text, out first);
                    int.TryParse(textBox2.Text, out last);
                    result[index].Width = first;
                    result[index].Height = last;
                    result[index].IsActive = currentSelection2.IsActive;
                    JsonWriteLoad.WriteJson(_fileName, result);

                    LoadToGrid(_fileName);
                    element = -1;
                    currentSelection2 = null;
                    if (currentSelection2 == null)
                    {
                        button3.Enabled = false;
                    }
                    StringBuilder str = new StringBuilder();
                    str.Append("Total Resolutions : ");
                    str.Append(dataGridView1.Rows.Count);
                    label4.Text = str.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CallDelegate()
        {
            try
            {
                delEnableOptions delEnableOptions = new delEnableOptions(EnableButtons);
                button2.BeginInvoke(delEnableOptions);
                button3.BeginInvoke(delEnableOptions);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void EnableButtons()
        {
            bool enable = true;
            if (!button2.Enabled)
            {
                button2.Enabled = enable;
            }
            if (!button3.Enabled)
            {
                button3.Enabled = enable;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
            int index = e.RowIndex;
            try
            {
                if (index >= 0)
                {
                    bool check = false;
                    if (dataGridView1.CurrentCell.ColumnIndex == 0)
                    {
                        check = Convert.ToBoolean(dataGridView1.CurrentCell.Value);
                    }
                     
                    var selected = dataGridView1.Rows[index];
                    
                    element = index;
                    currentSelection2 = selected.DataBoundItem as Resolution;
                    currentSelection2.IsActive = check;
                    textBox1.Text =Convert.ToString( currentSelection2.Width);
                    textBox2.Text =Convert.ToString( currentSelection2.Height);
                    CallDelegate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
