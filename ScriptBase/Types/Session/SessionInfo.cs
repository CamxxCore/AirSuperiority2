using AirSuperiority.ScriptBase.Entities;
using GTA;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Types
{
    public struct SessionPlayer
    {
        public int TeamIdx { get; set; }

        public Player PlayerRef { get; set; }

        public SessionPlayer(int teamIdx, Player player)
        {
            TeamIdx = teamIdx;
            PlayerRef = player;
        }

        public void Update()
        {
            if (PlayerRef != null)
            {
                PlayerRef.OnUpdate(Game.GameTime);
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

        public SessionPlayer AddPlayer(int playerIndex, int teamIdx, Player PlayerRef)
        {
            SessionPlayer player = new SessionPlayer(teamIdx, PlayerRef);
            Players[playerIndex] = player;
            return player;
        }

        public SessionPlayer AddPlayer(int playerIndex, int teamIdx)
        {
            return AddPlayer(playerIndex, teamIdx, null);
        }
    }
}
