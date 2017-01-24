using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Extensions
{
    class EngineExtinguisher : ScriptEntityExtension<Vehicle>
    {
        const int TotalEffectTime = 5000;

        private int effectStartTime = 0;

        LoopedParticle extingusherPtx = new LoopedParticle("core", "ent_sht_extinguisher");

        GameSound extinguisherSfx = new GameSound("SPRAY", "CARWASH_SOUNDS");

        private bool wasActive = false;

        public bool IsActive
        {
            get
            {
                return Game.GameTime - effectStartTime < TotalEffectTime;
            }
        }

        public override void OnEntityAttached(ScriptEntity<Vehicle> entity)
        {
            if (!extingusherPtx.IsLoaded)
            {
                extingusherPtx.Load();
            }

            base.OnEntityAttached(entity);
        }

        public void Start()
        {
            if (IsActive) return;

            Bone boneIdx = (Bone)Function.Call<int>((Hash)0xFB71170B7E76ACBA, Entity.Ref.Handle, "afterburner");

            extingusherPtx.Start(Entity.Ref, 4f, new Vector3(0f, 1f, 0), new Vector3(89.5f, 0f, 0), boneIdx);

            extinguisherSfx.Play(Entity.Ref);

            effectStartTime = Game.GameTime;

            wasActive = true;
        }

        public override void OnUpdate()
        {
            if (IsActive)
            {
                Entity.Ref.Repair();
            }

            else if (wasActive)
            {
                extingusherPtx.Remove();

                extinguisherSfx.Stop();

                wasActive = false;
            }

            base.OnUpdate();
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
