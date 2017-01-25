using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Extensions;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Entities
{
    public class LocalParticipant : GameParticipant
    {
        public LocalParticipant(string name, int teamIndex) : base(name, teamIndex)
        { }

        public override void CreateEntity(LevelSpawn spawnPoint)
        {
            Vector3 position = Utility.EnsureValidSpawnPos(spawnPoint.Position);

            Model model = new Model(VehicleHash.Lazer);

            if (!model.IsLoaded)
                model.Request(1000);

            var vehicle = World.CreateVehicle(model, position, spawnPoint.Heading);

            vehicle.LodDistance = 2000;
            vehicle.EngineRunning = true;
            vehicle.BodyHealth = 0.01f;
            vehicle.MaxHealth = 1;

            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle, true);

            Ped player = Game.Player.Character;

            player.SetIntoVehicle(vehicle, VehicleSeat.Driver);

            Manage(player, vehicle);
        }

        public override void Manage(Ped ped, Vehicle vehicle)
        {
            base.Manage(ped, vehicle);

            Vehicle.AddExtension(new VehicleSpawnLerpingCamera());
            Vehicle.AddExtension(new VehicleSpawnVelocityBooster());           
        }

        public override void OnUpdate(int gameTime)
        {
            if (Game.IsControlJustPressed(0, Control.ScriptLB) || Game.IsControlJustPressed(0, (Control)48))
            {
                Vehicle.DoFireExtinguisher();
            }

            else if (Game.IsControlJustPressed(0, Control.ScriptRB) || Game.IsControlJustPressed(0, (Control)337))
            {
                Vehicle.DoIRFlares();
            }

            base.OnUpdate(gameTime);
        }
    }
}
