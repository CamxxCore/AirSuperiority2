using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority
{
    /// <summary>
    /// Represents a native hash.
    /// </summary>
    public sealed class CustomNative
    {
        public static CustomNative GET_IS_VEHICLE_ENGINE_RUNNING = new CustomNative(0xAE31E7DF9B5B132E);
        public static CustomNative GET_PED_SOURCE_OF_DEATH = new CustomNative(0x93C8B64DEB84728C);

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="value">The hash value.</param>
        public CustomNative(ulong value)
        {
            Value = value;
        }

        public ulong Value { get; private set; }

        public static implicit operator CustomNative(GTA.Native.Hash value)
        {
            return value;
        }

        public static implicit operator GTA.Native.Hash(CustomNative hash)
        {
            return (GTA.Native.Hash)hash.Value;
        }

        public static implicit operator ulong(CustomNative hash)
        {
            return hash.Value;
        }
    }
}
