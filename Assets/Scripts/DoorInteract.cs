using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NFTPortal
{
    public class DoorInteract : MonoBehaviour
    {
        public bool open = false;
        public Animator anim;

        void Start()
        {
            //anim = GetComponent<Animator>();
            Debug.Log("GOT THE ANIMATOR");
        }

        void OnTriggerEnter(Collider other)

        {
            print("OnTriggerEnter" + other.name);
            if (other.CompareTag("MainCamera"))
            {
                anim.SetBool("open", true);
                Debug.Log("OPENING THE DOOR");
            }

        }

        //void OnTriggerExit(Collider other)

        //{
        //print("OnTriggerExit" + other.name);
        //if (other.CompareTag("MainCamera"))
        //    {
        //        anim.SetBool("open", false);
        //        Debug.Log("OPENING THE DOOR");
        //    }
        //}
    }
}
