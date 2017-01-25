using System;
using System.Threading;
using System.Windows.Forms;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Logic;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Native;

namespace AirSuperiority.ScriptBase
{
    public class Main : ScriptThread
    {
        public Main()
        {
            PreInit();
            KeyDown += OnKeyDown;
        }

        private void PreInit()
        {
            Utility.ToggleOnlineDLC(true);
            AddExtension("sess", new SessionManager());
            AddExtension("input", new InputManager());
            AddExtension("ui", new DisplayManager());
            AddExtension("map", new LevelManager());
            AddExtension("ai", new AIManager());    
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N)
            {
                foreach (var vehicle in World.GetAllVehicles())
                {
                    vehicle.Delete();
                }

                var extension = GetExtension("sess") as SessionManager;

                extension.Initialize(0, 12,3);

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
