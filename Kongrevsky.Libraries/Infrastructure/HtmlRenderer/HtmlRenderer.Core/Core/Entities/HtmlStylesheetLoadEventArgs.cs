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

namespace HtmlRenderer.Core.Core.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Invoked when a stylesheet is about to be loaded by file path or URL in 'link' element.<br/>
    /// Allows to overwrite the loaded stylesheet by providing the stylesheet data manually, or different source (file or URL) to load from.<br/>
    /// Example: The stylesheet 'href' can be non-valid URI string that is interpreted in the overwrite delegate by custom logic to pre-loaded stylesheet object<br/>
    /// If no alternative data is provided the original source will be used.<br/>
    /// </summary>
    public sealed class HtmlStylesheetLoadEventArgs : EventArgs
    {
        #region Fields and Consts

        /// <summary>
        /// the source of the stylesheet as found in the HTML (file path or URL)
        /// </summary>
        private readonly string _src;

        /// <summary>
        /// collection of all the attributes that are defined on the link element
        /// </summary>
        private readonly Dictionary<string, string> _attributes;

        /// <summary>
        /// provide the new source (file path or URL) to load stylesheet from
        /// </summary>
        private string _setSrc;

        /// <summary>
        /// provide the stylesheet to load
        /// </summary>
        private string _setStyleSheet;

        /// <summary>
        /// provide the stylesheet data to load
        /// </summary>
        private CssData _setStyleSheetData;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="src">the source of the image (file path or URL)</param>
        /// <param name="attributes">collection of all the attributes that are defined on the image element</param>
        internal HtmlStylesheetLoadEventArgs(string src, Dictionary<string, string> attributes)
        {
            this._src = src;
            this._attributes = attributes;
        }

        /// <summary>
        /// the source of the stylesheet as found in the HTML (file path or URL)
        /// </summary>
        public string Src
        {
            get { return this._src; }
        }

        /// <summary>
        /// collection of all the attributes that are defined on the link element
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get { return this._attributes; }
        }

        /// <summary>
        /// provide the new source (file path or URL) to load stylesheet from
        /// </summary>
        public string SetSrc
        {
            get { return this._setSrc; }
            set { this._setSrc = value; }
        }

        /// <summary>
        /// provide the stylesheet to load
        /// </summary>
        public string SetStyleSheet
        {
            get { return this._setStyleSheet; }
            set { this._setStyleSheet = value; }
        }

        /// <summary>
        /// provide the stylesheet data to load
        /// </summary>
        public CssData SetStyleSheetData
        {
            get { return this._setStyleSheetData; }
            set { this._setStyleSheetData = value; }
        }
    }
}