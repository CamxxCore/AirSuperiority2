using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirSuperiority.ScriptBase.Types;
using System.Drawing;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Extension for getting a random item from a list
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="items">The list</param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this IEnumerable<T> items)
        {
            if (items.Count() < 1) return default(T);
            var random = new Random(Guid.NewGuid().GetHashCode());
            return (T)(object)items.ToArray()[random.Next(0, items.Count())];
        }

        public static Vector3 ToVector3(this IEnumerable<float> arr)
        {
            return new Vector3(arr.ElementAt(0), arr.ElementAt(1), arr.ElementAt(2));
        }

        /// <summary>
        /// Whether the position is within the provided boundaries.
        /// </summary>
        /// <param name="toggle"></param>
        public static bool IsWithin(this Vector3 v, Vector3 min, Vector3 max)
        {
            bool b = false;
            b ^= (min.X > max.X ? v.X < min.X && v.X > max.X : v.X < max.X && v.X > min.X) ^ 
                (min.Y > max.Y ? v.Y < min.Y && v.Y > max.Y : v.Y < max.Y && v.Y > min.Y) ^
                (min.Z > max.Z ? v.Z < min.Z && v.Z > max.Z : v.Z < max.Z && v.Z > min.Z);
            return b;
        }

        /// <summary>
        /// Extension for converting a team color index to its equivelent system color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToSystemColor(this TeamColor color)
        {
            switch (color)
            {
                case TeamColor.Blue:
                    return Color.Blue;
                case TeamColor.Fuchsia:
                    return Color.Fuchsia;
                case TeamColor.Gold:
                    return Color.Gold;
                case TeamColor.Gray:
                    return Color.Gray;
                case TeamColor.Green:
                    return Color.Green;
                case TeamColor.Orange:
                    return Color.Orange;
                case TeamColor.Purple:
                    return Color.Purple;
                case TeamColor.Red:
                    return Color.Red;
                case TeamColor.SkyBlue:
                    return Color.SkyBlue;
                case TeamColor.Yellow:
                    return Color.Yellow;
                default: return Color.Empty;
            }
        }
    }
}
