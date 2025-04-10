using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneAxisRotation : MonoBehaviour
{
    [SerializeField]
    private GameObject rotationAnchor;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject leftController;
    [SerializeField]
    private GameObject lCon;
    [SerializeField]
    private GameObject rightController;
    [SerializeField]
    private GameObject rCon;
    [SerializeField, Range(1.0f, 2.0f)]
    private float rotAmplifier = 1.5f;


    void LateUpdate()
    {
        Vector3 anchorPos = rotationAnchor.transform.position;
        Vector3 currentCamRot = mainCamera.transform.eulerAngles;
        Debug.Log("Kamera: " + currentCamRot.y);
        float normalizedY = currentCamRot.y > 180 ? currentCamRot.y - 360f : currentCamRot.y;
        float amplifiedCamRot = rotAmplifier * normalizedY;
        amplifiedCamRot = Mathf.Repeat(amplifiedCamRot, 360f);
        Debug.Log("Amplified: " + amplifiedCamRot);
        currentCamRot.y = amplifiedCamRot;
        mainCamera.transform.eulerAngles = currentCamRot;

        
        Quaternion leftConRot = leftController.transform.rotation;
        leftController.transform.RotateAround(anchorPos, Vector3.up, rotAmplifier * leftConRot.eulerAngles.y);
        Quaternion lConRot = lCon.transform.rotation;
        lCon.transform.RotateAround(anchorPos, Vector3.up, rotAmplifier * lConRot.eulerAngles.y);
        Quaternion rightConRot = rightController.transform.rotation;
        rightController.transform.RotateAround(anchorPos, Vector3.up, rotAmplifier * rightConRot.eulerAngles.y);
        Quaternion rConRot = rCon.transform.rotation;
        rCon.transform.RotateAround(anchorPos, Vector3.up, rotAmplifier * rConRot.eulerAngles.y);
    }
}
