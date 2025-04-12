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
    private GameObject locomotionBase;
    [SerializeField]
    private GameObject camOffset;
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
        Vector3 anchorRot = new Vector3(0, 0, 0);
        Quaternion camRot = mainCamera.transform.localRotation;
        if (locomotionBase.transform.localRotation.eulerAngles.x == 90)
        {
            Debug.Log("X: " + camRot.eulerAngles.x);
            Vector3 currentCamRot = camRot.eulerAngles;
            Debug.Log("OLDROT: " + currentCamRot.z);

            Vector3 target = camOffset.transform.forward;
            target = Quaternion.Euler(-90f, camRot.eulerAngles.y, 0) * target;
            float alignment = Vector3.Dot(mainCamera.transform.forward, target);
            Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 5, Color.green, 0.1f);
            Debug.DrawRay(mainCamera.transform.position, target * 5, Color.yellow, 0.1f);

            if (alignment > 0)
            //if(currentCamRot.x > -90f)
            {
                
                Debug.Log("Größer");
                
                float normalizedZ = currentCamRot.z < 180 ? currentCamRot.z : currentCamRot.z;
                Debug.Log("NEWROT1: " + normalizedZ);
                float amplifiedCamRot = (rotAmplifier - 1f) * normalizedZ;
                //amplifiedCamRot = Mathf.Clamp(amplifiedCamRot, -180f, 180f);
                Debug.Log("NEWROT2: " + amplifiedCamRot);
                anchorRot.z = amplifiedCamRot;
                
            }
            //else if(currentCamRot.x < -90f)
            else if (alignment < 0)
            {
                
                Debug.Log("Kleiner");
                
                float normalizedZ = currentCamRot.z < 180 ? currentCamRot.z - 180f: currentCamRot.z + 180f;
                Debug.Log("NEWROT1: " + normalizedZ);
                float amplifiedCamRot = (rotAmplifier - 1f) * normalizedZ;
                //amplifiedCamRot = Mathf.Clamp(amplifiedCamRot, -180f, 180f);
                Debug.Log("NEWROT2: " + amplifiedCamRot);
                anchorRot.z = amplifiedCamRot;
                
            }

        }
        else
        {
            Vector3 currentCamRot = camRot.eulerAngles;
            float normalizedY = currentCamRot.y > 180 ? currentCamRot.y - 360f : currentCamRot.y;
            float amplifiedCamRot = (rotAmplifier - 1f) * normalizedY;
            amplifiedCamRot = Mathf.Clamp(amplifiedCamRot, -180f, 180f);
            anchorRot.y = amplifiedCamRot;
        }
        rotationAnchor.transform.localRotation = Quaternion.Euler(anchorRot);

        /*
        Quaternion leftConRot = leftController.transform.rotation;
        leftController.transform.RotateAround(anchorPos, Vector3.up, rotAmplifier * leftConRot.eulerAngles.y);
        Quaternion lConRot = lCon.transform.rotation;
        lCon.transform.RotateAround(anchorPos, Vector3.up, rotAmplifier * lConRot.eulerAngles.y);
        Quaternion rightConRot = rightController.transform.rotation;
        rightController.transform.RotateAround(anchorPos, Vector3.up, rotAmplifier * rightConRot.eulerAngles.y);
        Quaternion rConRot = rCon.transform.rotation;
        rCon.transform.RotateAround(anchorPos, Vector3.up, rotAmplifier * rConRot.eulerAngles.y);
        */
    }
}
