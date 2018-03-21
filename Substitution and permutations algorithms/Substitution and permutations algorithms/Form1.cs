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

namespace Substitution_and_permutations_algorithms
{
    enum operationTypeEnum { Encryption = 0, Decryption = 1, Unknown = 2 }    
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        static private byte[] currentFile;
        static operationTypeEnum currentState = operationTypeEnum.Unknown;

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
        private void deAndenCryption(byte[] source, operationTypeEnum operationType)
        {
            if (operationType == operationTypeEnum.Encryption)
            {
                buttonEncryption.Enabled = false;
                buttonDecryption.Enabled = true;
            }
            else if (operationType == operationTypeEnum.Decryption)
            {                
                buttonEncryption.Enabled = true;
                buttonDecryption.Enabled = false;
            }

            byte[] key = Encoding.UTF8.GetBytes(textBoxKey.Text);//key
            ushort[] numKey = new ushort[key.Length];//number key
            ushort keyLength = (ushort)key.Length;//Counter
            ushort maxCharVal = 0;//Max char value
            ushort ind = 0;//current index
            /*string key to number key*/
            for (ushort i = 0; i < key.Length; i++)
            {
                for (ushort j = 0; j < key.Length; j++)
                {
                    if (maxCharVal < key[j] && numKey[j] == 0)
                    {
                        maxCharVal = key[j];
                        ind = j;
                    }
                }
                numKey[ind] = keyLength--;
                maxCharVal = 0;
                ind = 0;
            }
            /*encryption*/
            byte[] result = new byte[source.Length];
            uint sizeEncrpttion = (uint)((source.Length / numKey.Length) * numKey.Length);
            uint remainder = (uint)(source.Length - sizeEncrpttion);
            for (uint i = 0; i < sizeEncrpttion; i += (uint)numKey.Length)
            {
                for (uint j = 0; j < numKey.Length; j++)
                {
                    if(operationType == operationTypeEnum.Encryption)
                        result[i + j] = source[i + numKey[j] - 1];
                    else if(operationType == operationTypeEnum.Decryption)
                        result[i + numKey[j] - 1] = source[i + j];
                }
            }
            /*end of the array without encryption*/
            Array.Copy(source, sizeEncrpttion, result, sizeEncrpttion, remainder);
            /*Show to richEditor*/
            currentFile = result;
            richTextBoxFile.Text = Encoding.UTF8.GetString(result);
        }
        /*
        Processing buttonEncryption
        */
        private void buttonEncryption_Click(object sender, EventArgs e)
        {
            byte[] source = Encoding.UTF8.GetBytes(richTextBoxFile.Text);//source
            deAndenCryption(source, operationTypeEnum.Encryption);            
        }
        /*
        Processing buttonDecryption
        */
        private void buttonDecryption_Click(object sender, EventArgs e)
        {
            deAndenCryption(currentFile, operationTypeEnum.Decryption);            
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
