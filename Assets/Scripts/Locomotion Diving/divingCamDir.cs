using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class divingCamDir : MonoBehaviour
{
    [SerializeField]
    private GameObject locomotion;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject refPoint;
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
    private GameObject lCon;
    [SerializeField]
    private GameObject rCon;
    [SerializeField]
    private InputActionReference leftConPos;
    [SerializeField]
    private InputActionReference rightConPos;

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
            lastLeftDistRefPoint = Vector3.Distance(lastLeftConPos, refPoint.transform.position);
            lastRightDistRefPoint = Vector3.Distance(lastRightConPos, refPoint.transform.position);
            vibration = locomotion.GetComponent<Vibration>();
        }
        sensor = breathe.GetComponent<useSensor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(leftConPos != null && rightConPos != null)
        {
            currentLeftConLocalPos = leftConPos.action.ReadValue<Vector3>();
            currentRightConLocalPos = rightConPos.action.ReadValue<Vector3>();
            currentLeftConPos = lCon.transform.position;
            currentRightConPos = rCon.transform.position;

            // Distanzberechnung
            leftDistRefPoint = Vector3.Distance(currentLeftConPos, refPoint.transform.position);
            rightDistRefPoint = Vector3.Distance(currentRightConPos, refPoint.transform.position);
            conSeparation = Vector3.Distance(currentLeftConPos, currentRightConPos);
            refPointPenalty = Mathf.Abs(leftDistRefPoint - rightDistRefPoint);
            
            // Falls sich der Abstand beider Controller zum Nutzer verringert: Beschleunigungs-Phase
            if(leftDistRefPoint < lastLeftDistRefPoint - movementThreshold && rightDistRefPoint < lastRightDistRefPoint - movementThreshold)
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

                Vector3 forwardDirection = mainCamera.transform.forward;
                Vector3 movement = forwardDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;

                sensor.currentLocomotionState = 2;
            }
            // Falls sich der Abstand beider Controller zum Nutzer vergrößert: Vorbereitungs-Phase mit Abbremsen
            else if (leftDistRefPoint > lastLeftDistRefPoint + movementThreshold && rightDistRefPoint > lastRightDistRefPoint + movementThreshold)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);

                Vector3 forwardDirection = mainCamera.transform.forward;
                Vector3 movement = forwardDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
                sensor.currentLocomotionState = 1;
            }
            // Sonst: Abbremsen
			else
            {
				currentSpeed -= deceleration * Time.deltaTime;
				currentSpeed = Mathf.Max(currentSpeed, 0f);
				
				Vector3 forwardDirection = mainCamera.transform.forward;
                Vector3 movement = forwardDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
                sensor.currentLocomotionState = 0;
            }	

            lastLeftConPos = currentLeftConPos;
            lastRightConPos = currentRightConPos;
            lastLeftDistRefPoint = leftDistRefPoint;
            lastRightDistRefPoint = rightDistRefPoint;
            //Debug.Log("Speed: " + currentSpeed);

            // Bei gewisser Geschwindigkeit Controller vibrieren lassen
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
    // Liste der letzten gespeicherten Positionen
    private void UpdateList(List<Vector3> posList, Vector3 pos)
    {
        posList.Add(pos);
        if(posList.Count > posListLimit)
        {
            posList.RemoveAt(0);
        }
    }
    // Liste für den letzten Bewegungsraum
    private bool IsPosNearPosInList(List<Vector3> posList, Vector3 currentPos, float threshold)
    {
        foreach (Vector3 pos in posList)
        {
            if(Vector3.Distance(pos, currentPos) >= threshold)
            {
                return false;
            }
        }
        return true;
    }
}
