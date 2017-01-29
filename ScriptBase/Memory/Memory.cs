using System;
using System.Runtime.InteropServices;

namespace AirSuperiority.ScriptBase.Memory
{
    public static class MemoryAccess
    {
        private static IntPtr flyingMusicFnAddr;

        private static byte[] bOriginal = new byte[5];

        private static bool flyingMusicPatched = false;

        static MemoryAccess()
        {
            flyingMusicFnAddr = new Pattern("\x73\x0A\xC7\x83\x00\x00\x00\x00\x00\x00\x00\x00\x48\x8B\xCB", "xxxx????????xxx").Get(-0x24);
        }

        public static void PatchFlyingMusic()
        {
            Marshal.Copy(flyingMusicFnAddr, bOriginal, 0, 5);

            Marshal.Copy(new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }, 0, flyingMusicFnAddr, 5);

            flyingMusicPatched = true;
        }

        public static void OnExit()
        {
            if (flyingMusicPatched)
            {
                Marshal.Copy(bOriginal, 0, flyingMusicFnAddr, 5);
            }
        }
    }
}
