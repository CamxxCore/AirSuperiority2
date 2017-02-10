
namespace AirSuperiority.ScriptBase.Types
{
    /// <summary>
    /// Class to hold base team infos (texture asset, relationship group etc.)
    /// </summary>
    public class TeamData
    {
        public int Index { get; private set; }
        public string FriendlyName { get; private set; }
        public int RelationshipGroup { get; private set; }
        public TeamTextureAsset Asset { get; private set; }
        public TeamActiveInfo Current { get; private set; }

        public TeamData(int index, string name, TeamTextureAsset asset, int relationshipGroup)
        {
            Index = index;
            FriendlyName = name;
            RelationshipGroup = relationshipGroup;
            Asset = asset;
            Current = new TeamActiveInfo();
        }
    }
}
