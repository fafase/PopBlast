using System.Collections.Generic;
using Unity.Services.Core;

namespace Tools
{
    public class LoginSignalData : SignalData
    {
        public ServicesInitializationState State { get; }

        public LoginSignalData(ServicesInitializationState state)
        {
            State = state;
        }
    }

    public class MetaLanding : SignalData {  }

    public class FlushOperation : SignalData
    {
        public Dictionary<string, string> Data { get; private set; }
        public FlushOperation(Dictionary<string, string> data)
        {
            Data = data;
        }
    }
}
