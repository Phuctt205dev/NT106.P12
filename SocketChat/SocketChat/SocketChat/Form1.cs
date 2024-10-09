using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SocketChat
{
    public partial class Form1 : Form
    {
        private Socket serverSocket = null;
        private bool started = false;
        private int _port = 11000;
        private static int _buff_size = 2048;
        private delegate void SafeCallDelegate(string text);

        public Form1()
        {
            InitializeComponent();
            serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (started)
                {
                    started = false;
                    button2.Text = "Listen on port 11000";
                    serverSocket.Close();
                }
                else
                {
                    button2.Text = "Listening on port 11000";
                    listen();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listen()
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(textBox1.Text), _port));
            serverSocket.Listen(10);
            started = true;
            UpdateChatHistoryThreadSafe("Start listening at " + _port);

            serverSocket.BeginAccept(new AsyncCallback(onAccepting), serverSocket);
        }

        public void onAccepting(IAsyncResult ar)
        {
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ar);
            UpdateChatHistoryThreadSafe("Accept connection from " + clientSocket.RemoteEndPoint.ToString());

            // Gọi lại BeginAccept để chấp nhận kết nối mới
            serverSocket.BeginAccept(new AsyncCallback(onAccepting), serverSocket);

            // Bắt đầu nhận dữ liệu từ client
            byte[] _buffer = new byte[_buff_size]; // Tạo bộ nhớ đệm riêng cho mỗi client
            clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), new ClientData { Socket = clientSocket, Buffer = _buffer });
        }

        public void onReceive(IAsyncResult ar)
        {
            // Lấy client và buffer từ đối tượng truyền vào
            ClientData clientData = (ClientData)ar.AsyncState;
            Socket clientSocket = clientData.Socket;
            byte[] _buffer = clientData.Buffer;

            try
            {
                int readbytes = clientSocket.EndReceive(ar);
                if (readbytes > 0)
                {
                    string receivedText = Encoding.UTF8.GetString(_buffer, 0, readbytes);
                    UpdateChatHistoryThreadSafe(receivedText + "\n");

                    // Tiếp tục nhận dữ liệu
                    clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(onReceive), clientData);
                }
                else
                {
                    // Đóng kết nối nếu không nhận được dữ liệu
                    clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                clientSocket.Close();
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

        // Lớp lưu trữ thông tin của từng client
        private class ClientData
        {
            public Socket Socket { get; set; }
            public byte[] Buffer { get; set; }
        }
    }
}
