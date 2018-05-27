using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworkDevices;

namespace UserInterface
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var computer = new Computer(textBox2.Text, textBox3.Text);
            textBox2.Text = "";
            textBox3.Text = "";
            computer.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var hackerComputer = new HackerComputer(textBox2.Text, textBox3.Text);
            textBox2.Text = "";
            textBox3.Text = "";
            hackerComputer.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var router = new Router(textBox1.Lines);
            textBox1.Text = "";
            router.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sw = new Switch(textBox1.Lines);
            textBox1.Text = "";
            sw.Show();
        }
    }
}
