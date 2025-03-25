using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class activeAmplifier : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraParent;
    [SerializeField]
    private InputActionProperty leftSelect;

    private bool isActive = false;

    // Update is called once per frame
    void Update()
    {
        if (leftSelect.action.WasPerformedThisFrame())
        {
            isActive = !isActive;
            cameraParent.GetComponent<RotationAmplifier>().enabled = isActive;
            if(isActive == false)
            {
                cameraParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
