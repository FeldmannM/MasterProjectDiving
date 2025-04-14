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
    private GameObject refPointAnchor;
    [SerializeField]
    private GameObject refPoint;
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
        if (Mathf.Abs(locomotionBase.transform.localRotation.eulerAngles.x - 90f) < 1f)
        {
            Debug.Log("Liegend");
            rotationAnchor.transform.localRotation = Quaternion.identity;
            refPoint.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            refPointAnchor.transform.position = mainCamera.transform.position;
            Vector3 forwardRefPoint = refPoint.transform.forward;
            Vector3 forwardCam = mainCamera.transform.forward;
            forwardRefPoint.y = 0;
            forwardCam.y = 0;
            forwardRefPoint.Normalize();
            forwardCam.Normalize();
            Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 5f, Color.green, 0.1f);
            Debug.DrawRay(refPoint.transform.position, refPoint.transform.forward * 5f, Color.yellow, 0.1f);
            float angleY = - Vector3.SignedAngle(forwardRefPoint, forwardCam, Vector3.up);
            Debug.Log("Angle: " + angleY);
            float amplifiedY = (rotAmplifier - 1) * angleY;
            rotationAnchor.transform.rotation *= Quaternion.AngleAxis(amplifiedY, Vector3.forward);
        }
        else
        {
            Debug.Log("Sitzend");
            rotationAnchor.transform.localRotation = Quaternion.identity;
            refPoint.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            refPointAnchor.transform.position = mainCamera.transform.position;
            Vector3 forwardRefPoint = refPoint.transform.forward;
            Vector3 forwardCam = mainCamera.transform.forward;
            forwardRefPoint.y = 0;
            forwardCam.y = 0;
            forwardRefPoint.Normalize();
            forwardCam.Normalize();
            Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 5f, Color.green, 0.1f);
            Debug.DrawRay(refPoint.transform.position, refPoint.transform.forward * 5f, Color.yellow, 0.1f);
            float angleY = Vector3.SignedAngle(forwardRefPoint,forwardCam, Vector3.up);
            Debug.Log("Angle: " + angleY);
            float amplifiedY = (rotAmplifier - 1) * angleY;
            rotationAnchor.transform.rotation *= Quaternion.AngleAxis(amplifiedY, Vector3.up);
        }
    }
}
