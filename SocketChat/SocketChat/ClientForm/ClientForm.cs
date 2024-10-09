using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientForm
{
    public partial class ClientForm : Form
    {
        private Socket clientSocket = null;

        public ClientForm()
        {
            InitializeComponent();
            clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress serverIp = IPAddress.Parse(textBox1.Text); // Lấy IP server từ TextBox
                int serverPort = int.Parse(textBox2.Text);           // Lấy port từ TextBox
                IPEndPoint serverEp = new IPEndPoint(serverIp, serverPort);

                // Bắt đầu kết nối đến server
                clientSocket.BeginConnect(serverEp, new AsyncCallback(onConnecting), clientSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void onConnecting(IAsyncResult asyncResult)
        {
            try
            {
                Socket _client = (Socket)asyncResult.AsyncState;
                _client.EndConnect(asyncResult); // Hoàn tất kết nối

                UpdateChatHistoryThreadSafe("Connected to " + _client.RemoteEndPoint.ToString());
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
                if (clientSocket.Connected)
                {
                    string message = richTextBox2.Text;
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    clientSocket.Send(messageBytes);
                    richTextBox2.Text = ""; // Xóa nội dung sau khi gửi
                }
                else
                {
                    MessageBox.Show("Not connected to server.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Nút để chọn file và gửi file
        private void buttonSendFile_Click(object sender, EventArgs e)
        {
            if (clientSocket.Connected)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    SendFile(filePath);
                }
            }
            else
            {
                MessageBox.Show("Not connected to server.");
            }
        }

        private void SendFile(string filePath)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                string fileName = Path.GetFileName(filePath);
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

                byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
                clientSocket.Send(fileNameLengthBytes); // Gửi độ dài tên file trước
                clientSocket.Send(fileNameBytes);       // Gửi tên file

                clientSocket.Send(fileData);            // Gửi dữ liệu file
                UpdateChatHistoryThreadSafe("File sent: " + fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateChatHistoryThreadSafe(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                var d = new SafeCallDelegate(UpdateChatHistoryThreadSafe);
                richTextBox1.Invoke(d, new object[] { text });
            }
            else
            {
                richTextBox1.Text += text + "\n";
            }
        }

        private delegate void SafeCallDelegate(string text);
    }
}
