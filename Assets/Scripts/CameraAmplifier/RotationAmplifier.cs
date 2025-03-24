using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAmplifier : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject cameraParent;
    [SerializeField]
    private float amplifier = 1.0f;

    // Update is called once per frame
    void Update()
    {
        Quaternion camRot = mainCamera.transform.localRotation;
        Vector3 camEA = camRot.eulerAngles;
        //Debug.Log("CAM_EA:" + camEA);
        Vector3 correctedEA = new Vector3(
            camEA.x > 180 ? camEA.x - 360 : camEA.x,
            camEA.y > 180 ? camEA.y - 360 : camEA.y,
            camEA.z > 180 ? camEA.z - 360 : camEA.z
        );
        Vector3 ampliefiedEA = amplifier * correctedEA;
        //Debug.Log("AMP_EA:" + ampliefiedEA);
        cameraParent.transform.localRotation = Quaternion.Euler(ampliefiedEA);
    }
}
