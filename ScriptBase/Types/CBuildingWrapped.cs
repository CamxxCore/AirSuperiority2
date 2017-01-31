using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Types
{
    public class CBuildingWrapped
    {
        public IntPtr Address;
        public IntPtr DrawableAddr;
        public IntPtr ModelInfoAddr;
        public int ModelHash;
        public string AssetName;
        public Vector3 Position;
        public CMatrix Matrix;
    }
}
