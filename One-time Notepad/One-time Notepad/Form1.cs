using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace One_time_Notepad
{
    enum fileTypeEnum { sourceFile = 1, oneTimeNotepad = 2 }
    enum operationTypeEnum { Encryption = 0, Decryption = 1, Unknown = 2 }
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        static private byte[] currentFile;
        static private byte[] oneTimeNotepad;

        /*
        OPEN FILE 
        */
        private void openFile(fileTypeEnum fileType)
        {
            Stream fileStream = null;
            if (openFileDialogFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((fileStream = openFileDialogFile.OpenFile()) != null)
                    {
                        using (fileStream)
                        {

                            BinaryReader binaryReader = new BinaryReader(fileStream);

                            if (fileType == fileTypeEnum.sourceFile)
                            {
                                currentFile = binaryReader.ReadBytes((int)fileStream.Length);
                                richTextBoxFile.Text = Encoding.UTF8.GetString(currentFile);
                            }
                            else if (fileType == fileTypeEnum.oneTimeNotepad)
                            {
                                oneTimeNotepad = binaryReader.ReadBytes((int)fileStream.Length);
                                buttonEncryption.Enabled = true;
                                buttonDecryption.Enabled = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        /*Open a source file*/
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            openFile(fileTypeEnum.sourceFile);
        }
        /*Open an one time notepad*/
        private void buttonOpenNotepad_Click(object sender, EventArgs e)
        {
            openFile(fileTypeEnum.oneTimeNotepad);
        }
        /*
        SAVE FILE 
        */
        private void buttonSave_Click(object sender, EventArgs e)
        {
            Stream fileStream;
            if (saveFileDialogFile.ShowDialog() == DialogResult.OK)
            {
                if ((fileStream = saveFileDialogFile.OpenFile()) != null)
                {                    
                    StreamWriter writer = new StreamWriter(fileStream);
                    if(currentFile == null || currentFile.Length == 0)
                        writer.Write(richTextBoxFile.Text);
                    else
                        fileStream.Write(currentFile, 0, currentFile.Length);
                    writer.Flush();
                    fileStream.Close();
                }
            }
        }
        /*
        Algorithm decryption and encryption
        */
        private void deAndenCryption(operationTypeEnum operationType, byte[] source)
        {
            /*
            Warning about wrong a one-time notepad 
            */
            if ((oneTimeNotepad.Length != currentFile.Length) && operationType == operationTypeEnum.Encryption)
            {
                DialogResult result;
                // Displays the MessageBox.
                result = MessageBox.Show(
                    this,
                    "The length of a one-time notepad does not equal the source file. Do you want to continue this encryption?",
                    "Warning", 
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, 
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                    );
                if (result == DialogResult.No)
                    return;
            }
                    
            byte[] newSource = new byte[source.Length];
            for (int i = 0, j = 0; i < source.Length; i++, j++)
            {
                if (j < oneTimeNotepad.Length)
                    j = 0;
                newSource[i] = (byte)(source[i] ^ oneTimeNotepad[j]);
            }
            currentFile = newSource;
            richTextBoxFile.Text = Encoding.UTF8.GetString(newSource);
        }
        /*
        Processing buttonEncryption
        */
        private void buttonEncryption_Click(object sender, EventArgs e)
        {
            deAndenCryption(operationTypeEnum.Encryption, Encoding.UTF8.GetBytes(richTextBoxFile.Text));
        }
        /*
        Processing buttonDecryption
        */
        private void buttonDecryption_Click(object sender, EventArgs e)
        {            
            deAndenCryption(operationTypeEnum.Decryption, currentFile);
        }
        /*
        Processing buttonCancel
        */
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
