using System;
using System.IO;
using System.Windows.Forms;

namespace File_Explorer
{
    public partial class Form1 : Form
    {
        private string cutFilePath;     
        private bool isCutFolder;      
        private bool isCut;   
        
        public Form1()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            string selectedPath = e.Node.Tag.ToString();
            DisplayFiles(selectedPath);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                //cutFilePath = Path.Combine(treeView1.SelectedNode.Tag.ToString(), listView1.SelectedItems[0].Text);
                cutFilePath=textBox1.Text;
                isCut = false; // Đánh dấu thao tác là sao chép
                MessageBox.Show("Đã sao chép: " + cutFilePath);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một file hoặc thư mục để sao chép.");
            }
        }

        
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                //cutFilePath = Path.Combine(treeView1.SelectedNode.Tag.ToString(), listView1.SelectedItems[0].Text);
                cutFilePath = textBox1.Text;
                isCut = true; 

               
                isCutFolder = Directory.Exists(cutFilePath);

                MessageBox.Show("Đã cắt: " + cutFilePath);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một file hoặc thư mục để cắt.");
            }
        }

       
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cutFilePath))
            {

                string destinationPath = /*treeView1.SelectedNode.Tag.ToString();*/textBox1.Text;

                try
                {
                    string newFilePath = Path.Combine(destinationPath, Path.GetFileName(cutFilePath));

                    if (isCut)
                    {
                        
                        if (isCutFolder)
                        {
                            Directory.Move(cutFilePath, newFilePath); 
                            MessageBox.Show("Đã di chuyển thư mục đến: " + newFilePath);
                        }
                        else
                        {
                            File.Move(cutFilePath, newFilePath); 
                            MessageBox.Show("Đã di chuyển file đến: " + newFilePath);
                        }
                    }
                    else
                    {
                        
                        if (isCutFolder)
                        {
                            DirectoryCopy(cutFilePath, newFilePath, true); 
                            MessageBox.Show("Đã sao chép thư mục đến: " + newFilePath);
                        }
                        else
                        {
                            File.Copy(cutFilePath, newFilePath); 
                            MessageBox.Show("Đã sao chép file đến: " + newFilePath);
                        }
                    }

                   
                    DisplayFiles(destinationPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Không có mục nào để dán.");
            }
        }

        
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

       
        private void DisplayFiles(string path)
        {
            listView1.Items.Clear(); 

            if (Directory.Exists(path))
            {
                
                foreach (var dir in Directory.GetDirectories(path))
                {
                    listView1.Items.Add(new ListViewItem(Path.GetFileName(dir)) { Tag = dir });
                }

                foreach (var file in Directory.GetFiles(path))
                {
                    listView1.Items.Add(new ListViewItem(Path.GetFileName(file)) { Tag = file });
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string filePath = listView1.SelectedItems[0].Tag.ToString(); 

                if (File.Exists(filePath))
                {
                   
                    try
                    {
                        File.Delete(filePath);
                        MessageBox.Show("Tệp đã được xóa thành công!");
                        DisplayFiles(Path.GetDirectoryName(filePath)); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa tệp: " + ex.Message);
                    }
                }
                else if (Directory.Exists(filePath))
                {
                    
                    try
                    {
                        Directory.Delete(filePath, true); 
                        MessageBox.Show("Thư mục đã được xóa thành công!");
                        DisplayFiles(Path.GetDirectoryName(filePath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa thư mục: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một file hoặc thư mục để xóa.");
            }
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Chọn thư mục để xem nội dung";

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolderPath = folderBrowserDialog.SelectedPath;
                    DisplayFolderContents(selectedFolderPath);
                }
            }
        }

        
        private void DisplayFolderContents(string folderPath)
        {
            listView1.Items.Clear(); 

            string[] files = Directory.GetFiles(folderPath);
            string[] directories = Directory.GetDirectories(folderPath);

            foreach (string directory in directories)
            {
                listView1.Items.Add(new ListViewItem(Path.GetFileName(directory)) { Tag = directory });
            }

            foreach (string file in files)
            {
                listView1.Items.Add(new ListViewItem(Path.GetFileName(file)) { Tag = file });
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select your path." })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Web.Url = new Uri(fbd.SelectedPath);
                    textBox1.Text = fbd.SelectedPath; 
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (Web.CanGoBack)
                Web.GoBack();

        }

        private void button2_Click(object sender, EventArgs e)
        {
           if(Web.CanGoForward)
                Web.GoForward();


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string path = textBox1.Text;

            if (Directory.Exists(path))
            {
                listView1.Items.Clear();

                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
                    {
                        ListViewItem item = new ListViewItem(subDirectory.Name);
                        item.SubItems.Add("Directory");
                        listView1.Items.Add(item);
                    }

                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        ListViewItem item = new ListViewItem(file.Name);
                        item.SubItems.Add("File");
                        listView1.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void Web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
