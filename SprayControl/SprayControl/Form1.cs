using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;

namespace SprayControl
{
    public partial class Form1 : Form
    {
        private static string apiUrl = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void GetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage responce = await client.GetAsync(url))
                {
                    using (HttpContent content = responce.Content)
                    {
                        string myContent = await content.ReadAsStringAsync();    
                        richTextBox1.Text = myContent + "\n" + richTextBox1.Text;
                    }
                }
            }
        }

        private async void PostRequest(string url, string value)
        {
            IEnumerable<KeyValuePair<string, string>> kvp = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(value, value)
            };
            HttpContent cont = new FormUrlEncodedContent(kvp);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    using (HttpResponseMessage responce = await client.PostAsync(url, cont))
                    {
                        using (HttpContent content = responce.Content)
                        {
                            string myContent = await content.ReadAsStringAsync();
                            ProcessResponce(myContent);                            
                        }
                    }
                }
                catch (Exception e)
                {
                    richTextBox1.Text = e + "\n" + richTextBox1.Text;
                }
                
            }
        }

        private void ProcessResponce(String value)
        {
            if (value.Contains("99"))
            {
                button4.ResetBackColor();
                button4.UseVisualStyleBackColor = true;
                richTextBox1.Text = "Sending Command: Spray Trigger" + "\n" + "Recieved Response: Spray Triggerd." + "\n" + richTextBox1.Text;
            }
            else if (value.Contains("100"))
            {
                button1.ResetBackColor();
                button1.UseVisualStyleBackColor = true;
                richTextBox1.Text = "Sending Command: Enable Sensor" + "\n" + "Recieved Response: Sensing On!" + "\n" + richTextBox1.Text;
            }
            else if (value.Contains("101"))
            {
                button2.ResetBackColor();
                button2.UseVisualStyleBackColor = true;
                richTextBox1.Text = "Sending Command: Disable Sensor" + "\n" + "Recieved Response: Sensing Off!" + "\n" + richTextBox1.Text;
            }
            else if (value.Contains("102"))
            {
                button1.ResetBackColor();
                button1.UseVisualStyleBackColor = true;
                richTextBox1.Text = "Sending Command: Enable Water" + "\n" + "Recieved Response: Water On!" + "\n" + richTextBox1.Text;
            }
            else if (value.Contains("103"))
            {
                button3.ResetBackColor();
                button3.UseVisualStyleBackColor = true;
                richTextBox1.Text = "Sending Command: Disable Water" + "\n" + "Recieved Response: Water Off!" + "\n" + richTextBox1.Text;
            }
            else if (value.Contains("104"))
            {
                richTextBox1.Text = "Sending Command: All Stop!" + "\n" + "Recieved Response: All Systems Disabled" + "\n" + richTextBox1.Text;
            }

        }

        //all off button
        private void button1_Click(object sender, EventArgs e)
        {
            button1.BackColor = Color.Salmon;
            PostRequest(apiUrl, "off");
        }

        // this button clears the text log
        private void clearButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        // sensor button
        private void button2_Click(object sender, EventArgs e)
        {
            button2.BackColor = Color.LightYellow;
            PostRequest(apiUrl, "sensor");
        }

        //spray button
        private void button4_Click(object sender, EventArgs e)
        {
            button4.BackColor = Color.LightBlue;
            PostRequest(apiUrl, "spray");
        }

        //water button
        private void button3_Click(object sender, EventArgs e)
        {
            button3.BackColor = Color.LightBlue;
            PostRequest(apiUrl, "water");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void urlButton_Click(object sender, EventArgs e)
        {
            apiUrl = urlTextBox.Text;
            richTextBox1.Text = "URL: " + apiUrl + "\n" + "SAVED" + "\n" + richTextBox1.Text;
            urlTextBox.Clear();
        }

        private void urlTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
