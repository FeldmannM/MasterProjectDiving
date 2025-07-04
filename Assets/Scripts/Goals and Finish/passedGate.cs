using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passedGate : MonoBehaviour
{
    [SerializeField]
    private GameObject gate;
    [SerializeField]
    private AudioSource audioS;
    private Color colorGreen = Color.green;

    // Tor durchquert
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == gate.layer)
        {
            ChangeColor();
        }
        //Debug.Log("Hit");
    }

    // Farbe des Tors auf Grün ändern beim Durchqueren
    private void ChangeColor()
    {
        for(int i = 0; i < 4; i++)
        {
            Renderer renderer = gate.transform.GetChild(i).GetComponent<Renderer>();
            if (!renderer.material.color.Equals(colorGreen))
            {
                renderer.material.color = colorGreen;
                if (i == 0)
                {
                    audioS.Play();
                }
            }
        }
    }

}
