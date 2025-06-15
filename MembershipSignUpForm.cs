using Giles_Chen_test_1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

namespace Giles_Chen_test_1
{
    public partial class MembershipSignUpForm : Form
    {
        private readonly CafeContext dbContext;

        // Constructor accepting dbContext via Dependency Injection
        public MembershipSignUpForm(CafeContext dbContext)
        {
            InitializeComponent();
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            //MessageBox.Show("Database Context Initialized: " + (this.dbContext != null));

            this.MinimumSize = new Size(500, 500);
            this.Size = this.MinimumSize;

            this.Load += MembershipSignUp_Load;
            this.Resize += MembershipSignUp_Resize; // Handle resize
        }

        private void MembershipSignUp_Load(object sender, EventArgs e)
        {
            AdjustLayoutPosition(); // Call layout adjuster on form load
        }

        private void MembershipSignUp_Resize(object sender, EventArgs e)
        {
            AdjustLayoutPosition(); // Call layout adjuster on resize
        }

        private void AdjustLayoutPosition()
        {
            // Define padding between controls
            int paddingVertical = 15;
            int paddingHorizontal = 40; // Increase the horizontal padding for more space between labels and textboxes

            // Find the maximum width between the labels (label2, label3, label4) to align them uniformly
            int maxLabelWidth = Math.Max(label2.Width, Math.Max(label3.Width, label4.Width));

            // Center the title (label1)
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label1.Top = 100;

            // Position label2 (Name:) and textBox1
            label2.Left = (this.ClientSize.Width - maxLabelWidth - paddingHorizontal - textBox1.Width) / 2;
            label2.Top = label1.Bottom + paddingVertical;
            textBox1.Left = label2.Right + paddingHorizontal; // Right of the label
            textBox1.Top = label2.Top; // Align vertically with label2

            // Position label3 (Mobile:) and textBox2
            label3.Left = label2.Left; // Keep the same left alignment as label2
            label3.Top = label2.Bottom + paddingVertical;
            textBox2.Left = textBox1.Left; // Keep the same left alignment as textBox1
            textBox2.Top = label3.Top; // Align vertically with label3

            // Position label4 (Password:) and textBox3
            label4.Left = label2.Left; // Keep the same left alignment as label2
            label4.Top = label3.Bottom + paddingVertical;
            textBox3.Left = textBox1.Left; // Keep the same left alignment as textBox1
            textBox3.Top = label4.Top; // Align vertically with label4

            // Position button1 (Sign Up Button)
            button1.Left = (this.ClientSize.Width - button1.Width) / 2; // Center it horizontally
            button1.Top = textBox3.Bottom + paddingVertical;
        }



        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            string phoneNumber = textBox2.Text.Trim();
            string password = textBox3.Text.Trim();

            //MessageBox.Show($"Name: {name}, Phone: {phoneNumber}, Password: {password}");

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("All fields must be filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                
                if (dbContext == null)
                {
                    MessageBox.Show("Database context is not initialized.", "Database Error");
                    return;
                }

                
                var existingMember = dbContext.Members.FirstOrDefault(m => m.Phone == phoneNumber);
                if (existingMember != null)
                {
                    MessageBox.Show("A member with this phone number already exists. Please try agaon with different input", "Duplicate Member", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

               
                var newMember = new Member
                {
                    MemberId = int.Parse(phoneNumber),  
                    Phone = phoneNumber,
                    MbPassword = password,
                    Name = name,
                    Point = 0
                };

                dbContext.Members.Add(newMember);

                dbContext.SaveChanges();
                var savedMember = dbContext.Members.FirstOrDefault(g => g.Phone == phoneNumber);
                if (savedMember != null)
                {
                    MessageBox.Show("Sign up successful!", "Sign Up Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Member not found after saving.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the entity changes. See the inner exception for details.\n" + (ex.InnerException?.Message ?? ex.Message), "Error");
            }
        }

    }
}




