using System;
using System.Linq;
using AirSuperiority.Core.IO;
using AirSuperiority.ScriptBase.Helpers;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Types.Metadata
{
    public class SpawnPointAssetMetadata : XMLSimpleMetadata
    {
        public int MapIndex { get; private set; }
        public Vector3 Position { get; private set; }
        public float Heading { get; private set; }

        public override XMLSimpleMetadata ParseAttributes(XMLAttributesCollection c)
        {
            MapIndex = Convert.ToInt32(c["mapIdx"]);
            Position = c["position"].Split(',').Select(x => Convert.ToSingle(x)).ToVector3();
            Heading = Convert.ToSingle(c["h"]);
            return base.ParseAttributes(c);
        }
    }
}
