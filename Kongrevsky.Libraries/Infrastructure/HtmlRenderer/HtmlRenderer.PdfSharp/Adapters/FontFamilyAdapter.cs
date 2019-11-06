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
#if NETSTANDARD2_1
    using PdfSharpCore.Drawing;
#else
    using global::PdfSharp.Drawing;
#endif
    using HtmlRenderer.Core.Adapters;

    /// <summary>
    /// Adapter for WinForms Font object for core.
    /// </summary>
    internal sealed class FontFamilyAdapter : RFontFamily
    {
        /// <summary>
        /// the underline win-forms font.
        /// </summary>
        private readonly XFontFamily _fontFamily;

        /// <summary>
        /// Init.
        /// </summary>
        public FontFamilyAdapter(XFontFamily fontFamily)
        {
            this._fontFamily = fontFamily;
        }

        /// <summary>
        /// the underline win-forms font family.
        /// </summary>
        public XFontFamily FontFamily
        {
            get { return this._fontFamily; }
        }

        public override string Name
        {
            get { return this._fontFamily.Name; }
        }
    }
}