using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class divingWithSwivelChair : MonoBehaviour
{
    [SerializeField]
    private GameObject locomotion;
    [SerializeField]
    private Camera mainCamera;
	[SerializeField]
    private float maxSpeed = 1.5f;
	[SerializeField]
	private float acceleration = 5f;
	[SerializeField]
	private float deceleration = 0.75f;
    [SerializeField]
    private float movementThreshold = 0.004f;
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

    // Start is called before the first frame update
    void Start()
    {
        if (leftConPos != null && rightConPos != null)
        {
            lastLeftConPos = leftConPos.action.ReadValue<Vector3>();
            lastRightConPos = rightConPos.action.ReadValue<Vector3>();
            vibration = locomotion.GetComponent<Vibration>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Idee Tauchen während man sich auf einem Drehstuhl befindet, sodass die Dregung im Wasser über den physischen Stuhl erfolgen kann
        if (leftConPos != null && rightConPos != null)
        {
            currentLeftConPos = leftConPos.action.ReadValue<Vector3>();
            currentRightConPos = rightConPos.action.ReadValue<Vector3>();

            Vector3 movementLeft = currentLeftConPos - lastLeftConPos;
            Vector3 movementRight = currentRightConPos - lastRightConPos;

            Vector3 forwardDirection = mainCamera.transform.forward;
            float movementMagnitudeLeft = Vector3.Dot(movementLeft, forwardDirection);
            float movementMagnitudeRight = Vector3.Dot(movementRight, forwardDirection);

            // Beschleunigung
            if (movementMagnitudeLeft < - movementThreshold && movementMagnitudeRight < - movementThreshold)
            {
				currentSpeed += acceleration * Time.deltaTime;
				currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
				
                Vector3 movement = forwardDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
            }
            // Abbremsen
			else{
				currentSpeed -= deceleration * Time.deltaTime;
				currentSpeed = Mathf.Max(currentSpeed, 0f);
				
                Vector3 movement = forwardDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
			}	
			
            lastLeftConPos = currentLeftConPos;
            lastRightConPos = currentRightConPos;
            Debug.Log(currentSpeed);

            // Bei gewisser Geschwindigkeit Controller vibrieren lassen
            float bIntensity = (currentSpeed / maxSpeed) * 0.25f;
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
