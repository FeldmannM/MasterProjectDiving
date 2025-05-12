using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnTurtles : MonoBehaviour
{
    // Dieses Skript ist noch vom altem Projekt
    [SerializeField]
    private GameObject spawnField;
    [SerializeField]
    private GameObject turtles;

    void OnTriggerEnter(Collider other)
    {
        turtles.SetActive(true);
    }
}
