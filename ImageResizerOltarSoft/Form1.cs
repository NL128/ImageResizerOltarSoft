using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OltarSoftJson.Service;
using OltarSoftJson.Service.Models;

namespace ImageResizerOltarSoft
{
    public partial class Form1 : Form
    {
        private string DefaultExportImageName = "ExportedImage";
        string DestinationToSave = null;
        string[] files = null;
        string imgExtension = null;
        private string _fileName = @"ResolutionExt.json";
        private long ImageQuality = 100L;
        private float ppi = 150.0f;

        private static int ProgressIndex = 0;
        private float ProgressCompleteGlobal = 0;
        ProgressReport progressReport = null;
        int total = 1;
       public static int totalSectionCounts = 0;

        public delegate void delUIUpdate(string miliseconds);
        public delegate void delUIUpdate2(long imageQuality);
        public delegate void delUIUpdate3();
        BackgroundWorker worker = new BackgroundWorker();
        ThreadStart threadStart;
        Thread thread;
        public Form1()
        {

            InitializeComponent();
            if (!File.Exists(Path.GetFullPath(_fileName)))
            {
                File.Create(Path.GetFullPath(_fileName));
            }
            if (!File.Exists(Path.GetFullPath(@"Saved_Settings.json")))
            {
                File.Create(Path.GetFullPath(@"Saved_Settings.json"));
            }
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
          

            button1.Enabled = false;
            comboBox1.SelectedItem = null;
            comboBox1.Text = "Image Extensions";
            trackBar1.Value = 100;
            textBox1.Text = Convert.ToString(ppi);


         

            DelegateInvokeLoad(@"Saved_Settings.json");
            GetUserDefinedImageName();
        }
        private void GetUserDefinedImageName()
        {
            try
            {
                OtherSettings otherSettings = new OtherSettings();
                string name = otherSettings.GetUserDefinedName();
                if (!string.IsNullOrEmpty(name))
                {
                    DefaultExportImageName = name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       
        private void DelegateInvoke(string miliseconds)
        {
            try
            {
                delUIUpdate delUIUpdate = new delUIUpdate(UiUpdateMethod);

                label5.BeginInvoke(delUIUpdate, miliseconds);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UiUpdateMethod(string miliseconds)
        {
            label5.Text = "Total Seconds : " + miliseconds;
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                progressBar1.Value = progressBar1.Maximum;
                label1.Text = "Task Completed";
                if (!string.IsNullOrEmpty(DestinationToSave))
                {
                    string fdp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string windir = Environment.GetEnvironmentVariable("WINDIR");
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = DestinationToSave;


                    prc.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                progressBar1.Value = e.ProgressPercentage;

                label1.Text = e.ProgressPercentage.ToString() + " %";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;

                int totalImagesSelected = listBox1.Items.Count;
                int startPoint = 0;
                int endPoint = 1;

                CallWorkerForMultithreading(worker, startPoint, endPoint, totalImagesSelected);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private  void CallWorkerForMultithreading(BackgroundWorker worker,int startingPoint,int endPoint,int totalImagesSelected)
        {
            CallWorkerPerThread(worker,startingPoint, endPoint, totalImagesSelected);


        }
        private void CallWorkerPerThread(BackgroundWorker worker,int startingPoint,int endPoint,int totalImagesSelected)
        {
            try
            {
                DateTime timeStart = DateTime.Now;
                List<Models.Resolution> results = JsonWriteLoad.LoadJson(_fileName);


                int currentIndex = startingPoint;

                int indexFromBack = totalImagesSelected - 1;
                int currentIncrement = 0;

                //endPoint is not used yet
                while (currentIndex < totalImagesSelected)
                {

                    for (int res = 0, resBack = results.Count() - 1; res < results.Count(); res++, resBack--)
                    {
                        if (resBack <= res) { break; }

                        int totalCircles = ResizeImageToSpecificSize(currentIncrement, listBox1.Items[currentIndex].ToString(), DestinationToSave, ImageQuality, results[res].Width, results[res].Height);
                        int totalCircleFromBack = ResizeImageToSpecificSize(currentIncrement, listBox1.Items[indexFromBack].ToString(), DestinationToSave, ImageQuality, results[resBack].Width, results[resBack].Height);

                        if (totalCircles > totalCircleFromBack)
                        {
                            currentIncrement = totalCircles;
                        }
                        else if (totalCircles < totalCircleFromBack)
                        {
                            currentIncrement = totalCircleFromBack;
                        }
                        else
                        {
                            currentIncrement = totalCircles;
                        }

                        //MessageBox.Show("TC : " + totalCircles.ToString() + " TCFB : " + totalCircleFromBack.ToString());
                        int percentage = currentIncrement * 2 * 100 / total;
                        worker.ReportProgress(percentage);

                    }

                    currentIndex++;
                }


                int finalPercentage = currentIncrement * 2 * 100 / total;

                worker.ReportProgress(finalPercentage);
                DateTime timeEnd = DateTime.Now;
                TimeSpan diff = timeEnd - timeStart;
                threadStart = new ThreadStart(() => DelegateInvoke(diff.TotalSeconds.ToString()));
                thread = new Thread(threadStart);
                thread.IsBackground = true;
                thread.Start();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Unhandled Exception",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private Bitmap BT(string fileInfo,float maxWidth,float maxHeight)
        {
            Image image;
            using (FileStream myStream = new FileStream(fileInfo, FileMode.Open,FileAccess.Read))
            {
                
                image = Image.FromStream(myStream);


                myStream.Close();
                myStream.Dispose();
            }
            // Image image =  Image.FromFile(fileInfo);
           

            // Get the image's original width and height
            float originalWidth = image.Width;
            float originalHeight = image.Height;


            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            // To preserve the aspect ratio

            // New width and height based on aspect ratio
            if (!checkBox1.Checked)
            {
                ratioX = 1f;
                ratioY = 1f;
                originalWidth = (int)maxWidth;
                originalHeight = (int)maxHeight;
            }
            float ratio = Math.Min(ratioX, ratioY);
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);






            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            try
            {
                // Draws the image in the specified size with quality mode set to HighQuality
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;

                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                    graphics.Dispose();
                }

                newImage.SetResolution(ppi, ppi);
                image.Dispose();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return newImage;
        }
        private Task<Bitmap> GetBitMap(string fileInfo, float maxWidth, float maxHeight)
        {
            return Task.Run(() => BT(fileInfo, maxWidth, maxHeight));
        }
        private async Task<Bitmap> GetData(string fileInfo, float maxWidth, float maxHeight)
        {
            return await GetBitMap(fileInfo, maxWidth, maxHeight);
        }
        private  int CallRest(int currentCounter ,string fileInfo, string saveTo, long quality, float maxWidth, float maxHeight)
        {
            //image = Image.FromFile(fileInfo);

            int counter = -1;
            Bitmap newImage =   BT(fileInfo, maxWidth, maxHeight);
            if (newImage != null)
            {
                try
                {
                    ImageFormat imageFormat = ImageFormat.Jpeg;

                    if (string.IsNullOrEmpty(imgExtension))
                    {
                        imageFormat = ImageFormat.Jpeg;
                        imgExtension = ".jpg";
                    }

                    else if (!string.IsNullOrEmpty(imgExtension) && (imgExtension.ToLower() == ".jpg" || imgExtension.ToLower() == ".jpeg"))
                    {

                        imageFormat = ImageFormat.Jpeg;
                    }
                    else if (!string.IsNullOrEmpty(imgExtension) && imgExtension.ToLower() == ".png")
                    {

                        imageFormat = ImageFormat.Png;
                    }
                    // Get an ImageCodecInfo object that represents the JPEG codec.
                    ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(imageFormat);

                    // Create an Encoder object for the Quality parameter.
                    var encoder = System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object. 
                    EncoderParameters encoderParameters = new EncoderParameters(1);

                    // Save the image as a JPEG file with quality level.
                    EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
                    encoderParameters.Param[0] = encoderParameter;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("Exported_");
                    stringBuilder.Append(newImage.Width);
                    stringBuilder.Append("X");
                    stringBuilder.Append(newImage.Height);

                    string currentResolutionFolder = stringBuilder.ToString();
                    string folderPath = saveTo + "//" + currentResolutionFolder;
                    saveTo = folderPath + "//" + DefaultExportImageName + Guid.NewGuid() + imgExtension;
                    bool doesFolderExist = Directory.Exists(folderPath);
                    if (!doesFolderExist) { Directory.CreateDirectory(folderPath); }
                    if (Directory.Exists(folderPath))
                    {
                        newImage.Save(saveTo, imageCodecInfo, encoderParameters);
                    }
                    bool isSaved = File.Exists(saveTo);


                    //progressReport.Progressing(total, ref ProgressIndex, ref ProgressCompleteGlobal);
                    // image.Dispose();

                    // GC.Collect();

                    if (isSaved)
                    {
                        Thread.Sleep(2);
                        counter = ++currentCounter;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return counter;
        }
        private  int ResizeImageToSpecificSize(int currentCounter,string fileInfo,string saveTo,long quality, float maxWidth ,float maxHeight)
        {
          int counter=   CallRest(currentCounter, fileInfo, saveTo, quality, maxWidth, maxHeight);

            return counter;
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GetUserDefinedImageName();
                progressReport = new ProgressReport();

                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                DialogResult result = folderBrowserDialog.ShowDialog();
                label1.Text = "0%";
                if (result == DialogResult.OK)
                {
                    listBox1.Items.Clear();
                    //
                    // The user selected a folder and pressed the OK button.
                    // We print the number of files found.
                    //
                    files = Directory.GetFiles(folderBrowserDialog.SelectedPath);

                    // CallFromTask();
                    CallFromTaskTestWorker();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void CallFromTaskTestWorker()
        {
            try
            {
                ProgressIndex = 0;
                foreach (var f in files)
                {

                    FileInfo fileInfo = new FileInfo(f);
                    string ext = fileInfo.Extension.ToLower();

                    if (ext == ".jpg" || ext == ".png")
                    {


                        listBox1.Items.Add(f);


                    }
                }

                if (listBox1.Items.Count > 0)
                {



                    // lable1 is for percentage
                    //progressbar for progress of converting

                    List<Models.Resolution> results = JsonWriteLoad.LoadJson(_fileName);
                    total = listBox1.Items.Count * results.Count();

                    ProgressCompleteGlobal = 0f;

                    progressBar1.Maximum = total * 100 / total + 1;


                    label4.Text = "Total selected images " + listBox1.Items.Count.ToString() + "  Images Produced  : " + total;




                    if (!worker.IsBusy)
                    {
                        worker.RunWorkerAsync();
                    }



                }
                else
                {
                    string message = " Images not found please try again !";
                    listBox1.Items.Add(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CallFromTask()
        {
            /*
            foreach (var f in files)
            {

                FileInfo fileInfo = new FileInfo(f);
                string ext = fileInfo.Extension.ToLower();

                if (ext == ".jpg" || ext == ".png")
                {


                    listBox1.Items.Add(f);


                }
            }

            if (listBox1.Items.Count > 0)
            {



                // lable1 is for percentage
                //progressbar for progress of converting
                List<Models.Resolution> results = JsonWriteLoad.LoadJson(_fileName);
                total = listBox1.Items.Count * results.Count();
               
                ProgressIndex = 0;
                ProgressCompleteGlobal = 0f;
                
                progressBar1.Maximum = total + 1;
            
                label4.Text = "Total number of images "+ listBox1.Items.Count.ToString()  + "  Images Produced  : "+ total;

                Thread thread1 = new Thread(() => {
                    for (int v =0; v< listBox1.Items.Count/2;v++)
                    {
                        totalSectionCounts++;
                        for (int res =0,res2=results.Count()-1;res<results.Count();res++,res2--)
                        {
                            totalSectionCounts++;
                            

                            //quality level from  0L to 100L type is long 
                            if (res2 < res) { break; }
                            else
                            {
                                ResizeImageToSpecificSize(listBox1.Items[v].ToString(), DestinationToSave, ImageQuality, results[res].Width, results[res].Height);
                                ResizeImageToSpecificSize(listBox1.Items[v].ToString(), DestinationToSave, ImageQuality, results[res2].Width, results[res2].Height);
                               
                            }
                            //progressbar
                           
                            //ActionFinal();
                        }

                        progressReport.Progressing(total, ref ProgressIndex, ref ProgressCompleteGlobal);
                        //progressbar  
                        

                        //ActionFinal();



                    }
                });
                thread1.IsBackground = true;
                thread1.Name = "THreadBackgroundV1";
                thread1.Start();
                
              
                Thread thread2 = new Thread(() => {
                    for (int v = (listBox1.Items.Count / 2); v < listBox1.Items.Count; v++)
                    {
                        totalSectionCounts++;

                        for (int res = 0, res2 = results.Count() - 1; res < results.Count(); res++, res2--)
                        {
                            totalSectionCounts++;

                            

                            //quality level from  0L to 100L type is long 
                            if (res2 < res) { break; }
                            else
                            {
                                ResizeImageToSpecificSize(listBox1.Items[v].ToString(), DestinationToSave, ImageQuality, results[res].Width, results[res].Height);
                                ResizeImageToSpecificSize(listBox1.Items[v].ToString(), DestinationToSave, ImageQuality, results[res2].Width, results[res2].Height);
                                
                            }
                            //progressbar
                            
                            //ActionFinal();
                        }

                        progressReport.Progressing(total, ref ProgressIndex, ref ProgressCompleteGlobal);
                        //progressbar  
                        
                        //ActionFinal();



                    }
                });
                thread2.IsBackground = true;
                thread2.Name = "THreadBackgroundV2";
                thread2.Start();
               




            }
            else
            {
                string message = " Images not found please try again !";
                listBox1.Items.Add(message);
            }
            */
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    DestinationToSave = folderBrowserDialog.SelectedPath.Replace("\\", "//");

                    if (!string.IsNullOrEmpty(DestinationToSave))
                    {
                        button1.Enabled = true;
                    }
                    else
                    {
                        button1.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            imgExtension="."+comboBox1.SelectedItem.ToString().ToLower();
         
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Leave(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
            

        }

        private void resolutionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Resolutions resolutions = new Resolutions();
            resolutions.ShowDialog();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ImageQuality =(long) trackBar1.Value;
            DelegateInvoke2(ImageQuality);
        }
        private void DelegateInvoke2(long imageQuality)
        {
            delUIUpdate2 delUIUpdate = new delUIUpdate2(UiUpdateMethod2);

            textBox2.BeginInvoke(delUIUpdate, imageQuality);
        }
        private void UiUpdateMethod2(long imageQuality)
        {
            textBox2.Text = Convert.ToString( imageQuality);
        }
        private void DelegateInvoke3(long imageQuality)
        {
            delUIUpdate2 delUIUpdate = new delUIUpdate2(UiUpdateMethod3);

            trackBar1.BeginInvoke(delUIUpdate, imageQuality);
        }
        private void UiUpdateMethod3(long imageQuality)
        {
            trackBar1.Value = Convert.ToInt32(imageQuality);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string destinationFolder = Path.GetDirectoryName(listBox1.SelectedItem.ToString());
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = destinationFolder;
                openFileDialog.ShowDialog();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                float.TryParse(textBox1.Text, out ppi);
                if (ppi < 0)
                {

                    ppi = 150f;
                    textBox1.Text = ppi.ToString();
                    MessageBox.Show("PPI Can only be between 0 and 999 !");
                }
                else if (ppi > 999f)
                {
                    ppi = 999f;
                    textBox1.Text = ppi.ToString();
                    MessageBox.Show("PPI Can only be between 0 and 999 !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
      
        void ActionFinal()
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {

                progressBar1.Value = totalSectionCounts;


            }
            label1.Text = totalSectionCounts * 100/total  + " %";

        }
        

       

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            long val = 0;
            long.TryParse(textBox2.Text, out val);
            DelegateInvoke3(val);
        }

        private void DelegateInvokeLoad(string fileName)
        {
            delUIUpdate delUIUpdate = new delUIUpdate(UiUpdateMethodLoad);

            textBox1.BeginInvoke(delUIUpdate,fileName);
            comboBox1.BeginInvoke(delUIUpdate, fileName);
            trackBar1.BeginInvoke(delUIUpdate, fileName);
        }
        private void UiUpdateMethodLoad(string fileName)
        {
            LoadImages(fileName);
        }
        private void LoadImages(string fileName)
        {
            try
            {
                Settings settings = new Settings();
                settings = JsonWriteLoad.LoadJsonSettings(fileName);
                textBox1.Text = Convert.ToString(settings.PPI);
                comboBox1.Text = Convert.ToString(settings.ImageType);
                trackBar1.Value = (int)settings.Quality;
                checkBox1.Checked = settings.PreserveAspectRatio;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void resetSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DelegateInvokeLoad(@"Default_Settings.json");
                MessageBox.Show("Settings reset successfully, please press save to preserve changes!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Settings settings = new Settings();
                float ppi = 150;
                float.TryParse(textBox1.Text, out ppi);
                string defJPG = null;
                if (comboBox1.SelectedItem == null)
                {
                    defJPG = ".jpg";
                }
                else
                {
                    defJPG = comboBox1.SelectedItem.ToString();
                }
                settings.Initialize(ppi, trackBar1.Value, checkBox1.Checked, defJPG);
                JsonWriteLoad.WriteJsonSettings(@"Saved_Settings.json", settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DelegateInvokeLoad(@"Saved_Settings.json");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ExportSettings exportSettings = new ExportSettings();
            exportSettings.ShowDialog();
            exportSettings.Dispose();
        }
    }
}
