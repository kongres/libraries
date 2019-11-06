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

namespace HtmlRenderer.Core.Adapters.Entities
{
    using HtmlRenderer.Core.Core;

    /// <summary>
    /// Even class for handling keyboard events in <see cref="HtmlContainerInt"/>.
    /// </summary>
    public sealed class RKeyEvent
    {
        /// <summary>
        /// is control is pressed
        /// </summary>
        private readonly bool _control;

        /// <summary>
        /// is 'A' key is pressed
        /// </summary>
        private readonly bool _aKeyCode;

        /// <summary>
        /// is 'C' key is pressed
        /// </summary>
        private readonly bool _cKeyCode;

        /// <summary>
        /// Init.
        /// </summary>
        public RKeyEvent(bool control, bool aKeyCode, bool cKeyCode)
        {
            this._control = control;
            this._aKeyCode = aKeyCode;
            this._cKeyCode = cKeyCode;
        }

        /// <summary>
        /// is control is pressed
        /// </summary>
        public bool Control
        {
            get { return this._control; }
        }

        /// <summary>
        /// is 'A' key is pressed
        /// </summary>
        public bool AKeyCode
        {
            get { return this._aKeyCode; }
        }

        /// <summary>
        /// is 'C' key is pressed
        /// </summary>
        public bool CKeyCode
        {
            get { return this._cKeyCode; }
        }
    }
}