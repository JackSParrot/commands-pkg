using System;

namespace JackSParrot.Services.Network.Commands
{
    public interface IHttpClient
    {
        void Send(Petition petition, Action<Petition> onFinish = null);
    }
}

