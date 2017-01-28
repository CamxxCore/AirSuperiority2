
namespace AirSuperiority.ScriptBase.Types
{
    /// <summary>
    /// Information about the player that is likely to change.
    /// </summary>
    public struct LiveSessionInfo
    {
        /// <summary>
        /// Current player state.
        /// </summary>
        public PlayerState State;

        /// <summary>
        /// Players team number.
        /// </summary>
        public int TeamNum;

        /// <summary>
        /// Time at which the player last spawned.
        /// </summary>
        public int LastSpawnedTime;

        /// <summary>
        /// Time at which the player joined the session.
        /// </summary>
        public int JoinedTime;

        /// <summary>
        /// Live updated player stats.
        /// </summary>
        public PlayerStats Stats;
    }

    /// <summary>
    /// Holds static and updatable state information about a player.
    /// </summary>
    public class PlayerInfo
    {
        public PlayerInfo(string name, int teamIndex, int joinedTime)
        {
            Name = name;
            Sess.TeamNum = teamIndex;
            Sess.JoinedTime = joinedTime;
            Sess.State = PlayerState.Inactive; 
        }

        /// <summary>
        /// The players name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Live session info.
        /// </summary>
        public LiveSessionInfo Sess;
    }
}
