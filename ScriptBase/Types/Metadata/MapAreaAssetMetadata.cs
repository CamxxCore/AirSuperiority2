using System;
using System.Linq;
using AirSuperiority.Core.IO;
using AirSuperiority.ScriptBase.Helpers;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Types.Metadata
{
    public class MapAreaAssetMetadata : XMLSimpleMetadata
    {
        public string FriendlyName { get; private set; }
        public int LevelIndex { get; private set; }
        public string[] ItemPlacements { get; private set; }
        public Vector3 MapCenter { get; private set; }
        public Vector3 BoundsMin { get; private set; }
        public Vector3 BoundsMax { get; private set; }

        public override XMLSimpleMetadata ParseAttributes(XMLAttributesCollection c)
        {
            FriendlyName = c["name"];
            LevelIndex = Convert.ToInt32(c["mapIdx"]);
            MapCenter = c["mapCenter"].Split(',').Select(x => Convert.ToSingle(x)).ToVector3();
            BoundsMin = c["boundsMin"].Split(',').Select(x => Convert.ToSingle(x)).ToVector3();
            BoundsMax = c["boundsMax"].Split(',').Select(x => Convert.ToSingle(x)).ToVector3();
            ItemPlacements = c["placements"].Split(',');
            return base.ParseAttributes(c);
        }
    }
}
