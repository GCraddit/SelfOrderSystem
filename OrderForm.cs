using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Giles_Chen_test_1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Giles_Chen_test_1
{
    public partial class OrderForm : Form
    {
        private CafeContext dbContext;
        private DataGridView dataGridViewCurrentOrder = new DataGridView();
        private DbSet<Order> Orders => dbContext.Set<Order>();
        private List<OrderItem> currentOrderItems;
        private readonly IServiceProvider serviceProvider;



        public OrderForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider), "Service provider is not initialized.");
            }

            this.serviceProvider = serviceProvider;
            dbContext = serviceProvider.GetService<CafeContext>() ?? throw new ArgumentNullException(nameof(dbContext), "Database context is not initialized properly.");

            currentOrderItems = new List<OrderItem>();
            InitializeLayout(); // Set up the UI
            //PopulateDatabaseWithInitialItems(); 
            this.FormClosing += Form6_FormClosing;
            LoadData();
        }

        private void InitializeLayout()
        {
            this.Text = "Cafe Menu";
            this.Size = new Size(1200, 800);
            this.MinimumSize = new Size(1200, 800);

            // Label for the product list (left side)
            Label productListLabel = new Label
            {
                Text = "Product List",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            this.Controls.Add(productListLabel);

            // DataGridView for the menu (Product List)
            dataGridViewMenu = new DataGridView
            {
                Location = new Point(20, productListLabel.Bottom + 10), // 10px below the product list label
                Size = new Size(720, this.ClientSize.Height - productListLabel.Height - 100), // Dynamically calculate height
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Setting up the DataGridView columns for the product list
            dataGridViewMenu.RowTemplate.Height = 80;

            // Adjusting column widths to fill the available space
            dataGridViewMenu.Columns.Add(new DataGridViewImageColumn
            {
                DataPropertyName = "foodandbevImage",
                HeaderText = "Image",
                Width = 120,  // Set a fixed width for the image column
                ImageLayout = DataGridViewImageCellLayout.Zoom
            });
            dataGridViewMenu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "foodandbevName",
                HeaderText = "Name",
                Width = 150 // Adjust this width
            });
            dataGridViewMenu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "foodandbevPrice",
                HeaderText = "Price",
                Width = 100
            });
            dataGridViewMenu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "foodandbevDescription",
                HeaderText = "Description",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill // This will dynamically adjust to fit remaining space
            });
            dataGridViewMenu.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Width = 100
            });

            this.Controls.Add(dataGridViewMenu);

            // Label for the order summary (right side)
            Label orderSummaryLabel = new Label
            {
                Text = "Order Summary",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(750, 20),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Controls.Add(orderSummaryLabel);

            // DataGridView for the current order (right side)
            dataGridViewCurrentOrder = new DataGridView
            {
                Location = new Point(750, orderSummaryLabel.Bottom + 10), // 10px below the "Order Summary" label
                Size = new Size(400, 400),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 50 },
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Setting up the DataGridView columns for the current order
            dataGridViewCurrentOrder.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "foodandbevName",
                HeaderText = "Name",
                Width = 150
            });
            dataGridViewCurrentOrder.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                HeaderText = "Quantity",
                Width = 100
            });
            dataGridViewCurrentOrder.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalPrice",
                HeaderText = "Total Price",
                Width = 100
            });
            this.Controls.Add(dataGridViewCurrentOrder);

            // Button: Add to Order (below Product List)
            Button btnAddToOrder = new Button
            {
                Text = "Add to Order",
                Location = new Point(20, this.ClientSize.Height - 50), // Keep at bottom-left
                Size = new Size(200, 40), // Set height to 40, width to 100
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnAddToOrder.Click += btnAddFoodandbev_Click;
            this.Controls.Add(btnAddToOrder);

            // Button: View Total Amount (below Order Summary)
            Button btnViewTotalAmount = new Button
            {
                Text = "View Total Amount",
                Location = new Point(750, dataGridViewCurrentOrder.Bottom + 10),
                Size = new Size(100, 40), // Set height to 40, width to 100
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnViewTotalAmount.Click += btnViewTotalAmount_Click;
            this.Controls.Add(btnViewTotalAmount);

            // Button: Next (below the View Total Amount button)
            Button btnCompleteOrder = new Button
            {
                Text = "Next",
                Location = new Point(750, btnViewTotalAmount.Bottom + 10),
                Size = new Size(100, 40), // Set height to 40, width to 100
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnCompleteOrder.Click += btnCompleteOrder_Click;
            this.Controls.Add(btnCompleteOrder);

            // Button: Remove Selected Item (below the Next button)
            Button btnRemoveOrderItem = new Button
            {
                Text = "Remove Selected Item",
                Location = new Point(750, btnCompleteOrder.Bottom + 10),
                Size = new Size(100, 40), // Set height to 40, width to 100
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnRemoveOrderItem.Click += btnRemoveOrderItem_Click;
            this.Controls.Add(btnRemoveOrderItem);
        }



        private void btnRemoveOrderItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewCurrentOrder.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewCurrentOrder.SelectedRows[0];
                var orderItem = selectedRow.DataBoundItem as OrderItem;
                if (orderItem != null)
                {
                    currentOrderItems.Remove(orderItem);
                    UpdateCurrentOrderDataGridView();
                }
            }
            else
            {
                MessageBox.Show("Please select an item to remove.", "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void LoadData()
        {
            var foodandbevs = dbContext.Foodandbevs.ToList();
            string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "defaultImage.png");

            foreach (var item in foodandbevs)
            {
                if (string.IsNullOrEmpty(item.foodandbevName))
                {
                    item.foodandbevName = "Unknown Item";
                }

                if (item.foodandbevPrice == 0)
                {
                    item.foodandbevPrice = 0.00m;
                }

                if (string.IsNullOrEmpty(item.foodandbevDescription))
                {
                    item.foodandbevDescription = "No description available";
                }

                if (string.IsNullOrEmpty(item.foodandbevImagePath) || !System.IO.File.Exists(item.foodandbevImagePath))
                {
                    item.foodandbevImagePath = defaultImagePath;
                }

                try
                {
                    item.foodandbevImage = Image.FromFile(item.foodandbevImagePath);
                }
                catch
                {
                    item.foodandbevImage = Image.FromFile(defaultImagePath);
                }
            }

            dbContext.SaveChanges(); 

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = foodandbevs;
            dataGridViewMenu.DataSource = bindingSource;
        }





        // Button click event to add a food or beverage (example usage)
        private void btnAddFoodandbev_Click(object sender, EventArgs e)
        {
            if (dataGridViewMenu.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridViewMenu.SelectedRows)
                {
                    var foodandbev = row.DataBoundItem as Foodandbev;
                    if (foodandbev != null)
                    {
                        OrderItem existingItem = currentOrderItems.FirstOrDefault(i => i.Foodandbev.FoodandbevID == foodandbev.FoodandbevID);
                        if (existingItem != null)
                        {
                            existingItem.Quantity++;
                        }
                        else
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                OrderItemID = Guid.NewGuid(),
                                Foodandbev = foodandbev,
                                Quantity = 1
                            };
                            currentOrderItems.Add(orderItem);
                        }
                    }
                }
                UpdateCurrentOrderDataGridView();
                MessageBox.Show("Item(s) added to order.", "Added to Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select an item to add.", "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCurrentOrderDataGridView()
        {
            dataGridViewCurrentOrder.DataSource = null;
            dataGridViewCurrentOrder.DataSource = currentOrderItems;
        }

        private void btnCompleteOrder_Click(object sender, EventArgs e)
        {
            if (CompleteOrder())
            {
                
                var membershipForm = new MembershipForm(dbContext, serviceProvider);
                membershipForm.Show();
                
            }
        }


        private bool CompleteOrder()
        {
            if (currentOrderItems == null || currentOrderItems.Count == 0)
            {
                MessageBox.Show("No items in the order. Please add items first.", "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                Order newOrder = new Order
                {
                    OrderID = Guid.NewGuid(),
                    OrderItems = new List<OrderItem>(currentOrderItems),
                    OrderTime = DateTime.Now
                };

                dbContext.Orders.Add(newOrder);
                dbContext.SaveChanges();

                OrderManager.CurrentOrder = newOrder;

                //MessageBox.Show("Order completed successfully!");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
                return false;
            }
        }



        private void btnViewTotalAmount_Click(object sender, EventArgs e)
        {
            if (currentOrderItems == null || currentOrderItems.Count == 0)
            {
                MessageBox.Show("No items in the order. Please add items first.", "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal totalAmount = 0;

            foreach (var item in currentOrderItems)
            {
                totalAmount += item.GetTotalPrice();
            }

            MessageBox.Show($"The total amount for the current order is: ${totalAmount:F2}", "Total Amount", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeReturnButton()
        {
            Button btnReturn = new Button
            {
                Text = "Return to Main Menu",
                Location = new System.Drawing.Point(850, 50),
                Size = new System.Drawing.Size(150, 40)
            };
            btnReturn.Click += BtnReturn_Click;
            this.Controls.Add(btnReturn);
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            var mainForm = serviceProvider.GetService<WelcomeForm>();
            if (mainForm == null)
            {
                MessageBox.Show("Unable to return to the main form. Form1 could not be initialized.");
                return;
            }
            mainForm.Show();
            this.Close();
        }


        private void Form6_Load(object sender, EventArgs e)
        {
            LoadData();
        }


        private void PopulateDatabaseWithInitialItems()
        {
            if (!dbContext.Foodandbevs.Any())
            {
                // Get the default image path relative to the executable's directory
                string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "defaultImage.png");

                var initialItems = new List<Foodandbev>
        {
            new Foodandbev(FBType.Beverage) { foodandbevName = "Americano", foodandbevPrice = 3.00m, foodandbevDescription = "Classic coffee made with espresso", foodandbevImagePath = defaultImagePath },
            new Foodandbev(FBType.Beverage) { foodandbevName = "Latte", foodandbevPrice = 4.00m, foodandbevDescription = "Creamy coffee made with steamed milk", foodandbevImagePath = defaultImagePath },
            new Foodandbev(FBType.Beverage) { foodandbevName = "Orange Juice", foodandbevPrice = 2.50m, foodandbevDescription = "Freshly squeezed orange juice", foodandbevImagePath = defaultImagePath },
            new Foodandbev(FBType.Food) { foodandbevName = "Bagel", foodandbevPrice = 2.00m, foodandbevDescription = "Fresh bagel with cream cheese", foodandbevImagePath = defaultImagePath }
        };

                dbContext.Foodandbevs.AddRange(initialItems);
                dbContext.SaveChanges();
            }
        }


        private void Form6_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (e.CloseReason == CloseReason.UserClosing)
            {
                
                Application.Exit();
            }
        }
    }
}




