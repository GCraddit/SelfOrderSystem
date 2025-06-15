using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace Giles_Chen_test_1
{
    public partial class StaffMenuForm : Form
    {
        private Label titleLabel;
        private Label subTitleLabel; // Product
        private Label subTitleLabel2; // Merch
        private Button logoutButton;
        private Button addProductButton;
        private Button addMerchButton;
        private DataGridView productsDataGridView;
        private DataGridView merchDataGridView;
        private readonly CafeContext dbContext;

        public StaffMenuForm(CafeContext dbContext)
        {
            InitializeComponent();
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            //MessageBox.Show("StaffMenu Initialized");

            InitializeLayout();
            ReloadDataScreen();
        }

        private void InitializeLayout()
        {
            // Setting a larger form size for better usability
            this.MinimumSize = new Size(1600, 900);
            this.Text = "Staff Menu";

            // Initialize UI components
            InitializeLabels();
            InitializeButtons();
            InitializeDataGridViews();

            // Add Delete button columns
            AddDeleteButtonColumn(productsDataGridView);
            AddDeleteButtonColumn(merchDataGridView);

            // Bind events
            productsDataGridView.CellClick += productsDataGridView_CellClick;
            merchDataGridView.CellClick += merchDataGridView_CellClick;
            this.Resize += StaffLoginScreen_Resize;
            this.FormClosing += StaffMenuForm_FormClosing;

            AdjustElementPosition();
        }

        private void InitializeLabels()
        {
            titleLabel = CreateLabel("Staff Menu", 20, FontStyle.Bold);
            subTitleLabel = CreateLabel("Product List", 12, FontStyle.Bold);
            subTitleLabel2 = CreateLabel("Merch", 12, FontStyle.Bold);

            this.Controls.Add(titleLabel);
            this.Controls.Add(subTitleLabel);
            this.Controls.Add(subTitleLabel2);
        }

        private void InitializeButtons()
        {
            logoutButton = CreateButton("Logout", 100, 40, LogoutButton_Click);
            addProductButton = CreateButton("Add Product", 75, 40, AddProductButton_Click);
            addMerchButton = CreateButton("Add Merch", 75, 40, AddMerchButton_Click);

            this.Controls.Add(logoutButton);
            this.Controls.Add(addProductButton);
            this.Controls.Add(addMerchButton);
        }

        private void InitializeDataGridViews()
        {
            productsDataGridView = CreateDataGridView();
            merchDataGridView = CreateDataGridView();

            productsDataGridView.Columns.Add(CreateTextBoxColumn("ID"));
            productsDataGridView.Columns.Add(CreateImageColumn("Image"));
            productsDataGridView.Columns.Add(CreateTextBoxColumn("Name"));
            productsDataGridView.Columns.Add(CreateTextBoxColumn("Description"));
            productsDataGridView.Columns.Add(CreateTextBoxColumn("Price"));
            productsDataGridView.Columns.Add(CreateTextBoxColumn("Type"));

            merchDataGridView.Columns.Add(CreateTextBoxColumn("ID"));
            merchDataGridView.Columns.Add(CreateImageColumn("Image"));
            merchDataGridView.Columns.Add(CreateTextBoxColumn("Name"));
            merchDataGridView.Columns.Add(CreateTextBoxColumn("Description"));
            merchDataGridView.Columns.Add(CreateTextBoxColumn("Points"));

            this.Controls.Add(productsDataGridView);
            this.Controls.Add(merchDataGridView);
        }

        private Label CreateLabel(string text, int fontSize, FontStyle fontStyle)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Arial", fontSize, fontStyle),
                AutoSize = true
            };
        }

        private Button CreateButton(string text, int width, int height, EventHandler clickEvent)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(width, height)
            };
            button.Click += clickEvent;

            return button;
        }


        private DataGridView CreateDataGridView()
        {
            return new DataGridView
            {
                Size = new Size(300, 150),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToResizeColumns = false,
                AllowUserToAddRows = false,
                RowTemplate = { Height = 100 },
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True }
            };
        }

        private DataGridViewTextBoxColumn CreateTextBoxColumn(string name)
        {
            return new DataGridViewTextBoxColumn { Name = name };
        }

        private DataGridViewImageColumn CreateImageColumn(string name)
        {
            return new DataGridViewImageColumn
            {
                Name = name,
                HeaderText = name,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
        }

        private void AddDeleteButtonColumn(DataGridView dataGridView)
        {
            if (dataGridView.Columns["Delete"] == null)
            {
                DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn
                {
                    Name = "Delete",
                    HeaderText = "Delete",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.Red, ForeColor = Color.White }
                };
                dataGridView.Columns.Add(deleteButtonColumn);
            }
        }

        private void ReloadDataScreen()
        {
            LoadMenuItems();
            LoadMerchDataToGridView();
        }

        private void LoadMenuItems()
        {
            if (dbContext == null)
            {
                MessageBox.Show("Database context is not initialized.", "Error");
                return;
            }

            try
            {
                var foodItems = dbContext.Foodandbevs.ToList();
                productsDataGridView.Rows.Clear();

                foreach (var item in foodItems)
                {
                    productsDataGridView.Rows.Add(item.FoodandbevID, LoadImage(item.foodandbevImagePath), item.foodandbevName, item.foodandbevDescription, item.foodandbevPrice, item.Type);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}", "Error");
            }
        }

        private void LoadMerchDataToGridView()
        {
            try
            {
                var merchItems = dbContext.Merches.ToList();
                merchDataGridView.Rows.Clear();

                foreach (var item in merchItems)
                {
                    merchDataGridView.Rows.Add(item.MerchID, LoadImage(item.MerchImagePath), item.MerchName, item.MerchDescription, item.MerchPoints);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred while loading merch items: {ex.Message}", "Error");
            }
        }

        private Image LoadImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                return null;

            try
            {
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
                return null;
            }
        }

        private void DeleteProductFromDatabase(int productId)
        {
            var product = dbContext.Foodandbevs.FirstOrDefault(p => p.FoodandbevID == productId);
            if (product != null)
            {
                dbContext.Foodandbevs.Remove(product);
                dbContext.SaveChanges();
                MessageBox.Show("Product successfully deleted.");
                LoadMenuItems();
            }
            else
            {
                MessageBox.Show("Product not found.");
            }
        }

        private void DeleteMerchFromDatabase(int merchId)
        {
            var merchItem = dbContext.Merches.FirstOrDefault(m => m.MerchID == merchId);
            if (merchItem == null)
            {
                MessageBox.Show("Merch not found in the database.");
                return;
            }

            if (!string.IsNullOrEmpty(merchItem.MerchImagePath) && File.Exists(merchItem.MerchImagePath))
            {
                try
                {
                    File.Delete(merchItem.MerchImagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting image: {ex.Message}");
                    return;
                }
            }

            dbContext.Merches.Remove(merchItem);
            dbContext.SaveChanges();
            LoadMerchDataToGridView();
            MessageBox.Show("Merch successfully deleted from the database.");
        }

        private void productsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == productsDataGridView.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                int productId = Convert.ToInt32(productsDataGridView.Rows[e.RowIndex].Cells["ID"].Value);
                DeleteProductFromDatabase(productId);
                ReloadDataScreen();
            }
        }

        private void merchDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == merchDataGridView.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                int merchId = Convert.ToInt32(merchDataGridView.Rows[e.RowIndex].Cells["ID"].Value);
                DeleteMerchFromDatabase(merchId);
                ReloadDataScreen();
            }
        }

        private void AddProductButton_Click(object sender, EventArgs e)
        {
            using (AddProductForm addProductForm = new AddProductForm(dbContext))
            {
                if (addProductForm.ShowDialog() == DialogResult.OK)
                {

                    LoadMenuItems();
                }
            }
        }

        private void AddMerchButton_Click(object sender, EventArgs e)
        {
            using (AddMerchForm addMerchForm = new AddMerchForm(dbContext))
            {
                if (addMerchForm.ShowDialog() == DialogResult.OK)
                {

                    LoadMerchDataToGridView();
                }
            }
        }

        private bool isLoggingOut = false;

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                isLoggingOut = true;
                WelcomeForm mainForm = Application.OpenForms["WelcomeForm"] as WelcomeForm;
                if (mainForm != null)
                {
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show("Welcome screen not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            }
        }

        private void StaffLoginScreen_Resize(object sender, EventArgs e)
        {
            AdjustElementPosition();
        }

        private void AdjustElementPosition()
        {
            logoutButton.Location = new Point(this.ClientSize.Width - logoutButton.Width - 15, 15);
            titleLabel.Location = new Point(15, 15);
            subTitleLabel.Location = new Point(15, titleLabel.Bottom + 10);
            addProductButton.Location = new Point(this.ClientSize.Width - addProductButton.Width - 15, subTitleLabel.Top);
            productsDataGridView.Location = new Point(15, subTitleLabel.Bottom + 20);
            productsDataGridView.Width = this.ClientSize.Width - 30;
            productsDataGridView.Height = (this.ClientSize.Height - productsDataGridView.Top) / 3;
            subTitleLabel2.Location = new Point(15, productsDataGridView.Bottom + 20);
            addMerchButton.Location = new Point(this.ClientSize.Width - addMerchButton.Width - 15, subTitleLabel2.Top);
            merchDataGridView.Location = new Point(15, subTitleLabel2.Bottom + 20);
            merchDataGridView.Width = this.ClientSize.Width - 30;
            merchDataGridView.Height = productsDataGridView.Height;
        }

        private void StaffMenu_Load(object sender, EventArgs e)
        {

        }

        private void StaffMenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the form is closing due to a logout action, do not terminate the application
            if (!isLoggingOut && e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();  // Terminate the application only if it's not a logout
            }
        }

    }
}
