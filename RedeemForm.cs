using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Giles_Chen_test_1
{
    public partial class RedeemForm : Form
    {
        private string merchName;
        private string redeemCode;

        public RedeemForm(string merchName, string redeemCode)
        {
            this.merchName = merchName;
            this.redeemCode = redeemCode;

            // Initialize the form
            this.Text = "Redeem Your Merch";
            this.Size = new Size(400, 300);
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;

            // Instruction label
            var instructionLabel = new Label
            {
                Text = $"Please take your {merchName} at our counter or find our staff to give your redeem code: {redeemCode}\nPlease take a photo of your code!",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Padding = new Padding(20)
            };
            this.Controls.Add(instructionLabel);

            // Done button
            var doneButton = new Button
            {
                Text = "Done",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            doneButton.Click += DoneButton_Click;
            this.Controls.Add(doneButton);
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Please take a photo or note the 6-digit redeem code to show to our staff in order to collect the merch!", "Reminder", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void RedeemForm_Load(object sender, EventArgs e)
        {

        }
    }
}
