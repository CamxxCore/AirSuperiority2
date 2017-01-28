
namespace AirSuperiority.Core
{
    /// <summary>
    /// Base interface for a script entity.
    /// </summary>
    public interface IScriptEntity
    {
        event ScriptEntityEventHandler Alive;
        event ScriptEntityEventHandler Dead;
        event ScriptEntityEventHandler EnterWater;
        int TotalTicks { get; }
        int InWaterTicks { get; }
        int DeadTicks { get; }
    }
}
