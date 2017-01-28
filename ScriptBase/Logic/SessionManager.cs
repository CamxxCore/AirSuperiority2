using System;
using System.Collections.Generic;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Types.Metadata;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Entities;
using GTA;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Logic
{
    /// <summary>
    /// Class for managing all core game logic.
    /// </summary>
    public class SessionManager : ScriptExtension
    {
        private bool sessionActive = false;

        private SessionInfo current;

        private LevelSpawn[] activeSpawns;

        private TeamInfo[] activeTeams = new TeamInfo[0];

        public SessionManager(ScriptThread thread) : base(thread)
        {
            SetupRelationshipGroups();
        }

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
        /// Setup world relationship groups.
        /// </summary>
        public void SetupRelationshipGroups()
        {
            foreach (int group in rGroups)
            {
                foreach (int enemyGroup in rGroups.Where(x => x != group))
                {
                    World.SetRelationshipBetweenGroups(Relationship.Hate, group, enemyGroup);
                }
            }
        }

        /// <summary>
        /// Get active team info by its index.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public TeamInfo GetTeamByIndex(int teamIndex)
        {
            if (teamIndex < 0 || teamIndex > activeTeams.Length)
                throw new IndexOutOfRangeException("SessionManager.GetTeamByIndex(): No team with index '" + teamIndex + "'");
            return activeTeams[teamIndex];
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
        /// Scrap the current teams and get new ones.
        /// </summary>
        protected void GetNewTeams(int teamsCount)
        {
            var metadata = Resources.GetMetaEntry<TeamAssetMetadata>("TeamInfo");

            if (metadata.Count() < teamsCount)
            {
                throw new InvalidOperationException("SessionManager.GetNewTeams() - Not enough metadata entries to accomodate " 
                    + teamsCount + " teams! Found " + metadata.Count() + " entries.");
            }

            var ui = ScriptMain.GetDisplayManager();

            var previous = activeTeams;


            Array.Clear(activeTeams, 0, activeTeams.Length);
            

            activeTeams = new TeamInfo[teamsCount];

            for (int i = 0; i < activeTeams.Length; i++)
            {
                var foundTeam = metadata.Where(m =>
                !previous.Any(z => z?.FriendlyName == m.FriendlyName) &&
                !activeTeams.Any(y => y?.FriendlyName == m.FriendlyName)).GetRandomItem();

                var team = new TeamInfo(i, foundTeam.FriendlyName, (TeamColor)i, GetTeamSpawn(i), new TeamStatus());

                World.CreateBlip(team.SpawnPoint.Position);

                ui.SetTeamSlotFromMetadata(i, foundTeam);

                activeTeams[i] = team;
            }
        }

        /// <summary>
        /// Intitialize a new session with the given level number of players and teams.
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <param name="numPlayers"></param>
        public void Initialize(int levelIndex, int numPlayers, int numTeams)
        {
            ScriptMain.DebugPrint("SessionManager.Initialize() - Initializing a new session with {0} players and {1} total teams.", numPlayers, numTeams);

            SetupSpawnsForMapIndex(levelIndex, numTeams);

            GetNewTeams(numTeams);

            current = new SessionInfo();

            current.LevelIndex = levelIndex;

            current.Players = new SessionPlayer[numPlayers];
            
            for (int i = 0; i < numPlayers; i++)
            {
                var team = FindFreeTeam();

                current.AddPlayer(i, team.Index);

                Player player = CreatePlayer(i, team.Index, i < 1);

                ScriptMain.DebugPrint("Added a new player at slot '{0}' with name \"{1}\" teamIdx: {2}", i, player.Name, team.Index);
            }

            var mgr = ScriptMain.GetLevelManager();

            mgr.DoLoadLevel(current.LevelIndex);

            sessionActive = true;
        }

        /// <summary>
        /// Grab level spawns for the given map index.
        /// </summary>
        /// <param name="mapIndex"></param>
        protected void SetupSpawnsForMapIndex(int mapIndex, int numTeams)
        {
            var metadata = Resources.GetMetaEntry<SpawnPointAssetMetadata>("SpawnPoint");

            int count = metadata.Count();

            ScriptMain.DebugPrint("Found " + count + " spawn points defined in metadata for map index '" + mapIndex + "'");

            metadata = metadata.Where(m => m.MapIndex == mapIndex);

            if (count < numTeams)
            {
                throw new InvalidOperationException("SetupMapSpawns - Not enough spawn points defined in metadata for map index '" + mapIndex + "'! Found " + count + " entries.");
            }

            activeSpawns = metadata.Select(m => new LevelSpawn(m.Position, m.Heading)).ToArray();
        }

        /// <summary>
        /// Find the level spawn point for the given team index.
        /// </summary>
        /// <param name="teamIndex"></param>
        /// <returns></returns>
        protected LevelSpawn GetTeamSpawn(int teamIndex)
        {
            if (teamIndex < 0 || teamIndex > activeSpawns.Length)
                throw new ArgumentOutOfRangeException("SessionManager.GetTeamSpawn() - teamIndex out of range");
            return activeSpawns[teamIndex];
        }

        /// <summary>
        /// Instantiate a player for the session at the given slot.
        /// </summary>
        /// <param name="slotIndex"></param>
        /// <param name="teamIndex"></param>
        /// <param name="bIsLocal"></param>
        /// <returns></returns>
        private Player CreatePlayer(int slotIndex, int teamIndex, bool bIsLocal)
        {
            Player player;

             PlayerInfo info;

            if (bIsLocal)
            {
                info = new PlayerInfo(Game.Player.Name, teamIndex, Game.GameTime);

                player = new LocalPlayer(BaseThread, info);
            }

            else
            {
                info = new PlayerInfo(string.Format("aiplayer{0}", slotIndex), teamIndex, Game.GameTime);

                player = new AIPlayer(BaseThread, info);
            }

            player.Create(GetTeamByIndex(teamIndex).SpawnPoint);

            player.SetupExtensions();

            player.Ped.Ref.RelationshipGroup = rGroups[teamIndex];

            current.Players[slotIndex].EntityRef = player;

            return player;
        }

        public override void Dispose()
        {
            rGroups.ForEach(x => World.RemoveRelationshipGroup(x));

            base.Dispose();
        }
    }
}
