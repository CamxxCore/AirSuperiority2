
namespace AirSuperiority.ScriptBase.Types
{
    public struct AIState
    {
        public AIStatus Status { get; set; }
        public int NextDecisionTime { get; set; }
        
        public void SetStatus(AIStatus status)
        {
            Status = status;
        }

        public void SetNextDecisionTime(int time)
        {
            NextDecisionTime = time;
        }
    }
}
