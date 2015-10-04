// XML-RPC.NET library
// Copyright (c) 2001-2006, Charles Cook <charlescook@cookcomputing.com>
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
//

using System;

namespace CookComputing.XmlRpc.TestHelpers
{
    public class StateNameServer : SystemMethodsBase
    {
        [XmlRpcMethod("examples.getStateName")]
        public string GetStateName(int stateNumber)
        {
            if (stateNumber < 1 || stateNumber > _stateNames.Length)
                throw new XmlRpcFaultException(1, "Invalid state number");
            return _stateNames[stateNumber - 1];
        }

        [XmlRpcMethod("examples.getStateNameFromString")]
        public string GetStateNameFromString(string stateNumber)
        {
            int number = Convert.ToInt32(stateNumber);
            if (number < 1 || number > _stateNames.Length)
                throw new XmlRpcFaultException(1, "Invalid state number");
            return _stateNames[number - 1];
        }

        [XmlRpcMethod("examples.getStateStruct")]
        public string GetStateNames(StateStructRequest request)
        {
            if (request.state1 < 1 || request.state1 > _stateNames.Length)
                throw new XmlRpcFaultException(1, "State number 1 invalid");
            if (request.state2 < 1 || request.state2 > _stateNames.Length)
                throw new XmlRpcFaultException(1, "State number 1 invalid");
            if (request.state3 < 1 || request.state3 > _stateNames.Length)
                throw new XmlRpcFaultException(1, "State number 1 invalid");
            string ret = _stateNames[request.state1 - 1] + " "
                         + _stateNames[request.state2 - 1] + " "
                         + _stateNames[request.state3 - 1];
            return ret;
        }

        [XmlRpcMethod("examples.getStateNameStruct")]
        public StateStructResponse GetStateNameStruct(StateStructRequest request)
        {
            var response = new StateStructResponse();
            if (request.state1 < 1 || request.state1 > _stateNames.Length)
                throw new XmlRpcFaultException(1, "State number 1 invalid");
            if (request.state2 < 1 || request.state2 > _stateNames.Length)
                throw new XmlRpcFaultException(1, "State number 1 invalid");
            if (request.state3 < 1 || request.state3 > _stateNames.Length)
                throw new XmlRpcFaultException(1, "State number 1 invalid");
            response.stateName1 = _stateNames[request.state1 - 1];
            response.stateName2 = _stateNames[request.state2 - 1];
            response.stateName3 = _stateNames[request.state3 - 1];
            return response;
        }

        private readonly string[] _stateNames = { 
            "Alabama", "Alaska", "Arizona", "Arkansas",
            "California", "Colorado", "Connecticut", "Delaware", "Florida",
            "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", 
            "Kansas", "Kentucky", "Lousiana", "Maine", "Maryland", "Massachusetts",
            "Michigan", "Minnesota", "Mississipi", "Missouri", "Montana",
            "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico", 
            "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma",
            "Oregon", "Pennsylviania", "Rhose Island", "South Carolina", 
            "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", 
            "Washington", "West Virginia", "Wisconsin", "Wyoming" 
        };
    }
}