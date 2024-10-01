using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ServerChatForm
{
    public partial class Form1 : Form
    {
        private Thread listenThread;
        private TcpListener tcpListener;
        private bool stopChatServer = true;
        private readonly int _serverPort = 8000;
        private Dictionary<string, TcpClient> dict = new Dictionary<string, TcpClient>();

        public Form1()
        {
            InitializeComponent();
        }

        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(new IPEndPoint(IPAddress.Parse(textBox1.Text), _serverPort));
                tcpListener.Start();

                while (!stopChatServer)
                {
                    TcpClient _client = tcpListener.AcceptTcpClient();
                    StreamReader sr = new StreamReader(_client.GetStream());
                    StreamWriter sw = new StreamWriter(_client.GetStream()) { AutoFlush = true };
                    string username = sr.ReadLine();

                    if (username == null)
                    {
                        sw.WriteLine("Please pick a username");
                    }
                    else
                    {
                        if (!dict.ContainsKey(username))
                        {
                            Thread clientThread = new Thread(() => this.ClientRecv(username, _client));
                            dict.Add(username, _client);
                            clientThread.Start();
                        }
                        else
                        {
                            sw.WriteLine("Username already exists, pick another one");
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClientRecv(string username, TcpClient tcpClient)
        {
            StreamReader sr = new StreamReader(tcpClient.GetStream());
            try
            {
                while (!stopChatServer)
                {
                    string msg = sr.ReadLine();
                    if (msg.StartsWith("FILE:")) // Kiểm tra nếu tin nhắn bắt đầu bằng "FILE:"
                    {
                        string fileName = msg.Substring(5); // Lấy tên file từ tin nhắn
                        ReceiveFile(tcpClient, fileName);
                    }
                    else
                    {
                        string formattedMsg = $"[{DateTime.Now:MM/dd/yyyy h:mm tt}] {username}: {msg}\n";
                        foreach (TcpClient otherClient in dict.Values)
                        {
                            StreamWriter sw = new StreamWriter(otherClient.GetStream()) { AutoFlush = true };
                            sw.WriteLine(formattedMsg);
                        }
                        UpdateChatHistoryThreadSafe(formattedMsg);
                    }
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

        private void ReceiveFile(TcpClient client, string fileName)
        {
            using (NetworkStream stream = client.GetStream())
            {
                byte[] fileSizeBuffer = new byte[4];
                stream.Read(fileSizeBuffer, 0, 4);
                int fileSize = BitConverter.ToInt32(fileSizeBuffer, 0);

                byte[] fileData = new byte[fileSize];
                stream.Read(fileData, 0, fileSize);

                // Lưu file vào thư mục hiện tại
                File.WriteAllBytes(fileName, fileData);
                UpdateChatHistoryThreadSafe($"File received: {fileName}\n");
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
            if (stopChatServer)
            {
                stopChatServer = false;
                listenThread = new Thread(this.Listen);
                listenThread.Start();
                MessageBox.Show("Start listening for incoming connections");
                button1.Text = "Stop";
            }
            else
            {
                stopChatServer = true;
                button1.Text = "Start listening";
                tcpListener.Stop();
                listenThread = null;
            }
        }
    }
}
