using System;
using System.Collections.Generic;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Entities;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Types.Metadata;
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
        private SessionInfo current;

        private List<int> rGroups;

        private TeamData[] activeTeams = new TeamData[0];

        private DisplayManager displayMgr;

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

        public SessionManager()
        {
            displayMgr = ScriptThread.GetOrAddExtension<DisplayManager>();

            SetupRelationshipGroups();
        }

        /// <summary>
        /// Setup world relationship groups.
        /// </summary>
        public void SetupRelationshipGroups()
        {
            int maxTeams = ScriptThread.GetVar<int>("scr_maxteams").Value;

            rGroups = Enumerable.Range(0, maxTeams)
            .Select(i => World.AddRelationshipGroup(string.Format("team{0}", i))).ToList();

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

            var ui = ScriptThread.GetOrAddExtension<DisplayManager>();

            var previous = activeTeams;

            Array.Clear(activeTeams, 0, activeTeams.Length);
            
            activeTeams = new TeamData[teamsCount];

            for (int i = 0; i < activeTeams.Length; i++)
            {
                var metaEntry = metadata.Where(m =>
                !previous.Any(z => z?.FriendlyName == m.FriendlyName) &&
                !activeTeams.Any(y => y?.FriendlyName == m.FriendlyName)).GetRandomItem();

                var team = new TeamData(i, metaEntry.FriendlyName, new TeamTextureAsset(metaEntry.ImageAsset, metaEntry.AltImageAsset), rGroups[i]);

                ui.SetupTeamSlot(i, metaEntry.FriendlyName, metaEntry.ImageAsset);

                activeTeams[i] = team;
            }
        }

        /// <summary>
        /// Add score for the specified team.
        /// </summary>
        public void RegisterTeamScore(int teamIndex, int score)
        {
           var newScore = activeTeams[teamIndex].Current.Score += score;
            displayMgr.SetTeamScore(teamIndex, newScore / 100);
        }

        /// <summary>
        /// Intitialize a new session with the given level number of players and teams.
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <param name="numPlayers"></param>
        public void Initialize(int levelIndex, int numPlayers, int numTeams)
        {
            ScriptMain.DebugPrint("Initializing a new session with {0} players and {1} total teams.", numPlayers, numTeams);

            LevelManager levelMgr = ScriptThread.GetOrAddExtension<LevelManager>();

            InitTeamConstantData(numTeams);

            current = new SessionInfo(levelIndex, numPlayers);

            levelMgr.DoLoadLevel(current.LevelIndex);

            for (int i = 0; i < numPlayers; i++)
            {
                TeamData team = FindFreeTeam();

                Player player = CreatePlayer(i, team.Index, i < 1);

                player.OnDead += OnPlayerDead;

                current.AddPlayer(i, team.Index, player);

                //ScriptMain.DebugPrint("Added a new player at slot '{0}' with name \"{1}\" teamIdx: {2}", i, player.Name, team.Index);
            }

            ScriptThread.SetVar("scr_activesession", true);
        }

        private void OnPlayerDead(Player sender, EventArgs e)
        {
            sender.RegisterDeaths(1);

            var scoreAmount = ScriptThread.GetVar<int>("scr_score_per_death");

            RegisterTeamScore(sender.Info.Sess.TeamNum, scoreAmount.Value);

            var attackerEnt = Function.Call<Entity>(CustomNative.GET_PED_SOURCE_OF_DEATH, sender.Ped.Ref);

            if (attackerEnt != null)
            {
                ScriptMain.DebugPrint("OnPlayerDead: Found attacker entity.");

                var inflictor = current.Players
                    .FirstOrDefault(x => x.PlayerRef.Ped == attackerEnt || x.PlayerRef.Vehicle == attackerEnt);

                if (inflictor.PlayerRef != null && sender.Info.Sess.TeamNum != inflictor.TeamIdx)
                {
                    ScriptMain.DebugPrint("OnPlayerDead: Found inflicting player.");

                    inflictor.PlayerRef.RegisterKills(1);

                    scoreAmount = ScriptThread.GetVar<int>("scr_score_per_kill");

                    RegisterTeamScore(inflictor.TeamIdx, scoreAmount.Value);

                    if (inflictor.PlayerRef is LocalPlayer)
                    {
                        displayMgr.ShowKillPopup(sender.Name, sender.Info.Sess.TeamNum);
                    }
                }
            }

            // award assist points to players that inflicted damage..

            for (int x = 0; x < current.NumPlayers; x++)
            {
                if (current.Players[x].TeamIdx == sender.Info.Sess.TeamNum) continue;

                if (sender.Vehicle.Ref.HasBeenDamagedBy(current.Players[x].PlayerRef.Ped) || 
                    sender.Vehicle.Ref.HasBeenDamagedBy(current.Players[x].PlayerRef.Vehicle))
                {
                    current.Players[x].PlayerRef.RegisterKills(1);

                    scoreAmount = ScriptThread.GetVar<int>("scr_score_per_assist");

                    RegisterTeamScore(current.Players[x].TeamIdx, scoreAmount.Value);

                    if (current.Players[x].PlayerRef is LocalPlayer)
                    {
                        displayMgr.ShowKillPopup(sender.Name, sender.Info.Sess.TeamNum);
                    }
                }
            }
        }

        public override void OnUpdate(int gameTime)
        {
            for (int i = 0; i < current.NumPlayers; i++)
            {
                current.Players[i].Update();
            }

            base.OnUpdate(gameTime);
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
                new LocalPlayer().InitializeFrom(new PlayerInfo(Game.Player.Name, team, Game.GameTime)) : 
                new AIPlayer().InitializeFrom(new PlayerInfo(string.Format("aiplayer{0}", playerIndex), team, Game.GameTime));

            player.Create();

            return player;
        }

        public override void Dispose()
        {
            for (int i = 0; i < current.NumPlayers; i++)
            {
                current.Players[i].PlayerRef?.Dispose();
            }

            rGroups.ForEach(x => World.RemoveRelationshipGroup(x));

            base.Dispose();
        }
    }
}
