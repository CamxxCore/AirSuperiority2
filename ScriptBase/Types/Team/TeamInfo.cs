
namespace AirSuperiority.ScriptBase.Types
{
    public class TeamInfo
    {
        public int Index { get; private set; }
        public string FriendlyName { get; private set; }
        public TeamColor TeamColor { get; private set; }
        public TeamStatus Current { get; private set; }
        public int RelationshipGroup { get; private set; }

        public TeamInfo(int index, string name, TeamStatus info, int relationshipGroup)
        {
            Index = index;
            FriendlyName = name;
            TeamColor = (TeamColor)index;
            Current = info;
            RelationshipGroup = relationshipGroup;
        }
    }
}
