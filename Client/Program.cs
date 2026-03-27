using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading;
using System.IO;

namespace ASCII_Assault_App
{
    public partial class MainForm : Form
    {
        private TcpClient tcpClient;
        private NetworkStream clientStream;

        private TextBox textBoxOutput;
        private TextBox textBoxMessage;
        private Button buttonSendMessage;
        private Button buttonConnect;

        private string serverAddress = "127.0.0.1";
        private int serverPort = 6969;
        private bool isConnected = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "ASCII Assault";
            this.Size = new System.Drawing.Size(640, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.Black;

            textBoxOutput = new TextBox();
            textBoxOutput.Multiline = true;
            textBoxOutput.ReadOnly = true;
            textBoxOutput.ScrollBars = ScrollBars.Vertical;
            textBoxOutput.Location = new System.Drawing.Point(12, 12);
            textBoxOutput.Size = new System.Drawing.Size(600, 360);
            textBoxOutput.BackColor = System.Drawing.Color.Black;
            textBoxOutput.ForeColor = System.Drawing.Color.LimeGreen;
            textBoxOutput.Font = new System.Drawing.Font("Consolas", 10F);

            textBoxMessage = new TextBox();
            textBoxMessage.Location = new System.Drawing.Point(12, 390);
            textBoxMessage.Size = new System.Drawing.Size(420, 24);
            textBoxMessage.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            textBoxMessage.ForeColor = System.Drawing.Color.LimeGreen;
            textBoxMessage.Font = new System.Drawing.Font("Consolas", 10F);
            textBoxMessage.KeyDown += TextBoxMessage_KeyDown;

            buttonSendMessage = new Button();
            buttonSendMessage.Text = "Send";
            buttonSendMessage.Location = new System.Drawing.Point(440, 388);
            buttonSendMessage.Size = new System.Drawing.Size(80, 28);
            buttonSendMessage.FlatStyle = FlatStyle.Flat;
            buttonSendMessage.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            buttonSendMessage.ForeColor = System.Drawing.Color.LimeGreen;
            buttonSendMessage.Click += ButtonSendMessage_Click;

            buttonConnect = new Button();
            buttonConnect.Text = "Connect";
            buttonConnect.Location = new System.Drawing.Point(528, 388);
            buttonConnect.Size = new System.Drawing.Size(84, 28);
            buttonConnect.FlatStyle = FlatStyle.Flat;
            buttonConnect.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            buttonConnect.ForeColor = System.Drawing.Color.LimeGreen;
            buttonConnect.Click += ButtonConnect_Click;

            this.Controls.Add(textBoxOutput);
            this.Controls.Add(textBoxMessage);
            this.Controls.Add(buttonSendMessage);
            this.Controls.Add(buttonConnect);
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (isConnected)
                return;

            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(serverAddress, serverPort);
                clientStream = tcpClient.GetStream();
                isConnected = true;
                AppendOutput("Connected to server at " + serverAddress + ":" + serverPort);
                buttonConnect.Text = "Online";
                buttonConnect.Enabled = false;

                Thread listenThread = new Thread(ListenForMessages);
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            catch (Exception ex)
            {
                AppendOutput("Connection failed: " + ex.Message);
            }
        }

        private void ListenForMessages()
        {
            byte[] buffer = new byte[4096];
            while (isConnected)
            {
                try
                {
                    int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isConnected = false;
                        AppendOutput("Disconnected from server.");
                        break;
                    }
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    AppendOutput(message);
                }
                catch (IOException)
                {
                    isConnected = false;
                    AppendOutput("Connection lost.");
                    break;
                }
            }
        }

        private void ButtonSendMessage_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void TextBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMessage();
                e.SuppressKeyPress = true;
            }
        }

        private void SendMessage()
        {
            if (!isConnected || string.IsNullOrWhiteSpace(textBoxMessage.Text))
                return;

            try
            {
                byte[] data = Encoding.ASCII.GetBytes(textBoxMessage.Text);
                clientStream.Write(data, 0, data.Length);
                clientStream.Flush();
                AppendOutput("> " + textBoxMessage.Text);
                textBoxMessage.Clear();
            }
            catch (Exception ex)
            {
                AppendOutput("Send error: " + ex.Message);
            }
        }

        private void AppendOutput(string text)
        {
            if (textBoxOutput.InvokeRequired)
            {
                textBoxOutput.Invoke(new Action(() => AppendOutput(text)));
                return;
            }
            textBoxOutput.AppendText(text + Environment.NewLine);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
