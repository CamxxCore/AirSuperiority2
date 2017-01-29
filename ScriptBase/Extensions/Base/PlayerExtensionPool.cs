using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.ScriptBase.Extensions
{
    public class PlayerExtensionPool : List<PlayerExtensionBase>
    {
        /// <summary>
        /// Get an extension from the pool by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : PlayerExtensionBase
        {
            for (int i = 0; i < Count; i++)
            {
                var item = this[i] as T;

                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
