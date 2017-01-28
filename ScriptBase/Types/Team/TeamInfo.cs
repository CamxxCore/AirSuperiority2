
namespace AirSuperiority.ScriptBase.Types
{
    public class TeamInfo
    {
        public int Index { get; private set; }
        public string FriendlyName { get; private set; }
        public TeamColor TeamColor { get; private set; }
        public LevelSpawn SpawnPoint { get; private set; }
        public TeamStatus Current { get; private set; }

        public TeamInfo(int index, string name, TeamColor color, LevelSpawn spawnPoint, TeamStatus info)
        {
            Index = index;
            FriendlyName = name;
            TeamColor = color;
            SpawnPoint = spawnPoint;
            Current = info;    
        }
    }
}
