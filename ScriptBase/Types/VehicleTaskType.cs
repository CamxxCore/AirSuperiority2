using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.ScriptBase.Types
{
    /// <summary>
    /// Parameter to be used with the native function TASK_PLANE_MISSION
    /// </summary>
    public enum VehicleTaskType
    {
        None = 0,
        Unk = 1,
        CTaskVehicleRam = 2,
        CTaskVehicleBlock = 3,
        CTaskVehicleGoToPlane = 4,
        CTaskVehicleStop = 5,
        CTaskVehicleAttack = 6,
        CTaskVehicleFollow = 7,
        CTaskVehicleFleeAirborne = 8,
        CTaskVehicleCircle = 9,
        CTaskVehicleEscort = 10,
        CTaskVehicleFollowRecording = 15,
        CTaskVehiclePoliceBehaviour = 16,
        CTaskVehicleCrash = 17,
    }
}
