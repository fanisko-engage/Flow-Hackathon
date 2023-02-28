using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Video;


namespace NFTPortal
{
    public class NFTPortalInteractionController : MonoBehaviour
    {
        [SerializeField] private GameObject[] objectToEnableAfterThePortalIsPlaced;

        public Texture2D downloadedNFTImage;


        public string NFTtype;

        public string _pathToFile;

        public VideoPlayer canvas;

        public GameObject Sphere;
        public GameObject strangePortal;


        private void Start()
        {
            //canvas.loopPointReached += OnLoopPointReached;
            //canvas.gameObject.SetActive(true);
        }
        
        public void Initialize(string URL, string type, Action<Texture2D> onSuccessCallback=null
            )
        {
            print("URL" + URL);
            print("type" + type);
          
            NFTtype = type;
            if (type == "image")
            {
                StartCoroutine(DownloadNFTImageFromServer(URL, onSuccessCallback));
            }
            else if (type == "video")
            {
                StartCoroutine(DownloadNFTVideoFromServer(URL));
            }
        }

        private IEnumerator DownloadNFTImageFromServer(string imageURL, Action<Texture2D> onSuccessCallback)
        {
            Debug.LogError(imageURL);
            UnityWebRequest getImageRequest = UnityWebRequestTexture.GetTexture(imageURL);
            yield return getImageRequest.SendWebRequest();
            if (getImageRequest.result == UnityWebRequest.Result.Success)
            {
                var downloadedTexture = DownloadHandlerTexture.GetContent(getImageRequest);
                if (downloadedTexture != null)
                {
                    downloadedNFTImage = downloadedTexture;
                    onSuccessCallback?.Invoke(downloadedTexture);
                }
            }
            else
            {
                //todo may need to show some network error message..
            }
        }


        private IEnumerator DownloadNFTVideoFromServer(string _url)
        {
            print("_url" + _url);
            UnityWebRequest _videoRequest = UnityWebRequest.Get(_url);

            yield return _videoRequest.SendWebRequest();

            if (_videoRequest.isDone == false || _videoRequest.error != null)
            {
                Debug.Log("Request = " + _videoRequest.error);
            }

            Debug.Log("Video Done - " + _videoRequest.isDone);

            byte[] _videoBytes = _videoRequest.downloadHandler.data;
            print(_videoBytes.Length);
            _pathToFile = Path.Combine(Application.persistentDataPath, "movie.mp4");
            File.WriteAllBytes(_pathToFile, _videoBytes);
            Debug.Log(_pathToFile);
            //StartCoroutine(this.playThisURLInVideo(_pathToFile));
            yield return null;
        }

        //private void OnPortalPlaced(PortalCustomArguments tmpPortalCustomArguments)
        //{
        //    //todo need to enable the world and other stuffs...

        //    for (int i = 0; i < objectToEnableAfterThePortalIsPlaced.Length; i++)
        //    {
        //        objectToEnableAfterThePortalIsPlaced[i].SetActive(true);
        //    }
        //}

        //private void OnUnityExited(PortalCustomArguments tmpPortalCustomArguments)
        //{
        //    // sphereRenderer.ReleaseAllVideoAllocations();
        //    // SetPortalVidState(portalVidIdleState);
        //}

        private void OnDestroy()
        {
            //PortalEventBus.Instance.StopListening(PortalSystemEvents.PortalPlaced, OnPortalPlaced);
            //PortalEventBus.Instance.StopListening(PortalSystemEvents.OnUnityExited, OnUnityExited);
        }
    }
}