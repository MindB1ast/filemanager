using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form2 : Form
    {
        Form1 Form;
        public Form2(Form1 creator,ListViewItem[] items)
        {
            InitializeComponent();
            Form = creator;
            listView1.Items.AddRange(items);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string link = listView1.SelectedItems[0].SubItems[6].Text;


            if (listView1.SelectedItems[0].SubItems[1].Text == "")
            {

                

                
               Form.GoLink(link);
                Form.ChangeLinkText(link);
                Form.LinkBefore = link;
            }
            else
            {

                var p = new Process();
                p.StartInfo = new ProcessStartInfo(link)
                {
                    UseShellExecute = true
                };
                p.Start();// System.Diagnostics.Process.Start(link); 
               
               // Form.ChangeLinkText(link.Remove(link.LastIndexOf(@"\")));
            }
            //Form.GoLink(listView1.SelectedItems[0].SubItems[6].Text);
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
