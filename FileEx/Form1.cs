using System;
using System.IO;
using System.Windows.Forms;

namespace File_Explorer
{
    public partial class Form1 : Form
    {
        // Khai báo các biến cấp lớp
        private string cutFilePath;     // Đường dẫn của file/thư mục đã được cắt
        private bool isCutFolder;       // Xác định xem mục được cắt là thư mục hay file
        private bool isCut;   
        // Xác định nếu thao tác là cắt hoặc sao chép
        public Form1()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Cập nhật ListView khi người dùng chọn thư mục trong TreeView
            string selectedPath = e.Node.Tag.ToString();
            DisplayFiles(selectedPath);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Có thể xử lý thêm nếu cần khi mục trong ListView được chọn
        }

        // Tính năng sao chép (copy)
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                cutFilePath = Path.Combine(treeView1.SelectedNode.Tag.ToString(), listView1.SelectedItems[0].Text);
                isCut = false; // Đánh dấu thao tác là sao chép
                MessageBox.Show("Đã sao chép: " + cutFilePath);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một file hoặc thư mục để sao chép.");
            }
        }

        // Tính năng cắt (cut)
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                cutFilePath = Path.Combine(treeView1.SelectedNode.Tag.ToString(), listView1.SelectedItems[0].Text);
                isCut = true; // Đánh dấu thao tác là cắt

                // Kiểm tra xem đó là thư mục hay file
                isCutFolder = Directory.Exists(cutFilePath);

                MessageBox.Show("Đã cắt: " + cutFilePath);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một file hoặc thư mục để cắt.");
            }
        }

        // Tính năng dán (paste)
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cutFilePath))
            {
                // Lấy đường dẫn thư mục đích từ TreeView
                string destinationPath = treeView1.SelectedNode.Tag.ToString();

                try
                {
                    string newFilePath = Path.Combine(destinationPath, Path.GetFileName(cutFilePath));

                    if (isCut)
                    {
                        // Nếu thao tác là "cut"
                        if (isCutFolder)
                        {
                            Directory.Move(cutFilePath, newFilePath); // Di chuyển thư mục
                            MessageBox.Show("Đã di chuyển thư mục đến: " + newFilePath);
                        }
                        else
                        {
                            File.Move(cutFilePath, newFilePath); // Di chuyển file
                            MessageBox.Show("Đã di chuyển file đến: " + newFilePath);
                        }
                    }
                    else
                    {
                        // Nếu thao tác là "copy"
                        if (isCutFolder)
                        {
                            DirectoryCopy(cutFilePath, newFilePath, true); // Sao chép thư mục
                            MessageBox.Show("Đã sao chép thư mục đến: " + newFilePath);
                        }
                        else
                        {
                            File.Copy(cutFilePath, newFilePath); // Sao chép file
                            MessageBox.Show("Đã sao chép file đến: " + newFilePath);
                        }
                    }

                    // Cập nhật lại ListView để hiển thị nội dung mới
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

        // Hàm sao chép thư mục và tất cả nội dung bên trong
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

        // Hàm để cập nhật nội dung của ListView
        private void DisplayFiles(string path)
        {
            listView1.Items.Clear(); // Xóa nội dung cũ trong ListView

            if (Directory.Exists(path))
            {
                // Hiển thị các thư mục con
                foreach (var dir in Directory.GetDirectories(path))
                {
                    listView1.Items.Add(new ListViewItem(Path.GetFileName(dir)) { Tag = dir });
                }

                // Hiển thị các tệp
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
                string filePath = listView1.SelectedItems[0].Tag.ToString(); // Lấy đường dẫn của tệp/thư mục được chọn

                if (File.Exists(filePath))
                {
                    // Xóa tệp
                    try
                    {
                        File.Delete(filePath);
                        MessageBox.Show("Tệp đã được xóa thành công!");
                        DisplayFiles(Path.GetDirectoryName(filePath)); // Cập nhật danh sách
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa tệp: " + ex.Message);
                    }
                }
                else if (Directory.Exists(filePath))
                {
                    // Xóa thư mục
                    try
                    {
                        Directory.Delete(filePath, true); // Tham số true sẽ xóa cả thư mục con
                        MessageBox.Show("Thư mục đã được xóa thành công!");
                        DisplayFiles(Path.GetDirectoryName(filePath)); // Cập nhật danh sách
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
            // Hộp thoại để chọn thư mục
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

        // Hàm để hiển thị nội dung thư mục trong ListBox (có thể sử dụng ListView)
        private void DisplayFolderContents(string folderPath)
        {
            listView1.Items.Clear(); // Xóa các mục cũ trong ListView

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

    }
}
