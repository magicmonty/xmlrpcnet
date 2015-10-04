/* 
XML-RPC.NET library
Copyright (c) 2001-2011, Charles Cook <charlescook@cookcomputing.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

namespace CookComputing.XmlRpc
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;

    public class XmlRpcAsyncResult : IAsyncResult
    {
        private readonly object _syncLock = new object();
        private readonly AsyncCallback _userCallback;


        public XmlRpcFormatSettings XmlRpcFormatSettings { get; private set; }

        // IAsyncResult members
        public object AsyncState { 
            get { return _userAsyncState; } 
        }

        private object _userAsyncState;

        public WaitHandle AsyncWaitHandle {
            get
            {
                bool completed = _isCompleted;
                if (_manualResetEvent == null) {
                    lock (_syncLock) {
                        if (_manualResetEvent == null)
                            _manualResetEvent = new ManualResetEvent(completed);
                    }
                }

                if (!completed && _isCompleted)
                    _manualResetEvent.Set();

                return _manualResetEvent;
            }
        }

        private ManualResetEvent _manualResetEvent;

        public bool CompletedSynchronously { 
            get { return _completedSynchronously; } 
            set
            { 
                if (_completedSynchronously)
                    _completedSynchronously = value;
            }
        }

        private bool _completedSynchronously;

        public bool IsCompleted {
            get { return _isCompleted; } 
        }

        private bool _isCompleted;

        public CookieCollection ResponseCookies {
            get { return _responseCookies; }
        }

        internal CookieCollection _responseCookies;

        public WebHeaderCollection ResponseHeaders {
            get { return _responseHeaders; }
        }

        internal WebHeaderCollection _responseHeaders;

        // public members
        public void Abort()
        {
            if (_request != null)
                _request.Abort();
        }

        public Exception Exception {
            get { return _exception; } 
        }

        private Exception _exception;

        public XmlRpcClientProtocol ClientProtocol { 
            get { return _clientProtocol; } 
        }

        private XmlRpcClientProtocol _clientProtocol;

        //internal members
        internal XmlRpcAsyncResult(
            XmlRpcClientProtocol ClientProtocol, 
            XmlRpcRequest XmlRpcReq, 
            XmlRpcFormatSettings xmlRpcFormatSettings,
            WebRequest Request, 
            AsyncCallback UserCallback, 
            object UserAsyncState)
        {
            XmlRpcRequest = XmlRpcReq;
            _clientProtocol = ClientProtocol;
            _request = Request;
            _userAsyncState = UserAsyncState;
            _userCallback = UserCallback;
            _completedSynchronously = true;
            XmlRpcFormatSettings = xmlRpcFormatSettings;
        }

        internal void Complete(
            Exception ex)
        {
            _exception = ex;
            Complete();
        }

        internal void Complete()
        {
            try {
                if (ResponseStream != null) {
                    ResponseStream.Close();
                    ResponseStream = null;
                }
                if (ResponseBufferedStream != null)
                    ResponseBufferedStream.Position = 0;
            }
            catch (Exception ex) {
                if (_exception == null)
                    _exception = ex;
            }
            _isCompleted = true;
            try {
                if (_manualResetEvent != null)
                    _manualResetEvent.Set();
            }
            catch (Exception ex) {
                if (_exception == null)
                    _exception = ex;
            }
            if (_userCallback != null)
                _userCallback(this);
        }

        internal WebResponse WaitForResponse()
        {
            if (!_isCompleted)
                AsyncWaitHandle.WaitOne();
            if (_exception != null)
                throw _exception;
            return Response;
        }

        internal bool EndSendCalled { get; set; }

        internal byte[] Buffer { get; set; }

        internal WebRequest Request {
            get { return _request; }
        }

        private readonly WebRequest _request;

        internal WebResponse Response { get; set; }

        internal Stream ResponseStream { get; set; }

        internal XmlRpcRequest XmlRpcRequest { get; set; }

        internal Stream ResponseBufferedStream { get; set; }

    }
}