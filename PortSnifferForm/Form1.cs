using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortSnifferForm
{
    public partial class Form1 : Form
    {
        delegate void SetTextCallback(string text, string textBoxName);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text;
            textBox1.Text = "Sniffing...";
            Application.DoEvents();
            RunSniffer(ip);
        }

        public  async Task<bool> RunSniffer(string ip)
        {

            List<int> open = new List<int>();//holds ports that are open
            List<int> closed = new List<int>();//holds ports that are closed
            List<int> error = new List<int>();

            List<Task> tasks = new List<Task>();
            foreach (int i in Enumerable.Range(0, 65535))
            {
                tasks.Add(Sniff(i, ip, open, closed, error)); //sends each port out to get checked
            }
            await Task.WhenAll(tasks);//waits till all tasks have returned
            textBox1.Text = "Finished!";
            Application.DoEvents();
            return true;
        }

        public  async Task<bool> Sniff(int i, string ip, List<int> o, List<int> c, List<int> e)
        {
            Task<bool> sniffing = Task.Run(() => //runs each port async, when isAdded gets returned sniffing will be finished
            {
                Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                bool isAdded = false;
                try
                {

                    mySocket.Connect(ip, i);

                    //if a connection can be made, this works.  If not, it throws a runtime error.
                    o.Add(i);
                    SetText(i + ", ", "textBox3");
                   isAdded = true;

                }

                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        c.Add(i);
                        SetText(i + ", ", "textBox2");
                        isAdded = true;
                    }
                    else
                    { //unexpected error
                        e.Add(i);
                        SetText(i + ", ", "textBox4");
                        isAdded = true;
                    }
                }
                if (mySocket.Connected) mySocket.Disconnect(true);

                mySocket.Close();
                return isAdded;
            });
            var response = await sniffing;
            return response;
        }


        private void SetText(string text, string textBoxName)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (textBoxName == "textBox2")
            {
                if (this.textBox2.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text, textBoxName });
                }
                else
                {
                    this.textBox2.Text += text;
                }
            }
            if (textBoxName == "textBox3")
            {
                if (this.textBox3.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text, textBoxName });
                }
                else
                {
                    this.textBox3.Text += text;
                }
            }
            if (textBoxName == "textBox4")
            {
                if (this.textBox4.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text, textBoxName });
                }
                else
                {
                    this.textBox4.Text += text;
                }
            }

        }
    }
    
}
