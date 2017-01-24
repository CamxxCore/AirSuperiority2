using System;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Helpers
{
    public class GlobalScaleform
    {
        private int componentID;

        public GlobalScaleform(int componentID)
        {
            this.componentID = componentID;
        }

        protected void CallFunction(string functionName, params object[] args)
        {
            Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_FROM_HUD_COMPONENT, componentID, functionName);

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    Type type = arg.GetType();

                    if (type == typeof(bool))
                        Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL, (bool)arg);
                    else if (type == typeof(float))
                        Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT, (float)arg);
                    else if (type == typeof(int))
                        Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, (int)arg);
                    else if (type == typeof(string))
                        Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL, (string)arg);
                }
            }

            Function.Call(Hash._POP_SCALEFORM_MOVIE_FUNCTION_VOID);
        }
    }
}
