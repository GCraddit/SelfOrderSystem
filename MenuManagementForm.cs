using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Giles_Chen_test_1
{
    public partial class MenuManagementForm : Form
    {
        private readonly CafeContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private DataGridView dataGridViewMenuItems;

        public MenuManagementForm(CafeContext dbContext, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            InitializeDataGridView();
            LoadMenuItems();
            InitializeReturnButton();
        }

        private void MenuManagementForm_Load(object sender, EventArgs e)
        {
            if (_dbContext == null)
            {
                throw new ArgumentNullException(nameof(_dbContext), "The provided DbContext cannot be null.");
            }
        }

        private void InitializeDataGridView()
        {
            dataGridViewMenuItems = new DataGridView
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(800, 500),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                RowTemplate = { Height = 120 },
                AllowUserToAddRows = false
            };

            dataGridViewMenuItems.Columns.Add(new DataGridViewImageColumn
            {
                DataPropertyName = "foodandbevImage",
                HeaderText = "Image",
                Width = 150,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            });
            dataGridViewMenuItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "foodandbevName",
                HeaderText = "Name",
                Width = 200
            });
            dataGridViewMenuItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "foodandbevPrice",
                HeaderText = "Price",
                Width = 100
            });
            dataGridViewMenuItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "foodandbevDescription",
                HeaderText = "Description",
                Width = 300
            });
            dataGridViewMenuItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Width = 150
            });

            // Add DataGridView to the form
            this.Controls.Add(dataGridViewMenuItems);

            // Add buttons
            Button btnAddItem = new Button
            {
                Text = "Add Item",
                Location = new System.Drawing.Point(850, 50),
                Size = new System.Drawing.Size(120, 40)
            };
            btnAddItem.Click += BtnAddItem_Click;
            this.Controls.Add(btnAddItem);

            Button btnDeleteItem = new Button
            {
                Text = "Delete Item",
                Location = new System.Drawing.Point(850, 100),
                Size = new System.Drawing.Size(120, 40)
            };
            btnDeleteItem.Click += BtnDeleteItem_Click;
            this.Controls.Add(btnDeleteItem);
        }

        private void InitializeReturnButton()
        {
            Button btnReturn = new Button
            {
                Text = "Return",
                Location = new System.Drawing.Point(850, 150),
                Size = new System.Drawing.Size(120, 40)
            };
            btnReturn.Click += BtnReturn_Click;
            this.Controls.Add(btnReturn);
        }

        private void LoadMenuItems()
        {

            if (_dbContext == null)
            {
                MessageBox.Show("Database context is not initialized.");
                return;
            }

  
            var foodItems = _dbContext.Foodandbevs.ToList();
            foreach (var item in foodItems)
            {
                if (!string.IsNullOrEmpty(item.foodandbevImagePath) && System.IO.File.Exists(item.foodandbevImagePath))
                {
                    item.foodandbevImage = Image.FromFile(item.foodandbevImagePath);
                }
                else
                {
                    item.foodandbevImage = Image.FromFile("path/to/default/image.jpg"); // Use a default image path
                }
            }
            dataGridViewMenuItems.DataSource = new BindingList<Foodandbev>(foodItems);
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {

            AddMenuItemForm addMenuItemForm = new AddMenuItemForm(_dbContext);
            addMenuItemForm.ShowDialog();
            LoadMenuItems(); 
        }

        private void BtnDeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewMenuItems.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridViewMenuItems.SelectedRows)
                {
                    var foodandbev = row.DataBoundItem as Foodandbev;
                    if (foodandbev != null)
                    {
                        _dbContext.Foodandbevs.Remove(foodandbev);
                    }
                }
                _dbContext.SaveChanges();
                LoadMenuItems(); 
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            var mainForm = _serviceProvider.GetService<WelcomeForm>();
            if (mainForm == null)
            {
                MessageBox.Show("Unable to return to the main form. Form1 could not be initialized.");
                return;
            }
            mainForm.Show();
            this.Close();
        }
    }
}




