using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setPosAfterAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject bone;
    private Vector3 originalPos;

    // Da die Schildkröte sich selbst während der Animation nach vorne bewegt und damit bei einer erneuten Durchführung im Loop sich wieder nach hinten teleportiert, hatte ich dieses Skript schon für das alte Projekt geschrieben um dir fehlerhafte Animation auszubaden

    void Start()
    {
        originalPos = bone.transform.localPosition;
    }

    

    void LateUpdate()
    {
        bone.transform.localPosition = originalPos;

    }
}
