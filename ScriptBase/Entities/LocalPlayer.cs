﻿using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Extensions;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Entities
{
    public class LocalPlayer : Player
    {
        private bool fadingScreen = false;

        private EngineExtinguisher extinguisher;

        private IRFlareManager flareMgr;

        public LocalPlayer(ScriptThread thread, PlayerInfo info) : base(thread, info)
        { }

        public override void Create(LevelSpawn spawnPoint)
        {
            base.Create(spawnPoint);

            Vector3 position = Utility.EnsureValidSpawnPos(spawnPoint.Position);

            Model model = new Model(VehicleHash.Lazer);

            if (!model.IsLoaded)
                model.Request(1000);

            var vehicle = World.CreateVehicle(model, position, spawnPoint.Heading);

            vehicle.LodDistance = 2000;
            vehicle.EngineRunning = true;

            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle, true);

            Ped player = Game.Player.Character;

            player.SetIntoVehicle(vehicle, VehicleSeat.Driver);

            Manage(player, vehicle);       
        }

        /// <summary>
        /// Setup additional extensions.
        /// </summary>
        public override void SetupExtensions()
        {
            base.SetupExtensions();

            AddExtension(new SpawnVelocityBooster(BaseThread, this));
            AddExtension(new SpawnLerpingCamera(BaseThread, this));
            AddExtension(extinguisher = new EngineExtinguisher(BaseThread, this));
            AddExtension(flareMgr = new IRFlareManager(BaseThread, this));    
        }

        public override void OnUpdate(int gameTime)
        {
            if (Game.IsControlJustPressed(0, Control.ScriptLB) || Game.IsControlJustPressed(0, (Control)48))
            {
                extinguisher.Start();
            }

            else if (Game.IsControlJustPressed(0, Control.ScriptRB) || Game.IsControlJustPressed(0, (Control)337))
            {
                flareMgr.Start();
            }

            if (Info.Sess.State == PlayerState.Playing && fadingScreen)
            {
                Utility.FadeInScreen(1000);

                fadingScreen = false;
            }

            base.OnUpdate(gameTime);
        }
    }
}