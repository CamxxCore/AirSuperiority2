using System;
using System.Diagnostics;
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
    public class AIPlayer : Player
    {
        public override void Create()
        {
            base.Create();

            var levelMgr = ScriptThread.GetOrAddExtension<LevelManager>();

            var sessMgr = ScriptThread.GetOrAddExtension<SessionManager>();

            LevelSpawn spawnPoint = levelMgr.GetSpawnPoint(Info.Sess.TeamNum);

            Vector3 position = Utility.EnsureValidSpawnPos(spawnPoint.Position);

            Debug.Assert(position != null, "AIPlayer.Create: Position was invalid.");

            Debug.Assert(position != Vector3.Zero, "AIPlayer.Create: Position was invalid.");

            Model model = new Model(new VehicleHash[] {
                 VehicleHash.Lazer,
                 VehicleHash.Besra,
                 VehicleHash.Hydra }.GetRandomItem());

            //  Model model = new Model(VehicleHash.Lazer);

            if (!model.IsLoaded)
                model.Request(1000);

            var vehicle = World.CreateVehicle(model, position, spawnPoint.Heading);

            vehicle.LodDistance = 2000;
            vehicle.EngineRunning = true;

            vehicle.BodyHealth = 0.01f;

            vehicle.SteeringScale = 5.0f;

            vehicle.PetrolTankHealth = 0.1f;

            vehicle.Health = 10;

            //     vehicle.MaxSpeed = 210;

            vehicle.MaxSpeed = 310.0f;

            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle, true);

            model = new Model(PedHash.Pilot02SMM);

            if (!model.IsLoaded)
                model.Request(1000);

            Ped ped = World.CreatePed(model, position);

            TeamData team = sessMgr.GetTeamByIndex(Info.Sess.TeamNum);

            ped.RelationshipGroup = team.RelationshipGroup;

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 1, 1);

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 52, 0);

            ped.SetIntoVehicle(vehicle, VehicleSeat.Driver);

            Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, ped, 0x7E0EDF1E);

            Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped, false);

            Function.Call(Hash.SET_DRIVE_TASK_MAX_CRUISE_SPEED, ped, 400.0f);
            //
            //  Function.Call(Hash.DISABLE_VEHICLE_WEAPON, true, 0x7E0EDF1E, vehicle, ped); //vehicle_weapon_enemy_lazer
            //
            //  Function.Call(Hash.DISABLE_VEHICLE_WEAPON, true, 0xCF0896E0, vehicle, ped); //VEHICLE_WEAPON_PLANE_ROCKET

            //  Function.Call(Hash.DISABLE_VEHICLE_WEAPON, true, 0xF8A3939F, vehicle, ped); //VEHICLE_WEAPON_PLANE_ROCKET         

            //  Function.Call(Hash.DISABLE_VEHICLE_WEAPON, true, 0x9F1A91DE, vehicle, ped); //VEHICLE_WEAPON_PLANE_ROCKET    

            // Function.Call(Hash.DISABLE_VEHICLE_WEAPON, true, 0xE2822A29, vehicle, ped); //VEHICLE_WEAPON_PLANE_ROCKET                   w

            Manage(ped, vehicle);
        }

        public override void OnUpdate(int gameTime)
        {
            if (ActiveTarget != null)
            {
                Vehicle.Ref.ApplyForce(Vehicle.Ref.ForwardVector * 0.4f);
            }

            var outArg = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, Ped.Ref, outArg))
            {
                var weaponHash = Function.Call<int>(Hash.GET_HASH_KEY, "vehicle_weapon_plane_rocket");

                if (outArg.GetResult<int>() != weaponHash)
                    Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, Ped.Ref, weaponHash);
            }

            base.OnUpdate(gameTime);
        }

        /// <summary>
        /// Setup additional extensions.
        /// </summary>
        public override void SetupExtensions()
        {
            base.SetupExtensions();
            GetOrCreateExtension<AISteeringController>();
            GetOrCreateExtension<PilotAIController>();
            GetOrCreateExtension<EntityInfoOverlay>();
        }
    }
}
