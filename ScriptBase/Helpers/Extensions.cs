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
        /// Checks whether the point is within the given level bounds.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static bool BoundsCheck(this Vector3 point, LevelBounds vertices)
        {
            int j = 3;
            bool c = false;
           for (int i = 0; i < 3; j = i++)
                c ^= vertices[i].Y > point.Y ^ vertices[j].Y > point.Y && point.X < (vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) + vertices[i].X;
            return c;
        }

        /// <summary>
        /// Checks whether the point is within the bounds of the given verticies.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static bool BoundsCheck(this Vector3 point, Vector3[] vertices)
        {
            int j = vertices.Length - 1;
            bool c = false;
            for (int i = 0; i < vertices.Length; j = i++)
                c ^= vertices[i].Y > point.Y ^ vertices[j].Y > point.Y && point.X < (vertices[j].X - vertices[i].X) * (point.Y - vertices[i].Y) / (vertices[j].Y - vertices[i].Y) + vertices[i].X;
            return c;
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
