using Microsoft.EntityFrameworkCore;
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
    public partial class PaymentForm : Form
    {
        private Order currentOrder;
        private readonly IServiceProvider _serviceProvider;
        private readonly CafeContext dbContext;
        private readonly Member loggedInMember;
        private ComboBox comboPaymentMethods; // Declare ComboBox as a class member

        private Label lblStatus;
        private Button btnDone;

        public PaymentForm(IServiceProvider serviceProvider, CafeContext dbContext, Member member)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.loggedInMember = member;

            this.MinimumSize = new Size(1500, 500);
            this.MaximumSize = new Size(1500, 500);
            this.Size = this.MinimumSize;

            currentOrder = OrderManager.CurrentOrder;

            if (currentOrder == null)
            {
                MessageBox.Show("No current order found. Please add items to the order first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            button3.Visible = false; //merch button only available after payment success
            LoadPaymentOptions();
        }

        private void LoadPaymentOptions()
        {
            decimal totalAmount = currentOrder.GetTotalAmount();
            Label lblTotal = new Label();
            CultureInfo cultureInfo = new CultureInfo("en-AU");
            lblTotal.Text = $"Total Amount: {totalAmount.ToString("C2", cultureInfo)}";
            lblTotal.Location = new Point(50, 20);
            lblTotal.AutoSize = true;
            this.Controls.Add(lblTotal);

            Label lblPaymentMethod = new Label();
            lblPaymentMethod.Text = "Select Payment Method:";
            lblPaymentMethod.Location = new Point(50, 60);
            lblPaymentMethod.AutoSize = true;
            this.Controls.Add(lblPaymentMethod);

            // Initialize ComboBox for payment methods
            comboPaymentMethods = new ComboBox();
            comboPaymentMethods.Items.Add("Credit Card");
            comboPaymentMethods.Items.Add("Debit Card");
            comboPaymentMethods.Items.Add("Cash");
            comboPaymentMethods.Items.Add("Mobile Payment");
            comboPaymentMethods.Location = new Point(50, 100);
            comboPaymentMethods.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(comboPaymentMethods);

            Button btnConfirmPayment = new Button();
            btnConfirmPayment.Text = "Pay";
            btnConfirmPayment.Location = new Point(20, 150);
            btnConfirmPayment.Size = new Size(100, 40);
            btnConfirmPayment.Click += BtnConfirmPayment_Click;
            this.Controls.Add(btnConfirmPayment);

            Button btnReturnToOrder = new Button();
            btnReturnToOrder.Text = "Return";
            btnReturnToOrder.Location = new Point(150, 150);
            btnReturnToOrder.Size = new Size(100, 40);
            btnReturnToOrder.Click += BtnReturnToOrder_Click;
            this.Controls.Add(btnReturnToOrder);

            // Show transaction complete status
            lblStatus = new Label
            {
                Text = "Please Pay Your Order!",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(lblStatus);

            // Ensure button3 (Merch button) is located underneath the label and centered
            button3.Visible = false;
            button3.Size = new Size(150, 80);
            button3.Text = "Redeem My Points";
            this.Controls.Add(button3);

            btnDone = new Button();
            btnDone.Visible = false;
            btnDone.Size = new Size(150, 80);
            btnDone.Text = "Done";
            btnDone.Click += BtnDone_Click;
            this.Controls.Add(btnDone);

            // Call method to adjust layout
            AdjustInfoLayout();

            // Handle form resizing to adjust elements dynamically
            this.Resize += PaymentForm_Resize;
        }

        private void PaymentForm_Resize(object sender, EventArgs e)
        {
            AdjustInfoLayout(); // Call the method whenever the form is resized
        }

        private void AdjustInfoLayout()
        {
            // Position the status label 50px from the right edge
            lblStatus.Location = new Point(this.ClientSize.Width - lblStatus.Width - 400, 20);

            // Center the button3 below lblStatus
            button3.Location = new Point(lblStatus.Left + (lblStatus.Width - button3.Width) / 2, lblStatus.Bottom + 100);

            // Center the Done button below button3
            btnDone.Location = new Point(lblStatus.Left + (lblStatus.Width - btnDone.Width) / 2, button3.Bottom + 20);
        }

        private void BtnConfirmPayment_Click(object sender, EventArgs e)
        {
            // Check if a payment method has been selected
            if (comboPaymentMethods.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method before proceeding.", "Payment Method Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit if no payment method is selected
            }

            try
            {
                if (loggedInMember != null)
                {
                    // Calculate points based on 20% of total amount
                    decimal totalAmount = currentOrder.GetTotalAmount();
                    decimal calculatedPoints = totalAmount * 0.2m;
                    int pointsToAdd = (int)Math.Round(calculatedPoints, MidpointRounding.AwayFromZero);

                    // Update the member points
                    loggedInMember.Point += pointsToAdd;

                    // Save changes to the database
                    dbContext.SaveChanges();
                }

                MessageBox.Show("Payment successful! Thank you for your order.", "Payment Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // If a member is logged in, show the "Merch" button (button3) and don't restart the application
                if (loggedInMember != null)
                {
                    lblStatus.Text = "Transaction Complete!\n Reedeem Your Points!";
                    lblStatus.ForeColor = Color.Green;
                    // Make sure button3 (Merch button) is visible after payment for logged-in members
                    button3.Visible = true;
                }
                else
                {
                    lblStatus.Text = "Transaction Complete!";
                    lblStatus.ForeColor = Color.Green;
                }

                btnDone.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while processing the payment: " + ex.Message);
            }
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void BtnReturnToOrder_Click(object sender, EventArgs e)
        {
            if (dbContext == null)
            {
                MessageBox.Show("Database context is not initialized.");
                return;
            }

            Checkout checkoutForm = new Checkout(dbContext, _serviceProvider, loggedInMember, OrderManager.CurrentOrder);
            checkoutForm.Show();
            this.Close();
        }

        private void PaymentForm_Load_1(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dbContext == null)
            {
                MessageBox.Show("Database context is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var merchItem = dbContext.Merches.FirstOrDefault();

            if (merchItem != null)
            {
                /*MessageBox.Show($"Database context initialized successfully.\nFirst merchandise item:\n" +
                                $"ID - {merchItem.MerchID}\n" +
                                $"Name - {merchItem.MerchName}\n" +
                                $"Description - {merchItem.MerchDescription}\n" +
                                $"Points - {merchItem.MerchPoints}\n" +
                                $"Image Path - {merchItem.MerchImagePath}",
                                "Verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                */

                //asking redeem points
                DialogResult result = MessageBox.Show("You currently have [" + loggedInMember.Point.ToString() + "] point(s) in your account.\nRedeem your points with our special merchandises today!", "Redeem Points?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // If user selects Yes, open the MerchForm
                    MerchForm merchForm = new MerchForm(_serviceProvider, dbContext, loggedInMember);
                    merchForm.Show();
                }
                else
                {
                    // If user selects No, just dismiss the view (do nothing)
                    MessageBox.Show("Thank you for coming to our Cafe today!", "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Database context initialized, but no merchandise data found in the database.", "Verification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
