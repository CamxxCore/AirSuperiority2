using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirSuperiority.ScriptBase.Types;
using GTA.Math;
using GTA;

namespace AirSuperiority.ScriptBase.Entities
{
    interface IGameParticipant
    {
        void CreateEntity(LevelSpawn spawnPoint);
    }
}
