using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading;
using System.Linq;
using System;
using NetworkPackets;
using System.Windows.Forms;
using System.Drawing;

namespace NetworkDevices
{
    public sealed class Router : Form
    {

        public readonly List<NetworkInterface> Interfaces;
        private readonly ConcurrentDictionary<string, ISet<NetworkInterface>> routingTable = new ConcurrentDictionary<string, ISet<NetworkInterface>>();
        private TextBox textBox4;
        private Label label4;
        private readonly BufferBlock<string> queue = new BufferBlock<string>();

        public Router(string[] ipAddresses)
        {
            this.InitializeComponent();
            this.textBox4.Lines = ipAddresses;
            Interfaces = ipAddresses.Select(NetworkInterface.New).ToList();
            StartInterfaces();
            StartRouting();
            StartTableDistribution();
            this.textBox1.Text = "Router started..." + Environment.NewLine;
        }

        private void StartRouting() =>
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    var msg = await queue.ReceiveAsync();
                    var target = msg.Split('/')[1];
                    if (routingTable.ContainsKey(target))
                    {
                        var interfaces = routingTable[target];
                        var i = interfaces.OrderBy(x => x.Type == NetworkDeviceType.ROUTER).First();
                        i.Send(msg);
                        Log($"Interface {i.IpAddress}: Routing {msg.Split('/')[2]} to {target}");
                    }
                }
            });

        private bool IsBroadcast(string msg) => string.Compare(msg.Split('/')[1], "255.255.255.255") == 0;

        public void Connect(string sourceIp, string destinationIp) {
            var srcInterface = Interfaces.First(i => i.IpAddress == sourceIp);
            srcInterface.Send(destinationIp, $"{sourceIp}/{destinationIp}/{PacketType.HANDSHAKE.SYN}/{NetworkDeviceType.ROUTER}");
        }

        private void Log(string message)
        {
            BeginInvoke((Action)(() => textBox1.AppendText(message + Environment.NewLine)));
        }

        private void StartInterfaces() =>
            Interfaces.ForEach(i =>
            {
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        var data = await i.Receive();
                        var msg = data.Message;
                        var source = msg.Split('/')[0];
                        var target = msg.Split('/')[1];
                        var message = msg.Split('/')[2];
                        var type = msg.Split('/')[3];
                        if ((message == PacketType.HANDSHAKE.SYN || message == PacketType.HANDSHAKE.ACK) && target == i.IpAddress)
                        {
                            if (message == PacketType.HANDSHAKE.SYN)
                            {
                                Log($"Interface {i.IpAddress}: Recieved SYN from {source}");
                                i.Send($"{target}/{source}/ACK/{NetworkDeviceType.ROUTER}");
                            }
                            else
                            {
                                Log($"Interface {i.IpAddress}: Recieved SYN from {source}");
                                i.Type = type;
                            }
                            AddToRoutingTable(source, i);
                        }
                        else if (message == "DIST")
                        {
                            var recievedIps = msg.Split('/')[4];
                            var distibutedIps = Deserialize(recievedIps);
                            distibutedIps.Where(ip => ip != i.IpAddress).ToList().ForEach(ip => AddToRoutingTable(ip, i));
                        }
                        else if (IsBroadcast(msg)) {
                            Interfaces.Where(other => other.IpAddress != i.IpAddress).ToList().ForEach(other => {
                                other.Send(msg);    
                            });
                        }
                        else {
                            queue.Post(msg);
                        }
                    }
                });
            });

        private void AddToRoutingTable(string ip, NetworkInterface i) {
            if (routingTable.ContainsKey(ip))
            {
                var interfaces = routingTable[ip];
                interfaces.Add(i);
            }
            else
            {
                var interfaces = new HashSet<NetworkInterface> { i };
                routingTable[ip] = interfaces;
                Log($"Interface {i.IpAddress}: Learned route {ip}");
            }
        }

        private void StartTableDistribution() =>
            Task.Factory.StartNew(() => {
                while (true) {
                    Interfaces
                    .Where(i => i.Type == NetworkDeviceType.ROUTER)
                    .ToList()
                    .ForEach(i => {
    					var serializedTable = Serialize(routingTable);
                        var interfaces = string.Join("#", Interfaces.Select(x => x.IpAddress));
                        var routingInfo = Merge(serializedTable, interfaces);
                        i.Send($"{i.IpAddress}/192.168.0.255/DIST/{NetworkDeviceType.ROUTER}/{routingInfo}");
                    });
                    Thread.Sleep(100);
                }
            });

        private static string Serialize(IDictionary<string, ISet<NetworkInterface>> table) => string.Join("#", table.Keys);

		private static string[] Deserialize(string data)
		{
            if (data.Length == 0) {
                return new string[0];
            }
            var result = data.Split('#');
            return result;
        }

        private static string Merge(string s1, string s2)
        {
            if (s1.Length > 0 && s2.Length > 0)
            {
                return string.Join("#", s1, s2);
            }
            if (s1.Length > 0)
            {
                return s1;
            }
            return s2;
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.MenuText;
            this.textBox1.Enabled = false;
            this.textBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.textBox1.Location = new System.Drawing.Point(285, 25);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(346, 375);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(440, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Logs";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(29, 282);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 266);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Source Interface IP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 266);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Destination Interface IP";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(148, 282);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 308);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(219, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox4
            // 
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(73, 61);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(133, 193);
            this.textBox4.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(113, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Interfaces";
            // 
            // Router
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 412);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "Router";
            this.Text = "Router";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button1;

        private void button1_Click(object sender, EventArgs e)
        {
            Connect(textBox2.Text, textBox3.Text);
            textBox2.Text = "";
            textBox3.Text = "";
        }
    }
}
