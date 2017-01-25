using System;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Types.Metadata;
using AirSuperiority.ScriptBase.Helpers;
using GTA;

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

            MapAreaAssetMetadata mapData = metadata.FirstOrDefault(m => m.LevelIndex == levelIdx);

            if (mapData != null)
            {
                LevelInfo newLevel = new LevelInfo();

                newLevel.Name = mapData.FriendlyName;

                newLevel.Bounds = new LevelBounds(mapData.BoundsMinA, mapData.BoundsMinB, mapData.BoundsMaxA, mapData.BoundsMaxB);

                newLevel.Placements = mapData.ItemPlacements;

                level = newLevel;

                OnLoadLevel();
            }

            else
            {
                throw new ArgumentOutOfRangeException("levelIdx", "No level with index '" + levelIdx + "'");
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
        /// Unload the current level, if any.
        /// </summary>
        public void UnloadCurrentLevel()
        {
            if (level == null) return;

            Utility.RemoveItemPlacements(level.Placements);

            level = null;
        }

        /// <summary>
        /// Update the class.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void OnUpdate(int gameTime)
        {   
            if (sessionMgr.SessionActive)
            {
                foreach (var player in sessionMgr.Current.Players)
                {
                    var playerPos = player.EntityRef.Position;

                    if (!playerPos.BoundsCheck(level.Bounds))
                    {
                      //  UI.ShowSubtitle("Player out of bounds!");
                    }

                    else UI.ShowSubtitle("all good");
                }

               // tbc...
            }

            base.OnUpdate(gameTime);
        }
    }
}
