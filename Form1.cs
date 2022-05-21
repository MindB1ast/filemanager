using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Security.Principal;
using System.Diagnostics;
using FastSearchLibrary;
using System.Threading.Tasks;
using System.Threading;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {

        // Глобальные переменые
      
        static string copied = "";//сылка на то что скопировано или вырезано
        public string LinkBefore = @"C:\";//нужна для того чтобы при отмене набора адреса возрощался старый 
        ToolStripMenuItem copy = new ToolStripMenuItem("Копировать");
        ToolStripMenuItem delete = new ToolStripMenuItem("Удалить");
        ToolStripMenuItem paste = new ToolStripMenuItem("Вставить");
        ToolStripMenuItem open = new ToolStripMenuItem("Открыть");
        ToolStripMenuItem cut = new ToolStripMenuItem("Вырезать");
        ToolStripMenuItem rename = new ToolStripMenuItem("Переименовать");
        static bool CopyOrCut = false;//False для copy true для cut
       
   

  
       public ListViewItem[] SearchResult(string where,string filename) 
        {

            
            
            List<DirectoryInfo> directories = FastSearchLibrary.DirectorySearcher.GetDirectoriesFast(where, "*"+filename+"*");
            List<FileInfo> files = FastSearchLibrary.FileSearcher.GetFilesFast(where,"*"+filename+"*");
           
            List<ListViewItem> result=new List<ListViewItem>();
            foreach (var directory in directories)
            {
                ListViewItem item = new ListViewItem(new string[] { directory.Name, directory.Extension.ToString(), "",GetOwner(directory.FullName), directory.LastWriteTime.ToString(), directory.CreationTime.ToString(),directory.FullName });
                result.Add(item);
                //MessageBox.Show(directory.Name);
                
                
            }

            foreach (var file in files)
            {
                string name = file.Name;
                if (name.Contains('.')) { name = name.Remove(name.LastIndexOf('.')); }
                ListViewItem item = new ListViewItem(new string[] { name, file.Extension.ToString(), (file.Length / 1024).ToString(),GetOwner(file.FullName), file.LastWriteTime.ToString(), file.CreationTime.ToString(),file.FullName });
                result.Add(item);
               
               
            }
           
            return result.ToArray();
            

        }
        

        public string GetOwner(string link)
        {

            try
            {
                FileInfo fileInfo = new FileInfo(link);
                var File_Security = fileInfo.GetAccessControl();
                var SID = File_Security.GetOwner(typeof(SecurityIdentifier));
                var Owner = SID.Translate(typeof(NTAccount));
                return Owner.Value;
            }
            catch
            {
                return "";
            }

        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            if (Path.GetExtension(sourceDirName) == "")
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();

                // If the destination directory doesn't exist, create it.       
                Directory.CreateDirectory(destDirName);

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                try
                {
                    foreach (FileInfo file in files)
                    {

                        string tempPath = Path.Combine(destDirName, file.Name);
                        file.CopyTo(tempPath, false);

                    }
                }
                catch { MessageBox.Show("В этой папке уже есть файл с таким именем"); }
                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string tempPath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                    }
                }
            }
            else
            {
                try
                {
                    File.Copy(sourceDirName, destDirName);
                }
                catch { MessageBox.Show("В этой дериктории уже есть файл с таким именем"); }

            }
        }
        public void GoLink(string link)
        {

            string way = link;

            listView1.Items.Clear();


            if (link != @"C:\" && link!=@"D:\") { listView1.Items.Add(new ListViewItem(new string[] { "<<<", " ", " " })); }
            string[] files = Directory.GetFiles(way);
            EnumerationOptions enumerationOptions = new EnumerationOptions();
            enumerationOptions.AttributesToSkip = FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReparsePoint;
            enumerationOptions.IgnoreInaccessible = true;

            string[] directories = Directory.GetDirectories(way, "*", enumerationOptions);
            try
            {
                // перебор полученных файлов
                foreach (string file in directories)
                {

                    DirectoryInfo directoryInfo = new DirectoryInfo(file);
                    ListViewItem item = new ListViewItem(new string[] { Path.GetFileNameWithoutExtension(file), Path.GetExtension(file), "", "", directoryInfo.LastWriteTime.ToString(), directoryInfo.CreationTime.ToString() });
                    listView1.Items.Add(item);
                }
                foreach (string file in files)
                {

                    FileInfo fileinfo = new FileInfo(file);
                    ListViewItem item = new ListViewItem(new string[] { Path.GetFileNameWithoutExtension(file), Path.GetExtension(file), (fileinfo.Length / 1024).ToString(), GetOwner(file), fileinfo.LastWriteTime.ToString(), fileinfo.CreationTime.ToString() });
                    listView1.Items.Add(item);
                }
            }
            catch { MessageBox.Show(way); }

        }
       public void ChangeLinkText(string NewLink) 
        {
            textBox1.Text = NewLink;
        }

        public Form1()
        {
            InitializeComponent();


            // give tokenSource in constructor

            GoLink(textBox1.Text);
            listView1.ContextMenuStrip = contextMenuStrip1;

            contextMenuStrip1.Items.Add(open);
            open.Click += listView1_DoubleClick;
            delete.Click += Delete_Click;
            copy.Click += Copy_Click;
            cut.Click += Cut_Click;
            paste.Click += Paste_Click;
            rename.Click += Rename_Click;

        }

        private void Rename_Click(object sender, EventArgs e)
        {
            string filename = textBox1.Text + @"\" + listView1.SelectedItems[0].Text+ listView1.SelectedItems[0].SubItems[1].Text;
            while (filename.Contains(@"\\")) { filename = filename.Replace(@"\\",@"\"); }
            
            Form3 form3 = new Form3(this,filename);
            form3.ShowDialog();
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            CopyOrCut = true;
            if (textBox1.Text[textBox1.Text.Length - 1] == '\\')
                copied = textBox1.Text + listView1.SelectedItems[0].Text + listView1.SelectedItems[0].SubItems[1].Text;
            else
                copied = textBox1.Text + @"\" + listView1.SelectedItems[0].Text + listView1.SelectedItems[0].SubItems[1].Text;

        }

        private void Delete_Click(object sender, EventArgs e)
        {
            
            if (listView1.SelectedItems[0].SubItems[1].Text != "")
                File.Delete(textBox1.Text + @"\" + listView1.SelectedItems[0].Text + listView1.SelectedItems[0].SubItems[1].Text);
            else
                Directory.Delete(textBox1.Text + @"\" + listView1.SelectedItems[0].Text, true);
            GoLink(textBox1.Text);
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            CopyOrCut = false;
            if (textBox1.Text[textBox1.Text.Length - 1] == '\\')
                copied = textBox1.Text + listView1.SelectedItems[0].Text + listView1.SelectedItems[0].SubItems[1].Text;
            else
                copied = textBox1.Text + @"\" + listView1.SelectedItems[0].Text + listView1.SelectedItems[0].SubItems[1].Text;

        }





        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (textBox1.Text[textBox1.Text.Length - 1] == '\\')
            { textBox1.Text += listView1.SelectedItems[0].Text + listView1.SelectedItems[0].SubItems[1].Text; }
            else
                textBox1.Text += @"\" + listView1.SelectedItems[0].Text + listView1.SelectedItems[0].SubItems[1].Text;

            string link = textBox1.Text;


            if (listView1.SelectedItems[0].SubItems[1].Text == "" || link.IndexOf("<<<") != -1)
            {

                if (link.IndexOf("<<<") != -1)
                {


                    link = link.Remove(link.LastIndexOf(@"\"));
                    link = link.Remove(link.LastIndexOf(@"\") + 1);
                    //  link = link.Remove(link.LastIndexOf(@"\"));


                    textBox1.Text = link;
                }

                DoubleBackslashCheck();
                GoLink(link);
                LinkBefore = link;
            }
            else
            {

                var p = new Process();
                p.StartInfo = new ProcessStartInfo(link)
                {
                    UseShellExecute = true
                };
                p.Start();// System.Diagnostics.Process.Start(link); 
               textBox1.Text=textBox1.Text.Remove(textBox1.Text.LastIndexOf(@"\"));
            }

        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

            if (listView1.SelectedItems.Count > 0)//Если мы выбрали файл в катологе
            {

                //для опции копирования
                if (listView1.SelectedItems[0].Text.IndexOf("<<<") == -1 && !contextMenuStrip1.Items.Contains(copy))
                {

                    contextMenuStrip1.Items.Add(copy);

                }
                if (listView1.SelectedItems[0].Text.IndexOf("<<<") == -1 && !contextMenuStrip1.Items.Contains(cut))
                {

                    contextMenuStrip1.Items.Add(cut);

                }
                //для опции вставки
                if (copied != "" && listView1.SelectedItems[0].Text.IndexOf("<<<") == -1 && !contextMenuStrip1.Items.Contains(paste) && listView1.SelectedItems[0].SubItems[1].Text == "")
                {

                    contextMenuStrip1.Items.Add(paste);

                }
                //для опции удаления
                if (listView1.SelectedItems[0].Text.IndexOf("<<<") == -1 && !contextMenuStrip1.Items.Contains(delete))
                {

                    contextMenuStrip1.Items.Add(delete);

                }
                if (listView1.SelectedItems[0].Text.IndexOf("<<<") == -1 && !contextMenuStrip1.Items.Contains(rename))
                {
                    contextMenuStrip1.Items.Add(rename);
                }


            }
            else //если целью являеться не выбраный файл а текущий
            {
                contextMenuStrip1.Items.Remove(open);
                if (copied != "" && !contextMenuStrip1.Items.Contains(paste))
                {
                    contextMenuStrip1.Items.Add(paste);

                }
                if (contextMenuStrip1.Items.Count == 0) e.Cancel = true;//если в меню нет ни 1 пункта то оно не появиться
            }
        }

        private void Paste_Click(object sender, EventArgs e)
        {

            string destination = textBox1.Text;
            while (destination.Contains(@"\\")) { destination.Replace(@"\\", @"\"); }
            if (CopyOrCut == true)//Ветка с выризанием
            {
                try
                {

                    if (listView1.SelectedItems.Count != 0)//ветка с выризанием в выбраный предмет
                        Directory.Move(copied, destination + @"\" + listView1.SelectedItems[0].Text + @"\" + copied.Substring(copied.LastIndexOf('\\') + 1));
                    else//ветка выризанием в текущий файл
                        Directory.Move(copied, destination + @"\" + copied.Substring(copied.LastIndexOf('\\') + 1));
                }
                catch
                {
                    MessageBox.Show("Конечный файл в который следует поместить файлы, являеться дочерней для папки в которой они находятся ");
                }
            }
            else
            {
                //var TestOnLoop = (destination + @"\" + listView1.SelectedItems[0].Text + @"\" + copied.Substring(copied.LastIndexOf('\\') + 1)).IndexOf(copied) == -1;

                if (listView1.SelectedItems.Count != 0)//ветка с копированием в выбраный предмет
                {
                    if ((destination + @"\" + listView1.SelectedItems[0].Text + @"\" + copied.Substring(copied.LastIndexOf('\\') + 1)).IndexOf(copied) == -1)
                        DirectoryCopy(copied, destination + @"\" + listView1.SelectedItems[0].Text + @"\" + copied.Substring(copied.LastIndexOf('\\') + 1), true);
                    else MessageBox.Show("Конечный файл в который следует поместить файлы, являеться дочерней для папки в которой они находятся ");


                }
                else
                {
                    if (((destination + @"\" + copied.Substring(copied.LastIndexOf('\\') + 1)).IndexOf(copied)) == -1)
                        DirectoryCopy(copied, destination + @"\" + copied.Substring(copied.LastIndexOf('\\') + 1), true);
                    else MessageBox.Show("Конечный файл в который следует поместить файлы, являеться дочерней для папки в которой они находятся ");

                }
            }

            GoLink(textBox1.Text);
            copied = "";
            contextMenuStrip1.Items.Remove(paste);
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (contextMenuStrip1.Items.Contains(copy)) { contextMenuStrip1.Items.Remove(copy); }
            if (contextMenuStrip1.Items.Contains(cut)) { contextMenuStrip1.Items.Remove(cut); }
            if (contextMenuStrip1.Items.Contains(delete)) { contextMenuStrip1.Items.Remove(delete); }
            if (contextMenuStrip1.Items.Contains(paste)) { contextMenuStrip1.Items.Remove(paste); }
            if (contextMenuStrip1.Items.Contains(rename)) { contextMenuStrip1.Items.Remove(rename); }
            if (!contextMenuStrip1.Items.Contains(open)) { contextMenuStrip1.Items.Add(open); }


        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)//Для enter
        {
            if (e.KeyCode == Keys.Enter && (File.Exists(textBox1.Text) || Directory.Exists(textBox1.Text)))
            {
                

                DoubleBackslashCheck();
                GoLink(textBox1.Text);
                LinkBefore = textBox1.Text;

            }
            else
            if (e.KeyCode == Keys.Enter) { textBox1.Text = LinkBefore; }

        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            LinkBefore = textBox1.Text;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            textBox1.Text = LinkBefore;
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {

            textBox1.Text = LinkBefore;
        }
        public void DoubleBackslashCheck() { while (textBox1.Text.Contains(@"\\")) { textBox1.Text = textBox1.Text.Remove(textBox1.Text.LastIndexOf('\\'), 1); } }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            //char[] deaf = new char[] { '\\','/',':','"','<','>','|' };


            
            if (e.KeyCode == Keys.Enter)
            {
               

                Form2 result = new Form2(this,SearchResult(textBox1.Text,textBox2.Text));
                result.ShowDialog();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }


}
