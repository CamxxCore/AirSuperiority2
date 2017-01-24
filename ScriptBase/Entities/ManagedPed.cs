using AirSuperiority.Core;
using GTA;

namespace AirSuperiority.ScriptBase.Entities
{
    /// <summary>
    /// Class to represent an instance of a managed ped.
    /// </summary>
    public sealed class ManagedPed : ScriptPed
    {
        public ManagedPed(Ped baseRef) : base(baseRef)
        {
        }
    }
}
