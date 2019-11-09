using System;
using JackSParrot.JSON;

namespace JackSParrot.Services.Network.Commands
{
    public class PingCommand : Command
    {
        public PingCommand(Action<JSONObject, NetworkError> responseCallback):base("ping", responseCallback, true)
        {

        }
    }
}