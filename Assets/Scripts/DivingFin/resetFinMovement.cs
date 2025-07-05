using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class resetFinMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject first;
    [SerializeField]
    private GameObject _2nd;
    [SerializeField]
    private GameObject middle;
    [SerializeField]
    private GameObject _4th;
    [SerializeField]
    private GameObject end;
    [SerializeField]
    private RigBuilder rigBuilder;

    [SerializeField]
    private GameObject rotObject;
    private Quaternion lastRot;

    [SerializeField]
    private int counter;

    // Setze die Knochen der Fins auf die ursprüngliche Position zurück wenn sie wieder aktiviert werden
    void OnEnable()
    {
        lastRot = rotObject.transform.rotation;
        counter = 0;
        resetPos();
    }

    void Update()
    {
        Quaternion currentRot = rotObject.transform.rotation;
        if(currentRot != lastRot)
        {
            resetPos();
            lastRot = currentRot;
        }
    }

    IEnumerator WaitFramesForRig()
    {
        counter += 1;
        int id = counter;
        // Für 100 Frames warten
        for (int i = 0; i < 100; i++)
        {
            yield return null;
        }
        // Nur bei der letzten laufenden Coroutine ausführen
        if(id == counter)
        {
            rigBuilder.enabled = true;
        }
    }
    // Position der Knochen der Flossen zurücksetzen
    private void resetPos()
    {
        rigBuilder.enabled = false;
        first.transform.localPosition = new Vector3(8.08223704e-05f, 0.00166477845f, -0.0193978734f);
        first.transform.localRotation = Quaternion.Euler(86.8956223f, 156.887146f, 154.653473f);
        _2nd.transform.localPosition = new Vector3(-9.31322554e-12f, 0.00820959173f, -2.27009869e-11f);
        _2nd.transform.localRotation = Quaternion.Euler(357.368927f, 359.219116f, 3.22228599f);
        middle.transform.localPosition = new Vector3(-2.79396766e-11f, 0.024274895f, 8.14634402e-11f);
        middle.transform.localRotation = Quaternion.Euler(358.912445f, 0.591966271f, 357.226471f);
        _4th.transform.localPosition = new Vector3(-1.62981451e-11f, 0.0195471141f, 2.44472168e-11f);
        _4th.transform.localRotation = Quaternion.Euler(357.972321f, 0.269909561f, 1.01868272f);
        end.transform.localPosition = new Vector3(6.98491889e-11f, 0.0228941087f, -8.61473393e-11f);
        end.transform.localRotation = Quaternion.Euler(0.956116796f, 1.53507817f, 0.219536155f);
        StartCoroutine(WaitFramesForRig());
    }
}
