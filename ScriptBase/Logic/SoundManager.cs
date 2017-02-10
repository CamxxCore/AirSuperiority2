using System.IO;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Logic
{
    public class SoundManager : ScriptExtension
    {
        private static string[] soundAssets = new string[] {
            "SM_W1_START_ALL",
            "SM_W2_START",
            "SM_W3_START",
            "SM_W5_START",
            "SM_W6_START",
            "SM_W7_START",
            "SM_W8_START",
            "SM_W9_START",
            "SM_W10_START"
        };

        private int currentIndex;

        public void Step(int soundIndex = 0)
        {
            if (soundIndex > soundAssets.Length) return;
            if (soundIndex > 0)
            {
                TriggerMusicEvent(soundAssets[soundIndex]);
                currentIndex = soundIndex;
            }
            else
            {
                TriggerMusicEvent(soundAssets[currentIndex]);
                currentIndex++;
                currentIndex %= soundAssets.Length;
            }
        }

        public void PlayExternalSound(Stream soundStream)
        {
            if (!Utility.IsForeground()) return;
            SoundPlayerAsync.PlaySound(soundStream);
        }

        private bool TriggerMusicEvent(string soundAsset)
        {
            Function.Call(Hash.PREPARE_MUSIC_EVENT, soundAsset);
            return Function.Call<bool>(Hash.TRIGGER_MUSIC_EVENT, soundAsset);
        }
    }
}
