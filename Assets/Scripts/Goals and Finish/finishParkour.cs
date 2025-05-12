using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finishParkour : MonoBehaviour
{
    [SerializeField]
    private GameObject finish;
    [SerializeField]
    private GameObject locomotion;
    [SerializeField]
    private int passedGoals = 0;
    private int childCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Anzahl der Tore zum Durchtauchen
        childCount = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        // Alle Tore durchtaucht
        if(passedGoals == childCount)
        {
            finish.SetActive(true);
            finish.transform.position = locomotion.transform.position;
        }
    }
    // wenn ein Tor durchquert Zähler erhöhen
    public void addGoal()
    {
        passedGoals += 1;
    }
}
