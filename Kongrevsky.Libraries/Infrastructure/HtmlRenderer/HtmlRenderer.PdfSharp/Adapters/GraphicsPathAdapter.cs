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
    using System;
    using HtmlRenderer.Core.Adapters;
    using HtmlRenderer.Core.Adapters.Entities;

    /// <summary>
    /// Adapter for WinForms graphics path object for core.
    /// </summary>
    internal sealed class GraphicsPathAdapter : RGraphicsPath
    {
        /// <summary>
        /// The actual PdfSharp graphics path instance.
        /// </summary>
        private readonly XGraphicsPath _graphicsPath = new XGraphicsPath();

        /// <summary>
        /// the last point added to the path to begin next segment from
        /// </summary>
        private RPoint _lastPoint;

        /// <summary>
        /// The actual PdfSharp graphics path instance.
        /// </summary>
        public XGraphicsPath GraphicsPath
        {
            get { return this._graphicsPath; }
        }

        public override void Start(double x, double y)
        {
            this._lastPoint = new RPoint(x, y);
        }

        public override void LineTo(double x, double y)
        {
            this._graphicsPath.AddLine((float)this._lastPoint.X, (float)this._lastPoint.Y, (float)x, (float)y);
            this._lastPoint = new RPoint(x, y);
        }

        public override void ArcTo(double x, double y, double size, Corner corner)
        {
            float left = (float)(Math.Min(x, this._lastPoint.X) - (corner == Corner.TopRight || corner == Corner.BottomRight ? size : 0));
            float top = (float)(Math.Min(y, this._lastPoint.Y) - (corner == Corner.BottomLeft || corner == Corner.BottomRight ? size : 0));
            this._graphicsPath.AddArc(left, top, (float)size * 2, (float)size * 2, GetStartAngle(corner), 90);
            this._lastPoint = new RPoint(x, y);
        }

        public override void Dispose()
        { }

        /// <summary>
        /// Get arc start angle for the given corner.
        /// </summary>
        private static int GetStartAngle(Corner corner)
        {
            int startAngle;
            switch (corner)
            {
                case Corner.TopLeft:
                    startAngle = 180;
                    break;
                case Corner.TopRight:
                    startAngle = 270;
                    break;
                case Corner.BottomLeft:
                    startAngle = 90;
                    break;
                case Corner.BottomRight:
                    startAngle = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("corner");
            }
            return startAngle;
        }
    }
}