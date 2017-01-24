using System;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Types.Metadata;
using AirSuperiority.ScriptBase.Helpers;

namespace AirSuperiority.ScriptBase.Logic
{
    public class LevelManager : ScriptExtension
    {
        /// <summary>
        /// Current level.
        /// </summary>
        public LevelInfo Level { get { return level; } }

        private LevelInfo level;

        private SessionManager sessionMgr;

        public override void OnThreadAttached(ScriptThread thread)
        {
            sessionMgr = BaseThread.GetExtension("sess") as SessionManager;

            base.OnThreadAttached(thread);
        }

        /// <summary>
        /// Load level information for the given level index.
        /// </summary>
        /// <param name="levelIdx">The metadata- defined index of the level.</param>
        public void DoLoadLevel(int levelIdx)
        {
            UnloadCurrentLevel();

            var metadata = Resources.GetByName<MapAreaAssetMetadata>("MapArea");

            MapAreaAssetMetadata mapData = metadata.FirstOrDefault(x => x.LevelIndex == levelIdx);

            if (mapData != null)
            {
                LevelInfo newLevel = new LevelInfo();

                newLevel.Name = mapData.FriendlyName;

                newLevel.Bounds = new LevelBounds(mapData.BoundsMinA, mapData.BoundsMinB, mapData.BoundsMaxA, mapData.BoundsMaxB);

                newLevel.Placements = mapData.ItemPlacements;

                level = newLevel;

                Utility.LoadItemPlacements(newLevel.Placements);    
            }

            else
            {
                throw new ArgumentOutOfRangeException("levelIdx", "No level with index '" + levelIdx + "'");
            }
        }

        public void UnloadCurrentLevel()
        {
            if (level == null) return;

            Utility.RemoveItemPlacements(level.Placements);

            level = null;
        }


        public override void OnUpdate()
        {   
            if (sessionMgr.SessionActive)
            {
               // tbc...
            }

            base.OnUpdate();
        }
    }
}
