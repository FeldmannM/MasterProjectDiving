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
        // Berechnung der �nderung
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
                Debug.Log("Variable steigt wieder an. �berwachung wird zur�ckgesetzt.");
                ResetMonitoring();
                wasInc = true;
            }
        }
        else if (deltaValue < 0)
        {
            // Variable f�llt
            if (wasInc && !isMonitoringDecrease)
            {
                // �bergang vom Steigen zum Fallen erkannt
                wasInc = false;
                isMonitoringDecrease = true;
                Debug.Log("�bergang erkannt. Variable beginnt zu fallen.");
            }

            if (isMonitoringDecrease)
            {
                // Kontinuierliche Ausgabe des Abfalls
                Debug.Log("Variable f�llt um: " + (-deltaValue));
                // weitergeben an die Partikelmechanik
                transform.GetComponent<sensorParticles>().sValue = -deltaValue * 100;

                // �berpr�fung auf Stagnation
                if (Mathf.Abs(rateOfChange) < stagnationThreshold)
                {
                    Debug.Log("Abfall stagniert. �berwachung wird zur�ckgesetzt.");
                    //transform.GetComponent<sensorParticles>().sValue = 0f;
                    ResetMonitoring();
                }
            }
        }
        else
        {
            // Keine signifikante �nderung
            if (isMonitoringDecrease)
            {
                // �berpr�fung auf Stagnation
                if (Mathf.Abs(rateOfChange) < stagnationThreshold)
                {
                    Debug.Log("Abfall stagniert. �berwachung wird zur�ckgesetzt.");
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
