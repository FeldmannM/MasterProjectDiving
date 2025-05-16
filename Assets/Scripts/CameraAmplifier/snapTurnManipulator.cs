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
        // Der normale Snap Turn des XRI Paketes die y-Achse dreht, ich aber im Liegen eine Rotation der z-Achse benötige, existier ein Kind-Objekt mit diesem Skript
        if(locomotionBase.transform.localRotation.eulerAngles.x == 90)
        {
            // Überprüfe die aktuelle Rotation des XROrigin-Objekts und drehe sein Kind entlang der y-Rotation wieder zurück und drehe es dann um diese Rotation bei z
            Vector3 currentXRRot = xrorigin.transform.localRotation.eulerAngles;
            Vector3 newRot = new Vector3(0, -currentXRRot.y, -currentXRRot.y);
            turnManipulator.transform.localRotation = Quaternion.Euler(newRot);
            xrorigin.transform.localPosition = new Vector3(0, 0.125f, 0);
        }
        // Beim Sitzen soll keine zusätzliche Rotation verwendet werden
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
