using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;

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
}
