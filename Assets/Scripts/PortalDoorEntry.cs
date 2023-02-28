using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Rendering;

public class PortalDoorEntry : MonoBehaviour
{
    Transform device;
    //bool for checking if the device is not in the same direction as it was
    bool wasInFront;
    //bool for knowing that on the next change of state, what to set the stencil test
    [SerializeField] bool inOtherWorld;
    //This bool is on while device colliding, done so we ensure the shaders are being updated before render frames
    //Avoids flickering
    bool isColliding = false;
    //public Renderer portal360Renderer;
    [SerializeField]
    bool isSetMat = false;

    public GameObject[] _nftGlassFrames;
    public Renderer[] _portalWorldRenderers;

    public Material[] _collectorRoomObjectMaterials;


    void Start()
    {



        //start outside other world
        SetMaterials(false);
        device = Camera.main.transform;

    }

    private void OnDestroy()
    {
        SetMaterials(false);
    }


    void SetMaterials(bool fullRender)
    {
        var stencilTest = fullRender ? CompareFunction.NotEqual : CompareFunction.Equal;
        if (isSetMat)
        {
            foreach (var material in _collectorRoomObjectMaterials)
                material.SetInt("_StencilTest", (int)stencilTest);
        }
        else
        {
            foreach (var renderer in _portalWorldRenderers)
                renderer.sharedMaterial.SetInt("_StencilTest", (int)stencilTest);
        }
        foreach (var glassFrames in _nftGlassFrames)
            glassFrames.SetActive(fullRender);
        //foreach (var renderer in _portalWorldRenderers)
        //    renderer.sharedMaterial.SetInt("_StencilTest", (int)stencilTest);
        //portal360Renderer.sharedMaterial.SetInt("_StencilTest", (int)stencilTest);
        //Debug.Log("Stencil val : "+stencilTest);
    }

    bool GetIsInFront()
    {
        Vector3 worldPos = device.position + device.forward * Camera.main.nearClipPlane;
        Vector3 pos = transform.InverseTransformPoint(worldPos);
        return pos.z >= 0 ? true : false;
    }


    //This technique registeres if the device has hit the portal, flipping the bool

    void OnTriggerEnter(Collider other)
    {
        //if (other.transform != device)
        //{
        //    Debug.Log("not a cam");
        //    return;
        //}

        ////Important to do this for if the user re-enters the portal from the same side
        //wasInFront = GetIsInFront();
        //Debug.Log("it is a cam");
        //isColliding = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name != device.name)
            return;

        //Outside of the other world
        Vector3 worldPos = other.transform.position + other.transform.forward * Camera.main.nearClipPlane;
        if (transform.position.z > worldPos.z)//other.transform.position.z)
        {
            SetMaterials(false);
            Debug.Log("outside the world");
        }
        else
        {
            SetMaterials(true);
            Debug.Log("inside the world");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform != device)
            return;
        //isColliding = false;
    }


    /*If there has been a change in the relative position of the device to the portal, flip the
     *Stencil Test
     */

    void WhileCameraColliding()
    {
        if (!isColliding)
        {
            //Debug.Log("is not colliding with window");
            return;
        }
        bool isInFront = GetIsInFront();
        Debug.Log($"is in front {isInFront}, was in front {wasInFront}");
        //Debug.Log("is colliding with window");
        if ((isInFront && !wasInFront) || (wasInFront && !isInFront))
        {
            Debug.Log($"condition passed {inOtherWorld}");
            inOtherWorld = !inOtherWorld;
            SetMaterials(inOtherWorld);
        }
        wasInFront = isInFront;
    }


    void Update()
    {
        //WhileCameraColliding();
    }
}
