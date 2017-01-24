using System;
using System.Collections.Generic;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Types.Metadata;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Entities;
using GTA;
using GTA.Native;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Logic
{
    /// <summary>
    /// Class for managing all core game logic.
    /// </summary>
    public class SessionManager : ScriptExtension
    {
        private bool sessionActive = false;

        private SessionInfo current;

        private TeamInfo[] activeTeams = new TeamInfo[Constants.MaxConcurrentTeams];

        private LevelSpawn[] activeSpawns;

        public bool SessionActive
        {
            get
            {
                return sessionActive;
            }
        }

        /// <summary>
        /// Team- assigned relationship groups, as known to the game.
        /// </summary>
        private List<int> rGroups = Enumerable.Range(0, Constants.MaxConcurrentTeams)
            .Select(i => World.AddRelationshipGroup(string.Format("team{0}", i))).ToList();

        /// <summary>
        /// Get active team info by its index.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public TeamInfo GetTeamByIndex(int team)
        {
            return activeTeams[team];
        }

        /// <summary>
        /// Randomize active team data.
        /// </summary>
        private void GetNewTeams()
        {
            var teamData = Resources.GetByName<TeamAssetMetadata>("TeamInfo");

            var frontend = BaseThread.GetExtension("ui") as DisplayManager;

            TeamInfo[] prev = activeTeams;

            Array.Clear(activeTeams, 0, activeTeams.Length);

            for (int i = 0; i < Constants.MaxConcurrentTeams; i++)
            {
                var foundTeam = teamData.Where(x => !prev.Any(z => z?.FriendlyName == x.FriendlyName)).GetRandomItem();

                var team = new TeamInfo(i, foundTeam.FriendlyName, (TeamColor)i, GetTeamSpawn(i), new TeamStatus());

                frontend.SetTeamSlotFromMetadata(i, foundTeam);

                activeTeams[i] = team;
            }
        }

        /// <summary>
        /// Find the team with the least members.
        /// </summary>
        public TeamInfo FindFreeTeam()
        {
            int[] teamTotals = new int[activeTeams.Length];

            for (int i = 0; i < current.NumPlayers; i++)
            {
                teamTotals[current.Players[i].TeamIdx]++;
            }

            return activeTeams[Array.IndexOf(teamTotals, teamTotals.Min())];
        }

        /// <summary>
        /// Intitialize a new session with the given level and number of players.
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <param name="numPlayers"></param>
        public void Initialize(int levelIndex, int numPlayers)
        {
            SetupSpawnsForMapIndex(levelIndex);

            GetNewTeams();

            current = new SessionInfo();

            current.LevelIndex = levelIndex;

            current.NumPlayers = numPlayers;

            current.Players = new SessionPlayer[numPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                var team = FindFreeTeam();

                current.AddPlayer(i, team.Index);
            }

            var mgr = BaseThread.GetExtension("map") as LevelManager;

            mgr.DoLoadLevel(current.LevelIndex);

            sessionActive = true;
        }

        /// <summary>
        /// Grab level spawns for the given map index.
        /// </summary>
        /// <param name="mapIndex"></param>
        private void SetupSpawnsForMapIndex(int mapIndex)
        {
            var spawnMetadata = Resources.GetByName<SpawnPointAssetMetadata>("SpawnPoint");
            
            spawnMetadata = spawnMetadata.Where(x => x.MapIndex == mapIndex);

            if (spawnMetadata.Count() < Constants.MaxConcurrentTeams)
            {
                throw new InvalidOperationException("SetupMapSpawns - Not enough spawn points defined for map index '" + mapIndex + "'!");
            }

            activeSpawns = spawnMetadata.Select(x => new LevelSpawn(x.Position, x.Heading)).ToArray();
        }

        /// <summary>
        /// Find the level spawn point for the given team index.
        /// </summary>
        /// <param name="teamIndex"></param>
        /// <returns></returns>
        private LevelSpawn GetTeamSpawn(int teamIndex)
        {
            if (teamIndex < 0 || teamIndex > activeSpawns.Length)
                throw new ArgumentOutOfRangeException("GetTeamSpawn - teamIdx out of range");
            return activeSpawns[teamIndex];
        }

        /// <summary>
        /// Create a new participant for the script.
        /// </summary>
        /// <param name="teamIndex"></param>
        /// <param name="isLocal"></param>
        /// <returns></returns>
        private PlayerParticipant CreateParticipant(int teamIndex, bool isLocal)
        {
            // todo: cleanup and implement this function elsewhere.

            LevelSpawn spawnPoint = activeTeams[teamIndex].SpawnPoint;

            Vector3 position = Utility.EnsureValidSpawnPos(spawnPoint.Position);

            Model model = new Model(isLocal ? VehicleHash.Lazer : new VehicleHash[] {
                VehicleHash.Lazer,
                VehicleHash.Besra,
                VehicleHash.Hydra }.GetRandomItem());

            if (!model.IsLoaded)
                model.Request(1000);

            var vehicle = new ManagedFighter(World.CreateVehicle(model, position, spawnPoint.Heading));

            vehicle.Ref.LodDistance = 2000;
            vehicle.Ref.EngineRunning = true;
            vehicle.Ref.BodyHealth = 0.01f;
            vehicle.Ref.MaxHealth = 1;

            vehicle.LandingGearState = LandingGearState.Retracted;

            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle.Ref, true);

            ManagedPed ped;

            if (!isLocal)
            {
                model = new Model(PedHash.Pilot02SMM);

                if (!model.IsLoaded)
                    model.Request(1000);

                ped = new ManagedPed(World.CreatePed(model, position));

                ped.Ref.RelationshipGroup = rGroups[teamIndex];

                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Ref, 1, 1);

                Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Ref, 52, 0);

                Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped.Ref, false);
            }

            else
            {
                ped = new ManagedPed(Game.Player.Character);

                ped.Ref.RelationshipGroup = rGroups[teamIndex];        
            }

            ped.Ref.SetIntoVehicle(vehicle.Ref, VehicleSeat.Driver);

            return new PlayerParticipant(ped, vehicle);
        }

        public override void OnUpdate()
        {
            if (sessionActive)
            {
                for (int i = 0; i < current.NumPlayers; i++)
                {
                    if (current.Players[i].EntityRef == null)
                    {
                        bool bIsLocal = (i < 1);

                        var participant = CreateParticipant(current.Players[i].TeamIdx, bIsLocal);

                        if (bIsLocal)
                        {
                            participant.Vehicle.EnterWater += LocalPedEnterWater;

                            participant.Ped.ExitVehicle += LocalPedExitVehicle;
                        }

                        current.Players[i].EntityRef = participant;
                    }

                    current.Players[i].EntityRef.OnUpdate();
                }
            }

            base.OnUpdate();
        }

        private void LocalPedExitVehicle(IScriptEntity sender, ScriptEntityEventArgs args)
        {
            // todo: cleanup and implement this function elsewhere.

            UI.ShowSubtitle("exit");

            current.Players[0].EntityRef.Remove();

            var participant = CreateParticipant(current.Players[0].TeamIdx, true);

            participant.Vehicle.EnterWater += LocalPedEnterWater;

            participant.Ped.ExitVehicle += LocalPedExitVehicle;

            current.Players[0].EntityRef = participant;
        }

        private void LocalPedEnterWater(IScriptEntity sender, ScriptEntityEventArgs args)
        {
            // todo: cleanup and implement this function elsewhere.

            UI.ShowSubtitle("water");

            current.Players[0].EntityRef.Remove();

            var participant = CreateParticipant(current.Players[0].TeamIdx, true);

            participant.Vehicle.EnterWater += LocalPedEnterWater;

            participant.Ped.ExitVehicle += LocalPedExitVehicle;

            current.Players[0].EntityRef = participant;
        }

        public override void Dispose()
        {
            rGroups.ForEach(x => World.RemoveRelationshipGroup(x));

            base.Dispose();
        }
    }
}
