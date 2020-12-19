using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CC_IA_2
{
    public partial class Program: Form
    {
        private FolderBrowserDialog folderBrowserDialog1;
        private Button button1;
        private Label label1;

        public FolderBrowserDialog FolderBrowserDialog1 { get => folderBrowserDialog1; set => folderBrowserDialog1 = value; }

        public Program()
        {
            InitializeComponent();
        }
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
            //string connection = ConfigurationManager.AppSettings.Get("ConnectionString");
            
        }

        public static async void upload_to_azure(string ConnectionString,string selectedpath)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);
                ////Create a unique name for the container
                string containerName = "cccontainer";//"quickstartblobs" + Guid.NewGuid().ToString();
                CloudBlobClient client = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference(containerName);
                var isCreated = container.CreateIfNotExists();
                container.SetPermissionsAsync(new BlobContainerPermissions
                { PublicAccess = BlobContainerPublicAccessType.Blob });

                string localPath = selectedpath;//"G:/My Drive/KJSCE/CC/IA-2";

            //----------------CODE FOR UPLOADING MULTIPLE FILES ON CONTAINER-----------------
            DirectoryInfo dir = new DirectoryInfo(localPath);

            foreach (FileInfo flInfo in dir.GetFiles())
            {
                string name = flInfo.Name;
                string localFilePath = Path.Combine(localPath, name);
                long size = flInfo.Length;
                DateTime creationTime = flInfo.CreationTime;
                Console.WriteLine("{0, -30:g} {1,-12:N0} {2} ", name, size, creationTime);
                CloudBlockBlob blob = container.GetBlockBlobReference(name);
                FileStream uploadFileStream = File.OpenRead(localFilePath);
                blob.UploadFromStream(uploadFileStream);
                uploadFileStream.Close();
                WriteToFile(name +" Uploaded to Cloud");
            }
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                WriteToFile("Error Message: " + ex.Message);
            }

        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\Logfile" + ".txt";
            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine(Message);
            }

        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(26, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Browse Folder Path";
            // 
            // folderBrowserDialog1
            // 
            this.FolderBrowserDialog1.HelpRequest += new System.EventHandler(this.folderBrowserDialog1_HelpRequest);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(270, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(423, 158);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "Program";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Clicked","Message",MessageBoxButtons.OK,MessageBoxIcon.Information);
            string selectedPath;
            var t = new Thread((ThreadStart)(() => {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
                fbd.ShowNewFolderButton = true;
                if (fbd.ShowDialog() == DialogResult.Cancel)
                    return;

                selectedPath = fbd.SelectedPath;
                string connection = ConfigurationManager.AppSettings.Get("ConnectionString");
                upload_to_azure(connection, selectedPath);
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();


        }
    }

}
