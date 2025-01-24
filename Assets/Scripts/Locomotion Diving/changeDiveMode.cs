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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rightP.action.WasPerformedThisFrame())
        {
            Quaternion currentRot = locomotionBase.transform.rotation;
            float newRotX = 0f;
            if(currentRot.eulerAngles.x == 0f)
            {
                newRotX = 270f;
            }
            else if (currentRot.eulerAngles.x == 270f)
            {
                newRotX = 180f;
            }
            else if (currentRot.eulerAngles.x == 180f)
            {
                newRotX = 0f;
            }
            Quaternion newRot = Quaternion.Euler(newRotX, 90f, 0f);
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
