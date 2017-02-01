using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using ServerClient;
namespace httpMethodsApp
{
    public enum encodingType { Origin, TwoBitEncoding, HuffmanWithSampling, HuffmanWithoutSampling, Huffman };
    public partial class MainForm : Form
    {
        private string filesDirectory = "";
        private HttpServerController httpServerController;
        private bool useStandardHeaders = true;
        

        public MainForm()
        {
            InitializeComponent();
            
        }


        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();


            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.filesDirectory = folderBrowser.SelectedPath;
                if (httpServerController != null)
                {
                    httpServerController._httpServer.includingPath = this.filesDirectory;
                }

            }
        }

      
        private void MainForm_Load(object sender, EventArgs e)
        {
            setUPServer();

        }
        private void setUPServer()
        {
            
            httpServerController = new HttpServerController(AppSettings.Port);
            httpServerController._httpServer.useStandardHeaders = this.useStandardHeaders;

            if (filesDirectory != "")
            {
                httpServerController._httpServer.includingPath = Path.GetDirectoryName(filesDirectory);
            }
        }
        private void btnStartServer_Click(object sender, EventArgs e)
        {

            if (btnStartServer.Text == "Start Server")
            {

                if (filesDirectory != "")
                {
                    httpServerController._httpServer.includingPath = filesDirectory;
                }
                httpServerController._httpServer.startedListeningCallback = new StartedListening(ServerhasStarted);

                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                httpServerController.start();


            }
            else
            {
                httpServerController.stop();
                btnStartServer.Text = "Start Server";

                MessageBox.Show("The Server has stopped");


            }



        }
        private void ServerhasStarted(bool started, String errorMessage)
        {
            if (InvokeRequired)
            {


                this.Invoke((MethodInvoker)delegate()
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;

                    if (started)
                    {
                        btnStartServer.Text = "Stop Server";

                        MessageBox.Show("The Server has started");
                    }
                    else
                    {
                        MessageBox.Show(errorMessage);
                    }

                });
            }
        }
        
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (httpServerController != null)
            {
                httpServerController.stop();
            }
        }


        private void rdStandardHeaders_CheckedChanged(object sender, EventArgs e)
        {
            if( rdStandardHeaders.Checked ) useStandardHeaders = true;
        }

        private void rdModifiedHeaders_CheckedChanged(object sender, EventArgs e)
        {
            if (rdModifiedHeaders.Checked) useStandardHeaders = false;
        }


       

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettingsForm appSettingForm = new AppSettingsForm();
            if (appSettingForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                httpServerController._httpServer.PortNumber = AppSettings.Port;
            }
        }

       

       





       


        
    }
}




