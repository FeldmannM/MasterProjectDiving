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
    private AudioSource audioS;
    [SerializeField]
    private int passedGoals = 0;
    private int childCount = 0;
    private bool end = false;

    // Start is called before the first frame update
    void Start()
    {
        // Anzahl der Tore zum Durchtauchen
        childCount = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        // Alle Tore durchtaucht und damit Ende abspielen
        if(passedGoals == childCount && !end)
        {
            finish.SetActive(true);
            finish.transform.position = locomotion.transform.position;
            audioS.Play();
            end = true;
        }
    }
    // wenn ein Tor durchquert Z�hler erh�hen
    public void addGoal()
    {
        passedGoals += 1;
    }
}
