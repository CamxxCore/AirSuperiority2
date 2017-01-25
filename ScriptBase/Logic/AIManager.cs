using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;

namespace AirSuperiority.ScriptBase.Logic
{
    public class AIManager : ScriptExtension
    {
        LevelManager levelMgr;

        SessionManager sessionMgr;

        private AIState[] aiStates;

        public override void OnThreadAttached(ScriptThread thread)
        {
            sessionMgr = thread.GetExtension("sess") as SessionManager;

            levelMgr = thread.GetExtension("map") as LevelManager;

            aiStates = new AIState[sessionMgr.Current.NumPlayers];

            base.OnThreadAttached(thread);
        }

        /// <summary>
        /// Update the class.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void OnUpdate(int gameTime)
        {
            for (int i = 0; i < sessionMgr.Current.NumPlayers; i++)
            {
                SessionPlayer player = sessionMgr.Current.Players[i];

                // tbc...
            }

            base.OnUpdate(gameTime);
        }
    }
}
