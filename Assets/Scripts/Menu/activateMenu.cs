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
    private InputActionProperty rightP;
    [SerializeField]
    private List<GameObject> levers;
    [SerializeField]
    private List<Vector3> leverRotations;

    private bool isMoving = false;
    private bool isVisible = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving && rightP.action.WasPerformedThisFrame())
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
                menuT.position = Vector3.Lerp(startPos, targetPos, 1 - Mathf.Pow(1 - t, 2));
            }
            else
            {
                menuT.position = Vector3.Lerp(startPos, targetPos, Mathf.Pow(t, 2));
            }
            passedTime += Time.deltaTime;
            yield return null;
        }
        menuT.position = targetPos;
    }
}
