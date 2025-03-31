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
        childCount = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {
        if(passedGoals == childCount)
        {
            finish.SetActive(true);
            finish.transform.position = locomotion.transform.position;
        }
    }
    public void addGoal()
    {
        passedGoals += 1;
    }
}
