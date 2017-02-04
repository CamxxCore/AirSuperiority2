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

        private TeamData[] activeTeams;

        private LevelManager levelMgr;

        public SessionManager(ScriptThread thread) : base(thread)
        {
            activeTeams = new TeamData[0];
            levelMgr = thread.Get<LevelManager>();
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
        public TeamData GetTeamByIndex(int teamIndex)
        {
            if (teamIndex < 0 || teamIndex > activeTeams.Length)
                throw new IndexOutOfRangeException("SessionManager.GetTeamByIndex(): No team with index '" + teamIndex + "'");
            return activeTeams[teamIndex];
        }


        /// <summary>
        /// Find the team with the least members.
        /// </summary>
        public TeamData FindFreeTeam()
        {
            int[] teamTotals = new int[activeTeams.Length];

            for (int i = 0; i < current.NumPlayers; i++)
            {
                teamTotals[current.Players[i].TeamIdx]++;
            }

            return activeTeams[Array.IndexOf(teamTotals, teamTotals.Min())];
        }

        /// <summary>
        /// Get new team data.
        /// </summary>
        protected void InitTeamConstantData(int teamsCount)
        {
            var metadata = Resources.GetMetaEntry<TeamAssetMetadata>("TeamInfo");

            if (metadata.Count() < teamsCount)
            {
                throw new InvalidOperationException("SessionManager.GetNewTeams() - Not enough metadata entries to accomodate " 
                    + teamsCount + " teams! Found " + metadata.Count() + " entries.");
            }

            var ui = BaseThread.Get<DisplayManager>();

            var previous = activeTeams;

            Array.Clear(activeTeams, 0, activeTeams.Length);
            
            activeTeams = new TeamData[teamsCount];

            for (int i = 0; i < activeTeams.Length; i++)
            {
                var foundTeam = metadata.Where(m =>
                !previous.Any(z => z?.FriendlyName == m.FriendlyName) &&
                !activeTeams.Any(y => y?.FriendlyName == m.FriendlyName)).GetRandomItem();

                var team = new TeamData(i, foundTeam.FriendlyName, new ActiveTeamInfo(), rGroups[i]);

                ui.SetTeamSlotFromMetadata(i, foundTeam);

                activeTeams[i] = team;
            }
        }

        /// <summary>
        /// Add score for the specified team.
        /// </summary>
        public void AddTeamScore(int teamIndex, int score)
        {
            activeTeams[teamIndex].Current.Score += score;
        }

        /// <summary>
        /// Intitialize a new session with the given level number of players and teams.
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <param name="numPlayers"></param>
        public void Initialize(int levelIndex, int numPlayers, int numTeams)
        {
            ScriptMain.DebugPrint("Initializing a new session with {0} players and {1} total teams.", numPlayers, numTeams);
    
            levelMgr.DoLoadLevel(current.LevelIndex);

            // Utility.FadeOutScreen(900);

            InitTeamConstantData(numTeams);

            current = new SessionInfo(levelIndex, numPlayers);

            for (int i = 0; i < numPlayers; i++)
            {
                TeamData team = FindFreeTeam();

                Player player = CreatePlayer(i, team.Index, i < 1);

                current.AddPlayer(i, team.Index, player);

                //ScriptMain.DebugPrint("Added a new player at slot '{0}' with name \"{1}\" teamIdx: {2}", i, player.Name, team.Index);
            }

            Utility.FadeInScreen(700);

            sessionActive = true;
        }

        /// <summary>
        /// Instantiate and setup a new player instance.
        /// </summary>
        /// <param name="slotIndex"></param>
        /// <param name="teamIndex"></param>
        /// <param name="bIsLocal"></param>
        /// <returns></returns>
        private Player CreatePlayer(int playerIndex, int team, bool bIsLocal)
        {
            var player = bIsLocal ? 
                new LocalPlayer(BaseThread).InitializeFrom(new PlayerInfo(Game.Player.Name, team, Game.GameTime)) : 
                new AIPlayer(BaseThread).InitializeFrom(new PlayerInfo(string.Format("aiplayer{0}", playerIndex), team, Game.GameTime));
            player.Create();
            return player;
        }

        public override void Dispose()
        {
            rGroups.ForEach(x => World.RemoveRelationshipGroup(x));
            base.Dispose();
        }
    }
}
