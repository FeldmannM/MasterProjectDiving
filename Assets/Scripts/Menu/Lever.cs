using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField]
    HingeJoint hinge;
    private float prevAngle;
    [SerializeField]
    private GameObject HandL;
    [SerializeField]
    private GameObject HandR;
    [SerializeField]
    private GameObject DivingFinL;
    [SerializeField]
    private GameObject DivingFinR;
    [SerializeField]
    private GameObject locomotionBase;
    private changeDiveMode divingMode;
    [SerializeField]
    private bool firstStart = true;
    [SerializeField]
    private bool PosLever = false;
    [SerializeField]
    private bool MethodLever = false;
    [SerializeField]
    private bool DirLever = false;


    // Start is called before the first frame update
    void Start()
    {
        if (firstStart)
        {
            Debug.Log("First Start");
            transform.rotation = Quaternion.AngleAxis(hinge.limits.min, transform.TransformDirection(hinge.axis)) * transform.rotation;
            prevAngle = hinge.limits.min;
            divingMode = locomotionBase.GetComponent<changeDiveMode>();
            firstStart = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if((Mathf.Abs(hinge.angle - prevAngle) > 0.001f) && hinge.angle != 0)
        {
            if(hinge.angle < 0)
            {
                if (PosLever)
                {
                    if(prevAngle > 0)
                    {
                        Quaternion newRot = Quaternion.Euler(0f, 0f, 0f);
                        prevAngle = hinge.angle;
                        locomotionBase.GetComponent<activateMenu>().recalculateMenuPos(newRot, 0);
                    }
                }
                else if (MethodLever){
                    if(divingMode.currentIndex == 2 || divingMode.currentIndex == 3)
                    {   
                        divingMode.scripts[divingMode.currentIndex].enabled = false;
                        divingMode.scripts[divingMode.currentIndex - 2].enabled = true;
                        divingMode.currentIndex -= 2;
                    }
                }
                else if (DirLever)
                {
                    if (divingMode.currentIndex == 1 || divingMode.currentIndex == 3)
                    {
                        divingMode.scripts[divingMode.currentIndex].enabled = false;
                        divingMode.scripts[divingMode.currentIndex - 1].enabled = true;
                        divingMode.currentIndex -= 1;
                    }
                }
            }
            else
            {
                if (PosLever)
                {
                    if (prevAngle < 0)
                    {
                        Quaternion newRot = Quaternion.Euler(90f, 0f, 0f);
                        prevAngle = hinge.angle;
                        locomotionBase.GetComponent<activateMenu>().recalculateMenuPos(newRot, 1);
                    }
                }
                else if (MethodLever)
                {
                    if (divingMode.currentIndex == 0 || divingMode.currentIndex == 1)
                    {
                        divingMode.scripts[divingMode.currentIndex].enabled = false;
                        divingMode.scripts[divingMode.currentIndex + 2].enabled = true;
                        divingMode.currentIndex += 2; 
                    }
                }
                else if (DirLever)
                {
                    if (divingMode.currentIndex == 0 || divingMode.currentIndex == 2)
                    {
                        divingMode.scripts[divingMode.currentIndex].enabled = false;
                        divingMode.scripts[divingMode.currentIndex + 1].enabled = true;
                        divingMode.currentIndex += 1;
                    }
                }
            }
            prevAngle = hinge.angle;
        }
    }

    public void setMenuStatus(bool activeStatus)
    {
        if (!activeStatus)
        {
            if (divingMode.currentIndex == 2 || divingMode.currentIndex == 3)
            {
                HandL.SetActive(false);
                HandR.SetActive(false);
                DivingFinL.SetActive(true);
                DivingFinR.SetActive(true);
            }
        }
    }
}
