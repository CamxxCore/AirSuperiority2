using System;
using AirSuperiority.Core.IO;

namespace AirSuperiority.ScriptBase.Types.Metadata
{
    public class SoundAssetMetadata : XMLSimpleMetadata
    {
        public bool Streamed { get; private set; }
        public string WavPath { get; private set; }
        public string Alias { get; private set; }

        public override XMLSimpleMetadata ParseAttributes(XMLAttributesCollection c)
        {
            Streamed = Convert.ToBoolean(c["streamed"]);
            WavPath = c["path"];
            Alias = c["name"];
            return base.ParseAttributes(c);
        }
    }
}
