using GTA.Math;

namespace AirSuperiority.ScriptBase.Types
{
    public struct LevelSpawn
    {
        public LevelSpawn(Vector3 position, float heading)
        {
            Position = position;
            Heading = heading;
        }

        public Vector3 Position;
        public float Heading;
    }
}
