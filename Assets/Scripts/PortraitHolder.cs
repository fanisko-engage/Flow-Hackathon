using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace NFTPortal
{
    public class PortraitHolder : MonoBehaviour
    {
        public MeshRenderer textureToChange;

        public GameObject tapHeretxtObj;

        public bool isOccupied;

        //public Transform portraitPosition;
        VideoPlayer myVideoPlayer;

        public UIButtonHandler uiButtonHandler;

        public Animator windowAnimator;
        private static readonly int Property = Animator.StringToHash("Open Door");

      

        private void Start()
        {
            //windowAnimator = GetComponentInChildren<Animator>();
            uiButtonHandler.saveDataFun();
            myVideoPlayer = GetComponent<VideoPlayer>();
            textureToChange = transform.GetChild(0).GetComponent<MeshRenderer>();
        }
        public void windowAnimationFun()
        {
            StartCoroutine(windowAnimationPlayFun());
        }

        public IEnumerator windowAnimationPlayFun()
        {
            if (tapHeretxtObj != null)
                tapHeretxtObj.SetActive(false);
            //todo need to trigger the animation...
            windowAnimator.SetTrigger(Property);
            yield return new WaitForSeconds(2f);
            uiButtonHandler.buttonCanvas.SetActive(true);
            
        }

        //public void SetDownloadedImage(Texture2D downloadedImage)
        //{
        //    textureToChange.gameObject.SetActive(true);
        //    textureToChange.material.mainTexture = downloadedImage;
        //    isOccupied = true;
        //}

        //public void SetDownloadedVideo(string _url)
        //{
        //    isOccupied = true;
        //    StartCoroutine(PlayDownloadedVideo(_url));
        //}

        //public IEnumerator PlayDownloadedVideo(string _url)
        //{
        //    //todo need to trigger the animation...
        //    windowAnimator.SetTrigger(Property);
        //    yield return new WaitForSeconds(2f);
        //    uiButtonHandler.buttonCanvas.SetActive(true);
        //   // StartCoroutine(VideoPlayFun(_url));

        //}

        public IEnumerator VideoPlayFun(string _url)
        {

            myVideoPlayer.source = UnityEngine.Video.VideoSource.Url;
            myVideoPlayer.targetMaterialRenderer = textureToChange;
#if UNITY_EDITOR
            myVideoPlayer.url = "file://" + _url;
#else
            myVideoPlayer.url = _url;
#endif

            myVideoPlayer.Prepare();

            while (myVideoPlayer.isPrepared == false)
            {
                yield return null;
            }

            textureToChange.gameObject.SetActive(true);
            Debug.Log("Video should play");
            myVideoPlayer.Play();
        }
    }
}