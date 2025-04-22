using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class activeAmplifier : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraParent;
    [SerializeField]
    private InputActionProperty rightConSec;

    private bool isActive = false;

    // Update is called once per frame
    void Update()
    {
        if (rightConSec.action.WasPerformedThisFrame())
        {
            isActive = !isActive;
            cameraParent.GetComponent<OneAxisRotation>().enabled = isActive;
            if(isActive == false)
            {
                cameraParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
