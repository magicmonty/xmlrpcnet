using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading;

namespace CookComputing.XmlRpc.TestHelpers
{
    public static class StateNameService
    {
        private static HttpChannel _channel;

        public static void Start(int port)
        {
            try
            {
                var props = new Hashtable();
                props["name"] = "MyHttpChannel";
                props["port"] = port;

                _channel = new HttpChannel(
                   props,
                   null,
                   new XmlRpcServerFormatterSinkProvider()
                );
                ChannelServices.RegisterChannel(_channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(
                  typeof(StateNameServer),
                  "statename.rem",
                  WellKnownObjectMode.Singleton);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static void Stop()
        {
            ChannelServices.UnregisterChannel(_channel);
            Thread.Sleep(100);
        }
    }
}