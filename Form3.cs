using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form3 : Form
    {
        static Form1 form1;
        string link;
        public Form3(Form1 form,string link)
        {
            
            InitializeComponent();
            label1.Text = "Rename file " +Path.GetFileName(link);//для надписи о названии изменяемого файла
             form1 = form;
            this.link = link;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            Rename(link,textBox1.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public static void Rename(string link, string newName)
        {
           
             if (!File.Exists(Path.GetDirectoryName(link) + @"\" + newName) && !Directory.Exists(Path.GetDirectoryName(link) + @"\" + newName))
             {

                
                    if (newName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                    {
                    if (Path.GetExtension(link) != "")
                    {
                        FileInfo fileInfo = new FileInfo(link);
                        fileInfo.MoveTo(fileInfo.Directory.FullName + @"\" + newName+fileInfo.Extension);

                        
                        form1.GoLink(fileInfo.Directory.FullName);
                    }
                    else 
                    {
                        
                        DirectoryInfo directoryInfo= new DirectoryInfo(link);
                       
                        directoryInfo.MoveTo(directoryInfo.FullName.Remove(directoryInfo.FullName.LastIndexOf(@"\"))+@"\"+newName);
                        form1.GoLink(directoryInfo.Parent.ToString());

                        
                    }
                    }
                    else 
                { MessageBox.Show("The file name contains forbidden characters"); }
                

             }
            else MessageBox.Show("File with this name already exist");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
           
        }
    }
}
