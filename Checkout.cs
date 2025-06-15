using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Giles_Chen_test_1
{
    public partial class Checkout : Form
    {
        private Order currentOrder;
        private readonly CafeContext dbContext;
        private readonly Member loggedInMember;
        private readonly IServiceProvider _serviceProvider;

        public Checkout(CafeContext context, IServiceProvider serviceProvider, Member member = null, Order order = null)
        {
            InitializeComponent();
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
            loggedInMember = member;
            currentOrder = order ?? OrderManager.CurrentOrder;
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            
            if (this._serviceProvider == null)
            {
                MessageBox.Show("Checkout: serviceProvider is null");
            }
            else
            {
                //MessageBox.Show("Checkout: serviceProvider is initialized", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (currentOrder == null)
            {
                MessageBox.Show("Order is not initialized. Please add items to the order before proceeding to checkout.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

            LoadOrderDetails();

            this.MinimumSize = new Size(700, 450);
            this.MaximumSize = new Size(1000, 500);
            this.Size = this.MaximumSize;
        }

        private void LoadOrderDetails()
        {
            DataGridView dataGridViewOrderDetails = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(400, 300),
                AutoGenerateColumns = false,
                ReadOnly = true
            };

            dataGridViewOrderDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FoodandbevName",
                HeaderText = "Name",
                Width = 150
            });
            dataGridViewOrderDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                HeaderText = "Quantity",
                Width = 100
            });
            dataGridViewOrderDetails.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalPrice",
                HeaderText = "Total Price",
                Width = 150
            });

            dataGridViewOrderDetails.DataSource = currentOrder.OrderItems;
            this.Controls.Add(dataGridViewOrderDetails);

            decimal totalAmount = currentOrder.OrderItems.Sum(item => item.GetTotalPrice());
            CultureInfo cultureInfo = new CultureInfo("en-AU");
            Label lblTotalAmount = new Label
            {
                Text = $"Total Amount: {totalAmount.ToString("C2", cultureInfo)}",
                Location = new Point(20, 340),
                Size = new Size(400, 30),
                Font = new Font("Arial", 14, FontStyle.Bold)
            };
            this.Controls.Add(lblTotalAmount);

            Button btnNext = new Button
            {
                Text = "Next",
                Location = new Point(450, 20),
                Size = new Size(100, 40) // Setting the button size
            };
            btnNext.Click += BtnNext_Click;
            this.Controls.Add(btnNext);

            Button btnReturnToOrder = new Button
            {
                Text = "Return",
                Location = new Point(450, 80),
                Size = new Size(100, 40) // Setting the button size
            };
            btnReturnToOrder.Click += BtnReturnToOrder_Click;
            this.Controls.Add(btnReturnToOrder);

            // Adding padding between buttons and the member information
            int padding = 20;
            InitializeMemberInfoGroupBox(new Point(btnNext.Right + padding, btnNext.Top));
        }

        private void InitializeMemberInfoGroupBox(Point location)
        {
            // Create GroupBox for member information only if loggedInMember is not null
            if (loggedInMember == null)
            {
                return; // No member, so no need to display the member info
            }

            GroupBox groupBoxMemberInfo = new GroupBox
            {
                Text = "Member Information",
                Location = location, // Set the location based on button position with padding
                Size = new Size(300, 150)
            };

            // Create Labels and TextBoxes for Member ID, Name, and Points
            Label lblMemberId = new Label
            {
                Text = "Member ID:",
                Location = new Point(10, 30),
                AutoSize = true
            };
            TextBox txtMemberId = new TextBox
            {
                Location = new Point(150, 25),
                Size = new Size(150, 20),
                ReadOnly = true,
                Text = loggedInMember.MemberId.ToString()
            };

            Label lblName = new Label
            {
                Text = "Name:",
                Location = new Point(10, 60),
                AutoSize = true
            };
            TextBox txtName = new TextBox
            {
                Location = new Point(150, 55),
                Size = new Size(150, 20),
                ReadOnly = true,
                Text = loggedInMember.Name
            };

            Label lblPoints = new Label
            {
                Text = "Points:",
                Location = new Point(10, 90),
                AutoSize = true
            };
            TextBox txtPoints = new TextBox
            {
                Location = new Point(150, 85),
                Size = new Size(150, 20),
                ReadOnly = true,
                Text = loggedInMember.Point.ToString()
            };

            // Add Labels and TextBoxes to the GroupBox
            groupBoxMemberInfo.Controls.Add(lblMemberId);
            groupBoxMemberInfo.Controls.Add(txtMemberId);
            groupBoxMemberInfo.Controls.Add(lblName);
            groupBoxMemberInfo.Controls.Add(txtName);
            groupBoxMemberInfo.Controls.Add(lblPoints);
            groupBoxMemberInfo.Controls.Add(txtPoints);

            // Add GroupBox to the form
            this.Controls.Add(groupBoxMemberInfo);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_serviceProvider == null)
            {
                MessageBox.Show("Service provider is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            PaymentForm paymentForm = new PaymentForm(_serviceProvider, dbContext, loggedInMember);
            paymentForm.Show();
            this.Hide();
        }


        private void Checkout_Load(object sender, EventArgs e)
        {

        }
         private void BtnReturnToOrder_Click(object sender, EventArgs e)
        {

            this.Close();
        }

    }
}
