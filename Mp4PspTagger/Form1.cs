// Mp4PspTagger application.  This is a Windows UI example of how to use the MSVTagger DLL component.
// v0.11 written by Jason Frazier.  This is freeware.
//
// "Memory Stick" is a registered trademark of Sony Corporation.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using MsvTagger;

namespace Form1
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItemFile;
        private System.Windows.Forms.MenuItem menuItemOpenFile;
        private System.Windows.Forms.MenuItem menuItemExit;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.Label labelOpenFile;
        private System.Windows.Forms.TextBox textBoxUSMTName;
        private System.Windows.Forms.Button buttonReadTags;
        private System.Windows.Forms.Button buttonUpdateTags;
        private System.Windows.Forms.TextBox textBoxUSMTDate;
        private System.Windows.Forms.TextBox textBoxUSMTEncoder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSegmentsFound;
        private System.Windows.Forms.Label label4;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItemFile = new System.Windows.Forms.MenuItem();
            this.menuItemOpenFile = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.labelOpenFile = new System.Windows.Forms.Label();
            this.textBoxUSMTName = new System.Windows.Forms.TextBox();
            this.buttonReadTags = new System.Windows.Forms.Button();
            this.buttonUpdateTags = new System.Windows.Forms.Button();
            this.textBoxUSMTDate = new System.Windows.Forms.TextBox();
            this.textBoxUSMTEncoder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSegmentsFound = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.menuItemFile,
                                                                                      this.menuItem1});
            // 
            // menuItemFile
            // 
            this.menuItemFile.Index = 0;
            this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                         this.menuItemOpenFile,
                                                                                         this.menuItemExit});
            this.menuItemFile.Text = "File";
            // 
            // menuItemOpenFile
            // 
            this.menuItemOpenFile.Index = 0;
            this.menuItemOpenFile.Text = "Open...";
            this.menuItemOpenFile.Click += new System.EventHandler(this.menuItemOpenFile_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Index = 1;
            this.menuItemExit.Text = "Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 1;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.menuItem2});
            this.menuItem1.Text = "Help";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "About...";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(8, 16);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(360, 22);
            this.textBoxFileName.TabIndex = 0;
            this.textBoxFileName.Text = "";
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Location = new System.Drawing.Point(368, 16);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(24, 23);
            this.buttonOpenFile.TabIndex = 1;
            this.buttonOpenFile.Text = "...";
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // labelOpenFile
            // 
            this.labelOpenFile.Location = new System.Drawing.Point(8, 0);
            this.labelOpenFile.Name = "labelOpenFile";
            this.labelOpenFile.Size = new System.Drawing.Size(200, 16);
            this.labelOpenFile.TabIndex = 2;
            this.labelOpenFile.Text = "Select File:";
            // 
            // textBoxUSMTName
            // 
            this.textBoxUSMTName.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxUSMTName.Location = new System.Drawing.Point(8, 96);
            this.textBoxUSMTName.Name = "textBoxUSMTName";
            this.textBoxUSMTName.ReadOnly = true;
            this.textBoxUSMTName.Size = new System.Drawing.Size(384, 22);
            this.textBoxUSMTName.TabIndex = 3;
            this.textBoxUSMTName.Text = "";
            // 
            // buttonReadTags
            // 
            this.buttonReadTags.Location = new System.Drawing.Point(8, 40);
            this.buttonReadTags.Name = "buttonReadTags";
            this.buttonReadTags.Size = new System.Drawing.Size(80, 23);
            this.buttonReadTags.TabIndex = 4;
            this.buttonReadTags.Text = "Read Tags";
            this.buttonReadTags.Click += new System.EventHandler(this.buttonReadTags_Click);
            // 
            // buttonUpdateTags
            // 
            this.buttonUpdateTags.Location = new System.Drawing.Point(96, 40);
            this.buttonUpdateTags.Name = "buttonUpdateTags";
            this.buttonUpdateTags.Size = new System.Drawing.Size(96, 23);
            this.buttonUpdateTags.TabIndex = 5;
            this.buttonUpdateTags.Text = "Update Tags";
            this.buttonUpdateTags.Click += new System.EventHandler(this.buttonUpdateTags_Click);
            // 
            // textBoxUSMTDate
            // 
            this.textBoxUSMTDate.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxUSMTDate.Location = new System.Drawing.Point(8, 138);
            this.textBoxUSMTDate.Name = "textBoxUSMTDate";
            this.textBoxUSMTDate.ReadOnly = true;
            this.textBoxUSMTDate.Size = new System.Drawing.Size(384, 22);
            this.textBoxUSMTDate.TabIndex = 6;
            this.textBoxUSMTDate.Text = "";
            // 
            // textBoxUSMTEncoder
            // 
            this.textBoxUSMTEncoder.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxUSMTEncoder.Location = new System.Drawing.Point(8, 181);
            this.textBoxUSMTEncoder.Name = "textBoxUSMTEncoder";
            this.textBoxUSMTEncoder.ReadOnly = true;
            this.textBoxUSMTEncoder.Size = new System.Drawing.Size(384, 22);
            this.textBoxUSMTEncoder.TabIndex = 6;
            this.textBoxUSMTEncoder.Text = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Title:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Date created:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Encoder used:";
            // 
            // textBoxSegmentsFound
            // 
            this.textBoxSegmentsFound.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxSegmentsFound.Location = new System.Drawing.Point(8, 224);
            this.textBoxSegmentsFound.Name = "textBoxSegmentsFound";
            this.textBoxSegmentsFound.ReadOnly = true;
            this.textBoxSegmentsFound.Size = new System.Drawing.Size(384, 22);
            this.textBoxSegmentsFound.TabIndex = 6;
            this.textBoxSegmentsFound.Text = "";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Data segments found:";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(400, 262);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxUSMTDate);
            this.Controls.Add(this.buttonUpdateTags);
            this.Controls.Add(this.buttonReadTags);
            this.Controls.Add(this.textBoxUSMTName);
            this.Controls.Add(this.labelOpenFile);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.textBoxUSMTEncoder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxSegmentsFound);
            this.Controls.Add(this.label4);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Memory Stick Video Tagger";
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

        private void readTags()
        {
            MsvTags mp4 = new MsvTags(this.textBoxFileName.Text);
            // get title
            try
            {
                textBoxUSMTName.Text = mp4.GetTagTitle();
                textBoxUSMTName.ReadOnly = false;
            }
            catch (MsvTagNotFoundException caught)
            {
                textBoxUSMTName.Text = "<" + caught.Message.ToUpper() + ">";
                textBoxUSMTName.ReadOnly = true;
            }
            catch (Exception caught){textBoxUSMTName.Text = "<" + caught.Message.ToUpper() + ">";}
            // get date
            try
            { textBoxUSMTDate.Text = mp4.GetTagDate(); }
            catch (MsvTagNotFoundException caught){textBoxUSMTDate.Text = "<" + caught.Message.ToUpper() + ">";}
            catch (Exception caught){textBoxUSMTDate.Text = "<" + caught.Message.ToUpper() + ">";}
            // get encoder
            try 
            {
                textBoxUSMTEncoder.Text = mp4.GetTagEncoder();
                textBoxUSMTEncoder.ReadOnly = false;
            }
            catch (MsvTagNotFoundException caught)
            {
                textBoxUSMTEncoder.Text = "<" + caught.Message.ToUpper() + ">";
                textBoxUSMTEncoder.ReadOnly = true;
            }
            catch (Exception caught){textBoxUSMTEncoder.Text = "<" + caught.Message.ToUpper() + ">";}
            // get MSV data segment list
            try {textBoxSegmentsFound.Text = mp4.GetTagSegmentListString();}
            catch (Exception caught){textBoxSegmentsFound.Text = "<" + caught.Message.ToUpper() + ">";}
            // finished with MP4, must dispose to free file handle
            // THIS IS REQUIRED, or you will be unable to later open your file until mp4 is disposed!!!
            mp4.Dispose();
        }

        private void updateTags()
        {
            MsvTags mp4 = new MsvTags(this.textBoxFileName.Text);
            if (!textBoxUSMTName.ReadOnly)
                mp4.SetTagTitle(textBoxUSMTName.Text);
            if (!textBoxUSMTEncoder.ReadOnly)
                mp4.SetTagEncoder(textBoxUSMTEncoder.Text);
            // finished with MP4, must dispose to free file handle
            mp4.Dispose();
            readTags();
        }

        private void showFileChooser()
        {            
            if (System.IO.Directory.Exists(initPath + "\\MP_ROOT"))
            {
                initPath += "\\MP_ROOT";
                if (System.IO.Directory.Exists(initPath + "\\100MNV01"))
                    initPath += "\\100MNV01";
            }
            System.Windows.Forms.OpenFileDialog getInputFile = new System.Windows.Forms.OpenFileDialog();
            getInputFile.Filter = "Memory Stick Video (.MP4) Files|*.mp4";
            getInputFile.Title = "Select a Memory Stick Video File";
            getInputFile.InitialDirectory = initPath;
            if(getInputFile.ShowDialog() == DialogResult.OK)
            {
                textBoxFileName.Text = getInputFile.FileName;
                int dirCount = getInputFile.FileName.IndexOf('\\');
                initPath = getInputFile.FileName.Substring(0, getInputFile.FileName.LastIndexOf('\\'));
            }
        }

        private void buttonOpenFile_Click(object sender, System.EventArgs e)
        {
            showFileChooser();
        }
        
        private void buttonReadTags_Click(object sender, System.EventArgs e)
        {
            if (textBoxFileName.Text != "")
            {
                readTags();
            }
        }

        private void menuItem2_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Memory Stick Video Tagger v0.11\r\n\r\n" + 
                "Copyright © 2005 Jason Frazier.  This is freeware.\r\n\r\n" + 
                "\"Memory Stick\" is a registered trademark of Sony Corporation.");        
        }

        private void menuItemExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void menuItemOpenFile_Click(object sender, System.EventArgs e)
        {
            showFileChooser();
            if (textBoxFileName.Text != "")
            {
                readTags();
            }
        }

        string initPath = System.Environment.GetEnvironmentVariable("HOMEDRIVE");

        private void buttonUpdateTags_Click(object sender, System.EventArgs e)
        {
            this.updateTags();
        }
	}
}
