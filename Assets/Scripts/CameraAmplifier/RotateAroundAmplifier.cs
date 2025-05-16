using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundAmplifier : MonoBehaviour
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

    // waiting for tracking
    void LateUpdate()
    {
        Vector3 anchorPos = rotationAnchor.transform.position;
        Quaternion currentCamRot = mainCamera.transform.localRotation;
        Quaternion amplifiedRot = QuaternionMult(currentCamRot, rotAmplifier - 1);
        // Headset und Controller entsprechend rotieren
        rotateObjectAroundObject(mainCamera.gameObject, anchorPos, amplifiedRot);
        rotateObjectAroundObject(leftController, anchorPos, amplifiedRot);
        rotateObjectAroundObject(lCon, anchorPos, amplifiedRot);
        rotateObjectAroundObject(rightController, anchorPos, amplifiedRot);
        rotateObjectAroundObject(rCon, anchorPos, amplifiedRot);
    }
    // Rotation um alle Achsen mit RotateAround damit kein Gimbal Lock entsteht 
    void rotateObjectAroundObject(GameObject go, Vector3 anchor, Quaternion angle)
    {
        go.transform.RotateAround(anchor, Vector3.right, angle.eulerAngles.x);
        go.transform.RotateAround(anchor, Vector3.up, angle.eulerAngles.y);
        go.transform.RotateAround(anchor, Vector3.forward, angle.eulerAngles.z);
    }

    // mit Quaternion rechnen da EulerAngles 
    private Quaternion QuaternionMult(Quaternion rot, float amplifier)
    {
        // Quaternion zerlegen
        float angle = Mathf.Acos(rot.w) * 2.0f;
        Vector3 axis = new Vector3(rot.x, rot.y, rot.z).normalized;
        float amplifiedAngle = amplifier * angle;
        Quaternion amplifiedQuaternion = new Quaternion(
            axis.x * Mathf.Sin(amplifiedAngle / 2.0f),
            axis.y * Mathf.Sin(amplifiedAngle / 2.0f),
            axis.z * Mathf.Sin(amplifiedAngle / 2.0f),
            Mathf.Cos(amplifiedAngle / 2.0f)
        );
        return amplifiedQuaternion;
    }
}
