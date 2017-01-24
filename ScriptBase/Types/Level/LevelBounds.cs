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
        public Vector3 MinA, MinB, MaxA, MaxB;

        public Vector3 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return MinA;
                    case 1:
                        return MinB;
                    case 2:
                        return MaxA;
                    case 3:
                        return MaxB;
                    default:
                        throw new ArgumentOutOfRangeException("Vector3 Item get: Index out of range.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        MinA = value;
                        return;
                    case 1:
                        MinB = value;
                        return;
                    case 2:
                        MaxA = value;
                        return;
                    case 3:
                        MaxB = value;
                        return;
                    default:
                        throw new ArgumentOutOfRangeException("Vector3 Item set: Index out of range.");
                }
            }
        }

        public LevelBounds(Vector3 minA, Vector3 minB, Vector3 maxA, Vector3 maxB)
        {
            MinA = minA;
            MinB = minB;
            MaxA = maxA;
            MaxB = maxB;
        }
    }
}
