using JackSParrot.JSON;

namespace JackSParrot.Services.Network.Commands
{
    public class TrackEventCommand : Command
    {
        public TrackEventCommand(JSONObject data) : base("track_event")
        {
            foreach(var kvp in data)
            {
                Parameters[kvp.Key] = kvp.Value.Clone();
            }
        }
    }
}
