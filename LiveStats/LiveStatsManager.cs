using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;

namespace AirSuperiority.LiveStats
{
    public static class RemoteStatsManager
    {
        private const string EndpointURI = "https://camx.me/asupstatsvc/asupservice.svc";

        public static bool Initialized { get; private set; } = false;

        private static WCFClientInstance service;
        public static WCFClientInstance Service { get { return service; } }

        /// <summary>
        /// Establish connection to the stat server endpoint.
        /// </summary>
        public static void Initialize()
        {
            var binding = new WSHttpBinding(SecurityMode.Transport);

            binding.CloseTimeout = TimeSpan.Parse("00:01:00");
            binding.OpenTimeout = TimeSpan.Parse("00:01:00");
            binding.ReceiveTimeout = TimeSpan.Parse("00:15:00");
            binding.SendTimeout = TimeSpan.Parse("00:15:00");
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas.MaxDepth = 32;
            binding.ReaderQuotas.MaxStringContentLength = 8192;
            binding.ReaderQuotas.MaxArrayLength = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;

            service = new WCFClientInstance(binding, new EndpointAddress(EndpointURI));

            if (IsConnected())
            {
                Initialized = true;
            }
        }

        private static bool IsConnected()
        {
            try
            {
                service.TryConnect();
                return true;
            }

            catch (EndpointNotFoundException)
            {
            }

            catch (Exception)
            {
            }

            return false;
        }

        /// <summary>
        /// Close connection to service endpoint.
        /// </summary>
        public static void Close()
        {
            try
            {
                service.Close();
            }

            catch (EndpointNotFoundException)
            {
                service.Abort();
            }
            catch (Exception)
            {
                service.Abort();
            }

            Initialized = false;

            //  GTA.UI.Notify("~r~Air Superiority stat service disconnected.");
        }

        /// <summary>
        /// Update player experience based on the current value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="showUI"></param>
        public static async void UpdatePlayerEXP(int uid, int value, bool showUI)
        {
            if (Initialized)
            {
                try
                {
                    await service.UpdatePlayerStatAsync(uid, "totalExp", value);
                    return;
                }

                catch (EndpointNotFoundException)
                {
                }

                catch (Exception)
                {
                }

                Close();
            }
        }

        public static async void UpdatePlayerRank(int uid, int value)
        {
            if (!Initialized) return;

            try
            {
                await service.UpdatePlayerStatAsync(uid, "currentRank", value);
                return;
            }

            catch (EndpointNotFoundException)
            {
            }

            catch (Exception)
            {
            }

            Close();
        }

        public static async void SetPlayerRank(int uid, int value)
        {
            if (!Initialized) return;

            try
            {
                await service.SetPlayerStatAsync(uid, "currentRank", value);
                return;
            }

            catch (EndpointNotFoundException)
            {
            }

            catch (Exception)
            {
            }

            Close();
        }

        public static async void UpdatePlayerKills(int uid, int value)
        {
            if (!Initialized) return;

            try
            {
                await service.UpdatePlayerStatAsync(uid, "totalKills", value);
                return;
            }

            catch (EndpointNotFoundException)
            {
            }

            catch (Exception)
            {
            }

            Close();
        }

        public static async void UpdatePlayerDeaths(int uid, int value)
        {
            if (!Initialized) return;

            try
            {
                await service.UpdatePlayerStatAsync(uid, "totalDeaths", value);
                return;
            }

            catch (EndpointNotFoundException)
            {
            }

            catch (Exception)
            {
            }

            Close();
        }

        public static UserInfo[] GetUserStatList()
        {
            if (!Initialized) return new UserInfo[0];
            try
            {
                return service.GetUserStatTable()
                    .OrderBy(x => x.TotalExp)
                    .ThenBy(x => x.TotalKills)
                    .Reverse()
                    .ToArray();
            }

            catch
            {
                return null;
            }
        }
    }
}


