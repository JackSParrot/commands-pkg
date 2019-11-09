using System;
using System.Collections.Generic;

namespace JackSParrot.Services.Network.Commands
{
    public class Petition
    {
        public enum SendMethod
        {
            Get,
            Post
        }

        public readonly Dictionary<string, string> Headers = null;
        public SendMethod Method = SendMethod.Get;
        public string Url {get; private set;}
        public NetworkError Error {get; set;}
        public int Retries = 4;
        byte[] _data = null;
        string _response;

        public Petition(string url, SendMethod method = SendMethod.Get)
        {
            Url = url;
            Headers = new Dictionary<string, string>();
            Method = method;
            Error = null;
        }

        public void SetData(string data)
        {
            _data = System.Text.Encoding.ASCII.GetBytes(data.ToCharArray());
        }

        public string GetData()
        {
            return System.Text.Encoding.ASCII.GetString(_data);
        }

        public void SetRawData(byte[] data)
        {
            _data = data;
        }

        public byte[] GetRawData()
        {
            return _data;
        }

        public string GetResponse()
        {
            return _response;
        }

        public void SetResponse(string response)
        {
            _response = response;
        }

        public void SetRawResponse(byte[] response)
        {
            _response = System.Text.Encoding.ASCII.GetString(response);
        }
    }
}

