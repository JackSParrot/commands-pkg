using System;
using UnityEngine.Networking;
using System.Collections;
using JackSParrot.Utils;

namespace JackSParrot.Services.Network.Commands
{
    public class UnityHttpClient : IHttpClient
    {
        public void Send(Petition petition, Action<Petition> onFinish = null)
        {
            SharedServices.GetService<CoroutineRunner>().StartCoroutine(this, SendCoroutine(petition, onFinish));
        }

        static IEnumerator SendCoroutine(Petition petition, Action<Petition> onFinish)
        {
            NetworkError error = null;
            string response = string.Empty;
            int retries = 0;
            do
            {
                var uploader = new UploadHandlerRaw(petition.GetRawData());
                uploader.contentType = "application/json";

                var downloadHandler = new DownloadHandlerBuffer();

                var request = new UnityWebRequest(petition.Url);
                request.method = petition.Method == Petition.SendMethod.Post ? UnityWebRequest.kHttpVerbPOST : UnityWebRequest.kHttpVerbGET;
                request.useHttpContinue = false;
                request.chunkedTransfer = false;
                request.redirectLimit = 50;
                request.timeout = 60;
                if(petition.Method == Petition.SendMethod.Post)
                {
                    request.SetRequestHeader("Content-Type", "application/json");
                }
                request.uploadHandler = uploader;
                request.downloadHandler = downloadHandler;

                yield return request.SendWebRequest();

                if(request.isNetworkError || request.isHttpError)
                {
                    error = new NetworkError((int)request.responseCode, request.error);
                    SharedServices.GetService<ICustomLogger>().LogError("Network error error: {" + error.Code + ": " + error.Message + "} for petition: " + petition.GetData());
                }
                else
                {
                    response = request.downloadHandler.text;
                    error = null;
                }
                ++retries;
            }
            while(error != null && retries < petition.Retries);

            petition.SetResponse(response);
            petition.Error = error;
            onFinish?.Invoke(petition);
        }
    }
}

