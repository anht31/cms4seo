//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Web.Configuration;
//using cms4seo.Model.Entities;

//namespace cms4seo.Common.Helpers
//{
//    public static class ImgHelpers
//    {
//        private static readonly string path = WebConfigurationManager.AppSettings["PhotoPath"];


//        /// <summary>
//        ///     Resize image by pixel
//        /// </summary>
//        /// <param name="iImage">bitmap image type</param>
//        /// <param name="width">width of imaget</param>
//        /// <returns>Image already resize</returns>
//        public static Bitmap ResizeImage(this Bitmap iImage, int width)
//        {
//            var x = 0;
//            var y = 0;
//            float newx = 0;
//            float newy = 0;
//            x = iImage.Width;
//            y = iImage.Height;
//            if (x < width)
//            {
//                newx = x;
//                newy = y;
//            }
//            else
//            {
//                if (x >= y)
//                {
//                    newx = width;
//                    newy = newx*y/x;
//                }
//                else
//                {
//                    newy = width;
//                    newx = newy*x/y;
//                }
//            }

//            //Receives an image as input and scales it up or down
//            var g = default(Graphics);
//            var sImage = new Bitmap(Convert.ToInt32(newx), Convert.ToInt32(newy));
//            g = Graphics.FromImage(sImage);
//            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
//            g.DrawImage(iImage, 0, 0, sImage.Width, sImage.Height);
//            // ResizeImage = sImage
//            g.Dispose();

//            return sImage;
//        }


//        /// <summary>
//        ///     Get type of Image
//        /// </summary>
//        /// <param name="filename">full file type of Image (include extension)</param>
//        /// <returns>Type of Image</returns>
//        public static ImageFormat GetImageFormat(this string filename)
//        {
//            var Ext = Path.GetExtension(filename).ToLower();
//            ImageFormat myFormat = null;

//            if (Ext == ".jpg" | Ext == ".jpeg")
//            {
//                myFormat = ImageFormat.Jpeg;
//            }

//            if (Ext == ".bmp")
//            {
//                myFormat = ImageFormat.Bmp;
//            }

//            if (Ext == ".gif")
//            {
//                myFormat = ImageFormat.Gif;
//            }

//            if (Ext == ".tiff")
//            {
//                myFormat = ImageFormat.Tiff;
//            }

//            if (Ext == ".tif")
//            {
//                myFormat = ImageFormat.Tiff;
//            }

//            if (Ext == ".png")
//            {
//                myFormat = ImageFormat.Png;
//            }

//            return myFormat;
//        }
//    }
//}