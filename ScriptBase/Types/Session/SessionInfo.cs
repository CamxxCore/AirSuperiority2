using AirSuperiority.ScriptBase.Entities;
using GTA;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Types
{
    public struct SessionPlayer
    {
        public int TeamIdx { get; set; }

        public Player EntityRef { get; set; }

        public SessionPlayer(int teamIdx, Entities.Player entityRef)
        {
            TeamIdx = teamIdx;
            EntityRef = entityRef;
        }

        public void Update()
        {
            if (EntityRef != null)
            {
                EntityRef.OnUpdate(Game.GameTime);
            }
        }
    }

    public struct SessionInfo
    {
        public int LevelIndex { get; set; }

        public SessionPlayer[] Players { get; set; }

        public SessionInfo(int levelIndex, int numPlayers)
        {
            LevelIndex = levelIndex;
            Players = new SessionPlayer[numPlayers];
        }

        public int NumPlayers
        {
            get
            {
                return Players == null ? 0 : Players.Length;
            }
        }

        public SessionPlayer AddPlayer(int playerIndex, int teamIdx, Player entityRef)
        {
            SessionPlayer player = new SessionPlayer(teamIdx, entityRef);
            Players[playerIndex] = player;
            return player;
        }

        public SessionPlayer AddPlayer(int playerIndex, int teamIdx)
        {
            return AddPlayer(playerIndex, teamIdx, null);
        }
    }
}
