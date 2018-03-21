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

namespace XOR_algorithm
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        static private byte[] currentFile;        

        /*
        OPEN FILE 
        */
        private void buttonOpen_Click(object sender, EventArgs e)
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
                            currentFile = binaryReader.ReadBytes( (int)fileStream.Length );

                            richTextBoxFile.Text = Encoding.UTF8.GetString(currentFile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
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
        private void deAndenCryption(byte[] source)
        {
            byte[] key = Encoding.UTF8.GetBytes(textBoxKey.Text);
            byte[] newSource = new byte[source.Length];
            for (int i = 0, j = 0; i < source.Length; i++, j++)
            {
                if (j < key.Length)
                    j = 0;
                newSource[i] = (byte)(source[i] ^ key[j]);
            }
            currentFile = newSource;
            richTextBoxFile.Text = Encoding.UTF8.GetString(newSource);
        }
        /*
        Processing buttonEncryption
        */
        private void buttonEncryption_Click(object sender, EventArgs e)
        {
            deAndenCryption(Encoding.UTF8.GetBytes(richTextBoxFile.Text));
        }
        /*
        Processing buttonDecryption
        */
        private void buttonDecryption_Click(object sender, EventArgs e)
        {            
            deAndenCryption(currentFile);
        }
        /*
        Processing buttonCancel
        */
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /*
        Processing textBoxKey
        event - TextChanged
        */
        private void textBoxKey_TextChanged(object sender, EventArgs e)
        {            
            try
            {
                /*
                check whether key is correct
                */
                Regex rgx = new Regex("^[A-Za-z0-9]*$");
                Match m = rgx.Match(textBoxKey.Text);
                if (!m.Success)
                    throw new Exception("Error! Use only A-Z, a-z, 0-9");
                /*
                Activate and deactivate buttonDecryption and buttonEncryption buttons
                Depending on the textBoxKey field
                */
                if (textBoxKey.Text.Length == 0)
                {
                    buttonDecryption.Enabled = false;
                    buttonEncryption.Enabled = false;
                }
                else
                {
                    buttonDecryption.Enabled = true;
                    buttonEncryption.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                textBoxKey.Undo();
                return;
            }
        }
    }
}
