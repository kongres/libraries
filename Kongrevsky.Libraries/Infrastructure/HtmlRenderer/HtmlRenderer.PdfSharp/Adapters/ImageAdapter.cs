// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"
namespace HtmlRenderer.PdfSharp.Adapters
{
#if NETCOREAPP2_2
    using PdfSharpCore.Drawing;
#else
    using global::PdfSharp.Drawing;
#endif
    using HtmlRenderer.Core.Adapters;

    /// <summary>
    /// Adapter for WinForms Image object for core.
    /// </summary>
    internal sealed class ImageAdapter : RImage
    {
        /// <summary>
        /// the underline win-forms image.
        /// </summary>
        private readonly XImage _image;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ImageAdapter(XImage image)
        {
            this._image = image;
        }

        /// <summary>
        /// the underline win-forms image.
        /// </summary>
        public XImage Image
        {
            get { return this._image; }
        }

        public override double Width
        {
            get { return this._image.PixelWidth; }
        }

        public override double Height
        {
            get { return this._image.PixelHeight; }
        }

        public override void Dispose()
        {
            this._image.Dispose();
        }
    }
}