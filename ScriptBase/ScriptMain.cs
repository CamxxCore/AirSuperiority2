#define DEBUG
using System;
using System.Linq;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows.Forms;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Memory;
using AirSuperiority.ScriptBase.Logic;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Native;

namespace AirSuperiority.ScriptBase
{
    public class ScriptMain : ScriptThread
    {
        private static InputManager inputMgr;
        private static SessionManager sessionMgr;
        private static DisplayManager displayMgr;
        private static LevelManager levelMgr;

        public ScriptMain()
        {
            PreInit();
            KeyDown += OnKeyDown;
        }

        private void PreInit()
        {
            Game.Globals[4].SetInt(1); // disable respawn_controller

            Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "respawn_controller");

            Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, true);

            Function.Call(Hash.SET_WANTED_LEVEL_MULTIPLIER, 0.0);

            Function.Call(Hash.SET_MAX_WANTED_LEVEL, 0);

            MemoryAccess.PatchFlyingMusic();

            //Utility.ToggleOnlineDLC(true);

            inputMgr = new InputManager(this);

            levelMgr = new LevelManager(this);

            sessionMgr = new SessionManager(this);

            displayMgr = new DisplayManager(this);
        }

        /// <summary>
        /// Writes debug output to the screen.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">The objects to format.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DebugPrint(string format, params object[] args)
        {
#if DEBUG
            displayMgr.WriteDebugLine(string.Format(format, args));
#endif
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.M)
            {
                displayMgr.ShowWarningThisFrame("ENEMY LOCKING");
            }

            if (e.KeyCode == Keys.N)
            {
              // Utility.FadeInScreen(1);

               foreach (var vehicle in World.GetAllEntities())
                {
                    vehicle.Delete();
                }

                displayMgr.HideScoreboard();

                sessionMgr.Initialize(0, 12, 2);        
            }
        }

        public override void OnUpdate(int gameTime)
        {
            Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.0f);

            Function.Call(Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.0f);

            Function.Call((Hash)0x90B6DA738A9A25DA, 0.0f);

            base.OnUpdate(gameTime);
        }

        protected override void Dispose(bool A_0)
        {
            Game.Globals[4].SetInt(0); // enable respawn_controller

            Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, false);

            MemoryAccess.OnExit();

            base.Dispose(A_0);
        }
    }
}
