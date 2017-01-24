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
        public Vector3 BoundsMinA { get; private set; }
        public Vector3 BoundsMinB { get; private set; }
        public Vector3 BoundsMaxA { get; private set; }
        public Vector3 BoundsMaxB { get; private set; }

        public override XMLSimpleMetadata ParseAttributes(XMLAttributesCollection c)
        {
            FriendlyName = c["name"];
            LevelIndex = Convert.ToInt32(c["mapIdx"]);
            BoundsMinA = c["boundsMinA"].Split(',').Select(x => Convert.ToSingle(x)).ToVector3();
            BoundsMinA = c["boundsMinB"].Split(',').Select(x => Convert.ToSingle(x)).ToVector3();
            BoundsMinA = c["boundsMaxA"].Split(',').Select(x => Convert.ToSingle(x)).ToVector3();
            BoundsMinA = c["boundsMaxB"].Split(',').Select(x => Convert.ToSingle(x)).ToVector3();
            ItemPlacements = c["placements"].Split(',');
            return base.ParseAttributes(c);
        }
    }
}
