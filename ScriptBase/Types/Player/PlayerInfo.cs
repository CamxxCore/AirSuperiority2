
namespace AirSuperiority.ScriptBase.Types
{


    

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
        public int SpawnedTime;

        /// <summary>
        /// Live updated player stats.
        /// </summary>
        public PlayerStats Stats;
    }

    public struct PlayerInfo
    {
        /// <summary>
        /// The players name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The players rank.
        /// </summary>
        public int Rank;

        /// <summary>
        /// Live session info.
        /// </summary>
        public LiveSessionInfo Sess;
    }
}
