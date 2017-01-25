using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
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

        /// <summary>
        /// Update the class.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void OnUpdate(int gameTime)
        {
            Gamepad.Update();

            if (controlsDisabled)
            {
                Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);
            }

            base.OnUpdate(gameTime);
        }

        /// <summary>
        /// Disable user controls.
        /// </summary>
        public void DisableControls()
        {
            controlsDisabled = true;
        }

        /// <summary>
        /// Renable user controls.
        /// </summary>
        public void EnableControls()
        {
            controlsDisabled = false;
        }
    }
}
