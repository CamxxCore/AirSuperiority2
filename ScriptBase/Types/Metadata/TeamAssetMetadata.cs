using AirSuperiority.Core.IO;

namespace AirSuperiority.ScriptBase.Types.Metadata
{
    public class TeamAssetMetadata : XMLSimpleMetadata
    {
        public string FriendlyName { get; private set; }
        public string ImageAsset { get; private set; }
        public string AltImageAsset { get; private set; }

        public override XMLSimpleMetadata ParseAttributes(XMLAttributesCollection c)
        {
            FriendlyName = c["name"];
            ImageAsset = c["imageAsset"];
            AltImageAsset = c["altAsset"];
            return base.ParseAttributes(c);
        }
    }
}
