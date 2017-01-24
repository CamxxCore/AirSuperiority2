using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using GTA.Math;

namespace AirSuperiority.ScriptBase.Helpers
{
    public static class Utility
    {
        /// <summary>
        /// Fade out screen
        /// </summary>
        /// <param name="wait">The time to sleep while fading.</param>
        /// <param name="duration">The duration of the fade effect.</param>
        public static void FadeOutScreen(int duration)
        {
            Function.Call(Hash.DO_SCREEN_FADE_OUT, duration);
        }

        /// <summary>
        /// Fade in screen
        /// </summary>
        /// <param name="wait">The time to sleep while fading.</param>
        /// <param name="duration">The duration of the fade effect.</param>
        public static void FadeInScreen(int duration)
        {
            Function.Call(Hash.DO_SCREEN_FADE_IN, duration);
        }

        /// <summary>
        /// Load an item placement list by name.
        /// </summary>
        /// <param name="iplName">The IPL name.</param>
        public static void LoadItemPlacement(string iplName)
        {
            Function.Call(Hash.REQUEST_IPL, iplName);

          //  while (!Function.Call<bool>(Hash.IS_IPL_ACTIVE, iplName))
          //      Script.Wait(0);
        }

        /// <summary>
        /// Load multiple item placement lists from a list
        /// </summary>
        /// <param name="iplNames">The IPL names.</param>
        public static void LoadItemPlacements(IEnumerable<string> iplNames)
        {
            foreach (string ipl in iplNames)
            {
                LoadItemPlacement(ipl);
            }
        }

        /// <summary>
        /// Remove an item placement list by name.
        /// </summary>
        /// <param name="iplName">The IPL name.</param>
        public static void RemoveItemPlacement(string iplName)
        {
            if (Function.Call<bool>(Hash.IS_IPL_ACTIVE, iplName))
                Function.Call(Hash.REMOVE_IPL, iplName);
        }


        /// <summary>
        /// Remove multiple item placement lists from a list
        /// </summary>
        /// <param name="iplNames">The IPL names.</param>
        public static void RemoveItemPlacements(string[] iplNames)
        {
            foreach (string ipl in iplNames)
            {
                RemoveItemPlacement(ipl);
            }
        }

        /// <summary>
        /// Get a free location for spawning relative to the provided position.
        /// </summary>
        /// <returns></returns>
        public static Vector3 EnsureValidSpawnPos(Vector3 position)
        {
            for (int i = 0; i < 20; i++)
            {

                var pos = position.Around(20 * i);

                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, pos.X, pos.Y, pos.Z, 5f, 5f, 5f, 0) &&
                    !World.GetAllVehicles().Any(v => v.Position.DistanceTo(pos) < 20f))
                {
                    return pos;
                }
            }

            return position;
        }
    }
}
