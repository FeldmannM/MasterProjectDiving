using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAmplifier : MonoBehaviour
{
    [SerializeField]
    private GameObject rotationAnchor;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField, Range(1.0f, 2.0f)]
    private float rotAmplifier = 1.5f;

    private Quaternion initCamRot;
    private Quaternion initRARot;

    // Start is called before the first frame update
    void Start()
    {
        initCamRot = mainCamera.transform.localRotation;
        initRARot = rotationAnchor.transform.localRotation;
    }

    // Warte aufs Tracking, war ein weiterer Versuch über Quaternions die Rotation an kritischen Stellen wie exakte 90 Grad Drehungen zu berechnen.
    void LateUpdate()
    {
        Quaternion currentCamRot = mainCamera.transform.localRotation;
        Quaternion deltaRot = Quaternion.Inverse(initCamRot) * currentCamRot;
        Quaternion amplifiedDeltaRot = QuaternionMult(deltaRot, rotAmplifier - 1f);
        Quaternion virtualCamRot = initCamRot * amplifiedDeltaRot;
        //Quaternion diffRot = Quaternion.Inverse(currentCamRot) * virtualCamRot;
        //rotationAnchor.transform.localRotation = initRARot * diffRot;
        rotationAnchor.transform.localRotation = initRARot * amplifiedDeltaRot;

        Debug.Log("Real Rotation: " + currentCamRot.eulerAngles);
        Debug.Log("Virtual Rotation: " + virtualCamRot.eulerAngles);
        //Debug.Log("RotationAnchor Adjustment: " + diffRot.eulerAngles);
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
