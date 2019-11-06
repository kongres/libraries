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

namespace HtmlRenderer.Core.Core.Dom
{
    using System;
    using HtmlRenderer.Core.Adapters;
    using HtmlRenderer.Core.Adapters.Entities;
    using HtmlRenderer.Core.Core.Handlers;
    using HtmlRenderer.Core.Core.Utils;

    /// <summary>
    /// CSS box for image element.
    /// </summary>
    internal sealed class CssBoxImage : CssBox
    {
        #region Fields and Consts

        /// <summary>
        /// the image word of this image box
        /// </summary>
        private readonly CssRectImage _imageWord;

        /// <summary>
        /// handler used for image loading by source
        /// </summary>
        private ImageLoadHandler _imageLoadHandler;

        /// <summary>
        /// is image load is finished, used to know if no image is found
        /// </summary>
        private bool _imageLoadingComplete;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="parent">the parent box of this box</param>
        /// <param name="tag">the html tag data of this box</param>
        public CssBoxImage(CssBox parent, HtmlTag tag)
            : base(parent, tag)
        {
            this._imageWord = new CssRectImage(this);
            Words.Add(this._imageWord);
        }

        /// <summary>
        /// Get the image of this image box.
        /// </summary>
        public RImage Image
        {
            get { return this._imageWord.Image; }
        }

        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        protected override void PaintImp(RGraphics g)
        {
            // load image if it is in visible rectangle
            if (this._imageLoadHandler == null)
            {
                this._imageLoadHandler = new ImageLoadHandler(HtmlContainer, OnLoadImageComplete);
                this._imageLoadHandler.LoadImage(GetAttribute("src"), HtmlTag != null ? HtmlTag.Attributes : null);
            }

            var rect = CommonUtils.GetFirstValueOrDefault(Rectangles);
            RPoint offset = RPoint.Empty;

            if (!IsFixed)
                offset = HtmlContainer.ScrollOffset;

            rect.Offset(offset);

            var clipped = RenderUtils.ClipGraphicsByOverflow(g, this);

            PaintBackground(g, rect, true, true);
            BordersDrawHandler.DrawBoxBorders(g, this, rect, true, true);

            RRect r = this._imageWord.Rectangle;
            r.Offset(offset);
            r.Height -= ActualBorderTopWidth + ActualBorderBottomWidth + ActualPaddingTop + ActualPaddingBottom;
            r.Y += ActualBorderTopWidth + ActualPaddingTop;
            r.X = Math.Floor(r.X);
            r.Y = Math.Floor(r.Y);

            if (this._imageWord.Image != null)
            {
                if (r.Width > 0 && r.Height > 0)
                {
                    if (this._imageWord.ImageRectangle == RRect.Empty)
                        g.DrawImage(this._imageWord.Image, r);
                    else
                        g.DrawImage(this._imageWord.Image, r, this._imageWord.ImageRectangle);

                    if (this._imageWord.Selected)
                    {
                        g.DrawRectangle(GetSelectionBackBrush(g, true), this._imageWord.Left + offset.X, this._imageWord.Top + offset.Y, this._imageWord.Width + 2, DomUtils.GetCssLineBoxByWord(this._imageWord).LineHeight);
                    }
                }
            }
            else if (this._imageLoadingComplete)
            {
                if (this._imageLoadingComplete && r.Width > 19 && r.Height > 19)
                {
                    RenderUtils.DrawImageErrorIcon(g, HtmlContainer, r);
                }
            }
            else
            {
                RenderUtils.DrawImageLoadingIcon(g, HtmlContainer, r);
                if (r.Width > 19 && r.Height > 19)
                {
                    g.DrawRectangle(g.GetPen(RColor.LightGray), r.X, r.Y, r.Width, r.Height);
                }
            }

            if (clipped)
                g.PopClip();
        }

        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        /// <param name="g">the device to use</param>
        internal override void MeasureWordsSize(RGraphics g)
        {
            if (!this._wordsSizeMeasured)
            {
                if (this._imageLoadHandler == null && (HtmlContainer.AvoidAsyncImagesLoading || HtmlContainer.AvoidImagesLateLoading))
                {
                    this._imageLoadHandler = new ImageLoadHandler(HtmlContainer, OnLoadImageComplete);

                    if (this.Content != null && this.Content != CssConstants.Normal)
                        this._imageLoadHandler.LoadImage(this.Content, HtmlTag != null ? HtmlTag.Attributes : null);
                    else
                        this._imageLoadHandler.LoadImage(GetAttribute("src"), HtmlTag != null ? HtmlTag.Attributes : null);
                }

                MeasureWordSpacing(g);
                this._wordsSizeMeasured = true;
            }

            CssLayoutEngine.MeasureImageSize(this._imageWord);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (this._imageLoadHandler != null)
                this._imageLoadHandler.Dispose();
            base.Dispose();
        }


        #region Private methods

        /// <summary>
        /// Set error image border on the image box.
        /// </summary>
        private void SetErrorBorder()
        {
            SetAllBorders(CssConstants.Solid, "2px", "#A0A0A0");
            BorderRightColor = BorderBottomColor = "#E3E3E3";
        }

        /// <summary>
        /// On image load process is complete with image or without update the image box.
        /// </summary>
        /// <param name="image">the image loaded or null if failed</param>
        /// <param name="rectangle">the source rectangle to draw in the image (empty - draw everything)</param>
        /// <param name="async">is the callback was called async to load image call</param>
        private void OnLoadImageComplete(RImage image, RRect rectangle, bool async)
        {
            this._imageWord.Image = image;
            this._imageWord.ImageRectangle = rectangle;
            this._imageLoadingComplete = true;
            this._wordsSizeMeasured = false;

            if (this._imageLoadingComplete && image == null)
            {
                SetErrorBorder();
            }

            if (!HtmlContainer.AvoidImagesLateLoading || async)
            {
                var width = new CssLength(Width);
                var height = new CssLength(Height);
                var layout = (width.Number <= 0 || width.Unit != CssUnit.Pixels) || (height.Number <= 0 || height.Unit != CssUnit.Pixels);
                HtmlContainer.RequestRefresh(layout);
            }
        }

        #endregion
    }
}