using AirSuperiority.ScriptBase.Entities;

namespace AirSuperiority.ScriptBase.Types
{
    public struct SessionPlayer
    {
        public int TeamIdx { get; set; }

        public PlayerParticipant EntityRef { get; set; }

        public SessionPlayer(int teamIdx, PlayerParticipant entityRef)
        {
            TeamIdx = teamIdx;
            EntityRef = entityRef;
        }
    }

    public struct SessionInfo
    {
        public int LevelIndex { get; set; }
        public int NumPlayers { get; set; }
        public SessionPlayer[] Players { get; set; }

        public SessionPlayer AddPlayer(int playerIndex, int teamIdx, PlayerParticipant entityRef)
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
