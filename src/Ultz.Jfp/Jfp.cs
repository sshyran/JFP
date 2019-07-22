using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ultz.Jfp
{
    public static class Jfp
    {
        public const int Port = 285;
        public const int SecurePort = 343;

        internal static JfpProtocol? GetProtocol(JfpProtocol local, JfpProtocol remote)
        {
            // Client | Remote | Result
            // _______|________|________
            // 0      | 0      | 0
            // 1      | 0      | X
            // 1      | 1      | 1
            // 2      | 0      | 0
            // 2      | 1      | 1
            // 2      | 2      | 1
            switch (local)
            {
                case JfpProtocol.Jfp:
                    switch (remote)
                    {
                        case JfpProtocol.Jfp:
                            return JfpProtocol.Jfp;
                        case JfpProtocol.Ljfp:
                            return null;
                        case JfpProtocol.JfpOrLjfp:
                            return JfpProtocol.Jfp;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(remote), remote, null);
                    }
                case JfpProtocol.Ljfp:
                    switch (remote)
                    {
                        case JfpProtocol.Jfp:
                            return null;
                        case JfpProtocol.Ljfp:
                            return JfpProtocol.Ljfp;
                        case JfpProtocol.JfpOrLjfp:
                            return JfpProtocol.Ljfp;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(remote), remote, null);
                    }
                case JfpProtocol.JfpOrLjfp:
                    switch (remote)
                    {
                        case JfpProtocol.Jfp:
                            return JfpProtocol.Jfp;
                        case JfpProtocol.Ljfp:
                            return JfpProtocol.Ljfp;
                        case JfpProtocol.JfpOrLjfp:
                            return JfpProtocol.Ljfp;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(remote), remote, null);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(local), local, null);
            }
        }

        public static JfpClient Connect(Uri uri)
        {
            switch (uri.Scheme.ToLower())
            {
                case "jfps":
                {
                    IPEndPoint endPoint;
                    if (IPAddress.TryParse(uri.Host, out var ip))
                    {
                        endPoint = new IPEndPoint(ip, uri.IsDefaultPort ? SecurePort : uri.Port);
                    }
                    else
                    {
                        endPoint = Dns.GetHostAddresses(uri.Host)
                                       .Select(x => new IPEndPoint(x, uri.IsDefaultPort ? SecurePort : uri.Port))
                                       .FirstOrDefault(x => CanConnectAsync(x).GetAwaiter().GetResult()) ??
                                   throw new ArgumentException("Couldn't connect to any of the resolved IP addresses",
                                       nameof(uri), new TimeoutException());
                    }

                    var client = new TcpClient();
                    client.Connect(endPoint);
                    var stream = new SslStream(client.GetStream());
                    stream.AuthenticateAsClient(uri.Host);
                    return new JfpClient(stream);
                }
                case "jfp":
                {
                    IPEndPoint endPoint;
                    if (IPAddress.TryParse(uri.Host, out var ip))
                    {
                        endPoint = new IPEndPoint(ip, uri.IsDefaultPort ? Port : uri.Port);
                    }
                    else
                    {
                        endPoint = Dns.GetHostAddresses(uri.Host)
                                       .Select(x => new IPEndPoint(x, uri.IsDefaultPort ? Port : uri.Port))
                                       .FirstOrDefault(x => CanConnectAsync(x).GetAwaiter().GetResult()) ??
                                   throw new ArgumentException("Couldn't connect to any of the resolved IP addresses",
                                       nameof(uri), new TimeoutException());
                    }

                    var client = new TcpClient();

                    client.Connect(endPoint);
                    return new JfpClient(client.GetStream());
                }
                default:
                    throw new ArgumentException("The scheme on the given Uri in invalid for JFP(S)", nameof(uri));
            }
        }

        public static JfpClient Connect(string url)
        {
            return Connect(new Uri(url));
        }

        public static JfpClient Connect(IPEndPoint endPoint, bool secure = false)
        {
            return Connect((secure ? "jfps" : "jfp") + "://" + (endPoint.AddressFamily == AddressFamily.InterNetworkV6
                               ? "[" + endPoint.Address + "]"
                               : endPoint.ToString()));
        }

        private static async Task<bool> CanConnectAsync(IPEndPoint ipEndPoint)
        {
            try
            {
                if (ipEndPoint == null) return false;

                using (var tcpClient = new TcpClient())
                {
                    var connectTask = tcpClient.ConnectAsync(ipEndPoint.Address, ipEndPoint.Port);
                    var timeoutTask = Task.Delay(10000);
                    var finishedTask = await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false);

                    bool isAlive;
                    if (finishedTask == timeoutTask)
                    {
                        isAlive = false;
                    }
                    else
                    {
                        try
                        {
                            await connectTask.ConfigureAwait(false);
                            isAlive = true;
                        }
                        catch
                        {
                            isAlive = false;
                        }
                    }

                    return isAlive;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}