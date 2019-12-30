using JackSParrot.JSON;
using JackSParrot.Utils;
using System.Collections.Generic;

namespace JackSParrot.Services.Network.Commands
{
    public class EventTracker
    {
        static uint _lastId = 0U;
        string _path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "tracks.json");
        Dictionary<uint, JSONObject> _pending = new Dictionary<uint, JSONObject>();

        public EventTracker()
        {
            if(System.IO.File.Exists(_path))
            {
                string text = System.IO.File.ReadAllText(_path);
                try
                {
                    JSONObject jsonfile = JSON.JSON.LoadString(text);
                    foreach(var kvp in jsonfile)
                    {
                        uint id = 0;
                        if(uint.TryParse(kvp.Key, out id))
                        {
                            _pending.Add(id, kvp.Value.AsObject());
                            if(id > _lastId)
                            {
                                _lastId = id;
                            }
                        }
                        else
                        {
                            SharedServices.GetService<ICustomLogger>()?.LogError("Corrupted key in tracks file: {" + kvp.Key + ": " + kvp.Value);
                        }
                    }
                    System.IO.File.WriteAllText(_path, "{}");
                }
                catch(MalformedJSONException e)
                {
                    SharedServices.GetService<ICustomLogger>()?.LogError("Error parsing saved tracks: " + e.Message);
                }
            }
        }

        public void TrackEvent(string category, string userID, JSONObject data = null)
        {
            JSONObject toSend = new JSONObject();
            toSend["user_id"] = userID;
            toSend["ts"] = TimeUtils.Timestamp;
            toSend["category"] = category;
            if(data != null)
            {
                toSend["data"] = data;
            }
            var id = ++_lastId;
            SharedServices.GetService<CommandQueue>().AddCommand(new TrackEventCommand(id, toSend, OnTrackResponse));
            _pending.Add(id, toSend);
        }

        void OnTrackResponse(uint trackId, bool receivedByServer)
        {
            if(receivedByServer)
            {
                _pending.Remove(trackId);
            }
        }

        public void PersistPending()
        {
            var toSave = new JSONObject();
            foreach(var kvp in _pending)
            {
                toSave.Add(kvp.Key.ToString(), kvp.Value.Clone());
            }
            System.IO.File.WriteAllText(_path, toSave.ToString());
        }

        public void TrackPending()
        {
            var pending_copy = new Dictionary<uint, JSONObject>(_pending);
            _pending.Clear();
            foreach(var obj in pending_copy.Values)
            {
                var id = ++_lastId;
                SharedServices.GetService<CommandQueue>().AddCommand(new TrackEventCommand(id, obj, OnTrackResponse));
                _pending.Add(id, obj.Clone().AsObject());
            }
        }
    }
}

