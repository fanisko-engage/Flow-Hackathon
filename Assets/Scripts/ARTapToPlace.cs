using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlace : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]

    public GameObject objectToPlace;
    public GameObject placementIndicator;
    public GameObject tapTheGrid;
    public GameObject scanUI;
    
   
    //public GameObject _trackedPlaneHitPoint;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool indicatorDisplay = true;

    bool isPlacedPortalOnce = false;

    public bool isGameStarted = false;
    private bool tapTheGridActive;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
    ARPlaneManager m_planeManager;
   
    IEnumerator _couroutine;

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_planeManager = GetComponent<ARPlaneManager>();
    }
    private void Start()
    {
        _couroutine = EnableTapFunction();
        StartCoroutine(_couroutine);
        scanUI.SetActive(true);
        //  SetIsGameStarted(true);

        /*  placementPose.position = new Vector3(0, 0, 4f);
          var cameraForward = Camera.main.transform.forward;
          var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
          placementPose.rotation = Quaternion.LookRotation(cameraBearing);
          PlaceObject();*/
    }
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

   public void SetIsGameStarted(bool value)
    {
        isGameStarted = value;
    }
    void Update()
    {
        //if game is not started don't enalbe placement indicator 
        if (!isGameStarted)
        {
            return;

        }
        else
        {
            if (indicatorDisplay)
            {
                UpdatePlacementPose();
                UpdatePlacementIndicator();
            }
            if (!TryGetTouchPosition(out Vector2 touchPoint))
                return;
            if (!isPlacedPortalOnce) ///*placementPoseIsValid && 
            {
                isPlacedPortalOnce = true;
                PlaceObject();
                //_trackedPlaneHitPoint.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }

    }

    public void ResetTapToPlace()
    {
        indicatorDisplay = true;
        isPlacedPortalOnce = false;
        isGameStarted = false;
       // objectToPlace.SetActive(false);
    }

    private void PlaceObject()
    {
       
        objectToPlace.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        objectToPlace.transform.localScale = new Vector3(0.6f,0.6f,0.6f);
        objectToPlace.gameObject.SetActive(true);
        indicatorDisplay = false;
        placementIndicator.SetActive(false);
        tapTheGrid.SetActive(false);
        m_planeManager.enabled = false;
        m_RaycastManager.enabled = false;
        this.enabled = false;
        Debug.Log("Closing AR Plane Manager and Raycast Manager");
    }

    public void SetStartGame(bool _value)
    {
        isGameStarted = _value;
    }
   

    private void UpdatePlacementPose()
    {

        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        m_RaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinInfinity | TrackableType.PlaneWithinBounds | TrackableType.PlaneWithinPolygon |
            TrackableType.FeaturePoint);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            //if the ground is tracked for the first time then fire event, we using and limiting bool becuase this funciotn
            //is called multiple time and it not right to fire ground fire event for every frames
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            // if (!isPlacedPortalOnce)
            // {
            //     _trackedPlaneHitPoint.SetActive(true);
            //     _trackedPlaneHitPoint.transform.position = placementPose.position;
            //     _trackedPlaneHitPoint.transform.rotation = placementPose.rotation;
            // }
        }

    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            tapTheGrid.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            if(!tapTheGridActive)
            {
                  tapTheGrid.SetActive(true);
                  StartCoroutine(DisableGameObject(tapTheGrid));
                  tapTheGridActive=true;
            }
            
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

       IEnumerator EnableTapFunction()
    {
        SetIsGameStarted(false);
        yield return new WaitUntil(()=> m_planeManager.trackables.count >= 1);
        SetIsGameStarted(true);
        scanUI.SetActive(false);
        tapTheGrid.SetActive(true);
    }
    
    private IEnumerator DisableGameObject(GameObject obj)
    {
             yield return new WaitForSeconds(3f);
             if(obj.activeInHierarchy)
             obj.SetActive(false);
    }


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("Scene loaded");
    }
}
