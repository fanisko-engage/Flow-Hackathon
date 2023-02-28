using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
namespace NFTPortal
{
    public static class ButtonExtension
    {
        public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
        {
            button.onClick.AddListener(delegate ()
            {
                OnClick(param);
            });
        }
        //public static void AddEventListener<T>(this Button button, Action<T[]> OnClick, params T[] param)
        //{
        //    button.onClick.AddListener(delegate ()
        //    {
        //        OnClick(param);
        //    });
        //}
        public static void RemoveEventListener<T>(this Button button, T param, Action<T> OnClick)
        {
            button.onClick.RemoveListener(delegate ()
            {
                OnClick(param);
            });
        }
    }
    public class UIButtonHandler : MonoBehaviour
    {
        public GameObject buttonCanvas;

        public GameObject infoPanel;

        public List<string> loadingUrlList;

        // public Image placeCheckIcon;

        // public Sprite placeIcon, replaceIcon;

        public TextMeshProUGUI placeCheckText;

        bool placed = false;

        public GameObject showList;

        public int frameIndex;

        [SerializeField] ScrollRect scrollview = null;
        [SerializeField] GameObject scrollcontent = null;
        [SerializeField] GameObject scrollItemPrefab = null;
        public GameObject itemobj;

        public NFTPortalInteractionController nftPortalInteractionController;

        public Renderer portraitUi;

        string savedData = "";
        private void Start()
        {
          
            StartCoroutine(ListGenerateFun());
        }

        public void saveDataFun()
        {
            savedData = GetString(frameIndex.ToString());
            print("savedData" + savedData);
            if (savedData != "")
            {
                placed = true;
                placeCheckText.text = "Replace";
                nftPortalInteractionController.Initialize(savedData,
             "image", OnImageDownloaded);
            }
        }
        public void SetString(string KeyName, string Value)
        {
            PlayerPrefs.SetString(KeyName, Value);
        }

        public string GetString(string KeyName)
        {
            return PlayerPrefs.GetString(KeyName);
        }
        public void InfoClickFun()
        {
            infoPanel.SetActive(true);
        }
        public void sellClickFun()
        {

        }
        public void PlaceCheckFun()
        {
            if (placed)
            {
                //placeCheckIcon.sprite = placeIcon;
              //  placeCheckText.text = "Place";
                ShowListFun();
               // placed = false;
            }
            else if (!placed)
            {
                // placeCheckIcon.sprite = replaceIcon;
                placeCheckText.text = "Replace";
                ShowListFun();
                placed = true;
            }
        }
        public void ShowListFun()
        {
            showList.SetActive(true);
            ListGenerateFun();
        }
        public void AddPortalListURL(string url)
        {
            loadingUrlList.Add(url);
        }
        IEnumerator ListGenerateFun()
        {
            yield return new WaitForSeconds(2f);
            for (int j = 0; j <= loadingUrlList.Count - 1; j++)
            {
                itemobj = Instantiate(scrollItemPrefab);
                itemobj.transform.SetParent(scrollcontent.transform, false);
                itemobj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = loadingUrlList[j];
                itemobj.GetComponent<Button>().AddEventListener(j, ItemClicked);

                //itemobj.GetComponent<Button>().AddEventListener(ItemClicked1,j,"Ragava","Sat", 100 );

            }
        }

        //private void ItemClicked1<T>(T[] obj)
        //{
           
        //}

        void ItemClicked(int index)
        {
            print("clicked URL" + loadingUrlList[index]);
            SetString(frameIndex.ToString(), loadingUrlList[index]);
            nftPortalInteractionController.Initialize(loadingUrlList[index],
              "image", OnImageDownloaded);
            showList.SetActive(false);
        }

        private void OnImageDownloaded(Texture2D obj)
        {
            portraitUi.gameObject.SetActive(true);
            portraitUi.material.mainTexture = obj;
        }
    }
}
