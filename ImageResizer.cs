using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Giles_Chen_test_1
{
    public class ImageResizer
    {
        private readonly string _inputDirectory;
        private readonly string _outputDirectory;
        private readonly int _targetWidth;
        private readonly int _targetHeight;

        public ImageResizer(string inputDirectory, string outputDirectory, int targetWidth, int targetHeight)
        {
            _inputDirectory = inputDirectory;
            _outputDirectory = outputDirectory;
            _targetWidth = targetWidth;
            _targetHeight = targetHeight;


            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        public void ResizeAllImages()
        {

            var imageFiles = Directory.GetFiles(_inputDirectory);

            foreach (var imagePath in imageFiles)
            {
                try
                {
                    ResizeImage(imagePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error resizing image {imagePath}: {ex.Message}");
                }
            }
        }

        public void ResizeImage(string imagePath)
        {

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                throw new ArgumentException("The image path is invalid or does not exist.", nameof(imagePath));
            }

          
            using (Image originalImage = Image.FromFile(imagePath))
            {
                using (Bitmap tempImage = new Bitmap(originalImage))
                {
                    
                    using (Bitmap resizedImage = new Bitmap(_targetWidth, _targetHeight))
                    {
                        using (Graphics graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                            
                            graphics.DrawImage(tempImage, 0, 0, _targetWidth, _targetHeight);
                        }

                        
                        string fileName = Path.GetFileName(imagePath);
                        string outputFilePath = Path.Combine(_outputDirectory, fileName);

                        try
                        {
                            
                            if (!Directory.Exists(_outputDirectory))
                            {
                                Directory.CreateDirectory(_outputDirectory);
                            }

                           
                            resizedImage.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            Console.WriteLine($"Successfully resized: {fileName}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error saving image {fileName}: {ex.Message}");
                        }
                    }
                }
            }
        }

    }
}

