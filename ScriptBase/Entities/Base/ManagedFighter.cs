using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Extensions;
using GTA;

namespace AirSuperiority.ScriptBase.Entities
{
    /// <summary>
    /// Class to represent an instance of a managed fighter plane.
    /// </summary>
    public sealed class ManagedFighter : ScriptPlane
    {
        private const int ExtinguisherRechargeTime = 24000;

        private const int FlaresRechargeTime = 22000;

        private VehicleIRFlareManager flareManager = new VehicleIRFlareManager();

        private VehicleEngineExtinguisher fireExtinguisher = new VehicleEngineExtinguisher();

        private int lastFlaresDroppedTime = 0, lastExtinguisherUseTime = 0;

        public ManagedFighter(Vehicle baseRef) : base(baseRef)
        {
            AddExtension(flareManager);
            AddExtension(fireExtinguisher);           
        }

        public void DoIRFlares()
        {
            if (Game.GameTime - lastFlaresDroppedTime > FlaresRechargeTime)
            {
                flareManager.Start();

                lastFlaresDroppedTime = Game.GameTime;
            }
        }

        public void DoFireExtinguisher()
        {
            if (Game.GameTime - lastExtinguisherUseTime > ExtinguisherRechargeTime)
            {
                fireExtinguisher.Start();

                lastExtinguisherUseTime = Game.GameTime;
            }
        }
    }
}
