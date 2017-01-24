using System;
using GTA;
using GTA.Native;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Represents an airplane.
    /// </summary>
    public class ScriptPlane : ScriptEntity<Vehicle>
    {
        /// <summary>
        /// State of the vehicle landing gear.
        /// </summary>
        public LandingGearState LandingGearState
        {
            get { return (LandingGearState)Function.Call<int>(Hash._GET_VEHICLE_LANDING_GEAR, Ref.Handle); }
            set { Function.Call(Hash._SET_VEHICLE_LANDING_GEAR, Ref.Handle, (int)value); }
        }

        public ScriptPlane(Vehicle baseRef) : base(baseRef)
        { }

        public static ScriptPlane FromVehicle(Vehicle vehicle)
        {
            return new ScriptPlane(vehicle);
        }
    }

    public enum LandingGearState
    {
        Deployed,
        Closing,
        Opening,
        Retracted
    }
}
