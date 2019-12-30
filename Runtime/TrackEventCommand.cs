using JackSParrot.JSON;

namespace JackSParrot.Services.Network.Commands
{
    public class TrackEventCommand : Command
    {
        System.Action<uint, bool> _onResponseCallback;
        uint _id = 0U;

        public TrackEventCommand(uint id, JSONObject data, System.Action<uint, bool> onResponseCallback) : base("track_event")
        {
            _id = id;
            _onResponseCallback = onResponseCallback;
            foreach(var kvp in data)
            {
                Parameters[kvp.Key] = kvp.Value.Clone();
            }
            SetOnResponseCallback(OnResponseReceived);
        }

        void OnResponseReceived(JSONObject data, NetworkError error)
        {
            _onResponseCallback?.Invoke(_id, error == null && data["success"]);
        }
    }
}
