﻿using System;
using System.Collections.Generic;
using JackSParrot.JSON;

namespace JackSParrot.Services.Network.Commands
{
    public class CommandPacket
    {
        readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>();

        public CommandPacket()
        {

        }

        public void AddCommand(Command command)
        {
            _commands.Add(_commands.Count.ToString(), command);
        }

        public void Serialize(System.Text.StringBuilder sb)
        {
            JSONObject data = new JSONObject();
            using(var itr = _commands.GetEnumerator())
            {
                while(itr.MoveNext())
                {
                    data.Add(itr.Current.Key, itr.Current.Value.Data);
                }
            }
            data.Serialize(sb);
        }

        public void ParseResponse(string data, NetworkError receivedError)
        {
            NetworkError error = receivedError;
            JSONObject parsedResponse = error == null ? null : new JSONObject();
            if (error == null)
            {
                try
                {
                    parsedResponse = JSON.JSON.LoadString(data);
                }
                catch (Exception gotcha)
                {
                    error = new NetworkError(31337, gotcha.Message);
                    parsedResponse = new JSONObject();
                }
            }
            if(error != null)
            {
                using (var itr = _commands.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        itr.Current.Value.OnResponse(parsedResponse, error);
                    }
                }
                return;
            }
            using(var itr = parsedResponse.GetEnumerator())
            {
                while(itr.MoveNext())
                {
                    _commands[itr.Current.Key].OnResponse(itr.Current.Value.AsObject(), error);
                }
            }
        }
    }
}

