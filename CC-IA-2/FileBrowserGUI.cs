using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CC_IA_2
{
    public partial class FileBrowserGUI : Form
    {
        static void main()
        {
            Application.Run(new FileBrowserGUI());
            var guiForm = new FileBrowserGUI();
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                }
            }
            guiForm.ShowDialog();//This "opens" the GUI on your screen
        }
        public FileBrowserGUI()
        {
            InitializeComponent();
        }

        
    }
}
