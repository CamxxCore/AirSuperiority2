using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Entities;

namespace AirSuperiority.ScriptBase.Extensions
{
    public abstract class PlayerExtensionBase : ScriptExtension, IPlayerExtensionBase
    {
        /// <summary>
        /// Base player associated with this extension.
        /// </summary>
        public Player Player
        {
            get { return player; }

            set
            {
                if (player != null)
                {
                    OnPlayerDetached(player);
                }

                player = value;
                OnPlayerAttached(player);
            }
        }

        public PlayerExtensionBase(ScriptThread thread, Player player) : base(thread)
        {
            Player = player;
        }

        private Player player;

        /// <summary>
        /// Fired when the player is attached to this instance.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void OnPlayerAttached(Player player)
        { }

        /// <summary>
        /// Fired when the player is detached from this instance.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void OnPlayerDetached(Player player)
        { }
    }
}
