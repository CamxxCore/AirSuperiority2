#define DEBUG
using System;
using System.Linq;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows.Forms;
using AirSuperiority.Core;
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
         //   Utility.ToggleOnlineDLC(true);
            inputMgr = new InputManager(this);
            sessionMgr = new SessionManager(this);
            displayMgr = new DisplayManager(this);
            levelMgr = new LevelManager(this);
        }

        public static InputManager GetInputManager()
        {
            return inputMgr;
        }

        /// <summary>
        /// Get the session manager.
        /// </summary>
        public static SessionManager GetSessionManager()
        {
            return sessionMgr;
        }

        /// <summary>
        /// Get the display manager.
        /// </summary>
        public static DisplayManager GetDisplayManager()
        {
            return displayMgr;
        }

        /// <summary>
        /// Gets the level manager.
        /// </summary>
        public static LevelManager GetLevelManager()
        {
            return levelMgr;
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
               // var sess = sessionMgr;

               // UI.ShowSubtitle(GTA.Math.Vector3.Lerp(sess.GetTeamByIndex(0).SpawnPoint.Position, sess.GetTeamByIndex(1).SpawnPoint.Position, 0.5f).ToString(), 10000);
             //   var metadata = Resources.GetByName<Types.Metadata.MapAreaAssetMetadata>("MapArea");
             
            //    var mapData = metadata.FirstOrDefault(m => m.LevelIndex == 0);

              //  var rdm = Utility.GetRandomPositionInArea(mapData.BoundsMin, mapData.BoundsMax).ToString();

              //  UI.ShowSubtitle(rdm + " " + mapData.BoundsMax.ToString() + " " + mapData.BoundsMin.ToString());
            }

            if (e.KeyCode == Keys.N)
            {
               Utility.FadeInScreen(1);

               foreach (var vehicle in World.GetAllEntities())
                {
                    vehicle.Delete();
                }


                displayMgr.HideScoreboard();

                sessionMgr.Initialize(0, 32, 2);

            

             //   UI.ShowSubtitle(

                /* Blip wpBlip = new Blip(Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, 8));

                  if (Function.Call<bool>(Hash.IS_WAYPOINT_ACTIVE))
                  {
                      GTA.Math.Vector3 wpVec = Function.Call<GTA.Math.Vector3>(Hash.GET_BLIP_COORDS, wpBlip);

                      UI.ShowSubtitle(wpBlip.Position.ToString(), 10000);
                  }
                  GTA.World.CreateBlip(new GTA.Math.Vector3( 3103.298f, -4818.269f, 15.76617f));

                  var pos = GTA.Game.Player.Character.Position;

                  var rad = pos.Radiate(1430.0f, 12.0f, (float)curr);

                  curr--;

                  GTA.World.CreateBlip(rad);

                  GTA.UI.ShowSubtitle(rad.ToString(), 10000);

                  var dir = GTA.Math.Vector3.Normalize(pos - rad);

                  float heading = GTA.Native.Function.Call<float>(GTA.Native.Hash.GET_HEADING_FROM_VECTOR_2D, dir.X, dir.Y);*/
            }
        }

        protected override void Dispose(bool A_0)
        {
            foreach (var blip in World.GetActiveBlips())
            {
                blip.Remove();
            }

            base.Dispose(A_0);
        }
    }
}
