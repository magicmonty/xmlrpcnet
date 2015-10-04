using System.Collections.Generic;

namespace CookComputing.XmlRpc
{
    public class MappingStack : Stack<string>
    {
        public MappingStack (string parseType)
        {
            _parseType = parseType ?? string.Empty;
        }

        public new void Push (string str)
        {
            base.Push (str);
        }

        public string MappingType { get { return _parseType; } }

        private readonly string _parseType = string.Empty;
    }
}
