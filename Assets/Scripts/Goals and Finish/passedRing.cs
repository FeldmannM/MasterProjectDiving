using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passedRing : MonoBehaviour
{
    [SerializeField]
    private GameObject ring;
    private Color colorGreen = Color.green;

    // Ring durchquert
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == ring.layer)
        {
            ChangeColor();
        }
        Debug.Log("Hit");
    }


    // Farbe des Rings und seiner Befestigung auf Grün ändern beim Durchqueren
    private void ChangeColor()
    {
        for (int i = 0; i < 2; i++)
        {
            Renderer renderer = ring.transform.GetChild(i).GetComponent<Renderer>();
            if(!renderer.material.color.Equals(colorGreen))
            {
                renderer.material.color = colorGreen;
                // Ring als passiert markieren fürs Zielüberwachungs-Skript
                if(i == 0)
                {
                    GameObject.Find("Goals").GetComponent<finishParkour>().addGoal();
                }
            }
        }
    }
}
