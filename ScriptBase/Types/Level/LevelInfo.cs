using GTA.Math;

namespace AirSuperiority.ScriptBase.Types
{
    /// <summary>
    /// Base class to hold static level information.
    /// </summary>
    public class LevelInfo
    {
        public LevelInfo()
        { }

        public LevelInfo(string name, int levelIndex, LevelBounds bounds, LevelBounds aiBounds, LevelSpawn[] spawns, Vector3 mapCenter, string[] placements)
        {
            Name = name;
            LevelIndex = levelIndex;
            Bounds = bounds;
            AIBounds = aiBounds;
            SpawnPoints = spawns;
            MapCenter = mapCenter;
            Placements = placements;
        }

        public string Name { get; set; }

        public int LevelIndex { get; set; }

        public LevelBounds Bounds { get; set; }

        public LevelBounds AIBounds { get; set; }

        public LevelSpawn[] SpawnPoints { get; set; }

        public Vector3 MapCenter { get; set; }

        public string[] Placements { get; set; }
    }
}
