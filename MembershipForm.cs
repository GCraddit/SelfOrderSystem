using Giles_Chen_test_1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Giles_Chen_test_1
{
    public partial class MembershipForm : Form
    {
        private readonly CafeContext dbContext;
        private readonly IServiceProvider serviceProvider;

        // Constructor accepting dbContext via Dependency Injection
        public MembershipForm(CafeContext dbContext, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            
            if (this.serviceProvider == null)
            {
                //MessageBox.Show("MembershipForm: serviceProvider is null", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //MessageBox.Show("MembershipForm: serviceProvider is initialized", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            textBox2.PasswordChar = '*';

            // Set minimum size for the form
            this.MinimumSize = new Size(750, 600);
            this.Size = this.MinimumSize;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Resize += new EventHandler(MembershipForm_Resize);

            AdjustLayoutPosition();
        }


        private void Membership1_Load(object sender, EventArgs e)
        {
            AdjustLayoutPosition();
        }

        private void MembershipForm_Resize(object sender, EventArgs e)
        {
            // Adjust layout positions dynamically on form resize
            AdjustLayoutPosition();
        }

        private void AdjustLayoutPosition()
        {
            /* 
             * NOTE: 
             * button1 = sign in
             * button2 = sign up
             * button3 = continue without membership
             */

            // Define padding between controls
            int paddingVertical = 20;
            int paddingHorizontal = 20;
            int groupLabelPadding = 30; // Extra space between the "Yes, I'm a member" and "Member ID" section

            // First, ensure the title is in the center (as per your previous request)
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label1.Top = 100;

            // --- Left Group Layout ---
            // Set label2 (Yes, I'm a member) to bold
            label2.Font = new Font(label2.Font, FontStyle.Bold);
            int leftGroupLeft = this.ClientSize.Width / 4; // Position the left group one-fourth into the form width
            int leftGroupTop = label1.Bottom + 50; // Start below the title label

            // Position label2 (Yes, I'm a member)
            label2.Left = leftGroupLeft - (label2.Width / 2); // Center it horizontally relative to the left group
            label2.Top = leftGroupTop;

            // Position label3 (Member ID:) and textBox1 (Member ID textbox)
            label3.Left = leftGroupLeft - label3.Width - paddingHorizontal; // Align to the left of the textbox
            label3.Top = label2.Bottom + groupLabelPadding; // Add extra padding below label2
            textBox1.Left = label3.Right + paddingHorizontal; // Place next to label3
            textBox1.Top = label3.Top;

            // Position label4 (Password:) and textBox2 (Password textbox)
            label4.Left = leftGroupLeft - label4.Width - paddingHorizontal; // Align to the left of the textbox
            label4.Top = label3.Bottom + (2 * paddingVertical); // Add padding between Member ID and Password
            textBox2.Left = label4.Right + paddingHorizontal; // Place next to label4
            textBox2.Top = label4.Top;

            // Position button1 (Sign In) underneath label4 and textBox2
            button1.Left = leftGroupLeft - (button1.Width / 2); // Center it horizontally relative to the left group
            button1.Top = textBox2.Bottom + paddingVertical;

            // --- Right Group Layout ---
            // Set label5 (No, I'm not a member) to bold
            label5.Font = new Font(label5.Font, FontStyle.Bold);
            int rightGroupLeft = 3 * this.ClientSize.Width / 4; // Position the right group three-fourths into the form width
            int rightGroupTop = label1.Bottom + 50; // Align horizontally with the left group

            // Position label5 (No, I'm not a member) aligned with label2
            label5.Left = rightGroupLeft - (label5.Width / 2); // Center it horizontally relative to the right group
            label5.Top = rightGroupTop; // Align vertically with label2

            // Position button2 (Sign Up) below label5
            button2.Left = rightGroupLeft - (button2.Width / 2); // Center it horizontally relative to the right group
            button2.Top = label5.Bottom + groupLabelPadding; // Add extra padding below label5

            // Position button3 (Continue without Membership) below button2
            button3.Left = rightGroupLeft - (button3.Width / 2); // Center it horizontally relative to the right group
            button3.Top = button2.Bottom + paddingVertical;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Add any input validation logic here if necessary
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Add any input validation logic here if necessary
        }

        private void signinbutton1_Click(object sender, EventArgs e)
        {
            string phoneText = textBox1.Text.Trim();  // Phone number field instead of member ID
            string password = textBox2.Text.Trim();

            // Validate input
            if (string.IsNullOrEmpty(phoneText))
            {
                MessageBox.Show("Phone number cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Use Entity Framework to query the database to validate the member
                var member = dbContext?.Members.FirstOrDefault(g => g.Phone == phoneText && g.MbPassword == password);

                if (member == null)
                {
                    MessageBox.Show("Incorrect phone number or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Sign in successful.\n\nWelcome " + member.Name + "!", "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Ensure there is a valid current order
                if (OrderManager.CurrentOrder?.OrderItems == null || OrderManager.CurrentOrder.OrderItems.Count == 0)
                {
                    MessageBox.Show("No items in the order. Please add items before proceeding to checkout.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create and show Checkout form after completing the order
                Checkout checkout = new Checkout(dbContext, serviceProvider, member, OrderManager.CurrentOrder);
                checkout.Show();
                this.Hide();

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }





        private void button2_Click(object sender, EventArgs e)
        {
            if (dbContext == null)
            {
                MessageBox.Show("Database context is not initialized.");
                return;
            }

            // Creating an instance of Membership2 and passing dbContext to it
            MembershipSignUpForm signUpForm = new MembershipSignUpForm(dbContext);
            signUpForm.ShowDialog(); // Display as a modal dialog
        }

        private void Continuewithoutbutton3_Click(object sender, EventArgs e)
        {
            // Check if the current order is empty, prompt user to add items before proceeding to checkout
            if (OrderManager.CurrentOrder == null || OrderManager.CurrentOrder.OrderItems == null || OrderManager.CurrentOrder.OrderItems.Count == 0)
            {
                MessageBox.Show("Please add items to your order before proceeding to checkout.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Ensure dbContext is initialized properly
                if (dbContext == null)
                {
                    MessageBox.Show("Database context is not initialized.");
                    return;
                }

                // Fetch order items from the database using the OrderID
                var orderId = OrderManager.CurrentOrder.OrderID;

                // Fetching the related order items by using OrderID
                var orderItems = dbContext.OrderItems
                    .Where(item => item.OrderID == orderId)
                    .Include(item => item.Foodandbev)  // Including the related Foodandbev information
                    .ToList();

                if (orderItems == null || orderItems.Count == 0)
                {
                    MessageBox.Show("No order items found in the current order.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update the current order's items with the fetched items
                OrderManager.CurrentOrder.OrderItems = orderItems;

                // Proceed to create the Checkout form without any membership information
                Checkout checkout = new Checkout(dbContext, serviceProvider, null, OrderManager.CurrentOrder);

                checkout.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }



    }
}

