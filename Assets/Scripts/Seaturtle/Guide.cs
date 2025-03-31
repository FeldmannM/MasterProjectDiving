using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField]
    private GameObject turtle;
    [SerializeField]
    private List<Vector3> goals;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float rotSpeed = 20f;

    private int index = 0;
    private bool isRotating = false;

    // Update is called once per frame
    void Update()
    {
        // fluessige Bewegung
        if(index < goals.Count)
        {
            Vector3 direction = (goals[index] - turtle.transform.position).normalized;
            turtle.transform.position = Vector3.MoveTowards(turtle.transform.position, goals[index], speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, goals[index]) < 0.1f)
            {
                index++;
                if(index < goals.Count)
                {
                    StartCoroutine(RotateToGoal());
                }
                else
                {
                    turtle.SetActive(false);
                }
            }

            if (!isRotating)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                turtle.transform.rotation = Quaternion.RotateTowards(turtle.transform.rotation, rot, rotSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator RotateToGoal()
    {
        // Rotation in Richtung des naechsten Goals
        isRotating = true;
        if(index < goals.Count)
        {
            Vector3 direction = (goals[index] - turtle.transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(direction);
            while(Quaternion.Angle(transform.rotation, rot) > 0.1f)
            {
                turtle.transform.rotation = Quaternion.RotateTowards(turtle.transform.rotation, rot, rotSpeed * Time.deltaTime);
                yield return null;
            }
        }
        isRotating = false;
    }
}
