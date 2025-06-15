using Giles_Chen_test_1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Giles_Chen_test_1
{
    public partial class AddMerchForm : Form
    {
        private Label titleLabel;
        private Label imageLabel;
        private PictureBox merchPictureBox;
        private Button selectButton;
        private Label nameLabel;
        private TextBox nameTextBox;
        private Label descriptionLabel;
        private TextBox descriptionTextBox;
        private Label pointsLabel;
        private TextBox pointsTextBox;
        private Button submitButton;
        private readonly CafeContext _dbContext;

        public AddMerchForm(CafeContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            InitializeFormComponents();
            AdjustElementPosition();
        }

        public void InitializeFormComponents()
        {
            this.MinimumSize = new Size(700, 700);
            this.Size = new Size(700, 700);
            this.Text = "Add Merch";

            // Title Label
            titleLabel = new Label();
            titleLabel.Text = "Add Merch";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.AutoSize = true;
            this.Controls.Add(titleLabel);

            // Merch Image
            imageLabel = new Label();
            imageLabel.Text = "Image: ";
            imageLabel.AutoSize = true;
            this.Controls.Add(imageLabel);

            merchPictureBox = new PictureBox();
            merchPictureBox.Size = new Size(200, 200);
            merchPictureBox.BorderStyle = BorderStyle.FixedSingle;
            merchPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(merchPictureBox);

            selectButton = new Button();
            selectButton.Text = "Upload Image";
            selectButton.Size = new Size(140, 40); ;
            selectButton.Click += (sender, e) => UploadImage(merchPictureBox);
            this.Controls.Add(selectButton);

            // Merch Name
            nameLabel = new Label();
            nameLabel.Text = "Merch Name:";
            nameLabel.AutoSize = true;
            this.Controls.Add(nameLabel);

            nameTextBox = new TextBox();
            nameTextBox.Size = new Size(400, 50);
            this.Controls.Add(nameTextBox);

            // Merch Description
            descriptionLabel = new Label();
            descriptionLabel.Text = "Merch Description:";
            descriptionLabel.AutoSize = true;
            this.Controls.Add(descriptionLabel);

            descriptionTextBox = new TextBox();
            descriptionTextBox.Size = new Size(400, 50);
            this.Controls.Add(descriptionTextBox);

            // Points Field (integer for merch points)
            pointsLabel = new Label();
            pointsLabel.Text = "Merch Points:";
            pointsLabel.AutoSize = true;
            this.Controls.Add(pointsLabel);

            pointsTextBox = new TextBox();
            pointsTextBox.Size = new Size(400, 50);
            this.Controls.Add(pointsTextBox);

            // Generate random unique ID for the merch item
            int randomisedID = GenerateUniqueRandomID();

            // Submit Button
            submitButton = new Button();
            submitButton.Text = "Submit";
            submitButton.Size = new Size(100, 40);
            submitButton.Click += (sender, e) => SaveMerch(randomisedID, nameTextBox, descriptionTextBox, pointsTextBox, merchPictureBox);
            this.Controls.Add(submitButton);

            AdjustElementPosition();
        }

        private void SaveMerch(int id, TextBox txtName, TextBox txtDescription, TextBox txtPoints, PictureBox merchPictureBox)
        {
            // Validate empty inputs
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtDescription.Text) ||
                string.IsNullOrWhiteSpace(txtPoints.Text) ||
                merchPictureBox.Image == null)
            {
                MessageBox.Show("Please fill in all fields and ensure an image is selected.", "Empty Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate that points is a valid integer
            if (!int.TryParse(txtPoints.Text, out int points))
            {
                MessageBox.Show("Points must be a valid integer.", "Wrong Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create a new Merch object and set its properties
            Merch newMerch = new Merch
            {
                MerchID = id,
                MerchName = txtName.Text,
                MerchDescription = txtDescription.Text,
                MerchPoints = points,
                MerchImagePath = HandleImageSaving(merchPictureBox)
            };

            if (string.IsNullOrEmpty(newMerch.MerchImagePath))
            {
                return; // If image handling fails, stop the process.
            }

            // Add the new merch to the database using Entity Framework
            try
            {
                _dbContext.Merches.Add(newMerch);
                _dbContext.SaveChanges();
                MessageBox.Show($"'{newMerch.MerchName}' has been successfully added to the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;  // Close form and signal success
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving to the database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string HandleImageSaving(PictureBox merchPictureBox)
        {
            // Assuming the application has a more central way to handle images:
            string imagesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyApplicationImages");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            string sourcePath = merchPictureBox.Tag?.ToString();
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                MessageBox.Show("Selected image file does not exist.", "Empty Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string extension = Path.GetExtension(sourcePath);
            string uniqueImageName = $"{Guid.NewGuid()}{extension}";
            string destinationPath = Path.Combine(imagesFolder, uniqueImageName);

            try
            {
                File.Copy(sourcePath, destinationPath, overwrite: true);
                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving the image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }




        private int GenerateUniqueRandomID()
        {
            Random random = new Random();
            int newID;
            bool idExists;

            do
            {
                newID = random.Next(1, 1000); // Random between 1 to 9999
                                              // Check if merchID exists in the database
                idExists = CheckIfIDExistsInDatabase(newID);
            }
            while (idExists);

            return newID;
        }
        private bool CheckIfIDExistsInDatabase(int id)
        {
            // Assuming _dbContext is an instance of your DbContext available in this class.
            return _dbContext.Merches.Any(m => m.MerchID == id);
        }

        private void UploadImage(PictureBox pictureBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                pictureBox.Tag = openFileDialog.FileName;  // Store the path temporarily
            }
        }

        private void SubmitMerch(object sender, EventArgs e)
        {
            // Validate input and initialize new Merch object
            if (!ValidateInputs())
                return;

            var newMerch = new Merch
            {
                MerchName = nameTextBox.Text,
                MerchDescription = descriptionTextBox.Text,
                MerchPoints = int.Parse(pointsTextBox.Text),
                MerchImagePath = SaveImageToFileSystem(merchPictureBox.Tag?.ToString())
            };

            // Add to DbContext and save changes
            _dbContext.Add(newMerch);
            _dbContext.SaveChanges();

            MessageBox.Show("Merch added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private bool ValidateInputs()
        {
            // Validate all inputs similarly as in the original form
            return true;
        }

        private string SaveImageToFileSystem(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return null;

            string destPath = Path.Combine(Application.StartupPath, "Images", Path.GetFileName(imagePath));
            File.Copy(imagePath, destPath, true);
            return destPath;
        }

        private void AdjustElementPosition()
        {
            int margin = 15; // Margin space between controls

            titleLabel.Location = new Point(margin, margin);

            imageLabel.Location = new Point(margin, titleLabel.Bottom + 20);
            merchPictureBox.Location = new Point(margin, imageLabel.Bottom + 5);
            selectButton.Location = new Point(margin, merchPictureBox.Bottom + 10);

            nameLabel.Location = new Point(margin, selectButton.Bottom + 20);
            nameTextBox.Location = new Point(240, nameLabel.Top);

            descriptionLabel.Location = new Point(margin, nameTextBox.Bottom + 10);
            descriptionTextBox.Location = new Point(240, descriptionLabel.Top);

            pointsLabel.Location = new Point(margin, descriptionTextBox.Bottom + 10);
            pointsTextBox.Location = new Point(240, pointsLabel.Top);

            submitButton.Location = new Point(240, pointsTextBox.Bottom + 20);

            // Optionally, adjust the submit button to center it below the points text box
            submitButton.Location = new Point(240, pointsTextBox.Bottom + 20);
        }

        private void AddMerchForm_Resize(object sender, EventArgs e)
        {
            AdjustElementPosition(); // Call adjust function on form resize to maintain layout
        }

        private void AddMerchForm_Load(object sender, EventArgs e)
        {
            // Initialization code here
            // For example, you might want to set default values or load data into controls.
        }

    }
}
