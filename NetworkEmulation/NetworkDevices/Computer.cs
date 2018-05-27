using System;
using System.Threading.Tasks;
using Communications;
using Helpers;
using NetworkPackets;
using System.Windows.Forms;
using System.Drawing;

namespace NetworkDevices
{
    public class Computer : Form
    {

        public string IpAddress;
        public UdpClient client;
        protected string interfaceIp;

        public Computer(string ipAddress, string interfaceIp)
        {
            this.InitializeComponent();
            this.Text = $"Computer {ipAddress}";
            IpAddress = ipAddress;
            this.interfaceIp = interfaceIp;
            client = new UdpClient(ipAddress);
            StartLogging();
            client.Send(interfaceIp, $"{ipAddress}/{interfaceIp}/{PacketType.HANDSHAKE.SYN}/{NetworkDeviceType.COMPUTER}");
            this.textBox1.Text = "Computer started..." + Environment.NewLine;
        }

        private void Log(string message)
        {
            BeginInvoke((Action)(() => textBox1.AppendText(message + Environment.NewLine)));
        }

        private bool IsEcho(string msg) => msg.Contains(PacketType.ICMP.ICMP_ECHO_REQUEST);
        private bool IsBroadcast(string msg) => string.Compare(msg.Split('/')[1], "255.255.255.255") == 0;

        public void Send(string destinationIp, string message) => client.Send(interfaceIp, $"{IpAddress}/{destinationIp}/{message}/{NetworkDeviceType.COMPUTER}");

        public async Task<Received> Receive() {
			var recv = await client.Receive();
			var msg = recv.Message;
			if (IsEcho(msg))
			{
				var source = msg.Split('/')[0];
				Send(source, PacketType.ICMP.ICMP_ECHO_REPLY);
			}
			return recv;
        }

        public void StartLogging()
        {
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    var msg = await Receive();
                    var target = msg.Message.Split('/')[1];
                    if (target == IpAddress || IsBroadcast(msg.Message))
                    {
                        Log($"Recieved {msg.Message.Split('/')[2]} from {msg.Message.Split('/')[0]}");
                    }
                    else
                    {
                        Log($"Discarded {msg.Message.Split('/')[2]} from {msg.Message.Split('/')[0]}");
                    }
                }
            });
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(482, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Logs";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.MenuText;
            this.textBox1.Enabled = false;
            this.textBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(305, 25);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(362, 388);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 148);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(287, 20);
            this.textBox2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(106, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Destination IP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Message";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(12, 191);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(287, 20);
            this.textBox3.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(104, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Computer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 425);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "Computer";
            this.Text = "Computer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button1;

        private void button1_Click(object sender, EventArgs e)
        {
            Send(textBox2.Text, textBox3.Text);
        }
    }
}
