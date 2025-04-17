using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class divingMidDir : MonoBehaviour
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
    private float acceleration = 5f;
    [SerializeField]
    private float deceleration = 0.75f;
    [SerializeField]
    private float movementThreshold = 0.001f;
    [SerializeField]
    private float separationFactor = 0.2f;
    [SerializeField]
    private int posListLimit = 10;
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

    private float lastLeftDistRefPoint;
    private float lastRightDistRefPoint;
    private float leftDistRefPoint;
    private float rightDistRefPoint;
    private float conSeparation;
    private float refPointPenalty;

    private List<Vector3> lastLeftPosList = new List<Vector3>();
    private List<Vector3> lastRightPosList = new List<Vector3>();
    private Vector3 currentLeftConLocalPos;
    private Vector3 currentRightConLocalPos;

    private useSensor sensor;

    // Start is called before the first frame update
    void Start()
    {
        if (leftConPos != null && rightConPos != null)
        {
            lastLeftConPos = lCon.transform.position;
            lastRightConPos = rCon.transform.position;
            lastLeftDistRefPoint = Vector3.Distance(lastLeftConPos, neckAnchor.transform.position);
            lastRightDistRefPoint = Vector3.Distance(lastRightConPos, neckAnchor.transform.position);
            vibration = locomotion.GetComponent<Vibration>();
        }
        sensor = breathe.GetComponent<useSensor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (leftConPos != null && rightConPos != null)
        {
            currentLeftConLocalPos = leftConPos.action.ReadValue<Vector3>();
            currentRightConLocalPos = rightConPos.action.ReadValue<Vector3>();
            currentLeftConPos = lCon.transform.position;
            currentRightConPos = rCon.transform.position;

            // Distanzberechnung
            leftDistRefPoint = Vector3.Distance(currentLeftConPos, neckAnchor.transform.position);
            rightDistRefPoint = Vector3.Distance(currentRightConPos, neckAnchor.transform.position);
            conSeparation = Vector3.Distance(currentLeftConPos, currentRightConPos);
            refPointPenalty = Mathf.Abs(leftDistRefPoint - rightDistRefPoint);

            // Mittlerer Vektor
            Vector3 midCons = (lCon.transform.position + rCon.transform.position) / 2;
            Vector3 underCam = neckAnchor.transform.position;
            Vector3 middleDirection = midCons - underCam;
            //middleDirection.y = 0f;
            middleDirection = middleDirection.normalized;

            Debug.DrawLine(underCam, midCons, Color.red, 0.2f);
            Debug.DrawRay(locomotion.transform.position, middleDirection * 2, Color.blue, 0.1f);

            if (leftDistRefPoint < lastLeftDistRefPoint - movementThreshold && rightDistRefPoint < lastRightDistRefPoint - movementThreshold)
            {
                float effectiveSeparation = conSeparation - refPointPenalty;
                effectiveSeparation = Mathf.Max(0, effectiveSeparation * separationFactor);
                float effectiveDist = (lastLeftDistRefPoint - leftDistRefPoint + lastRightDistRefPoint - rightDistRefPoint) / 2;
                effectiveDist *= 400;

                currentSpeed += (acceleration * effectiveSeparation * effectiveDist) * Time.deltaTime;

                // Geringer Bewegungsraum
                if (IsPosNearPosInList(lastLeftPosList, currentLeftConLocalPos, 0.15f) || IsPosNearPosInList(lastRightPosList, currentRightConLocalPos, 0.15f))
                {
                    currentSpeed -= 5f * deceleration * Time.deltaTime;
                }
                UpdateList(lastLeftPosList, currentLeftConLocalPos);
                UpdateList(lastRightPosList, currentRightConLocalPos);

                currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

                Vector3 movement = middleDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
                sensor.currentLocomotionState = 2;
            }
            else if (leftDistRefPoint > lastLeftDistRefPoint + movementThreshold && rightDistRefPoint > lastRightDistRefPoint + movementThreshold)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);

                Vector3 movement = middleDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
                sensor.currentLocomotionState = 1;
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);

                Vector3 movement = middleDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
                sensor.currentLocomotionState = 0;
            }

            lastLeftConPos = currentLeftConPos;
            lastRightConPos = currentRightConPos;
            lastLeftDistRefPoint = leftDistRefPoint;
            lastRightDistRefPoint = rightDistRefPoint;
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

    private void UpdateList(List<Vector3> posList, Vector3 pos)
    {
        posList.Add(pos);
        if (posList.Count > posListLimit)
        {
            posList.RemoveAt(0);
        }
    }
    private bool IsPosNearPosInList(List<Vector3> posList, Vector3 currentPos, float threshold)
    {
        foreach (Vector3 pos in posList)
        {
            if (Vector3.Distance(pos, currentPos) >= threshold)
            {
                return false;
            }
        }
        return true;
    }

}
