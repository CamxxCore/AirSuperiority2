using System;

namespace AirSuperiority.ScriptBase.Helpers
{
    public class ButtonPressedEventArgs : EventArgs
    {
        private int _value;

        public ButtonPressedEventArgs(int value)
        {
            _value = value;
        }

        /// <summary>
        /// The amount of force applied to the button, from 0 - 254.
        /// </summary>
        public int Value { get { return _value; } }
    }
}


