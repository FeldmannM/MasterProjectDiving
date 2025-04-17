using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class divingWithFeetMidDir : MonoBehaviour
{
    [SerializeField]
    private GameObject locomotion;
    [SerializeField]
    private GameObject neckAnchor;
    [SerializeField]
    private GameObject breathe;
    [SerializeField]
    private float maxSpeed = 1.5f;
    [SerializeField]
    private float acceleration = 2f;
    [SerializeField]
    private float deceleration = 0.75f;
    [SerializeField]
    private float movementThreshold = 0.001f;
    [SerializeField]
    private InputActionReference leftConPos;
    [SerializeField]
    private InputActionReference rightConPos;
    [SerializeField]
    private GameObject lCon;
    [SerializeField]
    private GameObject rCon;

    private Vibration vibration;

    private Vector3 lastLeftConPos;
    private Vector3 lastRightConPos;
    private Vector3 currentLeftConPos;
    private Vector3 currentRightConPos;
    private float currentSpeed;

    private useSensor sensor;

    // Start is called before the first frame update
    void Start()
    {
        if (leftConPos != null && rightConPos != null)
        {
            lastLeftConPos = locomotion.transform.rotation * leftConPos.action.ReadValue<Vector3>();
            lastRightConPos = locomotion.transform.rotation * rightConPos.action.ReadValue<Vector3>();
            vibration = locomotion.GetComponent<Vibration>();
        }
        sensor = breathe.GetComponent<useSensor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (leftConPos != null && rightConPos != null)
        {
            currentLeftConPos = locomotion.transform.rotation * leftConPos.action.ReadValue<Vector3>();
            currentRightConPos = locomotion.transform.rotation * rightConPos.action.ReadValue<Vector3>();

            Vector3 midCons = (lCon.transform.position + rCon.transform.position) / 2;
            Vector3 underCam = neckAnchor.transform.position;
            Vector3 middleDirection = midCons - underCam;
            //middleDirection.y = 0f;
            middleDirection = middleDirection.normalized;

            Debug.DrawLine(underCam, midCons, Color.red, 0.2f);
            Debug.DrawRay(locomotion.transform.position, middleDirection * 2, Color.blue, 0.1f);

            if ((currentLeftConPos.y < lastLeftConPos.y - movementThreshold && currentRightConPos.y < lastRightConPos.y - movementThreshold)
                || (currentLeftConPos.y < lastLeftConPos.y - movementThreshold && currentRightConPos.y > lastRightConPos.y + movementThreshold)
                || (currentLeftConPos.y > lastLeftConPos.y + movementThreshold && currentRightConPos.y < lastRightConPos.y - movementThreshold))
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

                Vector3 movement = middleDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);

                Vector3 movement = middleDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
            }

            sensor.currentLocomotionState = 0;

            lastLeftConPos = currentLeftConPos;
            lastRightConPos = currentRightConPos;
            //Debug.Log(currentSpeed);

            float bIntensity = Mathf.Abs((currentSpeed / maxSpeed) * 0.25f);
            if (bIntensity > 0.025f)
            {
                vibration.activeVib = true;
                vibration.setAmplitude(bIntensity);
            }
            else
            {
                vibration.activeVib = false;
            }
        }
    }
}
