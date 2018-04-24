namespace Kongrevsky.Utilities.Image
{
    #region << Using >>

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    #endregion

    public static class ImageUtils
    {
        /// <summary>
        /// Returns thumbs of the image by specified sizes
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static Image ScaleImage(this Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        /// <summary>
        /// Returns MemoryStream of the Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Returns format of the Image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(this Image img)
        {
            if (img.RawFormat.Equals(ImageFormat.Jpeg))
                return ImageFormat.Jpeg;

            if (img.RawFormat.Equals(ImageFormat.Bmp))
                return ImageFormat.Bmp;

            if (img.RawFormat.Equals(ImageFormat.Png))
                return ImageFormat.Png;

            if (img.RawFormat.Equals(ImageFormat.Emf))
                return ImageFormat.Emf;

            if (img.RawFormat.Equals(ImageFormat.Exif))
                return ImageFormat.Exif;

            if (img.RawFormat.Equals(ImageFormat.Gif))
                return ImageFormat.Gif;

            if (img.RawFormat.Equals(ImageFormat.Icon))
                return ImageFormat.Icon;

            if (img.RawFormat.Equals(ImageFormat.MemoryBmp))
                return ImageFormat.MemoryBmp;

            if (img.RawFormat.Equals(ImageFormat.Tiff))
                return ImageFormat.Tiff;
            else
                return ImageFormat.Wmf;
        }

        /// <summary>
        /// Sets Image background color to specified color
        /// </summary>
        /// <param name="image"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Image SetBackgroundColor(this Image image, Color color)
        {
            //create a Bitmap the size of the image provided  
            var bmp = new Bitmap(image.Width, image.Height);

            //create a graphics object from the image  
            using (var gfx = Graphics.FromImage(bmp))
            {
                //clear image by color
                gfx.Clear(color);
                //now draw the image  
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }

            return bmp;
        }

        /// <summary>
        /// Set background color for image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Image SetBackgroudColor(this Image image, Color color)
        {
            //create a Bitmap the size of the image provided  
            var bmp = new Bitmap(image.Width, image.Height);

            //create a graphics object from the image  
            using (var gfx = Graphics.FromImage(bmp))
            {
                //clear image by color
                gfx.Clear(color);
                //now draw the image  
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }
            return bmp;
        }
    }
}