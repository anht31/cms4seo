using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace cms4seo.Common.TurboJpeg
{
    public static class Imager
    {

        public static string ImageQuality = "85L";


        /// <summary>
        /// Save image as jpeg
        /// </summary>
        /// <param name="path">path where to save</param>
        /// <param name="img">image to save</param>
        public static void SaveJpeg(string path, Image img)
        {
            var qualityParam = new EncoderParameter(Encoder.Quality, ImageQuality);
            var jpegCodec = GetEncoderInfo("image/jpeg");

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(path, jpegCodec, encoderParams);
        }

        /// <summary>
        /// Save image
        /// </summary>
        /// <param name="path">path where to save</param>
        /// <param name="img">image to save</param>
        /// <param name="imageCodecInfo">codec info</param>
        public static void Save(string path, Image img, ImageCodecInfo imageCodecInfo)
        {
            var qualityParam = new EncoderParameter(Encoder.Quality, ImageQuality);

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            img.Save(path, imageCodecInfo, encoderParams);
        }

        /// <summary>
        /// get codec info by mime type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == mimeType);
        }

        /// <summary>
        /// the image remains the same size, and it is placed in the middle of the new canvas
        /// </summary>
        /// <param name="image">image to put on canvas</param>
        /// <param name="width">canvas width</param>
        /// <param name="height">canvas height</param>
        /// <param name="canvasColor">canvas color</param>
        /// <returns></returns>
        public static Image PutOnCanvas(Image image, int width, int height, Color canvasColor)
        {
            var res = new Bitmap(width, height);
            using (var g = Graphics.FromImage(res))
            {
                g.Clear(canvasColor);
                var x = (width - image.Width) / 2;
                var y = (height - image.Height) / 2;
                g.DrawImageUnscaled(image, x, y, image.Width, image.Height);
            }

            return res;
        }

        /// <summary>
        /// the image remains the same size, and it is placed in the middle of the new canvas
        /// </summary>
        /// <param name="image">image to put on canvas</param>
        /// <param name="width">canvas width</param>
        /// <param name="height">canvas height</param>
        /// <returns></returns>
        public static Image PutOnWhiteCanvas(Image image, int width, int height)
        {
            return PutOnCanvas(image, width, height, Color.White);
        }


        /// <summary>
        /// resize an image and maintain aspect ratio
        /// </summary>
        /// <param name="image">image to resize</param>
        /// <param name="maxWidth">desired width</param>
        /// <param name="maxHeight">max height</param>
        /// <param name="onlyResizeIfWider">if image width is smaller than newWidth use image width</param>
        /// <returns>resized image</returns>
        public static Bitmap Resize(Image image, int maxWidth, int maxHeight, bool onlyResizeIfWider)
        {
            int newWidth = image.Width, newHeight = image.Height;

            // manual
            if (onlyResizeIfWider && image.Width > maxWidth)
            {
                newWidth = maxWidth;
                newHeight = image.Height * maxWidth / image.Width;
            }
            else
            {
                //auto
                if (image.Width > maxWidth)
                {
                    newWidth = maxWidth;
                    newHeight = image.Height * maxWidth / image.Width;
                }

                // next check condition maxHeight
                if (newHeight > maxHeight)
                {
                    newHeight = maxHeight;
                    newWidth = image.Width * maxHeight / image.Height;
                }
            }




            var res = new Bitmap(newWidth, newHeight);

            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return res;
        }


        //Image Resize Helper Method
        public static Bitmap ResizeImage(Image originalImage, int maxWidth, int maxHeight, bool onlyResizeIfWiderMock)
        {
            //calculate new Size
            int newWidth = originalImage.Width;
            int newHeight = originalImage.Height;
            double aspectRatio = (double)originalImage.Width / (double)originalImage.Height;
            if (aspectRatio <= 1 && originalImage.Width > maxWidth)
            {
                newWidth = maxWidth;
                newHeight = (int)Math.Round(newWidth / aspectRatio);
            }
            else if (aspectRatio > 1 && originalImage.Height > maxHeight)
            {
                newHeight = maxHeight;
                newWidth = (int)Math.Round(newHeight * aspectRatio);
            }
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                //--Quality Settings Adjust to fit your application
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(originalImage, 0, 0, newImage.Width, newImage.Height);
                return newImage;
            }
        }


        //Image Resize Helper Method
        /// <summary>
        /// Resize by Width or by Height
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="onlyResizeIfWiderMock"></param>
        /// <returns></returns>
        public static Bitmap ResizeBy(Image originalImage, int maxWidth, int maxHeight, bool onlyResizeIfWiderMock)
        {
            //calculate new Size
            int newWidth = originalImage.Width;
            int newHeight = originalImage.Height;
            double aspectRatio = (double)originalImage.Width / (double)originalImage.Height;
            if (maxHeight == 0 && originalImage.Width > maxWidth)
            {
                newWidth = maxWidth;
                newHeight = (int)Math.Round(newWidth / aspectRatio);
            }
            else if (maxWidth == 0 && originalImage.Height > maxHeight)
            {
                newHeight = maxHeight;
                newWidth = (int)Math.Round(newHeight * aspectRatio);
            }
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                //--Quality Settings Adjust to fit your application
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(originalImage, 0, 0, newImage.Width, newImage.Height);
                return newImage;
            }
        }

        /// <summary>
        /// Crop an image 
        /// </summary>
        /// <param name="img">image to crop</param>
        /// <param name="cropArea">rectangle to crop</param>
        /// <returns>resulting image</returns>
        public static Image Crop(Image img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            var bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return bmpCrop;
        }


        /// <summary>
        /// rotate Image for Mobile by Exif Orientation
        /// </summary>
        /// <param name="orientationValue">Exif Orientation</param>
        /// <returns>Orientation</returns>
        public static RotateFlipType GetOrientationToFlipType(int orientationValue)
        {
            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            switch (orientationValue)
            {
                case 1:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    rotateFlipType = RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    rotateFlipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    rotateFlipType = RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    rotateFlipType = RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    rotateFlipType = RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
            }
            return rotateFlipType;
        }


        /// <summary>
        /// Get MimeType of Image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string GetMimeType(this Image image)
        {
            return image.RawFormat.GetMimeType();
        }

        public static string GetMimeType(this ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
        }

    }
}
