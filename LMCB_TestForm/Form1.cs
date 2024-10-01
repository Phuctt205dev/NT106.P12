using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Xml.Linq;

namespace LMCB_TestForm
{
    public partial class Form1 : Form
    {
        private TcpClient tcpClient;
        private StreamReader sReader;
        private StreamWriter sWriter;
        private Thread clientThread;
        private int serverPort = 8000;
        private bool stopTcpClient = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void ClientRecv()
        {
            StreamReader sr = new StreamReader(tcpClient.GetStream());
            try
            {
                while (!stopTcpClient)
                {
                    string data = sr.ReadLine();
                    UpdateChatHistoryThreadSafe($"{data}\n");
                }
            }
            catch (SocketException)
            {
                tcpClient.Close();
                sr.Close();
            }
            catch (IOException)
            {
                tcpClient.Close();
                sr.Close();
            }
        }

        private delegate void SafeCallDelegate(string text);

        private void UpdateChatHistoryThreadSafe(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                var d = new SafeCallDelegate(UpdateChatHistoryThreadSafe);
                richTextBox1.Invoke(d, new object[] { text });
            }
            else
            {
                richTextBox1.AppendText(text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                sWriter.WriteLine(sendMsgTextBox.Text);
                sendMsgTextBox.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                stopTcpClient = false;
                this.tcpClient = new TcpClient();
                this.tcpClient.Connect(new IPEndPoint(IPAddress.Parse(textBox2.Text), serverPort));
                this.sWriter = new StreamWriter(tcpClient.GetStream()) { AutoFlush = true };
                sWriter.WriteLine(this.textBox1.Text);
                clientThread = new Thread(this.ClientRecv);
                clientThread.Start();
                MessageBox.Show("Connected");
            }
            catch (SocketException sockEx)
            {
                MessageBox.Show(sockEx.Message, "Network error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    SendFile(filePath);
                }
            }
        }

        private void SendFile(string filePath)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                NetworkStream networkStream = tcpClient.GetStream();

                // Gửi tên file trước
                string fileName = Path.GetFileName(filePath);
                sWriter.WriteLine($"FILE:{fileName}");

                // Gửi kích thước file trước
                byte[] fileSize = BitConverter.GetBytes(fileData.Length);
                networkStream.Write(fileSize, 0, fileSize.Length);

                // Gửi nội dung file
                networkStream.Write(fileData, 0, fileData.Length);
                MessageBox.Show("File sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending file: {ex.Message}");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Thiết lập các thuộc tính của OpenFileDialog nếu cần
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = "Chọn file để mở";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    try
                    {
                        // Đọc nội dung của file
                        string fileContent = File.ReadAllText(filePath);

                        // Hiển thị nội dung trong RichTextBox
                        richTextBox1.Text = fileContent; // Giả sử bạn có một RichTextBox để hiển thị nội dung
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Thiết lập các tùy chọn cho SaveFileDialog
                saveFileDialog.InitialDirectory = "C:\\"; // Thư mục khởi đầu
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; // Bộ lọc file
                saveFileDialog.Title = "Save a File"; // Tiêu đề hộp thoại

                // Nếu người dùng chọn một file hợp lệ
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName; // Lưu đường dẫn file

                    try
                    {
                        // Lưu nội dung RichTextBox vào file
                        File.WriteAllText(filePath, richTextBox1.Text);
                        MessageBox.Show("File saved successfully!"); // Thông báo thành công
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}"); // Thông báo lỗi
                    }
                }
            }
}

    }
}
