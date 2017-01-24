
namespace AirSuperiority.ScriptBase.Types
{
    /// <summary>
    /// Base class to hold static level information.
    /// </summary>
    public class LevelInfo
    {
        public string Name { get; set; }

        public int LevelIndex { get; set; }

        public LevelBounds Bounds { get; set; }

        public string[] Placements { get; set; }
    }
}
