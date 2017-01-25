using AirSuperiority.Core;
using GTA;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// An extension that retracts the planes landing gear on spawning.
    /// </summary>
    public class VehicleLandingGearManager : ScriptEntityExtension<Vehicle>
    {
        public override void OnEntityAttached(ScriptEntity<Vehicle> entity)
        {
            entity.Alive += Entity_Alive;

            base.OnEntityAttached(entity);
        }

        private void Entity_Alive(IScriptEntity sender, ScriptEntityEventArgs args)
        {
            Function.Call(Hash._SET_VEHICLE_LANDING_GEAR, Entity.Ref.Handle, (int)LandingGearState.Closing);
        }
    }
}
