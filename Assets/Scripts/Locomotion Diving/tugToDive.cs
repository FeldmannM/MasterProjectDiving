using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class tugToDive : MonoBehaviour
{
    [SerializeField]
    private GameObject locomotion;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private InputActionReference leftConPos;
    [SerializeField]
    private InputActionReference rightConPos;
    [SerializeField]
    private InputActionProperty leftGrip;
    [SerializeField]
    private InputActionProperty rightGrip;

    private Vibration vibration;

    private Vector3 initalLeftConPos;
    private Vector3 initalRightConPos;
    private Vector3 currentLeftConPos;
    private Vector3 currentRightConPos;

    // Start is called before the first frame update
    void Start()
    {
        if (leftConPos != null && rightConPos != null)
        {
            vibration = locomotion.GetComponent<Vibration>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (leftConPos != null && rightConPos != null)
        {
            currentLeftConPos = locomotion.transform.TransformPoint(leftConPos.action.ReadValue<Vector3>());
            currentRightConPos = locomotion.transform.TransformPoint(rightConPos.action.ReadValue<Vector3>());

            if (leftGrip.action.WasPressedThisFrame())
            {
                initalLeftConPos = locomotion.transform.TransformPoint(leftConPos.action.ReadValue<Vector3>());
            }
            if (rightGrip.action.WasPressedThisFrame())
            {
                initalRightConPos = locomotion.transform.TransformPoint(rightConPos.action.ReadValue<Vector3>());
            }
            
            if (leftGrip.action.WasReleasedThisFrame())
            {
                initalRightConPos = locomotion.transform.TransformPoint(rightConPos.action.ReadValue<Vector3>());
            }
            if (rightGrip.action.WasReleasedThisFrame())
            {
                initalLeftConPos = locomotion.transform.TransformPoint(leftConPos.action.ReadValue<Vector3>());
            }
            
            if (leftGrip.action.IsPressed() && rightGrip.action.IsPressed())
            {
                Vector3 averageDir = (initalLeftConPos - currentLeftConPos + initalRightConPos - currentRightConPos) / 2;
                locomotion.transform.position += averageDir;
            }
            else if (leftGrip.action.IsPressed())
            {
                Vector3 leftDir = initalLeftConPos - currentLeftConPos;
                locomotion.transform.position += leftDir;
            }
            else if (rightGrip.action.IsPressed())
            {
                Vector3 rightDir = initalRightConPos - currentRightConPos;
                locomotion.transform.position += rightDir;
            }
            
        }
    }
}