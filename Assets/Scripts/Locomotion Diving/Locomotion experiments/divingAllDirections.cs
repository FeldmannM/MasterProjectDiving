using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class divingAllDirections : MonoBehaviour
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
    private float movementThreshold = 0.005f;
    [SerializeField]
    private float accV = 2f;
    [SerializeField]
    private float decV = 0.25f;
    [SerializeField]
    private float moveVThreshold = 0.01f;
    [SerializeField]
    private float turnFactorAc = 50f;
    [SerializeField]
    private float turnFactorDec = 5f;
    [SerializeField]
    private float turnThreshold = 0.01f;
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

    private float verticalSpeed;

    private float turnSpeed;

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
        if (leftConPos != null && rightConPos != null)
        {
            currentLeftConPos = leftConPos.action.ReadValue<Vector3>();
            currentRightConPos = rightConPos.action.ReadValue<Vector3>();

            if (currentLeftConPos.z < lastLeftConPos.z - movementThreshold && currentRightConPos.z < lastRightConPos.z - movementThreshold)
            {
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

                Vector3 forwardDirection = locomotion.transform.forward;
                Vector3 movement = forwardDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);

                Vector3 forwardDirection = locomotion.transform.forward;
                Vector3 movement = forwardDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
            }


            if (currentLeftConPos.y < lastLeftConPos.y - moveVThreshold && currentRightConPos.y < lastRightConPos.y - moveVThreshold)
            {
                verticalSpeed += accV * Time.deltaTime;
                verticalSpeed = Mathf.Clamp(verticalSpeed, -maxSpeed, maxSpeed);

                Vector3 movement = Vector3.up * verticalSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
            }
            else if (currentLeftConPos.y > lastLeftConPos.y + moveVThreshold && currentRightConPos.y > lastRightConPos.y + moveVThreshold)
            {
                verticalSpeed -= accV * Time.deltaTime;
                verticalSpeed = Mathf.Clamp(verticalSpeed, -maxSpeed, maxSpeed);

                Vector3 movement = Vector3.up * verticalSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
            }
            else
            {
                if(verticalSpeed > 0)
                {
                    verticalSpeed -= decV * Time.deltaTime;
                    verticalSpeed = Mathf.Max(verticalSpeed, 0f);

                    Vector3 movement = Vector3.up * verticalSpeed * Time.deltaTime;
                    locomotion.transform.position += movement;

                }
                else if (verticalSpeed < 0)
                {
                    verticalSpeed += decV * Time.deltaTime;
                    verticalSpeed = Mathf.Min(verticalSpeed, 0f);

                    Vector3 movement = Vector3.up * verticalSpeed * Time.deltaTime;
                    locomotion.transform.position += movement;
                }
            }


            if (currentLeftConPos.x < lastLeftConPos.x - turnThreshold && currentRightConPos.x < lastRightConPos.x - turnThreshold)
            {
                turnSpeed += turnFactorAc * Time.deltaTime;
                locomotion.transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
                //locomotion.transform.Rotate(mainCamera.transform.up, turnSpeed * Time.deltaTime);
            }
            else if (currentLeftConPos.x > lastLeftConPos.x + turnThreshold && currentRightConPos.x > lastRightConPos.x + turnThreshold)
            {
                turnSpeed -= turnFactorAc * Time.deltaTime;
                locomotion.transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
                //locomotion.transform.Rotate(mainCamera.transform.up, turnSpeed * Time.deltaTime);
            }
            else
            {
                if (turnSpeed > 0)
                {
                    turnSpeed -= turnFactorDec * Time.deltaTime;
                    turnSpeed = Mathf.Max(turnSpeed, 0f);
                    locomotion.transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);

                }
                else if (turnSpeed < 0)
                {
                    turnSpeed += turnFactorDec * Time.deltaTime;
                    turnSpeed = Mathf.Min(turnSpeed, 0f);
                    locomotion.transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
                }
            }

            lastLeftConPos = currentLeftConPos;
            lastRightConPos = currentRightConPos;
            Debug.Log(currentSpeed + ", " + verticalSpeed + ", " + turnSpeed);

            float bIntensity = 0;
            if (currentSpeed > 0)
            {
                bIntensity = (currentSpeed / maxSpeed) * 0.25f;
            }
            else if (Mathf.Abs(verticalSpeed) > 0)
            {
                bIntensity = (Mathf.Abs(verticalSpeed) / maxSpeed) * 0.25f;
            }
            else if (Mathf.Abs(turnSpeed) > 1f)
            {
                bIntensity = Mathf.Abs(turnSpeed) / 25f;
            }

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
