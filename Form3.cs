using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Giles_Chen_test_1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            
            Button btnResizeImages = new Button();
            btnResizeImages.Text = "Resize Images";
            btnResizeImages.Location = new Point(20, 20); 
            this.Controls.Add(btnResizeImages);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void btnResizeImages_Click(object sender, EventArgs e)
        {
            string inputDirectory = @"F:\local\Giles Chen test 1\images"; 
            string outputDirectory = @"F:\local\Giles Chen test 1\resized_images"; 

            int targetWidth = 200;  
            int targetHeight = 200;

            ImageResizer resizer = new ImageResizer(inputDirectory, outputDirectory, targetWidth, targetHeight);
            resizer.ResizeAllImages();

            MessageBox.Show("Image resizing completed."); 
        }
    }
}
