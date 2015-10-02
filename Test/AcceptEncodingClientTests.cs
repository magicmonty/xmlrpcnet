// --------------------------------------------------------------------------------------------------------------------
// <copyright file="acceptencodingclient.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Net;
using System.Threading;
using NUnit.Framework;

#if !FX1_0

namespace CookComputing.XmlRpc
{
    [TestFixture]
    public class AcceptEncodingClientTests
    {
        private bool _running;

        private HttpListener _lstner;

        private string _encoding;

        [TestFixtureSetUp]
        public void Setup()
        {
            _lstner = new System.Net.HttpListener();
            _lstner.Prefixes.Add("http://127.0.0.1:11002/");
            var thrd = new Thread(Run);
            _running = true;
            _lstner.Start();
            thrd.Start();
        }

        private void Run()
        {
            try
            {
                while (_running)
                {
                    const string Xml = @"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>Alabama</value>
    </param>
  </params>
</methodResponse>";

                    var context = _lstner.GetContext();
                    switch (_encoding)
                    {
                        case "gzip":
                            context.Response.Headers.Add("Content-Encoding", "gzip");
                            break;
                        case "deflate":
                            context.Response.Headers.Add("Content-Encoding", "deflate");
                            break;
                    }

                    context.Response.ContentEncoding = System.Text.Encoding.UTF32;
                    var respStm = context.Response.OutputStream;
                    Stream compStm;
                    switch (_encoding)
                    {
                        case "gzip":
                            compStm = new System.IO.Compression.GZipStream(
                                respStm,
                                System.IO.Compression.CompressionMode.Compress);
                            break;
                        case "deflate":
                            compStm = new System.IO.Compression.DeflateStream(
                                respStm,
                                System.IO.Compression.CompressionMode.Compress);
                            break;
                        default:
                            compStm = null;
                            break;
                    }

                    if (compStm == null)
                        continue;

                    var wrtr = new StreamWriter(compStm);
                    wrtr.Write(Xml);
                    wrtr.Close();
                }
            }
            catch (HttpListenerException)
            {
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _running = false;
            _lstner.Stop();
        }

        [Test]
        public void GZipCall()
        {
            _encoding = "gzip";
            var proxy = XmlRpcProxyGen.Create<IStateName>();
            proxy.Url = "http://127.0.0.1:11002/";
            proxy.EnableCompression = true;
            var name = proxy.GetStateName(1);
        }

        [Test]
        public void DeflateCall()
        {
            _encoding = "deflate";
            var proxy = XmlRpcProxyGen.Create<IStateName>();
            proxy.Url = "http://127.0.0.1:11002/";
            proxy.EnableCompression = true;
            var name = proxy.GetStateName(1);
        }
    }
}

#endif