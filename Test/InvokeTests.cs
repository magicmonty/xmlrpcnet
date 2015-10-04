using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using CookComputing.XmlRpc.TestHelpers;
using Shouldly;

namespace CookComputing.XmlRpc
{
    public struct TestStruct
    {
        public int x;
        public int y;
    }

    [XmlRpcUrl("http://localhost/test/")]
    class Foo : XmlRpcClientProtocol
    {
        [XmlRpcMethod]
        public int Send_Param(object[] toSend)
        {
            return (int)Invoke("Send_Param", toSend);
        }

        [XmlRpcMethod]
        public int SendTwoParams(int param1, int param2)
        {
            return (int)Invoke("SendTwoParams", new object[] { param1 });
        }

        [XmlRpcMethod]
        public string Send(string str)
        {
            return (string)Invoke("Send", new object[] { str });
        }

        [XmlRpcMethod]
        public string Send(TestStruct strct)
        {
            return (string)Invoke("Send", new object[] { strct });
        }
    }

    [XmlRpcUrl("http://localhost:8005/statename.rem")]
    class StateName : XmlRpcClientProtocol
    {
        [XmlRpcMethod("examples.getStateName")]
        public string GetStateNameUsingMethodName(int stateNumber)
        {
            return (string)Invoke("GetStateNameUsingMethodName",
              new object[] { stateNumber });
        }

        [XmlRpcMethod("examples.getStateNameFromString")]
        public string GetStateNameUsingMethodName(string stateNumber)
        {
            return (string)Invoke("GetStateNameUsingMethodName",
              new object[] { stateNumber });
        }

        [XmlRpcMethod("examples.getStateName")]
        public string GetStateNameUsingMethodInfo(int stateNumber)
        {
            return (string)Invoke(MethodBase.GetCurrentMethod(),
              new object[] { stateNumber });
        }

        [XmlRpcMethod("examples.getStateNameFromString")]
        public string GetStateNameUsingMethodInfo(string stateNumber)
        {
            return (string)Invoke(MethodBase.GetCurrentMethod(),
              new object[] { stateNumber });
        }

        [XmlRpcMethod("examples.getStateName")]
        public IAsyncResult BeginGetStateName(int stateNumber, AsyncCallback callback,
          object asyncState)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(),
              new object[] { stateNumber }, callback, asyncState);
        }

        [XmlRpcMethod("examples.getStateName")]
        public IAsyncResult BeginGetStateName(int stateNumber)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(),
              new object[] { stateNumber }, null, null);
        }

        public string EndGetStateName(IAsyncResult asr)
        {
            return (string)EndInvoke(asr);
        }

        [XmlRpcMethod("examples.getStateNameStruct")]
        public IAsyncResult BeginGetStateNameStruct(
            StateStructRequest request,
            AsyncCallback callback, 
            object asyncState)
        {
            return BeginInvoke(
                MethodBase.GetCurrentMethod(),
                new object[] { request }, 
                null, 
                null);
        }

        public StateStructResponse EndGetStateNameStruct(IAsyncResult asr)
        {
            return EndInvoke<StateStructResponse>(asr);
        }
    }

    [TestFixture]
    public class InvokeTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            StateNameService.Start(8005);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            StateNameService.Stop();
        }

        [Test]
        public void MakeSynchronousCalls()
        {
            var proxy = new StateName();
            proxy.GetStateNameUsingMethodName(1).ShouldBe("Alabama");
            proxy.GetStateNameUsingMethodInfo(1).ShouldBe("Alabama");
            proxy.GetStateNameUsingMethodName("1").ShouldBe("Alabama");
            proxy.GetStateNameUsingMethodInfo("1").ShouldBe("Alabama");
        }

        [Test]
        public void MakeSystemListMethodsCall()
        {
            var proxy = new StateName();
            proxy
                .SystemListMethods()
                .ShouldBe(new [] 
                {
                    "examples.getStateName",
                    "examples.getStateNameFromString",
                    "examples.getStateNameStruct",
                    "examples.getStateStruct"
                });
        }

        class CBInfo
        {
            public ManualResetEvent _evt;
            public Exception _excep;
            public string _ret;
            public CBInfo(ManualResetEvent evt)
            {
                _evt = evt;
            }
        }

        private static void StateNameCallback(IAsyncResult asr)
        {
            var clientResult = (XmlRpcAsyncResult)asr;
            var proxy = (StateName)clientResult.ClientProtocol;
            var info = (CBInfo)asr.AsyncState;
            try
            {
                info._ret = proxy.EndGetStateName(asr);
            }
            catch (Exception ex)
            {
                info._excep = ex;
            }

            info._evt.Set();
        }

        [Test]
        public void MakeAsynchronousCallIsCompleted()
        {
            var proxy = new StateName();
            var asr1 = proxy.BeginGetStateName(1);
            while (!asr1.IsCompleted)
                Thread.Sleep(10);

            proxy.EndGetStateName(asr1).ShouldBe("Alabama");
        }

        [Test]
        public void MakeAsynchronousCallWait()
        {
            var proxy = new StateName();
            var asr2 = proxy.BeginGetStateName(1);
            asr2.AsyncWaitHandle.WaitOne();

            proxy.EndGetStateName(asr2).ShouldBe("Alabama");
        }

        [Test]
        public void MakeAsynchronousCallCallBack()
        {
            var proxy = new StateName();
            var evt = new ManualResetEvent(false);
            var info = new CBInfo(evt);
            proxy.BeginGetStateName(1, StateNameCallback, info);

            evt.WaitOne();
            info._excep.ShouldBeNull();
            info._ret.ShouldBe("Alabama");
        }

        [Test]
        public void AsynchronousCallBackReturningStruct()
        {
            var proxy = new StateName();
            var asr = proxy.BeginGetStateNameStruct(
                new StateStructRequest
                {
                    state1 = 1,
                    state2 = 2,
                    state3 = 3,
                },
                null,
                null);

            asr.AsyncWaitHandle.WaitOne();

            var response = proxy.EndGetStateNameStruct(asr);

            response.ShouldSatisfyAllConditions(
                () => response.stateName1.ShouldBe("Alabama"),
                () => response.stateName2.ShouldBe("Alaska"),
                () => response.stateName3.ShouldBe("Arizona"));
        }

        // TODO: add sync fault exception
        // TODO: add async fault exception

        [Test]
        public void Massimo()
        {
            var parms = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var foo = new Foo();
            Should.Throw<XmlRpcInvalidParametersException>(
                () => foo.Send_Param(parms));
        }

        [Test]
        public void NullArg()
        {
            var foo = new Foo();
            Should.Throw<XmlRpcNullParameterException>(
                () => foo.Send(null));
        }
    }
}