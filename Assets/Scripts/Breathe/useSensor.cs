using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class useSensor : MonoBehaviour
{
    public float sensorValue;
    [SerializeField]
    private float changeThreshold = 0.015f;
    [SerializeField]
    private float stagnationThreshold = 0.001f;

    private float previousValue;
    private bool wasInc = false;
    private bool isMonitoringDecrease = false;

    // Start is called before the first frame update
    void Start()
    {
        previousValue = sensorValue;
    }

    // Update is called once per frame
    void Update()
    {
        // Berechnung der Änderung
        float deltaValue = sensorValue - previousValue;
        float deltaTime = Time.deltaTime;
        float rateOfChange = deltaValue / deltaTime;

        if(Mathf.Abs(deltaValue) < changeThreshold)
        {
            deltaValue = 0;
        }

        // Feststellen der aktuellen Tendenz
        if (deltaValue > 0)
        {
            // Variable steigt
            if (!wasInc && !isMonitoringDecrease)
            {
                wasInc = true;
                Debug.Log("Variable beginnt zu steigen.");
            }
            else if (isMonitoringDecrease)
            {
                // Variable steigt wieder nach Beginn des Abfalls
                Debug.Log("Variable steigt wieder an. Überwachung wird zurückgesetzt.");
                ResetMonitoring();
                wasInc = true;
            }
        }
        else if (deltaValue < 0)
        {
            // Variable fällt
            if (wasInc && !isMonitoringDecrease)
            {
                // Übergang vom Steigen zum Fallen erkannt
                wasInc = false;
                isMonitoringDecrease = true;
                Debug.Log("Übergang erkannt. Variable beginnt zu fallen.");
            }

            if (isMonitoringDecrease)
            {
                // Kontinuierliche Ausgabe des Abfalls
                Debug.Log("Variable fällt um: " + (-deltaValue));
                // weitergeben an die Partikelmechanik
                transform.GetComponent<sensorParticles>().sValue = -deltaValue * 100;

                // Überprüfung auf Stagnation
                if (Mathf.Abs(rateOfChange) < stagnationThreshold)
                {
                    Debug.Log("Abfall stagniert. Überwachung wird zurückgesetzt.");
                    //transform.GetComponent<sensorParticles>().sValue = 0f;
                    ResetMonitoring();
                }
            }
        }
        else
        {
            // Keine signifikante Änderung
            if (isMonitoringDecrease)
            {
                // Überprüfung auf Stagnation
                if (Mathf.Abs(rateOfChange) < stagnationThreshold)
                {
                    Debug.Log("Abfall stagniert. Überwachung wird zurückgesetzt.");
                    //transform.GetComponent<sensorParticles>().sValue = 0f;
                    ResetMonitoring();
                }
            }
        }

        //Debug.Log("SensorValue: " + sensorValue);
        // Aktualisierung des vorherigen Wertes
        previousValue = sensorValue;
    }

    void ResetMonitoring()
    {
        wasInc = false;
        isMonitoringDecrease = false;
    }
}
