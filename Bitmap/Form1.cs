using System.Windows.Forms;
using System.Drawing;

namespace cmaurer
{
    public partial class Form1 : Form
    {
        Bitmap imgOriginal;
        Bitmap imgCopy;
        Random rnd = new Random();
        Boolean isGrayscale = false;

        double uniform1 = 0.0;
        double uniform2 = 0.0;
        double uniform3 = 0.0;

        int randomValue1 = 0;
        int randomValue2 = 0;
        int randomValue3 = 0;

        public Form1()
        {
            InitializeComponent();
            LoadBitmap();
        }

        private void LoadBitmap()
        {
            // Retrieve the image.
            imgOriginal = new Bitmap(@"../../../data/earth.bmp", true);

            UpdateBitmap(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void MakeGrayscale()
        {
            isGrayscale = false;
            ToggleGreyscale();
            imgOriginal = imgCopy;
        }

        private void ToggleGreyscale()
        {
            try
            {
                imgCopy = (Bitmap)imgOriginal.Clone();

                isGrayscale = !isGrayscale;

                if (isGrayscale)
                {
                    int x, y;

                    // Loop through the images pixels to reset color.
                    for (x = 0; x < imgCopy.Width; x++)
                    {
                        for (y = 0; y < imgCopy.Height; y++)
                        {
                            Color pixelColor = imgCopy.GetPixel(x, y);

                            int g = (int)((0.3 * (double)pixelColor.R) + (0.59 * (double)pixelColor.G) + (0.11 * (double)pixelColor.B));
                            Color newColor = Color.FromArgb(g, g, g);

                            imgCopy.SetPixel(x, y, newColor);
                        }
                    }
                }

                // Set the PictureBox to display the image.
                pbImage.Image = imgCopy;

            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error." +
                    "Check the path to the image file.");
            }

        }
        private void UpdateBitmap(double amount = 100)
        {
            try
            {
                imgCopy = (Bitmap)imgOriginal.Clone();

                int x, y;

                // Loop through the images pixels to reset color.
                for (x = 0; x < imgCopy.Width; x++)
                {
                    for (y = 0; y < imgCopy.Height; y++)
                    {
                        Color pixelColor = imgCopy.GetPixel(x, y);

                        uniform1 = uniform();
                        uniform2 = uniform();
                        uniform3 = uniform();

                        // values between (-0.5..+0,5) * (0..100)
                        randomValue1 = Math.Max((int)(uniform1 * amount), 0);
                        randomValue2 = Math.Max((int)(uniform2 * amount), 0);
                        randomValue3 = Math.Max((int)(uniform3 * amount), 0);

                        int r = (pixelColor.R + randomValue1) % 256;
                        int g = (pixelColor.G + randomValue2) % 256;
                        int b = (pixelColor.B + randomValue3) % 256;

                        /*
                        r = Math.Min(pixelColor.R + randomValue1, 255);
                        g = Math.Min(pixelColor.G + randomValue2, 255);
                        b = Math.Min(pixelColor.B + randomValue3, 255);
                        */

                        Color newColor = Color.FromArgb(r, g, b);
                        imgCopy.SetPixel(x, y, newColor);
                    }
                }

                // Set the PictureBox to display the image.
                pbImage.Image = imgCopy;

            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error." +
                    "Check the path to the image file.");
            }
        }

        /// <summary>
        /// return a value between -0,5 and +0,5
        /// </summary>
        /// <returns></returns>
        private double uniform()
        {
            //
            // Next(0x8000) : 0 .. 0x7fff
            // / 0x7FFFF: 0..1
            // -0.5: -05 .. +0,5
            //
            double d = rnd.NextDouble() - 0.5;
            return d;
        }

        private void tbValue_Scroll(object sender, EventArgs e)
        {
            int value = tbUniform.Value;
            lblValueUniform.Text = value.ToString();

            UpdateBitmap(value);
            UpdateInfoText();

        }

        private void cmdGrayscale_Click(object sender, EventArgs e)
        {
            ToggleGreyscale();
        }

        private void UpdateInfoText()
        {
            lblInfo.Text = string.Format(@"The R/G/B value of each pixel is changed by so much:
R:{0}/{1}
G:{2}/{3}
B:{4}/{5}",
uniform1, randomValue1,
uniform2, randomValue2,
uniform3, randomValue3
);

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateInfoText();
        }

        private void cmdMakeGrayscale_Click(object sender, EventArgs e)
        {
            MakeGrayscale();
        }
    }
}
