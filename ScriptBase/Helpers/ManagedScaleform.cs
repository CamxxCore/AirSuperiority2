using System;
using GTA.Native;
using GTA;

namespace AirSuperiority.ScriptBase.Helpers
{
    public class ManagedScaleform
    {
        public Scaleform BaseInstance {  get { return baseInstance; } }

        private Scaleform baseInstance;

        public ManagedScaleform(string name)
        {
            baseInstance = new Scaleform(Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, name));
        }

        public void CallFunction(string functionName, params object[] args)
        {
            baseInstance.CallFunction(functionName, args);
        }

        public void Render()
        {
            baseInstance.Render2D();
        }
    }
}
