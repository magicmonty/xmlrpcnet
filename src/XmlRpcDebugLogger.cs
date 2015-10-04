using System;
using System.Diagnostics;
using System.IO;

namespace CookComputing.XmlRpc
{
    public class XmlRpcDebugLogger : XmlRpcLogger
    {
        protected override void OnRequest(object sender, XmlRpcRequestEventArgs e)
        {
            DumpStream(e.RequestStream);
        }

        protected override void OnResponse(object sender, XmlRpcResponseEventArgs e)
        {
            DumpStream(e.ResponseStream);
        }

        private static void DumpStream(Stream stm)
        {
            using (var trdr = new StreamReader(stm))
            {
                var s = trdr.ReadToEnd();
                Debug.WriteLine(s);
                stm.Position = 0;
            }
        }
    }
}
