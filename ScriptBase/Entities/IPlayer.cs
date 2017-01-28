using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Extensions;
using GTA.Math;
using GTA;

namespace AirSuperiority.ScriptBase.Entities
{
    public interface IPlayer
    {
        PlayerInfo Info { get; }
        ScriptPed Ped { get; }
        ScriptPlane Vehicle { get; }
        List<PlayerExtensionBase> Extensions { get; }
        void Create(LevelSpawn spawnPoint);
    }
}
