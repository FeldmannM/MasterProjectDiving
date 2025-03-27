using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapTurnManipulator : MonoBehaviour
{
    [SerializeField]
    private GameObject locomotionBase;
    [SerializeField]
    private GameObject xrorigin;
    [SerializeField]
    private GameObject turnManipulator;

    // Update is called once per frame
    void Update()
    {
        if(locomotionBase.transform.localRotation.eulerAngles.x == 90)
        {
            Vector3 currentXRRot = xrorigin.transform.localRotation.eulerAngles;
            Vector3 newRot = new Vector3(0, -currentXRRot.y, -currentXRRot.y);
            turnManipulator.transform.localRotation = Quaternion.Euler(newRot);
            xrorigin.transform.localPosition = new Vector3(0, 0.125f, 0);
        }
        else if(turnManipulator.transform.localRotation != Quaternion.identity)
        {
            turnManipulator.transform.localRotation = Quaternion.identity;
        }
        else if (xrorigin.transform.localPosition != new Vector3(0, 0.125f, 0))
        {
            xrorigin.transform.localPosition = new Vector3(0, 0.125f, 0);
        }
    }
}
