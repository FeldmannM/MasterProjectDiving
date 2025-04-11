using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class activateMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject locomotionBase;
    [SerializeField]
    private InputActionProperty rightP;
    [SerializeField]
    private InputActionProperty rightGrab;
    [SerializeField]
    private InputActionProperty leftGrab;
    [SerializeField]
    private List<GameObject> levers;
    [SerializeField]
    private List<Vector3> leverRotations;
    private Coroutine activeCoroutine;

    private bool isMoving = false;
    private bool isVisible = false;
    private int posLever = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving && rightP.action.WasPerformedThisFrame() && !rightGrab.action.IsPressed() && !leftGrab.action.IsPressed())
        {
            if (!isVisible)
            {
                StartCoroutine(MenuDown());
            }
            else
            {
                StartCoroutine(MenuUp());
            }
        }
    }

    IEnumerator MenuDown()
    {
        isMoving = true;
        menu.SetActive(true);
        Vector3 forwardXZ = mainCamera.transform.forward;
        forwardXZ.y = 0;
        Vector3 menuPos = mainCamera.transform.position + forwardXZ.normalized * 0.75f;
        Vector3 spawnPos = menuPos + Vector3.up * 50f;
        menu.transform.position = spawnPos;
        Vector3 lookToCamera = menuPos - mainCamera.transform.position;
        lookToCamera.y = 0;
        menu.transform.rotation = Quaternion.LookRotation(lookToCamera);
        yield return StartCoroutine(MoveToPos(menu.transform, menuPos, true));
        isVisible = true;
        isMoving = false;
        for (int i = 0; i < leverRotations.Count; i++)
        {
            levers[i].SetActive(true);
            levers[i].transform.localRotation = Quaternion.Euler(leverRotations[i]);
            levers[i].GetComponent<Lever>().enabled = true;
        }
    }

    IEnumerator MenuUp()
    {
        for (int i = 0; i < leverRotations.Count; i++)
        {
            leverRotations[i] = levers[i].transform.localRotation.eulerAngles;
            levers[i].GetComponent<Lever>().enabled = false;
            levers[i].transform.localRotation = Quaternion.Euler(new Vector3(0,180,0));
            levers[i].SetActive(false);
        }
        isMoving = true;
        Vector3 endPos = menu.transform.position + Vector3.up * 50f;
        yield return StartCoroutine(MoveToPos(menu.transform, endPos, true));
        isMoving = false;
        menu.SetActive(false);
        isVisible = false;
    }


    IEnumerator MoveToPos(Transform menuT, Vector3 targetPos, bool moveDown)
    {
        float passedTime = 0f;
        Vector3 startPos = menuT.position;
        while (passedTime < 3f)
        {
            float t = passedTime / 3f;
            if (moveDown)
            {
                menuT.position = Vector3.Lerp(startPos, targetPos, 1 - Mathf.Pow(1 - t, 3));
            }
            else
            {
                menuT.position = Vector3.Lerp(startPos, targetPos, Mathf.Pow(t, 3f));
            }
            passedTime += Time.deltaTime;
            yield return null;
        }
        menuT.position = targetPos;
    }

    public void recalculateMenuPos(Quaternion newRot, int newLeverPos)
    {
        if(activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
        activeCoroutine = StartCoroutine(RecalculatePos(newRot, newLeverPos));
    }
    IEnumerator RecalculatePos(Quaternion newRot, int newLeverPos)
    {
        isMoving = true;

        while (rightGrab.action.IsPressed() || leftGrab.action.IsPressed())
        {
            yield return null;
        }

        if(newLeverPos != posLever)
        {
            posLever = newLeverPos;
            locomotionBase.transform.rotation = newRot;

            for (int i = 0; i < leverRotations.Count; i++)
            {
                leverRotations[i] = levers[i].transform.localRotation.eulerAngles;
                levers[i].GetComponent<Lever>().enabled = false;
                levers[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                levers[i].SetActive(false);
            }
            Vector3 forwardXZ = mainCamera.transform.forward;
            forwardXZ.y = 0;
            menu.transform.position = mainCamera.transform.position + forwardXZ.normalized * 0.75f;
            Vector3 lookToCamera = menu.transform.position - mainCamera.transform.position;
            lookToCamera.y = 0;
            menu.transform.rotation = Quaternion.LookRotation(lookToCamera);
            levers[0].transform.localPosition = new Vector3 (-0.3f, 0f, -0.01f);
            for (int i = 0; i < leverRotations.Count; i++)
            {
                levers[i].SetActive(true);
                levers[i].transform.localRotation = Quaternion.Euler(leverRotations[i]);
                levers[i].GetComponent<Lever>().enabled = true;
            }
        }

        isMoving = false;
    }
}
