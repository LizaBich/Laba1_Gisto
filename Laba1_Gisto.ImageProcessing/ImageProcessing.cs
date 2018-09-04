using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Laba1_Gisto.ImageProcessing
{
    public class ImageProcessing
    {
        public IList<IDictionary<byte, int>> Calculate(Bitmap image)
        {
            var rGisto = new Dictionary<byte, int>();
            var gGisto = new Dictionary<byte, int>();
            var bGisto = new Dictionary<byte, int>();
            var result = new List<IDictionary<byte, int>>();

            for (var x = 0; x < image.Width; ++x)
            {
                for (var y = 0; y < image.Height; ++y)
                {
                    var pixel = image.GetPixel(x, y);
                    var rPower = pixel.R;
                    var gPower = pixel.G;
                    var bPower = pixel.B;

                    if (!rGisto.ContainsKey(rPower))
                    {
                        rGisto.Add(rPower, 0);
                    }
                    if (!gGisto.ContainsKey(gPower))
                    {
                        gGisto.Add(gPower, 0);
                    }
                    if (!bGisto.ContainsKey(bPower))
                    {
                        bGisto.Add(bPower, 0);
                    }

                    rGisto[rPower]++;
                    gGisto[gPower]++;
                    bGisto[bPower]++;
                }
            }

            result.Add(rGisto);
            result.Add(gGisto);
            result.Add(bGisto);

            return result;
        }

        public Bitmap LogCorrection(Bitmap origin, double c)
        {
            var newImage = new Bitmap(origin.Width, origin.Height, PixelFormat.Format24bppRgb);

            for (var i = 0; i < origin.Width; ++i)
            {
                for (var j = 0; j < origin.Height; ++j)
                {
                    var pixel = origin.GetPixel(i, j);

                    var newR = (byte)(c * Math.Log(1 + pixel.R));
                    var newG = (byte)(c * Math.Log(1 + pixel.G));
                    var newB = (byte)(c * Math.Log(1 + pixel.B));
                    
                    newImage.SetPixel(i, j, Color.FromArgb(newR, newG, newB));
                }
            }

            return newImage;
        }

        public Bitmap Resize(Bitmap original, int customWidth, int customHeight)
        {
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            float ratioX = (float)customWidth / (float)originalWidth;
            float ratioY = (float)customHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(original, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public Bitmap RobertsonFilter(Bitmap origin)
        {
            var newImage = new Bitmap(origin.Width, origin.Height, PixelFormat.Format24bppRgb);

            var filteredPixels = this.Calculateresponse(origin);
            
            for (var i = 0; i < origin.Width; ++i)
            {
                for (var j = 0; j < origin.Height; ++j)
                {
                    newImage.SetPixel(i, j, filteredPixels[origin.Height * i + j]);
                }
            }

            return newImage;
        }

        // pixel0, pixel1
        // pixel2, pixel3
        private IList<Color> Calculateresponse(Bitmap origin)
        {
            var result = new List<Color>();
            for (var i = 0; i < origin.Width; ++i)
            {
                for (var j = 0; j < origin.Height; ++j)
                {
                    var pixel0 = origin.GetPixel(i, j);
                    var pixel1 = i == origin.Width - 1? Color.Black: origin.GetPixel(i + 1, j);
                    var pixel2 = j == origin.Height - 1 ? Color.Black : origin.GetPixel(i, j + 1);
                    var pixel3 = i == origin.Width - 1 || j == origin.Height - 1? Color.Black: origin.GetPixel(i + 1, j + 1);

                    var rG = Math.Pow(Math.Pow(pixel0.R - pixel3.R, 2) + Math.Pow(pixel1.R - pixel2.R, 2), 0.5);
                    var gG = Math.Pow(Math.Pow(pixel0.G - pixel3.G, 2) + Math.Pow(pixel1.G - pixel2.G, 2), 0.5);
                    var bG = Math.Pow(Math.Pow(pixel0.B - pixel3.B, 2) + Math.Pow(pixel1.B - pixel2.B, 2), 0.5);

                    result.Add(Color.FromArgb((byte)rG, (byte)gG, (byte)bG));
                }
            }

            return result;
        }
    }
}
