#define DEBUG
using System;
using System.Linq;
using System.Threading;
using System.Runtime.CompilerServices;
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
        private static DisplayManager displayMgr;

        private const int RespawnClrGlobalIdx = 4;

        public ScriptMain()
        {
            RegisterVar("scr_activesession", false);
            RegisterVar("scr_maxteams", 32);
            RegisterVar("scr_maxplayers", 64);
            RegisterVar("scr_hardcore", false);
            RegisterVar("scr_score_per_kill", 1000);
            RegisterVar("scr_score_per_assist", 500);
            RegisterVar("scr_score_per_death", 0);

            SetupExtensions();
        }

        private void SetupExtensions()
        {
            AddExtension<MenuManager>();
            displayMgr = GetOrAddExtension<DisplayManager>();  
        }

        private void SetupWorld()
        {
            Utility.RequestPTFXAsset("scr_oddjobtraffickingair"); // for water explosions

            Game.Globals[RespawnClrGlobalIdx].SetInt(1); // disable respawn_controller

            Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "respawn_controller");

            Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, true);

            Game.WantedMultiplier = 0.0f;

            Game.MaxWantedLevel = 0;

            //  Utility.ToggleOnlineDLC(true);

            MemoryAccess.PatchFlyingMusic();
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

        public override void OnUpdate(int gameTime)
        {
            if (GetVar<bool>("scr_activesession").Value)
            {
                Function.Call(Hash.SET_PED_DENSITY_MULTIPLIER_THIS_FRAME, 0.0f);

                Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.0f);

                Function.Call(Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.0f);

                Function.Call((Hash)0x90B6DA738A9A25DA, 0.0f);
            }

            base.OnUpdate(gameTime);
        }

        protected override void Dispose(bool A_0)
        {
            base.Dispose(A_0);

            Game.Globals[RespawnClrGlobalIdx].SetInt(0); // enable respawn_controller

            Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, false);

            MemoryAccess.OnExit();
        }
    }
}
