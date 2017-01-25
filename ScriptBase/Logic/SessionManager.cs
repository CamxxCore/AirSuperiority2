using System;
using System.Collections.Generic;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Types.Metadata;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Entities;
using AirSuperiority.ScriptBase.Extensions;
using GTA;

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

        /// <summary>
        /// Instance of the active session.
        /// </summary>
        public SessionInfo Current
        {
            get
            {
                return current;
            }
        }

        /// <summary>
        /// If the session is currently active.
        /// </summary>
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
        private void GetNewTeams(int teamsCount)
        {
            var metadata = Resources.GetByName<TeamAssetMetadata>("TeamInfo");

            var frontend = BaseThread.GetExtension("ui") as DisplayManager;

            TeamInfo[] prev = activeTeams;

            Array.Clear(activeTeams, 0, activeTeams.Length);

            activeTeams = new TeamInfo[teamsCount];

            for (int i = 0; i < activeTeams.Length; i++)
            {
                var foundTeam = metadata.Where(m => 
                !prev.Any(z => z?.FriendlyName == m.FriendlyName) && 
                !activeTeams.Any(y => y?.FriendlyName == m.FriendlyName)).GetRandomItem();

                var team = new TeamInfo(i, foundTeam.FriendlyName, (TeamColor)i, GetTeamSpawn(i), new TeamStatus());

               // World.CreateBlip(team.SpawnPoint.Position);

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
        /// Intitialize a new session with the given level number of players and teams.
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <param name="numPlayers"></param>
        public void Initialize(int levelIndex, int numPlayers, int numTeams)
        {
            SetupSpawnsForMapIndex(levelIndex, numTeams);

            GetNewTeams(numTeams);

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
        private void SetupSpawnsForMapIndex(int mapIndex, int numTeams)
        {
            var metadata = Resources.GetByName<SpawnPointAssetMetadata>("SpawnPoint");

            metadata = metadata.Where(m => m.MapIndex == mapIndex);

            if (metadata.Count() < numTeams)
            {
                throw new InvalidOperationException("SetupMapSpawns - Not enough spawn points defined in metadata for map index '" + mapIndex + "'! Found " + metadata.Count() + " entries.");
            }

            activeSpawns = metadata.Select(m => new LevelSpawn(m.Position, m.Heading)).ToArray();
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
        /// Instantiate a participant for the session at the given slot.
        /// </summary>
        /// <param name="slotIndex"></param>
        /// <param name="teamIndex"></param>
        /// <param name="bIsLocal"></param>
        /// <returns></returns>
        private GameParticipant CreateParticipant(int slotIndex, int teamIndex, bool bIsLocal)
        {
            GameParticipant participant;

            if (bIsLocal)
            {
                participant = new LocalParticipant(Game.Player.Name, teamIndex);
                participant.CreateEntity(activeTeams[teamIndex].SpawnPoint);
                participant.Vehicle.AddExtension(new VehicleLandingGearManager());
                participant.Vehicle.AddExtension(new VehicleSpawnVelocityBooster());
                participant.Vehicle.AddExtension(new VehicleSpawnLerpingCamera());
            }

            else
            {
                participant = new GameParticipant(string.Format("aiplayer{0}", slotIndex), teamIndex);
                participant.CreateEntity(activeTeams[teamIndex].SpawnPoint);
                participant.Vehicle.AddExtension(new VehicleLandingGearManager());
            }

            participant.OnDead += (s, e) => s.CreateEntity(activeTeams[s.Info.Sess.TeamNum].SpawnPoint);

            current.Players[slotIndex].EntityRef = participant;

            return participant;
        }

        /// <summary>
        /// Update the class.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void OnUpdate(int gameTime)
        {
            if (sessionActive)
            {
                for (int i = 0; i < current.NumPlayers; i++)
                {         
                    if (ReferenceEquals(current.Players[i].EntityRef, null))
                    {
                        CreateParticipant(i, current.Players[i].TeamIdx, i < 1);
                    }

                    current.Players[i].Update();
                }
            }

            base.OnUpdate(gameTime);
        }

        public override void Dispose()
        {
            //  remove relationship groups

            rGroups.ForEach(x => World.RemoveRelationshipGroup(x));

            base.Dispose();
        }
    }
}
