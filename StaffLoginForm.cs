using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Giles_Chen_test_1
{
    public partial class StaffLoginForm : Form
    {
        private Label titleLabel;
        private Label idLabel;
        private Label passwordLabel;
        private TextBox idTextBox;
        private TextBox passwordTextBox;
        private Button loginButton;
        private readonly CafeContext dbContext;
        private readonly IServiceProvider serviceProvider;
        private readonly CafeContext _dbContext;

        public bool isLoggedInStaff { get; set; } = false;  // Track login status

        public StaffLoginForm(IServiceProvider serviceProvider, CafeContext dbContext)
        {
            InitializeComponent();
            InitializeLayout();
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            MerchModel merchModel = new MerchModel(_dbContext);
            var merchItems = merchModel.GetMerchItems();
        }

        private void InitializeLayout()
        {
            this.MinimumSize = new Size(600, 400);
            this.Text = "Staff Login";

            // Title Label
            titleLabel = new Label
            {
                Text = "Staff Login",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 150) / 2, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(titleLabel);

            // Staff ID Label
            idLabel = new Label
            {
                Text = "Staff ID:",
                Font = new Font("Arial", 12),
                AutoSize = true
            };
            this.Controls.Add(idLabel);

            // Staff ID TextBox
            idTextBox = new TextBox
            {
                Width = 250
            };
            this.Controls.Add(idTextBox);

            // Password Label
            passwordLabel = new Label
            {
                Text = "Password:",
                Font = new Font("Arial", 12),
                AutoSize = true
            };
            this.Controls.Add(passwordLabel);

            // Password TextBox
            passwordTextBox = new TextBox
            {
                Width = 250,
                PasswordChar = '*'
            };
            this.Controls.Add(passwordTextBox);

            // Login Button
            loginButton = new Button
            {
                Text = "Login",
                Size = new Size(100, 40),
                Location = new Point((this.ClientSize.Width - 100) / 2, 220),
                Anchor = AnchorStyles.Top
            };
            loginButton.Click += LoginButton_Click;
            this.Controls.Add(loginButton);

            // Adjust element positions if form size changes
            this.Resize += StaffLogin_Resize;

            // Center elements initially
            AdjustElementPosition();
            this.FormClosing += StaffLoginForm_FormClosing;

        }

        private void StaffLogin_Resize(object sender, EventArgs e)
        {
            AdjustElementPosition();
        }

        private void AdjustElementPosition()
        {
            int titleLabelX = (this.ClientSize.Width - titleLabel.Width) / 2;
            int titleLabelY = 20;
            titleLabel.Location = new Point(titleLabelX, titleLabelY);

            // Position Staff ID label and TextBox
            int idLabelX = (this.ClientSize.Width - idTextBox.Width) / 2 - idLabel.Width - 10;  // 10px space between label and TextBox
            int idLabelY = titleLabel.Location.Y + titleLabel.Height + 40;
            idLabel.Location = new Point(idLabelX, idLabelY);

            int idTextBoxX = (this.ClientSize.Width - idTextBox.Width) / 2;
            int idTextBoxY = idLabel.Location.Y;
            idTextBox.Location = new Point(idTextBoxX, idTextBoxY);

            // Position Password label and TextBox
            int passwordLabelX = (this.ClientSize.Width - passwordTextBox.Width) / 2 - passwordLabel.Width - 10;  // 10px space between label and TextBox
            int passwordLabelY = idTextBox.Location.Y + idTextBox.Height + 20;
            passwordLabel.Location = new Point(passwordLabelX, passwordLabelY);

            int passwordTextBoxX = (this.ClientSize.Width - passwordTextBox.Width) / 2;
            int passwordTextBoxY = passwordLabel.Location.Y;
            passwordTextBox.Location = new Point(passwordTextBoxX, passwordTextBoxY);

            // Center the Login button below the password input
            int loginButtonX = (this.ClientSize.Width - loginButton.Width) / 2;
            int loginButtonY = passwordTextBox.Location.Y + passwordTextBox.Height + 30;
            loginButton.Location = new Point(loginButtonX, loginButtonY);
        }


        private bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(idTextBox.Text) || string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Please enter both a staff ID and a password.", "Error");
                return false;
            }
            if (!int.TryParse(idTextBox.Text.Trim(), out _))
            {
                MessageBox.Show("Staff ID must be an integer!", "Error");
                return false;
            }
            return true;
        }

        private void StaffLogin_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("StaffLogin Form is loaded");
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            var staffId = int.Parse(idTextBox.Text.Trim());
            var password = passwordTextBox.Text.Trim();

            var staff = dbContext.Staffs.FirstOrDefault(s => s.staffID == staffId && s.password == password);

            if (!IsValid())
            {
                return;
            }

            if (staff != null)
            {
                isLoggedInStaff = true;
                MessageBox.Show("Login successful", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);


                StaffMenuForm staffMenu = serviceProvider.GetService<StaffMenuForm>();

                if (staffMenu == null)
                {
                    MessageBox.Show("StaffMenu could not be initialized.", "Error");
                    return;
                }

                this.Hide();
                staffMenu.Show();
            }
            else
            {
                MessageBox.Show("Incorrect StaffID or Password! Please try again.", "Invalid Credentials");
            }
        }


        private void StaffLoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (e.CloseReason == CloseReason.UserClosing)
            {

                Application.Exit();
            }
        }
    }
}

