using Giles_Chen_test_1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Giles_Chen_test_1
{
    public partial class WelcomeForm : Form
    {
        private IServiceProvider serviceProvider;
        private CafeContext dbContext;
        private CafeContext _context;
        private readonly CafeContext _dbContext;
        private readonly IServiceProvider _serviceProvider;


        public WelcomeForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.serviceProvider = serviceProvider;
            this.FormClosing += Form1_FormClosing;
            // Retrieve the DbContext using the service provider

            _dbContext = serviceProvider.GetService<CafeContext>();
            // Ensure the database is initialized properly
            _dbContext.InitializeStaffs();
            //MessageBox.Show("Form1 Initialized");

            // Set minimum size
            this.MinimumSize = new Size(800, 800);

            // Center the form initially
            this.StartPosition = FormStartPosition.CenterScreen;

            // Subscribe to Resize event for adjusting layout positions
            this.Resize += new EventHandler(WelcomeForm_Resize);

        }

        private void WelcomeForm_Resize(object sender, EventArgs e) 
        {
            AdjustLayoutPosition();
        }

        private void AdjustLayoutPosition()
        {
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;

            // Keep the central content (button1 and any text) in the center of the form
            button1.Left = (this.ClientSize.Width - button1.Width) / 2;
            button1.Top = (this.ClientSize.Height - button1.Height) / 2;

            // Adjust the StaffLogin button to be 20px from the top and right of the window
            button2.Left = this.ClientSize.Width - button2.Width - 20;
            button2.Top = 20;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dbContext = serviceProvider.GetService<CafeContext>();
            StaffLoginForm staffLogin = new StaffLoginForm(serviceProvider, dbContext);
            staffLogin.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OrderForm form6 = new OrderForm(serviceProvider);
            form6.Show();
            this.Hide();
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (e.CloseReason == CloseReason.UserClosing)
            {

                Application.Exit();
            }
        }


    }
}
