using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passedGate : MonoBehaviour
{
    [SerializeField]
    private GameObject gate;
    private Color colorGreen = Color.green;

    // Tor durchquert
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == gate.layer)
        {
            ChangeColor();
        }
        Debug.Log("Hit");
    }

    // Farbe des Tors auf Gr�n �ndern beim Durchqueren
    private void ChangeColor()
    {
        for(int i = 0; i < 4; i++)
        {
            Renderer renderer = gate.transform.GetChild(i).GetComponent<Renderer>();
            renderer.material.color = colorGreen;
        }
    }

}
