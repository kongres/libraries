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
    using HtmlRenderer.Core.Adapters.Entities;

    /// <summary>
    /// Adapter for WinForms pens objects for core.
    /// </summary>
    internal sealed class PenAdapter : RPen
    {
        /// <summary>
        /// The actual WinForms brush instance.
        /// </summary>
        private readonly XPen _pen;

        /// <summary>
        /// Init.
        /// </summary>
        public PenAdapter(XPen pen)
        {
            this._pen = pen;
        }

        /// <summary>
        /// The actual WinForms brush instance.
        /// </summary>
        public XPen Pen
        {
            get { return this._pen; }
        }

        public override double Width
        {
            get { return this._pen.Width; }
            set { this._pen.Width = value; }
        }

        public override RDashStyle DashStyle
        {
            set
            {
                switch (value)
                {
                    case RDashStyle.Solid:
                        this._pen.DashStyle = XDashStyle.Solid;
                        break;
                    case RDashStyle.Dash:
                        this._pen.DashStyle = XDashStyle.Dash;
                        if (Width < 2)
                            this._pen.DashPattern = new[] { 4, 4d }; // better looking
                        break;
                    case RDashStyle.Dot:
                        this._pen.DashStyle = XDashStyle.Dot;
                        break;
                    case RDashStyle.DashDot:
                        this._pen.DashStyle = XDashStyle.DashDot;
                        break;
                    case RDashStyle.DashDotDot:
                        this._pen.DashStyle = XDashStyle.DashDotDot;
                        break;
                    case RDashStyle.Custom:
                        this._pen.DashStyle = XDashStyle.Custom;
                        break;
                    default:
                        this._pen.DashStyle = XDashStyle.Solid;
                        break;
                }
            }
        }
    }
}