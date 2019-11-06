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
    /// Adapter for WinForms Font object for core.
    /// </summary>
    internal sealed class FontAdapter : RFont
    {
        #region Fields and Consts

        /// <summary>
        /// the underline win-forms font.
        /// </summary>
        private readonly XFont _font;

        /// <summary>
        /// the vertical offset of the font underline location from the top of the font.
        /// </summary>
        private double _underlineOffset = -1;

        /// <summary>
        /// Cached font height.
        /// </summary>
        private double _height = -1;

        /// <summary>
        /// Cached font whitespace width.
        /// </summary>
        private double _whitespaceWidth = -1;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        public FontAdapter(XFont font)
        {
            this._font = font;
        }

        /// <summary>
        /// the underline win-forms font.
        /// </summary>
        public XFont Font
        {
            get { return this._font; }
        }

        public override double Size
        {
            get { return this._font.Size; }
        }

        public override double UnderlineOffset
        {
            get { return this._underlineOffset; }
        }

        public override double Height
        {
            get { return this._height; }
        }

        public override double LeftPadding
        {
            get { return this._height / 6f; }
        }


        public override double GetWhitespaceWidth(RGraphics graphics)
        {
            if (this._whitespaceWidth < 0)
            {
                this._whitespaceWidth = graphics.MeasureString(" ", this).Width;
            }
            return this._whitespaceWidth;
        }

        /// <summary>
        /// Set font metrics to be cached for the font for future use.
        /// </summary>
        /// <param name="height">the full height of the font</param>
        /// <param name="underlineOffset">the vertical offset of the font underline location from the top of the font.</param>
        internal void SetMetrics(int height, int underlineOffset)
        {
            this._height = height;
            this._underlineOffset = underlineOffset;
        }
    }
}