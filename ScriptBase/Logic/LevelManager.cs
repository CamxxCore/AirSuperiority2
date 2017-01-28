using System;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Types.Metadata;
using AirSuperiority.ScriptBase.Helpers;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Logic
{
    public class LevelManager : ScriptExtension
    {
        /// <summary>
        /// Multiplier for the distance from map boundary to center
        /// inside which AI players will choose to fly.
        /// Lower values make for a larger area.
        /// </summary>
        private const float AIBoundsMultiplier = 0.4f;

        /// <summary>
        /// Current level.
        /// </summary>
        public LevelInfo Level { get { return level; } }

        private LevelInfo level;

        private SessionManager sessionMgr;

        public LevelManager(ScriptThread thread) : base(thread)
        {
            sessionMgr = ScriptMain.GetSessionManager();
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
                var innerBoundA = Vector3.Lerp(map.BoundsMin, map.BoundsMax, AIBoundsMultiplier);
                var innerBoundB = Vector3.Lerp(map.BoundsMin, map.BoundsMax, 1.0f - AIBoundsMultiplier);

                level = new LevelInfo(map.FriendlyName,
                    map.LevelIndex,
                    new LevelBounds(map.BoundsMin, map.BoundsMax),
                    new LevelBounds(innerBoundA, innerBoundB),
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
    }
}
