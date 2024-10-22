using System.Net;
using System.Net.Sockets;

namespace Shared.Utils
{
    public static class InternetUtils
    {
        public static string GetIp
        {
            get
            {
                try
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.ToString();
                        }
                    }

                    return string.Empty;
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }
    }
}