using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace MyProject
{
    class Picture
    {
        private List<Image> imagesToCompare;
        private List<long> hashes;

        public Picture()
        {
            imagesToCompare = new List<Image>();
            hashes = new List<long>();
        }
        public static string GetHash(string path)
        {
            try
            {
                return Convert.ToInt64(HashImage(Image.FromFile(path)), 2).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
        public static string HashImage(Image img)
        {
            img = CropImage(img, 8, 8);
            img = ColourfulToGray(img);
            img = GrayToBlackWhite(img);
            return BlackWhiteToHash(img);
        }
        public static Image CropImage(Image image, int nWidth, int nHeight)
        {
            int newWidth, newHeight;
            var coefH = (double)nHeight / (double)image.Height;
            var coefW = (double)nWidth / (double)image.Width;
            if (coefW >= coefH)
            {
                newHeight = (int)(image.Height * coefH);
                newWidth = (int)(image.Width * coefH);
            }
            else
            {
                newHeight = (int)(image.Height * coefW);
                newWidth = (int)(image.Width * coefW);
            }

            Image result = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(result))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
                g.Dispose();
            }
            return result;
        }
        public static Image ColourfulToGray(Image image)
        {
            Bitmap bmp = new Bitmap(image);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color c = bmp.GetPixel(i, j);
                    byte gray = (byte)(.21 * c.R + .71 * c.G + .071 * c.B);
                    bmp.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            return bmp;
        }
        public static Image GrayToBlackWhite(Image image)
        {
            Bitmap bmp = new Bitmap(image);
            int sum = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    sum += bmp.GetPixel(i, j).R;
                }
            }
            int avrg = sum / (bmp.Width * bmp.Height);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (bmp.GetPixel(i, j).R >= avrg) bmp.SetPixel(i, j, Color.White);
                    else bmp.SetPixel(i, j, Color.Black);
                }
            }
            return bmp;
        }
        public static string BlackWhiteToHash(Image image)
        {
            Bitmap bmp = new Bitmap(image);
            string result = "";
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (bmp.GetPixel(i, j).ToArgb() == Color.Black.ToArgb()) result += "1";
                    else result += "0";
                }
            }
            return result;
        }
    }
}
