using System;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Types.Metadata;
using AirSuperiority.ScriptBase.Helpers;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Logic
{
    public class LevelManager : ScriptExtension
    {
        /// <summary>
        /// Multiplier for the distance from map boundary to center
        /// inside which AI players will choose to fly.
        /// Lower values make for a larger area.
        /// </summary>
        private const float AIBoundsMultiplier = 0.69f;

        /// <summary>
        /// Multiplier that affects how close the spawn points are relative to the center of the map.
        /// Lower values make for a larger area.
        /// </summary>
        private const float SpawnDist = 0.69f;

        /// <summary>
        /// Current level.
        /// </summary>
        public LevelInfo Level { get { return level; } }

        private LevelInfo level;

        private SessionManager sessionMgr;

        public LevelManager(ScriptThread thread) : base(thread)
        {
            sessionMgr = thread.Get<SessionManager>();
        }

        private void SetupSpawnDist(ref LevelSpawn[] spawns, Vector3 center)
        {
            for (int i = 0; i < spawns.Length; i++)
            {
                spawns[i].Position = Vector3.Lerp(spawns[i].Position, center, SpawnDist);
            }
        }

        /// <summary>
        /// Load level information for the given level index.
        /// </summary>
        /// <param name="levelIdx">The metadata- defined index of the level.</param>
        public void DoLoadLevel(int levelIndex)
        {
            UnloadCurrentLevel();

            var metadata = Resources.GetMetaEntry<MapAreaAssetMetadata>("MapArea");

            MapAreaAssetMetadata map = metadata.FirstOrDefault(m => m.LevelIndex == levelIndex);

            if (map != null)
            {
                var boundA = Vector3.Lerp(map.BoundsMin, map.MapCenter, SpawnDist);

                var boundB = Vector3.Lerp(map.BoundsMax, map.MapCenter, SpawnDist);

                var innerBoundA = Vector3.Lerp(map.BoundsMin, map.BoundsMax, AIBoundsMultiplier);

                var innerBoundB = Vector3.Lerp(map.BoundsMin, map.BoundsMax, 1.0f - AIBoundsMultiplier);

                var spawnPoints = GetSpawnsForLevel(map.LevelIndex);

                SetupSpawnDist(ref spawnPoints, map.MapCenter);

                level = new LevelInfo(map.FriendlyName,
                    map.LevelIndex,
                    new LevelBounds(map.BoundsMin, map.BoundsMax),
                    new LevelBounds(innerBoundA, innerBoundB),
                    spawnPoints,
                    map.MapCenter,
                    map.ItemPlacements);

                OnLoadLevel();
            }

            else
            {
                throw new ArgumentOutOfRangeException("levelIndex", "No level with index '" + levelIndex + "'");
            }
        }

        /// <summary>
        /// Find the level spawn point for the given team index.
        /// </summary>
        /// <param name="teamIndex"></param>
        /// <returns></returns>
        public LevelSpawn GetSpawnPoint(int spawnIdx)
        {
            if (spawnIdx < 0 || spawnIdx > level.SpawnPoints.Length)
                throw new ArgumentOutOfRangeException("LevelManager.GetSpawnPoint() - spawnIdx out of range");
            return level.SpawnPoints[spawnIdx];
        }

        /// <summary>
        /// Setup level spawns for the given map index.
        /// </summary>
        /// <param name="mapIndex"></param>
        protected LevelSpawn[] GetSpawnsForLevel(int levelIndex)
        {
            var metadata = Resources.GetMetaEntry<SpawnPointAssetMetadata>("SpawnPoint").ToList();

            return metadata
                .Where(m => m.MapIndex == levelIndex)
                .Select(m => new LevelSpawn(m.Position, m.Heading))
                .ToArray();
        }

        /// <summary>
        /// Called after a new level has been initialized to load any additional assets.
        /// </summary>
        public virtual void OnLoadLevel()
        {
            Utility.LoadItemPlacements(level.Placements);
        }

        /// <summary>
        /// Called before a level has been unloaded to unload any additional assets.
        /// </summary>
        public virtual void OnUnloadLevel()
        {
            Utility.RemoveItemPlacements(level.Placements);
        }

        /// <summary>
        /// Unload the current level, if any.
        /// </summary>
        public void UnloadCurrentLevel()
        {
            if (level == null) return;

            OnUnloadLevel();

            level = null;
        }

        public override void Dispose()
        {
            UnloadCurrentLevel();

            base.Dispose();
        }
    }
}
