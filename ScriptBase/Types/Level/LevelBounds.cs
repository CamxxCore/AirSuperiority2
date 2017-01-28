using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Types
{
    public struct LevelBounds
    {
        public Vector3 Min, Max;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Min.X;
                    case 1:
                        return Min.Y;
                    case 2:
                        return Min.Z;
                    case 3:
                        return Max.X;
                    case 4:
                        return Max.Y;
                    case 5:
                        return Max.Z;

                    default:
                        throw new ArgumentOutOfRangeException("LevelBounds Item get: Index out of range.");
                }
            }
        }

        public LevelBounds(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
    }
}
