using Microsoft.EntityFrameworkCore;
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
    public partial class AddMenuItemForm : Form
    {
        private CafeContext dbContext;
        private string selectedImagePath;
        public AddMenuItemForm(CafeContext context)
        {
            InitializeComponent();
            dbContext = context;
            InitializeAddItemLayout();
        }
        private void InitializeAddItemLayout()
        {
            Label lblName = new Label { Text = "Name:", Location = new System.Drawing.Point(20, 20) };
            TextBox txtName = new TextBox { Location = new System.Drawing.Point(100, 20) };
            Label lblPrice = new Label { Text = "Price:", Location = new System.Drawing.Point(20, 60) };
            TextBox txtPrice = new TextBox { Location = new System.Drawing.Point(100, 60) };
            Label lblDescription = new Label { Text = "Description:", Location = new System.Drawing.Point(20, 100) };
            TextBox txtDescription = new TextBox { Location = new System.Drawing.Point(100, 100) };
            Label lblType = new Label { Text = "Type:", Location = new System.Drawing.Point(20, 140) };
            ComboBox cmbType = new ComboBox { Location = new System.Drawing.Point(100, 140), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(Enum.GetNames(typeof(FBType)));

            Label lblImage = new Label { Text = "Image:", Location = new System.Drawing.Point(20, 180) };
            Button btnSelectImage = new Button { Text = "Select Image", Location = new System.Drawing.Point(100, 180) };
            btnSelectImage.Click += BtnSelectImage_Click;

            Button btnSave = new Button { Text = "Save", Location = new System.Drawing.Point(20, 220) };
            btnSave.Click += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(txtName.Text) && decimal.TryParse(txtPrice.Text, out decimal price) && cmbType.SelectedItem != null)
                {
                    var newFoodandbev = new Foodandbev((FBType)Enum.Parse(typeof(FBType), cmbType.SelectedItem.ToString()))
                    {
                        foodandbevName = txtName.Text,
                        foodandbevPrice = price,
                        foodandbevDescription = txtDescription.Text,
                        foodandbevImagePath = selectedImagePath
                    };

                    // Resize the selected image and save it
                    if (!string.IsNullOrEmpty(selectedImagePath))
                    {
                        string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resized_images");
                        ImageResizer resizer = new ImageResizer(Path.GetDirectoryName(selectedImagePath), outputDirectory, 200, 200);
                        resizer.ResizeImage(selectedImagePath);
                        newFoodandbev.foodandbevImagePath = Path.Combine(outputDirectory, Path.GetFileName(selectedImagePath));
                    }

                    dbContext.Foodandbevs.Add(newFoodandbev);
                    dbContext.SaveChanges();
                    MessageBox.Show("Menu Item Added Successfully", "Success");
                    this.DialogResult = DialogResult.OK; // Indicate success
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter valid details.");
                }
            };

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblPrice);
            this.Controls.Add(txtPrice);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblType);
            this.Controls.Add(cmbType);
            this.Controls.Add(lblImage);
            this.Controls.Add(btnSelectImage);
            this.Controls.Add(btnSave);
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp; *.gif; *.tiff; *.jfif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff;*.jfif|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = openFileDialog.FileName;
                }
            }
        }
    }
}
