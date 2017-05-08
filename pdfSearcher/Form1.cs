using System;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace pdfSearcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ListBox getSubfolders(string path)
        {
            ListBox folderList = new ListBox();
            try
            {
                foreach (string item in Directory.GetDirectories(path))
                {

                    folderList.Items.AddRange(getSubfolders(item).Items);
                }
            }
            catch (Exception)
            {
                
            }
                
            

            
            return folderList;
        }

        ListBox findAllPdfsInFolder(string folderPath)
        {

            ListBox list = new ListBox();
            try
            {
                foreach (string item in Directory.GetFiles(folderPath))
                {
                    if (item.EndsWith(".pdf"))
                    {
                        list.Items.Add(item);
                    }

                }
            }
            catch (Exception)
            {
            }
            
            
            //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            return list;
        }

        ListBox findAllPdfsIncludesSubfolders(string foldername)
        {
            ListBox listbox = new ListBox();
            ListBox temp = new ListBox();
            temp.Items.AddRange(findAllPdfsInFolder(foldername).Items);
            foreach (string item in getSubfolders(foldername).Items)
            {
                if (Directory.Exists(item))
                {
                    temp.Items.AddRange(findAllPdfsInFolder(item).Items);
                }
            }
            listbox.Items.AddRange(temp.Items);
            return listbox;
        }

        ListBox searchWordinPdfFile(string fileName,string word)
        {
            ListBox list = new ListBox();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages/*pdfReader.NumberOfPages*/; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    if (currentText.ToLower().Contains(word.ToLower()))
                    {
                        list.Items.Add(fileName + ";" + page);
                    }
                    
                }
                pdfReader.Close();
            }
            return list;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            StringBuilder text = new StringBuilder();
            string foldername = textBox2.Text;

            foreach (string item in findAllPdfsIncludesSubfolders(foldername).Items)
            {
                listBox1.Items.AddRange(searchWordinPdfFile(item, textBox1.Text).Items);
            }    
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string foldername = (dialog.ShowDialog() == DialogResult.OK) ? dialog.SelectedPath : "";
            if (foldername!="")
            {
                textBox2.Text = foldername;
            }
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem!=null)
            {
                String path= listBox1.SelectedItem.ToString().Split(';')[0];
                try
                {
                    string page = listBox1.SelectedItem.ToString().Split(';')[1];
                    Process myProcess = new Process();
                    myProcess.StartInfo.FileName = @"C:\Program Files (x86)\Adobe\Reader 11.0\Reader\AcroRd32.exe";
                    myProcess.StartInfo.Arguments = "/A \"page="+page+"\" \""+path+"\"";
                    myProcess.Start();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }
    }
}
