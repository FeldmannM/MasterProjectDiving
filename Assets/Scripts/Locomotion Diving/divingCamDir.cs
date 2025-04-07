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
    private int posListLimit = 50;
    [SerializeField]
    private InputActionReference leftConPos;
    [SerializeField]
    private InputActionReference rightConPos;
    [SerializeField]
    private InputActionProperty leftGrab;
    [SerializeField]
    private InputActionProperty rightGrab;

    private Vibration vibration;

    private Vector3 lastLeftConPos;
    private Vector3 lastRightConPos;
    private Vector3 currentLeftConPos;
    private Vector3 currentRightConPos;
	private float currentSpeed;

    [SerializeField]
    private float lastLeftDistRefPoint;
    [SerializeField]
    private float lastRightDistRefPoint;
    [SerializeField]
    private float leftDistRefPoint;
    [SerializeField]
    private float rightDistRefPoint;
    private float conSeparation;
    private float refPointPenalty;

    [SerializeField]
    private List<Vector3> lastLeftPosList = new List<Vector3>();
    [SerializeField]
    private List<Vector3> lastRightPosList = new List<Vector3>();
    private Vector3 currentLeftConLocalPos;
    private Vector3 currentRightConLocalPos;

    // Start is called before the first frame update
    void Start()
    {
        if (leftConPos != null && rightConPos != null)
        {
            lastLeftConPos = locomotion.transform.TransformPoint(leftConPos.action.ReadValue<Vector3>());
            lastRightConPos = locomotion.transform.TransformPoint(rightConPos.action.ReadValue<Vector3>());
            lastLeftDistRefPoint = Vector3.Distance(lastLeftConPos, refPoint.transform.position);
            lastRightDistRefPoint = Vector3.Distance(lastRightConPos, refPoint.transform.position);
            vibration = locomotion.GetComponent<Vibration>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(leftConPos != null && rightConPos != null)
        {
            currentLeftConLocalPos = leftConPos.action.ReadValue<Vector3>();
            currentRightConLocalPos = rightConPos.action.ReadValue<Vector3>();
            currentLeftConPos = locomotion.transform.TransformPoint(leftConPos.action.ReadValue<Vector3>());
            currentRightConPos = locomotion.transform.TransformPoint(rightConPos.action.ReadValue<Vector3>());

            // Distanzberechnung
            leftDistRefPoint = Vector3.Distance(currentLeftConPos, refPoint.transform.position);
            rightDistRefPoint = Vector3.Distance(currentRightConPos, refPoint.transform.position);
            conSeparation = Vector3.Distance(currentLeftConPos, currentRightConPos);
            refPointPenalty = Mathf.Abs(leftDistRefPoint - rightDistRefPoint);
            

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
            }
			else{
				currentSpeed -= deceleration * Time.deltaTime;
				currentSpeed = Mathf.Max(currentSpeed, 0f);
				
				Vector3 forwardDirection = mainCamera.transform.forward;
                Vector3 movement = forwardDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
			}	

            lastLeftConPos = currentLeftConPos;
            lastRightConPos = currentRightConPos;
            lastLeftDistRefPoint = leftDistRefPoint;
            lastRightDistRefPoint = rightDistRefPoint;
            //Debug.Log("Speed: " + currentSpeed);

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
        if(posList.Count > posListLimit)
        {
            posList.RemoveAt(0);
        }
    }
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
