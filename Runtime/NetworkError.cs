
namespace JackSParrot.Services.Network.Commands
{
    public class NetworkError
    {
        public int Code;
        public string Message;

        public NetworkError(int code, string message)
        {
            Code = code;
            Message = message;
        }
        public override string ToString()
        {
            return "{Code: " + Code.ToString() + ", Message : " + Message + "}";
        }
    }
}

