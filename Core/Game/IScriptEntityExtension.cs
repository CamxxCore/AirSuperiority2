using GTA;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Base interface for entity extensions.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    public interface IScriptEntityExtension<T> where T : Entity
    {
        ScriptEntity<T> Entity { get; set; }
    }
}
