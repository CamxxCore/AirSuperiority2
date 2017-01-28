using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Native;
using GTA;

namespace AirSuperiority.ScriptBase.Helpers
{
    public delegate void ButtonPressedEventHandler(object sender, ButtonPressedEventArgs e);

    public delegate void TriggerChangedEventHandler(object sender, TriggerChangedEventArgs e);

    public delegate void AnalogStickChangedEventHandler(object sender, AnalogStickChangedEventArgs e);

    /// <summary>
    /// Interface to expose gamepad events.
    /// </summary>
    public class GamepadInterface
    {
        #region Declare Events
        /// <summary>
        /// Called when the user presses the A button.
        /// </summary>
        public event ButtonPressedEventHandler AButtonPressed;

        /// <summary>
        /// Called when the user presses the B button.
        /// </summary>
        public event ButtonPressedEventHandler BButtonPressed;

        /// <summary>
        /// Called when the user presses the X button.
        /// </summary>
        public event ButtonPressedEventHandler XButtonPressed;

        /// <summary>
        /// Called when the user presses the Y button.
        /// </summary>
        public event ButtonPressedEventHandler YButtonPressed;

        /// <summary>
        /// Called when the user presses the right trigger.
        /// </summary>
        public event TriggerChangedEventHandler RightTriggerChanged;

        /// <summary>
        /// Called when the user presses the left trigger.
        /// </summary>
        public event TriggerChangedEventHandler LeftTriggerChanged;

        /// <summary>
        /// Called when the user presses the right bumper.
        /// </summary>
        public event ButtonPressedEventHandler RightBumperPressed;

        /// <summary>
        /// Called when the user presses the left bumper.
        /// </summary>
        public event ButtonPressedEventHandler LeftBumperPressed;

        /// <summary>
        /// Called when the user uses the analog stick.
        /// </summary>
        public event AnalogStickChangedEventHandler LeftStickChanged;

        /// <summary>
        /// Called when the user uses the analog stick.
        /// </summary>
        public event AnalogStickChangedEventHandler RightStickChanged;

        /// <summary>
        /// Called when the user uses the analog stick.
        /// </summary>
        public event ButtonPressedEventHandler LeftStickPressed;

        /// <summary>
        /// Called when the user uses the analog stick.
        /// </summary>
        public event ButtonPressedEventHandler RightStickPressed;

        #endregion

        public void Update()
        {
            int xAxis = GetControlValue(Control.ScriptLeftAxisX);
            int yAxis = GetControlValue(Control.ScriptLeftAxisY);

            if (xAxis != 127 || yAxis != 127)
            {
                OnLeftStickChanged(new AnalogStickChangedEventArgs(xAxis, yAxis));
            }

            xAxis = GetControlValue(Control.ScriptRightAxisX);
            yAxis = GetControlValue(Control.ScriptRightAxisY);

            if (xAxis != 127 || yAxis != 127)
            {
                OnRightStickChanged(new AnalogStickChangedEventArgs(xAxis, yAxis));
            }

            if (GetControlValue(Control.ScriptLT) > 127)
                OnLeftTriggerChanged(new TriggerChangedEventArgs(GetControlValue(Control.ScriptLT)));
            if (GetControlValue(Control.ScriptRT) > 127)
                OnRightTriggerChanged(new TriggerChangedEventArgs(GetControlValue(Control.ScriptRT)));
            if (GetControlInput(Control.ScriptLS))
                OnLeftStickPressed(new ButtonPressedEventArgs(GetControlValue(Control.ScriptLS)));
            if (GetControlInput(Control.ScriptRS))
                OnRightStickPressed(new ButtonPressedEventArgs(GetControlValue(Control.ScriptRS)));
            if (GetControlInput(Control.ScriptRUp))
                OnYPressed(new ButtonPressedEventArgs(GetControlValue(Control.ScriptRUp)));
            if (GetControlInput(Control.ScriptRDown))
                OnAPressed(new ButtonPressedEventArgs(GetControlValue(Control.ScriptRDown)));
            if (GetControlInput(Control.ScriptRLeft))
                OnXPressed(new ButtonPressedEventArgs(GetControlValue(Control.ScriptRLeft)));
            if (GetControlInput(Control.ScriptRRight))
                OnBPressed(new ButtonPressedEventArgs(GetControlValue(Control.ScriptRRight)));
            if (GetControlInput(Control.ScriptLB))
                OnLBPressed(new ButtonPressedEventArgs(GetControlValue(Control.ScriptLB)));
            if (GetControlInput(Control.ScriptRB))
                OnRBPressed(new ButtonPressedEventArgs(GetControlValue(Control.ScriptRB)));
        }

        #region Event Handlers

        protected virtual void OnAPressed(ButtonPressedEventArgs e)
        {
            AButtonPressed?.Invoke(this, e);
        }

        protected virtual void OnBPressed(ButtonPressedEventArgs e)
        {
            BButtonPressed?.Invoke(this, e);
        }

        protected virtual void OnXPressed(ButtonPressedEventArgs e)
        {
            XButtonPressed?.Invoke(this, e);
        }

        protected virtual void OnYPressed(ButtonPressedEventArgs e)
        {
            YButtonPressed?.Invoke(this, e);
        }

        protected virtual void OnLBPressed(ButtonPressedEventArgs e)
        {
            LeftBumperPressed?.Invoke(this, e);
        }

        protected virtual void OnRBPressed(ButtonPressedEventArgs e)
        {
            RightBumperPressed?.Invoke(this, e);
        }

        protected virtual void OnLeftTriggerChanged(TriggerChangedEventArgs e)
        {
            LeftTriggerChanged?.Invoke(this, e);
        }

        protected virtual void OnRightTriggerChanged(TriggerChangedEventArgs e)
        {
            RightTriggerChanged?.Invoke(this, e);
        }

        protected virtual void OnLeftStickChanged(AnalogStickChangedEventArgs e)
        {
            LeftStickChanged?.Invoke(this, e);
        }

        protected virtual void OnRightStickChanged(AnalogStickChangedEventArgs e)
        {
            RightStickChanged?.Invoke(this, e);
        }

        protected virtual void OnLeftStickPressed(ButtonPressedEventArgs e)
        {
            LeftStickPressed?.Invoke(this, e);
        }

        protected virtual void OnRightStickPressed(ButtonPressedEventArgs e)
        {
            RightStickPressed?.Invoke(this, e);
        }

        #endregion

        private bool GetControlInput(Control control)
        {
            return Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, (int)control);
        }

        private int GetControlValue(Control control)
        {
            return Function.Call<int>(Hash.GET_CONTROL_VALUE, 0, (int)control);
        }
    }
}




