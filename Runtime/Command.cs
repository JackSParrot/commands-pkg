using System;
using JackSParrot.JSON;

namespace JackSParrot.Services.Network.Commands
{
    public class Command
    {
        public string CommandName;
        public JSONObject Data = null;
        public bool IsUrgent = false;
        protected JSONObject Parameters = new JSONObject();
        event Action<JSONObject, NetworkError> _onResponseReceived;

        public Command(string name, Action<JSONObject, NetworkError> responseCallback = null, bool urgent = false)
        {
            IsUrgent = urgent;
            CommandName = name;
            _onResponseReceived = responseCallback;
            Data = new JSONObject();
            Data["name"] = CommandName;
            Data["ts"] = Utils.TimeUtils.Timestamp;
            Data["data"] = Parameters;
        }

        public string Serialize()
        {
            return Data.ToString();
        }

        public void Deserialize(string data)
        {
            Data = new JSONParser().Parse(data).AsObject();
        }

        protected NetworkError ValidateResponse(JSONObject response)
        {
            if (response["result"]["error"])
            {
                return new NetworkError(31337, response["result"]["msg"]);
            }
            return null;
        }

        public void OnResponse(JSONObject response, NetworkError error)
        {
            if(error == null)
            {
                error = ValidateResponse(response);
            }
            _onResponseReceived?.Invoke(response, error);
        }
    }
}

