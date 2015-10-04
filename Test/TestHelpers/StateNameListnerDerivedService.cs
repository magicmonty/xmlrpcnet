namespace CookComputing.XmlRpc.TestHelpers
{
    public class StateNameListnerDerivedService : XmlRpcListenerService, IStateNameDerived
    {
        public string GetStateName(int stateNumber)
        {
            if (stateNumber < 1 || stateNumber > _stateNames.Length)
                throw new XmlRpcFaultException(1, "Invalid state number");
            return _stateNames[stateNumber - 1];
        }

        private static readonly string[] _stateNames = 
        { 
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