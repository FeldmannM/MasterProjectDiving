using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class changeDiveMode : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty rightP;
    [SerializeField]
    private InputActionProperty rightS;
    [SerializeField]
    private GameObject locomotionBase;
    [SerializeField]
    private List<MonoBehaviour> scripts;
    private int currentIndex = 0;

    private List<float> rotations = new List<float> { 0f, 90f, 180f, 270f };
    private int rotIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rightP.action.WasPerformedThisFrame())
        {
            rotIndex = (rotIndex + 1) % 4;
            float newRotX = rotations[rotIndex];
            Quaternion newRot = Quaternion.Euler(newRotX, 0f, 0f);
            locomotionBase.transform.rotation = newRot;
        }
        if (rightS.action.WasPerformedThisFrame())
        {
            scripts[currentIndex].enabled = false;
            currentIndex = (currentIndex + 1) % scripts.Count;
            scripts[currentIndex].enabled = true;
        }
    }
}
