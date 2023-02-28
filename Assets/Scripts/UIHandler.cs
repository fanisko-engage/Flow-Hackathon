using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    bool m_inverseSwipeFlag;
    [SerializeField]
    Sprite[] m_onBoardingScreens;
    [SerializeField]
    Image[] m_onBoardingScreensCounter;
    [SerializeField]
    GameObject m_currentOnboardingScreen, m_onBoardingScreenNavigatorHolder, m_walletConnectionScreenBtn,
        m_onBoardingScreenCounterHolder, m_loadScreen,m_enteringPrivateSpaceImage;

    GameObject leftNavigatorBtn, rightNavigatorBtn;

    Animator currentonboardingscreenanimator;

    bool allowswipe = false;

    int currentScreenIndex = 0;
    int swipeDirectionFlag = 1;
    Vector2 fingertouchstartpoint, fingertouchendpoint;
    // Start is called before the first frame update
    void Start()
    {
        currentonboardingscreenanimator = m_currentOnboardingScreen.GetComponent<Animator>();

        currentonboardingscreenanimator.enabled = false;

        leftNavigatorBtn = m_onBoardingScreenNavigatorHolder.transform.Find("Left").transform.gameObject;
        rightNavigatorBtn = m_onBoardingScreenNavigatorHolder.transform.Find("Right").transform.gameObject;
        m_walletConnectionScreenBtn.SetActive(false);

        //Setting initial state of Navigator buttons
        SetNavigatorBtnState(currentScreenIndex);
        SetOnBoardingScreenCounter(currentScreenIndex);

        //Default onboarding screen is set
        m_currentOnboardingScreen.GetComponent<Image>().sprite = m_onBoardingScreens[0];
        Color imgColor = m_currentOnboardingScreen.GetComponent<Image>().color;
        imgColor.a = 1f;
        m_currentOnboardingScreen.GetComponent<Image>().color = imgColor;

    }

    // Update is called once per frame
    void Update()
    {
        swipeDirectionFlag = SwipeDetection();

        if (swipeDirectionFlag == -1 && currentScreenIndex == 0)
            allowswipe = false;
        else if (swipeDirectionFlag == 1 && currentScreenIndex == 0)
            allowswipe = true;
        else if ((swipeDirectionFlag == 1 || swipeDirectionFlag == -1) && (currentScreenIndex >= 0 && currentScreenIndex < m_onBoardingScreens.Length-1))
            allowswipe = true;
        else if (swipeDirectionFlag == 1 && currentScreenIndex == m_onBoardingScreens.Length - 1)
            allowswipe = false;
        else if (swipeDirectionFlag == -1 && currentScreenIndex == m_onBoardingScreens.Length - 1)
            allowswipe = true;

        if (allowswipe)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
                ManuverBtwScreens(swipeDirectionFlag);
#elif !UNITY_EDITOR
        if (Input.GetTouch(0).phase == TouchPhase.Ended && Input.touchCount == 1)
            ManuverBtwScreens(swipeDirectionFlag);
#endif
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ManuverBtwScreens(int directionFlag)
    {
        Debug.Log($"Direction falg value {directionFlag}");

        currentonboardingscreenanimator.enabled = true;
        currentScreenIndex += directionFlag;
        PlayScreenTransistion(directionFlag);
        if (currentScreenIndex < m_onBoardingScreens.Length && currentScreenIndex >= 0)
        {

            SetNavigatorBtnState(currentScreenIndex);
            SetOnBoardingScreenCounter(currentScreenIndex);
            m_currentOnboardingScreen.GetComponent<Image>().sprite = m_onBoardingScreens[currentScreenIndex];
        }

    }

    async void PlayScreenTransistion(int directionFlag)
    {
        AnimatorStateInfo stateInfo = currentonboardingscreenanimator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length;

        if (directionFlag == -1)
            currentonboardingscreenanimator.Play("ScreenTransistionFromLeft");
        else if(directionFlag == 1)
            currentonboardingscreenanimator.Play("ScreenTransistionFromRight");

        await Task.Delay(200);
        currentonboardingscreenanimator.Play("Idle");

    }

    void SetOnBoardingScreenCounter(int currentScreenIndex)
    {
        for (int i = 0; i < m_onBoardingScreensCounter.Length; i++)
        {
            Vector3 tempScreenCounterScale = m_onBoardingScreensCounter[i].transform.localScale;
            Color screenCounterColor = m_onBoardingScreensCounter[i].color;
            if (i == currentScreenIndex)
            {
                m_onBoardingScreensCounter[i].transform.localScale = Vector3.one * 1.5f;
                screenCounterColor.a = 1f;
                m_onBoardingScreensCounter[i].color = screenCounterColor;
            }
            else
            {
                m_onBoardingScreensCounter[i].transform.localScale = Vector3.one * 1f;
                screenCounterColor.a = .5f;
                m_onBoardingScreensCounter[i].color = screenCounterColor;
            }
        }
    }

    void SetNavigatorBtnState(int screenIndexFlag)
    {
        if (screenIndexFlag == 0)
        {
            leftNavigatorBtn.GetComponent<Button>().interactable = false;
            m_walletConnectionScreenBtn.SetActive(false);
            //m_onBoardingScreenCounterHolder.SetActive(true);
        }
        else if (screenIndexFlag == m_onBoardingScreens.Length - 1)
        {
            rightNavigatorBtn.GetComponent<Button>().interactable = false;
            m_walletConnectionScreenBtn.SetActive(true);
            //m_onBoardingScreenCounterHolder.SetActive(false);
        }
        else
        {
            rightNavigatorBtn.GetComponent<Button>().interactable = leftNavigatorBtn.GetComponent<Button>().interactable = true;
            m_walletConnectionScreenBtn.SetActive(false);
            //m_onBoardingScreenCounterHolder.SetActive(true);
        }
    }

    private int SwipeDetection()
    {
        int swipedirectionflag = 1;
#if !UNITY_EDITOR
        if(Input.GetTouch(0).phase == TouchPhase.Began && Input.touchCount == 1)
        {
            fingertouchstartpoint = Input.GetTouch(0).position;
            fingertouchendpoint = fingertouchstartpoint;
        }
        else if(Input.GetTouch(0).phase == TouchPhase.Moved && Input.touchCount == 1)
        {
            fingertouchendpoint = Input.GetTouch(0).position;
        }
        else if(Input.GetTouch(0).phase == TouchPhase.Ended && Input.touchCount == 1)
        {
            Debug.Log($"touch start point {fingertouchstartpoint} and end point {fingertouchendpoint}");
            if ((fingertouchendpoint.x - fingertouchstartpoint.x) > 0f)
            {
                swipedirectionflag = 1;
                Debug.Log("swiping in +ve direction");

            }
            else if ((fingertouchendpoint.x - fingertouchstartpoint.x) < 0f)
            {
                swipedirectionflag = -1;
                Debug.Log("swiping in -ve direction");
            }
            Debug.Log("Touch ended");
        }
#elif UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Touch begun");
            fingertouchstartpoint = Input.mousePosition;
            fingertouchendpoint = fingertouchstartpoint;
            Debug.Log($"touch start point {fingertouchstartpoint} and end point {fingertouchendpoint}");
        }
        else if (Input.GetMouseButton(0))
        {
            fingertouchendpoint = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log($"touch start point {fingertouchstartpoint} and end point {fingertouchendpoint}");
            if ((fingertouchendpoint.x - fingertouchstartpoint.x) > 0f)
            {
                swipedirectionflag = 1;
                Debug.Log("swiping in +ve direction");

            }
            else if ((fingertouchendpoint.x - fingertouchstartpoint.x) < 0f)
            {
                swipedirectionflag = -1;
                Debug.Log("swiping in -ve direction");
            }
            Debug.Log("Touch ended");
        }
#endif
        if(m_inverseSwipeFlag)
            return -swipedirectionflag;
        else
            return -swipedirectionflag;
    }
    public void SceneLoader(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("Scene loaded");
    }


    public async void LoadNFTScene(string sceneName)
    {
        m_loadScreen.SetActive(true);
        await Task.Delay(2000);
        Debug.Log("Debug 1");
        m_enteringPrivateSpaceImage.SetActive(true);
        await Task.Delay(1000);
        Debug.Log("Debug 2");
        m_loadScreen.SetActive(false);
        SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("Scene loaded");
    }
}
