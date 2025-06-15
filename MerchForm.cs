using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public partial class MerchForm : Form
    {
        private FlowLayoutPanel flowPanel;
        private MerchModel merchModel;
        private readonly CafeContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly Member _loggedInMember;

        public MerchForm(IServiceProvider serviceProvider, CafeContext dbContext, Member loggedInMember)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));
            if (loggedInMember == null)
                throw new ArgumentNullException(nameof(loggedInMember));


            // Initialize fields
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
            _loggedInMember = loggedInMember;
            merchModel = new MerchModel(dbContext);

            // Initialize the form
            this.Text = "Merch Carousel";
            this.MinimumSize = new Size(1000, 400);
            this.MaximumSize = new Size(1000, 400);
            this.Size = this.MinimumSize;

            flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false, // Makes it horizontal scrolling
                FlowDirection = FlowDirection.LeftToRight
            };

            this.Controls.Add(flowPanel);

            LoadMerchItems();
        }

        private void LoadMerchItems()
        {
            List<Merch> merchItems = merchModel.GetMerchItems();

            foreach (var merch in merchItems)
            {
                flowPanel.Controls.Add(CreateMerchCard(merch));
            }
        }

        private Panel CreateMerchCard(Merch merch)
        {
            // Create the card panel
            var card = new Panel
            {
                Size = new Size(200, 300), // Adjust size as needed
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10)
            };

            // Title label
            var titleLabel = new Label
            {
                Text = merch.MerchName,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            card.Controls.Add(titleLabel);

            // Picture box for the image
            var pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom, // Scale image to fit
                Dock = DockStyle.Bottom,
                Height = 150, // Adjust height for image
                Margin = new Padding(15, 0, 15, 0) // Margin to make space between the image and card corner
            };

            // Try to load the image from file, fallback to default image if not found
            try
            {
                if (!string.IsNullOrEmpty(merch.MerchImagePath) && File.Exists(merch.MerchImagePath))
                {
                    pictureBox.Image = Image.FromFile(merch.MerchImagePath); // Load image from file path
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            catch (Exception)
            {
                // Handle file not found by loading the default image
                string defaultImagePath = Path.Combine(Application.StartupPath, "defaultImage.png");
                if (File.Exists(defaultImagePath))
                {
                    pictureBox.Image = Image.FromFile(defaultImagePath); // Fallback to default image
                }
                else
                {
                    MessageBox.Show($"Error: Default image not found at {defaultImagePath}", "Error");
                }
            }

            card.Controls.Add(pictureBox);

            // Description label
            var descriptionLabel = new Label
            {
                Text = merch.MerchDescription,
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 50
            };
            card.Controls.Add(descriptionLabel);

            // Value point label
            var valuePointLabel = new Label
            {
                Text = "Points: " + merch.MerchPoints,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(valuePointLabel);

            // Select button
            var selectButton = new Button
            {
                Text = "Select",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            selectButton.Click += (sender, e) =>
            {
                if (_loggedInMember.Point < merch.MerchPoints)
                {
                    MessageBox.Show($"Insufficient points. You need {merch.MerchPoints} points to redeem this item.", "Insufficient Points", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to redeem {merch.MerchName}?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    // Deduct the points
                    _loggedInMember.Point -= merch.MerchPoints;

                    // Save changes to the database
                    _dbContext.SaveChanges();

                    // Generate a 6-digit redeem code and open RedeemForm
                    string redeemCode = GenerateRedeemCode().ToString();
                    RedeemForm redeemForm = new RedeemForm(merch.MerchName, redeemCode);
                    redeemForm.ShowDialog();

                    MessageBox.Show("Redemption successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            card.Controls.Add(selectButton);

            return card;
        }

        private int GenerateRedeemCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }

        private void MerchForm_Load(object sender, EventArgs e)
        {

        }
    }
}
