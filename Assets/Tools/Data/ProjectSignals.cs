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

    public class MetaLanding : SignalData { }

    public class FlushOperation : SignalData { }

    public class LevelCompleteSignal : SignalData 
    {
        public int difficulty;
        public LevelCompleteSignal(int difficulty) 
        
        {
            this.difficulty = difficulty;
        }
    }
}
