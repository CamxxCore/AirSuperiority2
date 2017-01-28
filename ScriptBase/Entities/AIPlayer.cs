using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Extensions;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Entities
{
    public class AIPlayer : Player
    {
        public AIPlayer(ScriptThread thread, PlayerInfo info) : base(thread, info)
        {
        }

        public override void Create(LevelSpawn spawnPoint)
        {
            base.Create(spawnPoint);

            Vector3 position = Utility.EnsureValidSpawnPos(spawnPoint.Position);

            /* Model model = new Model(new VehicleHash[] {
                 VehicleHash.Lazer,
                 VehicleHash.Besra,
                 VehicleHash.Hydra }.GetRandomItem());
             */

            Model model = new Model(VehicleHash.Lazer);

            if (!model.IsLoaded)
                model.Request(1000);

            var vehicle = World.CreateVehicle(model, position, spawnPoint.Heading);

            vehicle.LodDistance = 2000;
            vehicle.EngineRunning = true;
            vehicle.BodyHealth = 0.01f;
            vehicle.MaxHealth = 1;

            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle, true);

            model = new Model(PedHash.Pilot02SMM);

            if (!model.IsLoaded)
                model.Request(1000);

            Ped ped = World.CreatePed(model, position);

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 1, 1);

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 52, 0);

            Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped, false);

            ped.SetIntoVehicle(vehicle, VehicleSeat.Driver);

            Manage(ped, vehicle);
        }

        /// <summary>
        /// Setup additional extensions.
        /// </summary>
        public override void SetupExtensions()
        {
            base.SetupExtensions();

            AddExtension(new PilotAIController(BaseThread, this));
            AddExtension(new EntityInfoOverlay(BaseThread, this));
        }
    }
}
