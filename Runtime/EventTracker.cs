using JackSParrot.JSON;
using JackSParrot.Utils;

namespace JackSParrot.Services.Network.Commands
{
    public class EventTracker
    {
        public void TrackEvent(string category, string userID, JSONObject data = null)
        {
            JSONObject toSend = new JSONObject();
            toSend["user_id"] = userID;
            toSend["ts"] = Utils.TimeUtils.Timestamp;
            toSend["category"] = category;
            if(data != null)
            {
                toSend["data"] = data;
            }
            SharedServices.GetService<CommandQueue>().AddCommand(new Network.Commands.TrackEventCommand(toSend));
        }
    }
}

