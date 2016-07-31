using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using KafkaNet.Model;
using KafkaNet.Protocol;

namespace KafkaNet
{
    public class DefaultKafkaConnectionFactory : IKafkaConnectionFactory
    {
        public IKafkaConnection Create(KafkaEndpoint endpoint, TimeSpan responseTimeoutMs, IKafkaLog log, TimeSpan? maximumReconnectionTimeout = null)
        {
            return new KafkaConnection(new KafkaTcpSocket(log, endpoint, maximumReconnectionTimeout), responseTimeoutMs, log);
        }

        public KafkaEndpoint Resolve(Uri kafkaAddress, IKafkaLog log)
        {
            var ipAddress = GetFirstAddress(kafkaAddress.Host, log);
            var ipEndpoint = new IPEndPoint(ipAddress, kafkaAddress.Port);

            var kafkaEndpoint = new KafkaEndpoint()
            {
                ServeUri = kafkaAddress,
                Endpoint = ipEndpoint
            };

            return kafkaEndpoint;
        }


        private static IPAddress GetFirstAddress(string hostname, IKafkaLog log)
        {
            try
            {
                //lookup the IP address from the provided host name
#if NETCORE
                var addresses = Dns.GetHostAddressesAsync(hostname).GetAwaiter().GetResult();
#else
                var addresses = Dns.GetHostAddresses(hostname);
#endif

                if (addresses.Length > 0)
                {
#if NETCORE
                    foreach (var address in addresses)
                    {
                        log.DebugFormat("Found address {0} for {1}", address, hostname);
                    }
#else
                    Array.ForEach(addresses, address => log.DebugFormat("Found address {0} for {1}", address, hostname));
#endif

                    var selectedAddress = addresses.FirstOrDefault(item => item.AddressFamily == AddressFamily.InterNetwork) ?? addresses.First();

                    log.DebugFormat("Using address {0} for {1}", selectedAddress, hostname);

                    return selectedAddress;
                }
            }
            catch
            {
                throw new UnresolvedHostnameException("Could not resolve the following hostname: {0}", hostname);
            }

            throw new UnresolvedHostnameException("Could not resolve the following hostname: {0}", hostname);
        }
    }
}
