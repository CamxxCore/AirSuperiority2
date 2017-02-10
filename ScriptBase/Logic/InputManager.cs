using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Logic
{
    /// <summary>
    /// Class to handle global game input.
    /// </summary>
    /// TODO: Remove this class because its not really needed?
    public class InputManager : ScriptComponent
    {
        public GamepadInterface Gamepad { get; } = new GamepadInterface();

        /// <summary>
        /// Update the class.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void OnUpdate(int gameTime)
        {
            Gamepad.Update();

            base.OnUpdate(gameTime);
        }

        /// <summary>
        /// Disable user controls.
        /// </summary>
        public void DisableControls()
        {
            Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);
        }

        /// <summary>
        /// Renable user controls.
        /// </summary>
        public void EnableControls()
        {
            Function.Call(Hash.ENABLE_ALL_CONTROL_ACTIONS, 0);
        }
    }
}
