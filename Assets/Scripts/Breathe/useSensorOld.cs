using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class useSensorOld : MonoBehaviour
{
    public float sensorValue;
    [SerializeField]
    private float changeThreshold = 0.0125f;
    [SerializeField]
    private float stagnationThreshold = 0.001f;
    [SerializeField]
    private float filterThreshold = 0.005f;
    [SerializeField, Range(0.0f, 1f)]
    private float filterValue = 0.75f;

    private float previousValue;
    private bool wasInc = false;
    private bool isMonitoringDecrease = false;

    //voraussichtliche maximale Abweichung der Werte
    private float maxDiff = 0.25f;

    private List<float> lastValues = new List<float>();
    private int maxCount = 20;

    // Start is called before the first frame update
    void Start()
    {
        previousValue = sensorValue;
    }

    // Update is called once per frame
    void Update()
    {
        // Bewegungsfilter anwenden
        sensorValue = MovementFilter(sensorValue);
        // Durschnittsflter anwenden
        //sensorValue = AverageFilter(sensorValue);

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
                transform.GetComponent<sensorParticles>().sValue = -deltaValue * 75;

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

    // dynamischer Filter um Schwankungen auszugleichen
    private float MovementFilter(float value)
    {
        float diff = Mathf.Abs(value - previousValue);
        if (diff > maxDiff)
        {
            maxDiff = diff;
        }
        float dynamicFilter = Mathf.Clamp((1f - (diff / maxDiff)) * filterValue, 0.001f, 1f);
        if (diff > filterThreshold)
        {
            value = value * dynamicFilter + previousValue * (1 - dynamicFilter);
        }
        return value;
    }

    private float AverageFilter(float value)
    {
        lastValues.Add(value);
        if(lastValues.Count > maxCount)
        {
            lastValues.RemoveAt(0);
        }
        float averageValue = 0f;
        foreach (float lV in lastValues)
        {
            averageValue += lV;
        }
        averageValue = averageValue / lastValues.Count;
        return averageValue;
    }
}
