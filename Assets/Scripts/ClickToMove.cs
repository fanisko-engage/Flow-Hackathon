using System;
using System.Collections;
using System.IO;
using UnityEngine;
//using UnityEngine.AI;

namespace NFTPortal
{
// Use physics raycast hit from mouse click to set agent destination
    //[RequireComponent(typeof(NavMeshAgent))]
    public class ClickToMove : MonoBehaviour
    {
        //NavMeshAgent m_Agent;
        public PortraitHolder[] portraitFrames;
        RaycastHit m_HitInfo = new RaycastHit();

        private bool isTouchedThePortrait;
        private PortraitHolder hittedObject;

        public LayerMask layerToHit;


       
        void Start()
        {
            //m_Agent = GetComponent<NavMeshAgent>();


            //  CheckAndInsertTheImagesToPortrait();
        }

        private void CheckAndInsertTheImagesToPortrait()
        {
            if (!Directory.Exists(GetNFTPath()))
            {
                Directory.CreateDirectory(GetNFTPath());

                Debug.LogError("Directory is not available...");
                return;
            }

            string[] imageFiles = Directory.GetFiles(GetNFTPath(), "*.png");
            foreach (var imageFile in imageFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(imageFile);
                Texture2D savedFileTexture = new Texture2D(2, 2);
                var fileBytes = File.ReadAllBytes(imageFile);
                if (savedFileTexture.LoadImage(fileBytes))
                {
                    //portraitFrames[int.Parse(fileName)].SetDownloadedImage(savedFileTexture);
                }
            }
        }

        void Update()
        {
            Debug.DrawRay(this.transform.forward * 1.1f, this.transform.forward, Color.red);
            if (Input.GetMouseButtonDown(0) /*&& !Input.GetKey(KeyCode.LeftShift)*/)
            {
                print("click enter");
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.Log("ray " + ray.origin);
                if (Physics.Raycast(ray, out m_HitInfo, 100f, layerToHit))
                {
                    print("m_HitInfo.point" + m_HitInfo.collider.gameObject.name);

                    if (m_HitInfo.collider.gameObject.CompareTag("Portrait"))
                    {
                        var portrait = m_HitInfo.collider.gameObject.GetComponent<PortraitHolder>();
                        //   m_Agent.destination = portrait.portraitPosition.position;

                        if (!isTouchedThePortrait && hittedObject == null && !portrait.isOccupied)
                        {
                            Debug.Log("Portrait hit pass ");
                            Debug.Log("frame name " + portrait.name);
                            isTouchedThePortrait = true;
                            hittedObject = portrait;
                        }

                        return;
                    }

                    //  m_Agent.destination = m_HitInfo.point;
                }
                else
                {
                    Debug.Log("no ray is casted");
                }
            }

            //var dist = m_Agent.remainingDistance;

            if (isTouchedThePortrait) //&& dist != Mathf.Infinity &&
                //m_Agent.pathStatus == NavMeshPathStatus.PathComplete && m_Agent.remainingDistance == 0)
            {
                isTouchedThePortrait = false;
                Debug.Log("ReachedTheDestination");

                StartCoroutine(PlayAnimationAndPlaceTheMedia());
            }
        }

        private IEnumerator PlayAnimationAndPlaceTheMedia()
        {
            //string NFTtype = FindObjectOfType<NFTPortalInteractionController>().NFTtype;
            //if (NFTtype == "image")
            //{
            //    var downloadedImage = FindObjectOfType<NFTPortalInteractionController>().downloadedNFTImage;
            //    hittedObject.SetDownloadedImage(downloadedImage);
            //}
            //else if (NFTtype == "video")
            //{
            //    hittedObject.SetDownloadedVideo(FindObjectOfType<NFTPortalInteractionController>()._pathToFile);
            //}

            hittedObject.windowAnimationFun();

            //todo save the file to the persitant datapath for future use...
            //  SaveImageToDevice(downloadedImage, Array.IndexOf(portraitFrames, hittedObject));
            hittedObject = null;
            yield return null;
        }


        private void SaveImageToDevice(Texture2D downloadedImage, int indexOf)
        {
            var imageBytes = downloadedImage.EncodeToPNG();
            var applicationPath = GetNFTPath();
            applicationPath = Path.Combine(applicationPath, indexOf + ".png");
            File.WriteAllBytes(applicationPath, imageBytes);
        }


        public string GetNFTPath()
        {
            var applicationPath = Application.persistentDataPath;
            return Path.Combine(applicationPath, "NFT");
        }

        
    }
}