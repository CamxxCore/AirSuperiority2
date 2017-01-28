using GTA;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Helpers
{
    public class GameSound
    {
        private string soundSet;
        private string sound;
        private int soundID;

        public bool Active { get; private set; }

        public GameSound(string sound, string soundSet)
        {
            this.Active = false;
            this.sound = sound;
            this.soundSet = soundSet;
            this.soundID = -1;
        }

        public static void Load(string audioBank)
        {
            Function.Call(Hash.REQUEST_SCRIPT_AUDIO_BANK, audioBank, false);
        }

        public void Play(Entity ent)
        {
            soundID = Function.Call<int>(Hash.GET_SOUND_ID);
            Function.Call(Hash.PLAY_SOUND_FROM_ENTITY, soundID, sound, ent.Handle, soundSet, 0, 0);
            Active = true;
        }

        public void Stop()
        {
            if (soundID == -1 || !Active) return;
            Function.Call(Hash.STOP_SOUND, soundID);
            Active = false;
        }

        public void Destroy()
        {
            if (soundID == -1 || !Active) return;
            Function.Call(Hash.RELEASE_SOUND_ID, soundID);
            soundID = -1;
            Active = false;
        }
    }
}


