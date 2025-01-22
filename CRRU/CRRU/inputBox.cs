using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRRU
{
    public partial class inputBox : Form
    {
        public string UserInput { get; set; }

        public inputBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserInput = textBox1.Text;  // สมมติว่า txtInput คือ TextBox ที่ใช้รับข้อมูล
            //this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UserInput = textBox1.Text;
                this.Close();
            }
        }
    }
}
