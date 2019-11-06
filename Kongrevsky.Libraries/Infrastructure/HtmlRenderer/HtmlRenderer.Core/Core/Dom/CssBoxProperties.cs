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
    using System.Globalization;
    using System.Text.RegularExpressions;
    using HtmlRenderer.Core.Adapters;
    using HtmlRenderer.Core.Adapters.Entities;
    using HtmlRenderer.Core.Core.Parse;
    using HtmlRenderer.Core.Core.Utils;

    /// <summary>
    /// Base class for css box to handle the css properties.<br/>
    /// Has field and property for every css property that can be set, the properties add additional parsing like
    /// setting the correct border depending what border value was set (single, two , all four).<br/>
    /// Has additional fields to control the location and size of the box and 'actual' css values for some properties
    /// that require additional calculations and parsing.<br/>
    /// </summary>
    internal abstract class CssBoxProperties
    {
        #region CSS Fields

        private string _backgroundColor = "transparent";
        private string _backgroundGradient = "none";
        private string _backgroundGradientAngle = "90";
        private string _backgroundImage = "none";
        private string _backgroundPosition = "0% 0%";
        private string _backgroundRepeat = "repeat";
        private string _borderTopWidth = "medium";
        private string _borderRightWidth = "medium";
        private string _borderBottomWidth = "medium";
        private string _borderLeftWidth = "medium";
        private string _borderTopColor = "black";
        private string _borderRightColor = "black";
        private string _borderBottomColor = "black";
        private string _borderLeftColor = "black";
        private string _borderTopStyle = "none";
        private string _borderRightStyle = "none";
        private string _borderBottomStyle = "none";
        private string _borderLeftStyle = "none";
        private string _borderSpacing = "0";
        private string _borderCollapse = "separate";
        private string _bottom;
        private string _color = "black";
        private string _content = "normal";
        private string _cornerNwRadius = "0";
        private string _cornerNeRadius = "0";
        private string _cornerSeRadius = "0";
        private string _cornerSwRadius = "0";
        private string _cornerRadius = "0";
        private string _emptyCells = "show";
        private string _direction = "ltr";
        private string _display = "inline";
        private string _fontFamily;
        private string _fontSize = "medium";
        private string _fontStyle = "normal";
        private string _fontVariant = "normal";
        private string _fontWeight = "normal";
        private string _float = "none";
        private string _height = "auto";
        private string _marginBottom = "0";
        private string _marginLeft = "0";
        private string _marginRight = "0";
        private string _marginTop = "0";
        private string _left = "auto";
        private string _lineHeight = "normal";
        private string _listStyleType = "disc";
        private string _listStyleImage = string.Empty;
        private string _listStylePosition = "outside";
        private string _listStyle = string.Empty;
        private string _overflow = "visible";
        private string _paddingLeft = "0";
        private string _paddingBottom = "0";
        private string _paddingRight = "0";
        private string _paddingTop = "0";
        private string _pageBreakInside = CssConstants.Auto;
        private string _right;
        private string _textAlign = string.Empty;
        private string _textDecoration = string.Empty;
        private string _textIndent = "0";
        private string _top = "auto";
        private string _position = "static";
        private string _verticalAlign = "baseline";
        private string _width = "auto";
        private string _maxWidth = "none";
        private string _wordSpacing = "normal";
        private string _wordBreak = "normal";
        private string _whiteSpace = "normal";
        private string _visibility = "visible";

        #endregion


        #region Fields

        /// <summary>
        /// Gets or sets the location of the box
        /// </summary>
        private RPoint _location;

        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        private RSize _size;

        private double _actualCornerNw = double.NaN;
        private double _actualCornerNe = double.NaN;
        private double _actualCornerSw = double.NaN;
        private double _actualCornerSe = double.NaN;
        private RColor _actualColor = RColor.Empty;
        private double _actualBackgroundGradientAngle = double.NaN;
        private double _actualHeight = double.NaN;
        private double _actualWidth = double.NaN;
        private double _actualPaddingTop = double.NaN;
        private double _actualPaddingBottom = double.NaN;
        private double _actualPaddingRight = double.NaN;
        private double _actualPaddingLeft = double.NaN;
        private double _actualMarginTop = double.NaN;
        private double _collapsedMarginTop = double.NaN;
        private double _actualMarginBottom = double.NaN;
        private double _actualMarginRight = double.NaN;
        private double _actualMarginLeft = double.NaN;
        private double _actualBorderTopWidth = double.NaN;
        private double _actualBorderLeftWidth = double.NaN;
        private double _actualBorderBottomWidth = double.NaN;
        private double _actualBorderRightWidth = double.NaN;

        /// <summary>
        /// the width of whitespace between words
        /// </summary>
        private double _actualLineHeight = double.NaN;

        private double _actualWordSpacing = double.NaN;
        private double _actualTextIndent = double.NaN;
        private double _actualBorderSpacingHorizontal = double.NaN;
        private double _actualBorderSpacingVertical = double.NaN;
        private RColor _actualBackgroundGradient = RColor.Empty;
        private RColor _actualBorderTopColor = RColor.Empty;
        private RColor _actualBorderLeftColor = RColor.Empty;
        private RColor _actualBorderBottomColor = RColor.Empty;
        private RColor _actualBorderRightColor = RColor.Empty;
        private RColor _actualBackgroundColor = RColor.Empty;
        private RFont _actualFont;

        #endregion


        #region CSS Properties

        public string BorderBottomWidth
        {
            get { return this._borderBottomWidth; }
            set
            {
                this._borderBottomWidth = value;
                this._actualBorderBottomWidth = Single.NaN;
            }
        }

        public string BorderLeftWidth
        {
            get { return this._borderLeftWidth; }
            set
            {
                this._borderLeftWidth = value;
                this._actualBorderLeftWidth = Single.NaN;
            }
        }

        public string BorderRightWidth
        {
            get { return this._borderRightWidth; }
            set
            {
                this._borderRightWidth = value;
                this._actualBorderRightWidth = Single.NaN;
            }
        }

        public string BorderTopWidth
        {
            get { return this._borderTopWidth; }
            set
            {
                this._borderTopWidth = value;
                this._actualBorderTopWidth = Single.NaN;
            }
        }

        public string BorderBottomStyle
        {
            get { return this._borderBottomStyle; }
            set { this._borderBottomStyle = value; }
        }

        public string BorderLeftStyle
        {
            get { return this._borderLeftStyle; }
            set { this._borderLeftStyle = value; }
        }

        public string BorderRightStyle
        {
            get { return this._borderRightStyle; }
            set { this._borderRightStyle = value; }
        }

        public string BorderTopStyle
        {
            get { return this._borderTopStyle; }
            set { this._borderTopStyle = value; }
        }

        public string BorderBottomColor
        {
            get { return this._borderBottomColor; }
            set
            {
                this._borderBottomColor = value;
                this._actualBorderBottomColor = RColor.Empty;
            }
        }

        public string BorderLeftColor
        {
            get { return this._borderLeftColor; }
            set
            {
                this._borderLeftColor = value;
                this._actualBorderLeftColor = RColor.Empty;
            }
        }

        public string BorderRightColor
        {
            get { return this._borderRightColor; }
            set
            {
                this._borderRightColor = value;
                this._actualBorderRightColor = RColor.Empty;
            }
        }

        public string BorderTopColor
        {
            get { return this._borderTopColor; }
            set
            {
                this._borderTopColor = value;
                this._actualBorderTopColor = RColor.Empty;
            }
        }

        public string BorderSpacing
        {
            get { return this._borderSpacing; }
            set { this._borderSpacing = value; }
        }

        public string BorderCollapse
        {
            get { return this._borderCollapse; }
            set { this._borderCollapse = value; }
        }

        public string CornerRadius
        {
            get { return this._cornerRadius; }
            set
            {
                MatchCollection r = RegexParserUtils.Match(RegexParserUtils.CssLength, value);

                switch (r.Count)
                {
                    case 1:
                        CornerNeRadius = r[0].Value;
                        CornerNwRadius = r[0].Value;
                        CornerSeRadius = r[0].Value;
                        CornerSwRadius = r[0].Value;
                        break;
                    case 2:
                        CornerNeRadius = r[0].Value;
                        CornerNwRadius = r[0].Value;
                        CornerSeRadius = r[1].Value;
                        CornerSwRadius = r[1].Value;
                        break;
                    case 3:
                        CornerNeRadius = r[0].Value;
                        CornerNwRadius = r[1].Value;
                        CornerSeRadius = r[2].Value;
                        break;
                    case 4:
                        CornerNeRadius = r[0].Value;
                        CornerNwRadius = r[1].Value;
                        CornerSeRadius = r[2].Value;
                        CornerSwRadius = r[3].Value;
                        break;
                }

                this._cornerRadius = value;
            }
        }

        public string CornerNwRadius
        {
            get { return this._cornerNwRadius; }
            set { this._cornerNwRadius = value; }
        }

        public string CornerNeRadius
        {
            get { return this._cornerNeRadius; }
            set { this._cornerNeRadius = value; }
        }

        public string CornerSeRadius
        {
            get { return this._cornerSeRadius; }
            set { this._cornerSeRadius = value; }
        }

        public string CornerSwRadius
        {
            get { return this._cornerSwRadius; }
            set { this._cornerSwRadius = value; }
        }

        public string MarginBottom
        {
            get { return this._marginBottom; }
            set { this._marginBottom = value; }
        }

        public string MarginLeft
        {
            get { return this._marginLeft; }
            set { this._marginLeft = value; }
        }

        public string MarginRight
        {
            get { return this._marginRight; }
            set { this._marginRight = value; }
        }

        public string MarginTop
        {
            get { return this._marginTop; }
            set { this._marginTop = value; }
        }

        public string PaddingBottom
        {
            get { return this._paddingBottom; }
            set
            {
                this._paddingBottom = value;
                this._actualPaddingBottom = double.NaN;
            }
        }

        public string PaddingLeft
        {
            get { return this._paddingLeft; }
            set
            {
                this._paddingLeft = value;
                this._actualPaddingLeft = double.NaN;
            }
        }

        public string PaddingRight
        {
            get { return this._paddingRight; }
            set
            {
                this._paddingRight = value;
                this._actualPaddingRight = double.NaN;
            }
        }

        public string PaddingTop
        {
            get { return this._paddingTop; }
            set
            {
                this._paddingTop = value;
                this._actualPaddingTop = double.NaN;
            }
        }

        public string PageBreakInside
        {
            get { return this._pageBreakInside; }
            set
            {
                this._pageBreakInside = value;
            }
        }

        public string Left
        {
            get { return this._left; }
            set
            {
                this._left = value;

                if (Position == CssConstants.Fixed)
                {
                    this._location = GetActualLocation(Left, Top);
                }
            }
        }

        public string Top
        {
            get { return this._top; }
            set {
                this._top = value;

                if (Position == CssConstants.Fixed)
                {
                    this._location = GetActualLocation(Left, Top);
                }

            }
        }

        public string Width
        {
            get { return this._width; }
            set { this._width = value; }
        }

        public string MaxWidth
        {
            get { return this._maxWidth; }
            set { this._maxWidth = value; }
        }

        public string Height
        {
            get { return this._height; }
            set { this._height = value; }
        }

        public string BackgroundColor
        {
            get { return this._backgroundColor; }
            set { this._backgroundColor = value; }
        }

        public string BackgroundImage
        {
            get { return this._backgroundImage; }
            set { this._backgroundImage = value; }
        }

        public string BackgroundPosition
        {
            get { return this._backgroundPosition; }
            set { this._backgroundPosition = value; }
        }

        public string BackgroundRepeat
        {
            get { return this._backgroundRepeat; }
            set { this._backgroundRepeat = value; }
        }

        public string BackgroundGradient
        {
            get { return this._backgroundGradient; }
            set { this._backgroundGradient = value; }
        }

        public string BackgroundGradientAngle
        {
            get { return this._backgroundGradientAngle; }
            set { this._backgroundGradientAngle = value; }
        }

        public string Color
        {
            get { return this._color; }
            set
            {
                this._color = value;
                this._actualColor = RColor.Empty;
            }
        }

        public string Content
        {
            get { return this._content; }
            set { this._content = value; }
        }

        public string Display
        {
            get { return this._display; }
            set { this._display = value; }
        }

        public string Direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }

        public string EmptyCells
        {
            get { return this._emptyCells; }
            set { this._emptyCells = value; }
        }

        public string Float
        {
            get { return this._float; }
            set { this._float = value; }
        }

        public string Position
        {
            get { return this._position; }
            set { this._position = value; }
        }

        public string LineHeight
        {
            get { return this._lineHeight; }
            set { this._lineHeight = string.Format(NumberFormatInfo.InvariantInfo, "{0}px", CssValueParser.ParseLength(value, Size.Height, this, CssConstants.Em)); }
        }

        public string VerticalAlign
        {
            get { return this._verticalAlign; }
            set { this._verticalAlign = value; }
        }

        public string TextIndent
        {
            get { return this._textIndent; }
            set { this._textIndent = NoEms(value); }
        }

        public string TextAlign
        {
            get { return this._textAlign; }
            set { this._textAlign = value; }
        }

        public string TextDecoration
        {
            get { return this._textDecoration; }
            set { this._textDecoration = value; }
        }

        public string WhiteSpace
        {
            get { return this._whiteSpace; }
            set { this._whiteSpace = value; }
        }

        public string Visibility
        {
            get { return this._visibility; }
            set { this._visibility = value; }
        }

        public string WordSpacing
        {
            get { return this._wordSpacing; }
            set { this._wordSpacing = NoEms(value); }
        }

        public string WordBreak
        {
            get { return this._wordBreak; }
            set { this._wordBreak = value; }
        }

        public string FontFamily
        {
            get { return this._fontFamily; }
            set { this._fontFamily = value; }
        }

        public string FontSize
        {
            get { return this._fontSize; }
            set
            {
                string length = RegexParserUtils.Search(RegexParserUtils.CssLength, value);

                if (length != null)
                {
                    string computedValue;
                    CssLength len = new CssLength(length);

                    if (len.HasError)
                    {
                        computedValue = "medium";
                    }
                    else if (len.Unit == CssUnit.Ems && GetParent() != null)
                    {
                        computedValue = len.ConvertEmToPoints(GetParent().ActualFont.Size).ToString();
                    }
                    else
                    {
                        computedValue = len.ToString();
                    }

                    this._fontSize = computedValue;
                }
                else
                {
                    this._fontSize = value;
                }
            }
        }

        public string FontStyle
        {
            get { return this._fontStyle; }
            set { this._fontStyle = value; }
        }

        public string FontVariant
        {
            get { return this._fontVariant; }
            set { this._fontVariant = value; }
        }

        public string FontWeight
        {
            get { return this._fontWeight; }
            set { this._fontWeight = value; }
        }

        public string ListStyle
        {
            get { return this._listStyle; }
            set { this._listStyle = value; }
        }

        public string Overflow
        {
            get { return this._overflow; }
            set { this._overflow = value; }
        }

        public string ListStylePosition
        {
            get { return this._listStylePosition; }
            set { this._listStylePosition = value; }
        }

        public string ListStyleImage
        {
            get { return this._listStyleImage; }
            set { this._listStyleImage = value; }
        }

        public string ListStyleType
        {
            get { return this._listStyleType; }
            set { this._listStyleType = value; }
        }

        #endregion CSS Propertier

        /// <summary>
        /// Gets or sets the location of the box
        /// </summary>
        public RPoint Location
        {
            get {
                if (this._location.IsEmpty && Position == CssConstants.Fixed)
                {
                    var left = Left;
                    var top = Top;

                    this._location = GetActualLocation(Left, Top);
                }
                return this._location;
            }
            set {
                this._location = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        public RSize Size
        {
            get { return this._size; }
            set { this._size = value; }
        }

        /// <summary>
        /// Gets the bounds of the box
        /// </summary>
        public RRect Bounds
        {
            get { return new RRect(Location, Size); }
        }

        /// <summary>
        /// Gets the width available on the box, counting padding and margin.
        /// </summary>
        public double AvailableWidth
        {
            get { return Size.Width - ActualBorderLeftWidth - ActualPaddingLeft - ActualPaddingRight - ActualBorderRightWidth; }
        }

        /// <summary>
        /// Gets the right of the box. When setting, it will affect only the width of the box.
        /// </summary>
        public double ActualRight
        {
            get { return Location.X + Size.Width; }
            set { Size = new RSize(value - Location.X, Size.Height); }
        }

        /// <summary>
        /// Gets or sets the bottom of the box. 
        /// (When setting, alters only the Size.Height of the box)
        /// </summary>
        public double ActualBottom
        {
            get { return Location.Y + Size.Height; }
            set { Size = new RSize(Size.Width, value - Location.Y); }
        }

        /// <summary>
        /// Gets the left of the client rectangle (Where content starts rendering)
        /// </summary>
        public double ClientLeft
        {
            get { return Location.X + ActualBorderLeftWidth + ActualPaddingLeft; }
        }

        /// <summary>
        /// Gets the top of the client rectangle (Where content starts rendering)
        /// </summary>
        public double ClientTop
        {
            get { return Location.Y + ActualBorderTopWidth + ActualPaddingTop; }
        }

        /// <summary>
        /// Gets the right of the client rectangle
        /// </summary>
        public double ClientRight
        {
            get { return ActualRight - ActualPaddingRight - ActualBorderRightWidth; }
        }

        /// <summary>
        /// Gets the bottom of the client rectangle
        /// </summary>
        public double ClientBottom
        {
            get { return ActualBottom - ActualPaddingBottom - ActualBorderBottomWidth; }
        }

        /// <summary>
        /// Gets the client rectangle
        /// </summary>
        public RRect ClientRectangle
        {
            get { return RRect.FromLTRB(ClientLeft, ClientTop, ClientRight, ClientBottom); }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public double ActualHeight
        {
            get
            {
                if (double.IsNaN(this._actualHeight))
                {
                    this._actualHeight = CssValueParser.ParseLength(Height, Size.Height, this);
                }
                return this._actualHeight;
            }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public double ActualWidth
        {
            get
            {
                if (double.IsNaN(this._actualWidth))
                {
                    this._actualWidth = CssValueParser.ParseLength(Width, Size.Width, this);
                }
                return this._actualWidth;
            }
        }

        /// <summary>
        /// Gets the actual top's padding
        /// </summary>
        public double ActualPaddingTop
        {
            get
            {
                if (double.IsNaN(this._actualPaddingTop))
                {
                    this._actualPaddingTop = CssValueParser.ParseLength(PaddingTop, Size.Width, this);
                }
                return this._actualPaddingTop;
            }
        }

        /// <summary>
        /// Gets the actual padding on the left
        /// </summary>
        public double ActualPaddingLeft
        {
            get
            {
                if (double.IsNaN(this._actualPaddingLeft))
                {
                    this._actualPaddingLeft = CssValueParser.ParseLength(PaddingLeft, Size.Width, this);
                }
                return this._actualPaddingLeft;
            }
        }

        /// <summary>
        /// Gets the actual Padding of the bottom
        /// </summary>
        public double ActualPaddingBottom
        {
            get
            {
                if (double.IsNaN(this._actualPaddingBottom))
                {
                    this._actualPaddingBottom = CssValueParser.ParseLength(PaddingBottom, Size.Width, this);
                }
                return this._actualPaddingBottom;
            }
        }

        /// <summary>
        /// Gets the actual padding on the right
        /// </summary>
        public double ActualPaddingRight
        {
            get
            {
                if (double.IsNaN(this._actualPaddingRight))
                {
                    this._actualPaddingRight = CssValueParser.ParseLength(PaddingRight, Size.Width, this);
                }
                return this._actualPaddingRight;
            }
        }

        /// <summary>
        /// Gets the actual top's Margin
        /// </summary>
        public double ActualMarginTop
        {
            get
            {
                if (double.IsNaN(this._actualMarginTop))
                {
                    if (MarginTop == CssConstants.Auto)
                        MarginTop = "0";
                    var actualMarginTop = CssValueParser.ParseLength(MarginTop, Size.Width, this);
                    if (MarginLeft.EndsWith("%"))
                        return actualMarginTop;
                    this._actualMarginTop = actualMarginTop;
                }
                return this._actualMarginTop;
            }
        }

        /// <summary>
        /// The margin top value if was effected by margin collapse.
        /// </summary>
        public double CollapsedMarginTop
        {
            get { return double.IsNaN(this._collapsedMarginTop) ? 0 : this._collapsedMarginTop; }
            set { this._collapsedMarginTop = value; }
        }

        /// <summary>
        /// Gets the actual Margin on the left
        /// </summary>
        public double ActualMarginLeft
        {
            get
            {
                if (double.IsNaN(this._actualMarginLeft))
                {
                    if (MarginLeft == CssConstants.Auto)
                        MarginLeft = "0";
                    var actualMarginLeft = CssValueParser.ParseLength(MarginLeft, Size.Width, this);
                    if (MarginLeft.EndsWith("%"))
                        return actualMarginLeft;
                    this._actualMarginLeft = actualMarginLeft;
                }
                return this._actualMarginLeft;
            }
        }

        /// <summary>
        /// Gets the actual Margin of the bottom
        /// </summary>
        public double ActualMarginBottom
        {
            get
            {
                if (double.IsNaN(this._actualMarginBottom))
                {
                    if (MarginBottom == CssConstants.Auto)
                        MarginBottom = "0";
                    var actualMarginBottom = CssValueParser.ParseLength(MarginBottom, Size.Width, this);
                    if (MarginLeft.EndsWith("%"))
                        return actualMarginBottom;
                    this._actualMarginBottom = actualMarginBottom;
                }
                return this._actualMarginBottom;
            }
        }

        /// <summary>
        /// Gets the actual Margin on the right
        /// </summary>
        public double ActualMarginRight
        {
            get
            {
                if (double.IsNaN(this._actualMarginRight))
                {
                    if (MarginRight == CssConstants.Auto)
                        MarginRight = "0";
                    var actualMarginRight = CssValueParser.ParseLength(MarginRight, Size.Width, this);
                    if (MarginLeft.EndsWith("%"))
                        return actualMarginRight;
                    this._actualMarginRight = actualMarginRight;
                }
                return this._actualMarginRight;
            }
        }

        /// <summary>
        /// Gets the actual top border width
        /// </summary>
        public double ActualBorderTopWidth
        {
            get
            {
                if (double.IsNaN(this._actualBorderTopWidth))
                {
                    this._actualBorderTopWidth = CssValueParser.GetActualBorderWidth(BorderTopWidth, this);
                    if (string.IsNullOrEmpty(BorderTopStyle) || BorderTopStyle == CssConstants.None)
                    {
                        this._actualBorderTopWidth = 0f;
                    }
                }
                return this._actualBorderTopWidth;
            }
        }

        /// <summary>
        /// Gets the actual Left border width
        /// </summary>
        public double ActualBorderLeftWidth
        {
            get
            {
                if (double.IsNaN(this._actualBorderLeftWidth))
                {
                    this._actualBorderLeftWidth = CssValueParser.GetActualBorderWidth(BorderLeftWidth, this);
                    if (string.IsNullOrEmpty(BorderLeftStyle) || BorderLeftStyle == CssConstants.None)
                    {
                        this._actualBorderLeftWidth = 0f;
                    }
                }
                return this._actualBorderLeftWidth;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border width
        /// </summary>
        public double ActualBorderBottomWidth
        {
            get
            {
                if (double.IsNaN(this._actualBorderBottomWidth))
                {
                    this._actualBorderBottomWidth = CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);
                    if (string.IsNullOrEmpty(BorderBottomStyle) || BorderBottomStyle == CssConstants.None)
                    {
                        this._actualBorderBottomWidth = 0f;
                    }
                }
                return this._actualBorderBottomWidth;
            }
        }

        /// <summary>
        /// Gets the actual Right border width
        /// </summary>
        public double ActualBorderRightWidth
        {
            get
            {
                if (double.IsNaN(this._actualBorderRightWidth))
                {
                    this._actualBorderRightWidth = CssValueParser.GetActualBorderWidth(BorderRightWidth, this);
                    if (string.IsNullOrEmpty(BorderRightStyle) || BorderRightStyle == CssConstants.None)
                    {
                        this._actualBorderRightWidth = 0f;
                    }
                }
                return this._actualBorderRightWidth;
            }
        }

        /// <summary>
        /// Gets the actual top border Color
        /// </summary>
        public RColor ActualBorderTopColor
        {
            get
            {
                if (this._actualBorderTopColor.IsEmpty)
                {
                    this._actualBorderTopColor = GetActualColor(BorderTopColor);
                }
                return this._actualBorderTopColor;
            }
        }

        protected abstract RPoint GetActualLocation(string X, string Y);

        protected abstract RColor GetActualColor(string colorStr);

        /// <summary>
        /// Gets the actual Left border Color
        /// </summary>
        public RColor ActualBorderLeftColor
        {
            get
            {
                if ((this._actualBorderLeftColor.IsEmpty))
                {
                    this._actualBorderLeftColor = GetActualColor(BorderLeftColor);
                }
                return this._actualBorderLeftColor;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border Color
        /// </summary>
        public RColor ActualBorderBottomColor
        {
            get
            {
                if ((this._actualBorderBottomColor.IsEmpty))
                {
                    this._actualBorderBottomColor = GetActualColor(BorderBottomColor);
                }
                return this._actualBorderBottomColor;
            }
        }

        /// <summary>
        /// Gets the actual Right border Color
        /// </summary>
        public RColor ActualBorderRightColor
        {
            get
            {
                if ((this._actualBorderRightColor.IsEmpty))
                {
                    this._actualBorderRightColor = GetActualColor(BorderRightColor);
                }
                return this._actualBorderRightColor;
            }
        }

        /// <summary>
        /// Gets the actual length of the north west corner
        /// </summary>
        public double ActualCornerNw
        {
            get
            {
                if (double.IsNaN(this._actualCornerNw))
                {
                    this._actualCornerNw = CssValueParser.ParseLength(CornerNwRadius, 0, this);
                }
                return this._actualCornerNw;
            }
        }

        /// <summary>
        /// Gets the actual length of the north east corner
        /// </summary>
        public double ActualCornerNe
        {
            get
            {
                if (double.IsNaN(this._actualCornerNe))
                {
                    this._actualCornerNe = CssValueParser.ParseLength(CornerNeRadius, 0, this);
                }
                return this._actualCornerNe;
            }
        }

        /// <summary>
        /// Gets the actual length of the south east corner
        /// </summary>
        public double ActualCornerSe
        {
            get
            {
                if (double.IsNaN(this._actualCornerSe))
                {
                    this._actualCornerSe = CssValueParser.ParseLength(CornerSeRadius, 0, this);
                }
                return this._actualCornerSe;
            }
        }

        /// <summary>
        /// Gets the actual length of the south west corner
        /// </summary>
        public double ActualCornerSw
        {
            get
            {
                if (double.IsNaN(this._actualCornerSw))
                {
                    this._actualCornerSw = CssValueParser.ParseLength(CornerSwRadius, 0, this);
                }
                return this._actualCornerSw;
            }
        }

        /// <summary>
        /// Gets a value indicating if at least one of the corners of the box is rounded
        /// </summary>
        public bool IsRounded
        {
            get { return ActualCornerNe > 0f || ActualCornerNw > 0f || ActualCornerSe > 0f || ActualCornerSw > 0f; }
        }

        /// <summary>
        /// Gets the actual width of whitespace between words.
        /// </summary>
        public double ActualWordSpacing
        {
            get { return this._actualWordSpacing; }
        }

        /// <summary>
        /// 
        /// Gets the actual color for the text.
        /// </summary>
        public RColor ActualColor
        {
            get
            {
                if (this._actualColor.IsEmpty)
                {
                    this._actualColor = GetActualColor(Color);
                }

                return this._actualColor;
            }
        }

        /// <summary>
        /// Gets the actual background color of the box
        /// </summary>
        public RColor ActualBackgroundColor
        {
            get
            {
                if (this._actualBackgroundColor.IsEmpty)
                {
                    this._actualBackgroundColor = GetActualColor(BackgroundColor);
                }

                return this._actualBackgroundColor;
            }
        }

        /// <summary>
        /// Gets the second color that creates a gradient for the background
        /// </summary>
        public RColor ActualBackgroundGradient
        {
            get
            {
                if (this._actualBackgroundGradient.IsEmpty)
                {
                    this._actualBackgroundGradient = GetActualColor(BackgroundGradient);
                }
                return this._actualBackgroundGradient;
            }
        }

        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public double ActualBackgroundGradientAngle
        {
            get
            {
                if (double.IsNaN(this._actualBackgroundGradientAngle))
                {
                    this._actualBackgroundGradientAngle = CssValueParser.ParseNumber(BackgroundGradientAngle, 360f);
                }

                return this._actualBackgroundGradientAngle;
            }
        }

        /// <summary>
        /// Gets the actual font of the parent
        /// </summary>
        public RFont ActualParentFont
        {
            get { return GetParent() == null ? ActualFont : GetParent().ActualFont; }
        }

        /// <summary>
        /// Gets the font that should be actually used to paint the text of the box
        /// </summary>
        public RFont ActualFont
        {
            get
            {
                if (this._actualFont == null)
                {
                    if (string.IsNullOrEmpty(FontFamily))
                    {
                        FontFamily = CssConstants.DefaultFont;
                    }
                    if (string.IsNullOrEmpty(FontSize))
                    {
                        FontSize = CssConstants.FontSize.ToString(CultureInfo.InvariantCulture) + "pt";
                    }

                    RFontStyle st = RFontStyle.Regular;

                    if (FontStyle == CssConstants.Italic || FontStyle == CssConstants.Oblique)
                    {
                        st |= RFontStyle.Italic;
                    }

                    if (FontWeight != CssConstants.Normal && FontWeight != CssConstants.Lighter && !string.IsNullOrEmpty(FontWeight) && FontWeight != CssConstants.Inherit)
                    {
                        st |= RFontStyle.Bold;
                    }

                    double fsize;
                    double parentSize = CssConstants.FontSize;

                    if (GetParent() != null)
                        parentSize = GetParent().ActualFont.Size;

                    switch (FontSize)
                    {
                        case CssConstants.Medium:
                            fsize = CssConstants.FontSize;
                            break;
                        case CssConstants.XXSmall:
                            fsize = CssConstants.FontSize - 4;
                            break;
                        case CssConstants.XSmall:
                            fsize = CssConstants.FontSize - 3;
                            break;
                        case CssConstants.Small:
                            fsize = CssConstants.FontSize - 2;
                            break;
                        case CssConstants.Large:
                            fsize = CssConstants.FontSize + 2;
                            break;
                        case CssConstants.XLarge:
                            fsize = CssConstants.FontSize + 3;
                            break;
                        case CssConstants.XXLarge:
                            fsize = CssConstants.FontSize + 4;
                            break;
                        case CssConstants.Smaller:
                            fsize = parentSize - 2;
                            break;
                        case CssConstants.Larger:
                            fsize = parentSize + 2;
                            break;
                        default:
                            fsize = CssValueParser.ParseLength(FontSize, parentSize, parentSize, null, true, true);
                            break;
                    }

                    if (fsize <= 1f)
                    {
                        fsize = CssConstants.FontSize;
                    }

                    this._actualFont = GetCachedFont(FontFamily, fsize, st);
                }
                return this._actualFont;
            }
        }

        protected abstract RFont GetCachedFont(string fontFamily, double fsize, RFontStyle st);

        /// <summary>
        /// Gets the line height
        /// </summary>
        public double ActualLineHeight
        {
            get
            {
                if (double.IsNaN(this._actualLineHeight))
                {
                    this._actualLineHeight = .9f * CssValueParser.ParseLength(LineHeight, Size.Height, this);
                }
                return this._actualLineHeight;
            }
        }

        /// <summary>
        /// Gets the text indentation (on first line only)
        /// </summary>
        public double ActualTextIndent
        {
            get
            {
                if (double.IsNaN(this._actualTextIndent))
                {
                    this._actualTextIndent = CssValueParser.ParseLength(TextIndent, Size.Width, this);
                }

                return this._actualTextIndent;
            }
        }

        /// <summary>
        /// Gets the actual horizontal border spacing for tables
        /// </summary>
        public double ActualBorderSpacingHorizontal
        {
            get
            {
                if (double.IsNaN(this._actualBorderSpacingHorizontal))
                {
                    MatchCollection matches = RegexParserUtils.Match(RegexParserUtils.CssLength, BorderSpacing);

                    if (matches.Count == 0)
                    {
                        this._actualBorderSpacingHorizontal = 0;
                    }
                    else if (matches.Count > 0)
                    {
                        this._actualBorderSpacingHorizontal = CssValueParser.ParseLength(matches[0].Value, 1, this);
                    }
                }


                return this._actualBorderSpacingHorizontal;
            }
        }

        /// <summary>
        /// Gets the actual vertical border spacing for tables
        /// </summary>
        public double ActualBorderSpacingVertical
        {
            get
            {
                if (double.IsNaN(this._actualBorderSpacingVertical))
                {
                    MatchCollection matches = RegexParserUtils.Match(RegexParserUtils.CssLength, BorderSpacing);

                    if (matches.Count == 0)
                    {
                        this._actualBorderSpacingVertical = 0;
                    }
                    else if (matches.Count == 1)
                    {
                        this._actualBorderSpacingVertical = CssValueParser.ParseLength(matches[0].Value, 1, this);
                    }
                    else
                    {
                        this._actualBorderSpacingVertical = CssValueParser.ParseLength(matches[1].Value, 1, this);
                    }
                }
                return this._actualBorderSpacingVertical;
            }
        }

        /// <summary>
        /// Get the parent of this css properties instance.
        /// </summary>
        /// <returns></returns>
        protected abstract CssBoxProperties GetParent();

        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        /// <returns></returns>
        public double GetEmHeight()
        {
            return ActualFont.Height;
        }

        /// <summary>
        /// Ensures that the specified length is converted to pixels if necessary
        /// </summary>
        /// <param name="length"></param>
        protected string NoEms(string length)
        {
            var len = new CssLength(length);
            if (len.Unit == CssUnit.Ems)
            {
                length = len.ConvertEmToPixels(GetEmHeight()).ToString();
            }
            return length;
        }

        /// <summary>
        /// Set the style/width/color for all 4 borders on the box.<br/>
        /// if null is given for a value it will not be set.
        /// </summary>
        /// <param name="style">optional: the style to set</param>
        /// <param name="width">optional: the width to set</param>
        /// <param name="color">optional: the color to set</param>
        protected void SetAllBorders(string style = null, string width = null, string color = null)
        {
            if (style != null)
                BorderLeftStyle = BorderTopStyle = BorderRightStyle = BorderBottomStyle = style;
            if (width != null)
                BorderLeftWidth = BorderTopWidth = BorderRightWidth = BorderBottomWidth = width;
            if (color != null)
                BorderLeftColor = BorderTopColor = BorderRightColor = BorderBottomColor = color;
        }

        /// <summary>
        /// Measures the width of whitespace between words (set <see cref="ActualWordSpacing"/>).
        /// </summary>
        protected void MeasureWordSpacing(RGraphics g)
        {
            if (double.IsNaN(ActualWordSpacing))
            {
                this._actualWordSpacing = CssUtils.WhiteSpace(g, this);
                if (WordSpacing != CssConstants.Normal)
                {
                    string len = RegexParserUtils.Search(RegexParserUtils.CssLength, WordSpacing);
                    this._actualWordSpacing += CssValueParser.ParseLength(len, 1, this);
                }
            }
        }

        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        /// <param name="everything">Set to true to inherit all CSS properties instead of only the ineritables</param>
        /// <param name="p">Box to inherit the properties</param>
        protected void InheritStyle(CssBox p, bool everything)
        {
            if (p != null)
            {
                this._borderSpacing = p._borderSpacing;
                this._borderCollapse = p._borderCollapse;
                this._color = p._color;
                this._emptyCells = p._emptyCells;
                this._whiteSpace = p._whiteSpace;
                this._visibility = p._visibility;
                this._textIndent = p._textIndent;
                this._textAlign = p._textAlign;
                this._verticalAlign = p._verticalAlign;
                this._fontFamily = p._fontFamily;
                this._fontSize = p._fontSize;
                this._fontStyle = p._fontStyle;
                this._fontVariant = p._fontVariant;
                this._fontWeight = p._fontWeight;
                this._listStyleImage = p._listStyleImage;
                this._listStylePosition = p._listStylePosition;
                this._listStyleType = p._listStyleType;
                this._listStyle = p._listStyle;
                this._lineHeight = p._lineHeight;
                this._wordBreak = p.WordBreak;
                this._direction = p._direction;

                if (everything)
                {
                    this._backgroundColor = p._backgroundColor;
                    this._backgroundGradient = p._backgroundGradient;
                    this._backgroundGradientAngle = p._backgroundGradientAngle;
                    this._backgroundImage = p._backgroundImage;
                    this._backgroundPosition = p._backgroundPosition;
                    this._backgroundRepeat = p._backgroundRepeat;
                    this._borderTopWidth = p._borderTopWidth;
                    this._borderRightWidth = p._borderRightWidth;
                    this._borderBottomWidth = p._borderBottomWidth;
                    this._borderLeftWidth = p._borderLeftWidth;
                    this._borderTopColor = p._borderTopColor;
                    this._borderRightColor = p._borderRightColor;
                    this._borderBottomColor = p._borderBottomColor;
                    this._borderLeftColor = p._borderLeftColor;
                    this._borderTopStyle = p._borderTopStyle;
                    this._borderRightStyle = p._borderRightStyle;
                    this._borderBottomStyle = p._borderBottomStyle;
                    this._borderLeftStyle = p._borderLeftStyle;
                    this._bottom = p._bottom;
                    this._cornerNwRadius = p._cornerNwRadius;
                    this._cornerNeRadius = p._cornerNeRadius;
                    this._cornerSeRadius = p._cornerSeRadius;
                    this._cornerSwRadius = p._cornerSwRadius;
                    this._cornerRadius = p._cornerRadius;
                    this._display = p._display;
                    this._float = p._float;
                    this._height = p._height;
                    this._marginBottom = p._marginBottom;
                    this._marginLeft = p._marginLeft;
                    this._marginRight = p._marginRight;
                    this._marginTop = p._marginTop;
                    this._left = p._left;
                    this._lineHeight = p._lineHeight;
                    this._overflow = p._overflow;
                    this._paddingLeft = p._paddingLeft;
                    this._paddingBottom = p._paddingBottom;
                    this._paddingRight = p._paddingRight;
                    this._paddingTop = p._paddingTop;
                    this._right = p._right;
                    this._textDecoration = p._textDecoration;
                    this._top = p._top;
                    this._position = p._position;
                    this._width = p._width;
                    this._maxWidth = p._maxWidth;
                    this._wordSpacing = p._wordSpacing;
                }
            }
        }
    }
}