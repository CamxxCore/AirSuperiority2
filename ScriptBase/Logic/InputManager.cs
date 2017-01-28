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

        public InputManager(ScriptThread thread) : base(thread)
        { }

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
