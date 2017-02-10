using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Extensions;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Logic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Entities
{
    public class LocalPlayer : Player
    {
        public override void Create()
        {
            base.Create();

            var levelMgr = ScriptThread.GetOrAddExtension<LevelManager>();

            var sessMgr = ScriptThread.GetOrAddExtension<SessionManager>();

            LevelSpawn spawnPoint = levelMgr.GetSpawnPoint(Info.Sess.TeamNum);

            // spawn above to avoid collision with teammates.

            Vector3 position = Utility.EnsureValidSpawnPos(spawnPoint.Position + new Vector3(0, 0, 2.0f));

            Model model = new Model(VehicleHash.Lazer);

            if (!model.IsLoaded)
                model.Request(1000);

            var vehicle = World.CreateVehicle(model, position, spawnPoint.Heading);

            vehicle.LodDistance = 2000;
            vehicle.EngineRunning = true;
            vehicle.BodyHealth = 1000;

            vehicle.MaxSpeed = 280.0f;

            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle, true);

            Ped ped = Game.Player.Character;

            TeamData team = sessMgr.GetTeamByIndex(Info.Sess.TeamNum);

            ped.RelationshipGroup = team.RelationshipGroup;

            ped.SetIntoVehicle(vehicle, VehicleSeat.Driver);

            Manage(ped, vehicle);       
        }

        /// <summary>
        /// Setup additional extensions.
        /// </summary>
        public override void SetupExtensions()
        {
            base.SetupExtensions();
            GetOrCreateExtension<SpawnVelocityBooster>();
            GetOrCreateExtension<SpawnLerpingCamera>();
            GetOrCreateExtension<PlayerHUDManager>();
            GetOrCreateExtension<OffscreenTargetTracker>();
        }

        public override void OnUpdate(int gameTime)
        {
            if (!ScriptThread.GetVar<bool>("scr_hardcore").Value)
            {
                if (Game.IsControlJustPressed(0, Control.ScriptLB) || Game.IsControlJustPressed(0, (Control)48))
                {
                    var extinguisher = GetExtension<EngineExtinguisher>();

                    extinguisher.Start();
                }

                else if (Game.IsControlJustPressed(0, Control.ScriptRB) || Game.IsControlJustPressed(0, (Control)337))
                {
                    var flareMgr = GetExtension<IRFlareManager>();

                    flareMgr.Start();
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
