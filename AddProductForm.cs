using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Giles_Chen_test_1
{
    public partial class AddProductForm : Form
    {
        private readonly CafeContext dbContext;
        private string selectedImagePath;
        private Label titleLabel;
        private Label imageLabel;
        private PictureBox productPictureBox;
        private Button uploadImageButton;
        private Label nameLabel;
        private TextBox nameTextBox;
        private Label descriptionLabel;
        private TextBox descriptionTextBox;
        private Label priceLabel;
        private TextBox priceTextBox;
        private Label typeLabel;
        private ComboBox typeComboBox;
        private Button saveButton;

        public AddProductForm(CafeContext context)
        {
            InitializeComponent();
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
            InitializeAddItemLayout();
        }

        private void InitializeAddItemLayout()
        {
            // Initial setup
            this.Text = "Add Product";
            this.Size = new Size(700, 700);

            // Title
            titleLabel = new Label
            {
                Text = "Add Product",
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true
            };
            this.Controls.Add(titleLabel);

            // Image
            imageLabel = new Label
            {
                Text = "Image:",
                AutoSize = true
            };
            this.Controls.Add(imageLabel);

            productPictureBox = new PictureBox
            {
                Size = new Size(200, 200),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(productPictureBox);

            uploadImageButton = new Button
            {
                Text = "Upload Image",
                Size = new Size(140, 40)
            };
            uploadImageButton.Click += (sender, e) => BtnSelectImage_Click(productPictureBox);
            this.Controls.Add(uploadImageButton);

            // Name
            nameLabel = new Label
            {
                Text = "Name:",
                AutoSize = true
            };
            this.Controls.Add(nameLabel);

            nameTextBox = new TextBox
            {
                Width = 300
            };
            this.Controls.Add(nameTextBox);

            // Description
            descriptionLabel = new Label
            {
                Text = "Description:",
                AutoSize = true
            };
            this.Controls.Add(descriptionLabel);

            descriptionTextBox = new TextBox
            {
                Width = 300,
                Height = 60,
                Multiline = true
            };
            this.Controls.Add(descriptionTextBox);

            // Price
            priceLabel = new Label
            {
                Text = "Price:",
                AutoSize = true
            };
            this.Controls.Add(priceLabel);

            priceTextBox = new TextBox
            {
                Width = 100
            };
            this.Controls.Add(priceTextBox);

            // Type
            typeLabel = new Label
            {
                Text = "Type:",
                AutoSize = true
            };
            this.Controls.Add(typeLabel);

            typeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200
            };
            typeComboBox.Items.AddRange(Enum.GetNames(typeof(FBType))); // Assuming FBType enum exists
            this.Controls.Add(typeComboBox);

            // Save to Database Button
            saveButton = new Button
            {
                Text = "Save",
                Size = new Size(100, 40)
            };
            saveButton.Click += (sender, e) => SaveProduct();
            this.Controls.Add(saveButton);

            AdjustElementPosition();
        }

        private void SaveProduct()
        {
            // Validate empty inputs
            if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                string.IsNullOrWhiteSpace(descriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(priceTextBox.Text) ||
                typeComboBox.SelectedItem == null ||
                productPictureBox.Image == null)
            {
                MessageBox.Show("Please fill in all fields and ensure an image is selected.", "Empty Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate that price is a valid decimal
            if (!decimal.TryParse(priceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid decimal value for the price.", "Wrong Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Parse the selected type from the ComboBox to FBType
            FBType selectedType = (FBType)Enum.Parse(typeof(FBType), typeComboBox.SelectedItem.ToString());

            // Create a new Foodandbev object and set its properties
            var newFoodandbev = new Foodandbev(selectedType)
            {
                foodandbevName = nameTextBox.Text,
                foodandbevDescription = descriptionTextBox.Text,
                foodandbevPrice = price
            };

            // Handle image resizing and saving
            if (!string.IsNullOrEmpty(selectedImagePath))
            {
                string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resized_images");

                // Ensure the resized_images directory exists
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Resize the selected image and save it
                ImageResizer resizer = new ImageResizer(Path.GetDirectoryName(selectedImagePath), outputDirectory, 200, 200);
                resizer.ResizeImage(selectedImagePath);

                // Save the resized image path to the database
                newFoodandbev.foodandbevImagePath = Path.Combine(outputDirectory, Path.GetFileName(selectedImagePath));
            }

            // Add the new product to the database
            try
            {
                dbContext.Foodandbevs.Add(newFoodandbev);
                dbContext.SaveChanges();
                MessageBox.Show($"'{newFoodandbev.foodandbevName}' has been successfully added to the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;  // Close form and signal success
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving to the database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdjustElementPosition()
        {
            titleLabel.Location = new Point(15, 15);

            imageLabel.Location = new Point(15, titleLabel.Bottom + 20);
            productPictureBox.Location = new Point(15, imageLabel.Bottom + 5);
            uploadImageButton.Location = new Point(15, productPictureBox.Bottom + 10);

            nameLabel.Location = new Point(15, uploadImageButton.Bottom + 20);
            nameTextBox.Location = new Point(120, nameLabel.Top);

            descriptionLabel.Location = new Point(15, nameTextBox.Bottom + 10);
            descriptionTextBox.Location = new Point(120, descriptionLabel.Top);

            priceLabel.Location = new Point(15, descriptionTextBox.Bottom + 10);
            priceTextBox.Location = new Point(120, priceLabel.Top);

            typeLabel.Location = new Point(15, priceTextBox.Bottom + 10);
            typeComboBox.Location = new Point(120, typeLabel.Top);

            saveButton.Location = new Point(120, typeComboBox.Bottom + 20);
        }

        private void BtnSelectImage_Click(PictureBox picImagePreview)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp; *.gif; *.tiff; *.jfif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff;*.jfif|All Files (*.*)|*.*"; ;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = openFileDialog.FileName;
                    picImagePreview.Image = Image.FromFile(selectedImagePath);

                    // Set the Tag property with the selected image path
                    picImagePreview.Tag = selectedImagePath;
                }
            }
        }


        private void AddProductForm_Load(object sender, EventArgs e)
        {

        }
    }
}


