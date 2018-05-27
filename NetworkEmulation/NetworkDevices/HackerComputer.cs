using System;
using System.Threading.Tasks;
using Communications;
using Helpers;
using NetworkPackets;
using System.Windows.Forms;
using System.Drawing;

namespace NetworkDevices
{
	public class HackerComputer : Form
	{

		public string IpAddress;
		public UdpClient client;
        protected string interfaceIp;

		public HackerComputer(string ipAddress, string interfaceIp)
		{
            this.InitializeComponent();
            this.Text = $"Hacker Computer {ipAddress}";
            IpAddress = ipAddress;
			this.interfaceIp = interfaceIp;
			client = new UdpClient(ipAddress);
            StartLogging();
            client.Send(interfaceIp, $"{ipAddress}/{interfaceIp}/{PacketType.HANDSHAKE.SYN}/{NetworkDeviceType.COMPUTER}");
            this.textBox1.Text = "Hacker computer started..." + Environment.NewLine;
        }

        private void Log(string message)
        {
            BeginInvoke((Action)(() => textBox1.AppendText(message + Environment.NewLine)));
        }

        public void Send(string sourceIp, string destinationIp, string message) => client.Send(interfaceIp, $"{sourceIp}/{destinationIp}/{message}/{NetworkDeviceType.COMPUTER}");

		public async Task<Received> Receive() => await client.Receive();

		public void StartLogging()
		{
			Task.Factory.StartNew(async () => {
				while (true)
				{
					var msg = await Receive();
					var target = msg.Message.Split('/')[1];
					if (target == IpAddress)
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
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(98, 250);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(109, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Source IP";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(9, 178);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(287, 20);
            this.textBox3.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(100, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Destination IP";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(9, 135);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(287, 20);
            this.textBox2.TabIndex = 9;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.MenuText;
            this.textBox1.Enabled = false;
            this.textBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(307, 25);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(362, 402);
            this.textBox1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(475, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Logs";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(109, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Message";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(9, 224);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(287, 20);
            this.textBox4.TabIndex = 14;
            // 
            // HackerComputer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 439);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "HackerComputer";
            this.Text = "HackerComputer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox4;

        private void button1_Click(object sender, EventArgs e)
        {
            Send(textBox3.Text, textBox2.Text, textBox4.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}