using Giles_Chen_test_1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Giles_Chen_test_1
{
    public partial class Membership3 : Form
    {
        private Order currentOrder;
        private readonly IServiceProvider serviceProvider;
        private readonly CafeContext dbContext;

        public Membership3(CafeContext dbContext, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            LoadMemberInfo();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


        private void LoadMemberInfo()
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Merchpage merchpage = new Merchpage();
            //merchpage.Show();
            //this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}