using System;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Logic
{
    /// <summary>
    /// Class to handle global game input.
    /// </summary>
    public class InputManager : ScriptExtension
    {
        public GamepadInterface Gamepad { get; } = new GamepadInterface();

        private bool controlsDisabled = false;

        public override void OnUpdate()
        {
            Gamepad.Update();

            if (controlsDisabled)
            {
                Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);
            }

            base.OnUpdate();
        }

        public bool ControlPressed(int control)
        {
            return Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, control);
        }

        public bool ControlReleased(int control)
        {
            return Function.Call<bool>(Hash.IS_CONTROL_JUST_RELEASED, 0, control);
        }

        public bool DisabledControlPressed(int control)
        {
            return Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_PRESSED, 0, control);
        }

        public bool DisabledControlReleased(int control)
        {
            return Function.Call<bool>(Hash.IS_DISABLED_CONTROL_JUST_RELEASED, 0, control);
        }

        public bool ControlPressed(Control control)
        {
            return ControlPressed((int)control);
        }

        public bool ControlReleased(Control control)
        {
            return ControlReleased((int)control);
        }

        public bool DisabledControlPressed(Control control)
        {
            return DisabledControlPressed((int)control);
        }

        public bool DisabledControlReleased(Control control)
        {
            return DisabledControlReleased((int)control);
        }

        public int GetControlValue(Control control)
        {
            return Function.Call<int>(Hash.GET_CONTROL_VALUE, 0, (int)control);
        }

        public void DisableControls()
        {
            controlsDisabled = true;
        }

        public void EnableControls()
        {
            controlsDisabled = false;
        }
    }
}
