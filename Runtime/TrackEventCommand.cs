using JackSParrot.JSON;

namespace JackSParrot.Services.Network.Commands
{
    public class TrackEventCommand : Command
    {
        public TrackEventCommand(JSONObject data):base("track_event")
        {
            Parameters["data"] = data;
        }
    }
}