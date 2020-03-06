using System.Collections.Generic;

namespace JackSParrot.Services.Network.Commands
{
    public interface ICommandQueue
    {
        void AddCommand(Command command);
        void Send();
        bool IsPaused();
        void Pause();
        void Resume();
    }

    public class CommandQueue : ICommandQueue
    {
        bool _paused = false;
        string _baseUrl = "";
        readonly Queue<Command> _commands = new Queue<Command>();
        IHttpClient _client = null;
        System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder();

        public bool Pending
        {
            get
            {
                return _commands.Count > 0;
            }
        }

        public CommandQueue(IHttpClient client, string baseUrl)
        {
            _baseUrl = baseUrl;
            _client = client;
        }

        public bool IsPaused()
        {
            return _paused;
        }

        public void Pause()
        {
            _paused = true;
        }

        public void Resume()
        {
            _paused = false;
        }

        public void AddCommand(Command command)
        {
            _commands.Enqueue(command);
            if(command.IsUrgent)
            {
                Send();
            }
        }

        public void Send()
        {
            DoSend();
        }

        void DoSend()
        {
            if(_commands.Count < 1 || _paused)
            {
                return;
            }
            CommandPacket packet = new CommandPacket();
            while(Pending)
            {
                packet.AddCommand(_commands.Dequeue());
            }
            Petition p = new Petition(_baseUrl, Petition.SendMethod.Post);
            _stringBuilder.Length = 0;
            packet.Serialize(_stringBuilder);
            p.SetData(_stringBuilder.ToString());
            _client.Send(p, (petition) => OnPacketFinished(petition, packet));
        }

        void OnPacketFinished(Petition pet, CommandPacket packet)
        {
            packet.ParseResponse(pet.GetResponse(), pet.Error);
            Utils.SharedServices.GetService<EventTracker>()?.PersistPending();
        }
    }
}
