using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Math;
using GTA.Native;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// Fighter jet engine extinguisher extension.
    /// </summary>
    public class EngineExtinguisher : PlayerExtensionBase
    {
        const int TotalEffectTime = 5000;

        private int effectStartTime = 0;

        LoopedParticle extingusherPtx = new LoopedParticle("core", "ent_sht_extinguisher");

        GameSound extinguisherSfx = new GameSound("SPRAY", "CARWASH_SOUNDS");

        private bool wasActive = false;

        public EngineExtinguisher(ScriptThread thread, Player player) : base(thread, player)
        { }

        public bool IsActive
        {
            get
            {
                return Game.GameTime - effectStartTime < TotalEffectTime;
            }
        }

        public override void OnPlayerAttached(Player player)
        {
            if (!extingusherPtx.IsLoaded)
            {
                extingusherPtx.Load();
            }

            base.OnPlayerAttached(player);
        }

        public void Start()
        {
            if (IsActive) return;

            Bone boneIdx = (Bone)Function.Call<int>((Hash)0xFB71170B7E76ACBA, Player.Vehicle.Ref.Handle, "afterburner");

            extingusherPtx.Start(Player.Vehicle.Ref, 4f, new Vector3(0f, 1f, 0), new Vector3(89.5f, 0f, 0), boneIdx);

            extinguisherSfx.Play(Player.Vehicle.Ref);

            effectStartTime = Game.GameTime;

            wasActive = true;
        }

        public override void OnUpdate(int gameTime)
        {
            if (IsActive)
            {
                Player.Vehicle.Ref.Repair();
            }

            else if (wasActive)
            {
                extingusherPtx.Remove();

                extinguisherSfx.Stop();

                wasActive = false;
            }

            base.OnUpdate(gameTime);
        }

        public override void Dispose()
        {
            if (extinguisherSfx.Active)
            {
                extinguisherSfx.Stop();

                extinguisherSfx = null;
            }

            if (extingusherPtx.Exists)
            {
                extingusherPtx.Remove();

                extingusherPtx = null;
            }

            base.Dispose();
        }
    }
}
