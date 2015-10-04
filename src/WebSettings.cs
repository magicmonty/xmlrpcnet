using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace CookComputing.XmlRpc
{
    public class WebSettings
    {
        public bool AllowAutoRedirect {
            get { return _allowAutoRedirect; }
            set { _allowAutoRedirect = value; }
        }

        private bool _allowAutoRedirect = true;

        [Browsable(false)]
        public X509CertificateCollection ClientCertificates {
            get { return _clientCertificates; }
        }

        private X509CertificateCollection _clientCertificates
      = new X509CertificateCollection();
        public string ConnectionGroupName {
            get { return _connectionGroupName; }
            set { _connectionGroupName = value; }
        }

        private string _connectionGroupName = null;

        [Browsable(false)]
        public ICredentials Credentials {
            get { return _credentials; }
            set { _credentials = value; }
        }

        private ICredentials _credentials = null;
        public bool EnableCompression {
            get { return _enableCompression; }
            set { _enableCompression = value; }
        }

        private bool _enableCompression = false;

        [Browsable(false)]
        public WebHeaderCollection Headers {
            get { return _headers; }
        }

        private WebHeaderCollection _headers = new WebHeaderCollection();

        public bool Expect100Continue {
            get { return _expect100Continue; }
            set { _expect100Continue = value; }
        }

        private bool _expect100Continue = false;

        public CookieContainer CookieContainer {
            get { return _cookies; }
        }

        private CookieContainer _cookies = new CookieContainer();

        public bool KeepAlive {
            get { return _keepAlive; }
            set { _keepAlive = value; }
        }

        private bool _keepAlive = true;

        public bool PreAuthenticate {
            get { return _preAuthenticate; }
            set { _preAuthenticate = value; }
        }

        private bool _preAuthenticate = false;

        [Browsable(false)]
        public Version ProtocolVersion {
            get { return _protocolVersion; }
            set { _protocolVersion = value; }
        }

        private Version _protocolVersion = HttpVersion.Version11;

        [Browsable(false)]
        public IWebProxy Proxy {
            get { return _proxy; }
            set { _proxy = value; }
        }

        private IWebProxy _proxy = null;

        public int Timeout {
            get { return _timeout; }
            set { _timeout = value; }
        }

        private int _timeout = 100000;

        public string Url {
            get { return _url; }
            set { _url = value; }
        }

        private string _url = null;

        public bool UseNagleAlgorithm {
            get { return _useNagleAlgorithm; }
            set { _useNagleAlgorithm = value; }
        }

        private bool _useNagleAlgorithm = false;

        public string UserAgent {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        private string _userAgent = "XML-RPC.NET";



    }
}
