using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class upAndDown : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty leftP;
    [SerializeField]
    private InputActionProperty leftS;
    [SerializeField]
    private GameObject locomotion;
    [SerializeField]
    private float maxSpeed = 1.5f;
    [SerializeField]
    private float accV = 0.5f;
    [SerializeField]
    private float decV = 1f;

    private Vibration vibration;
    private float verticalSpeed;
    private bool activeUpDown = false;

    // Start is called before the first frame update
    void Start()
    {
        vibration = locomotion.GetComponent<Vibration>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (leftS.action.IsPressed())
        {
            verticalSpeed += accV * Time.deltaTime;
            verticalSpeed = Mathf.Clamp(verticalSpeed, -maxSpeed, maxSpeed);

            Vector3 movement = Vector3.up * verticalSpeed * Time.deltaTime;
            locomotion.transform.position += movement;
        }
        else if (leftP.action.IsPressed())
        {
            verticalSpeed -= accV * Time.deltaTime;
            verticalSpeed = Mathf.Clamp(verticalSpeed, -maxSpeed, maxSpeed);

            Vector3 movement = Vector3.up * verticalSpeed * Time.deltaTime;
            locomotion.transform.position += movement;
        }
        else
        {
            if (verticalSpeed > 0)
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

        float bIntensity = Mathf.Abs((verticalSpeed / maxSpeed) * 0.25f);
        if (bIntensity > 0.025f)
        {
            vibration.activeVib = true;
            vibration.setAmplitude(bIntensity);
            activeUpDown = true;
        }
        else if(activeUpDown)
        {
            vibration.activeVib = false;
            activeUpDown = false;
        }


    }
}
