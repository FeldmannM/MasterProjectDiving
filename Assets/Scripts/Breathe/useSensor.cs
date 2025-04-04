using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class useSensor : MonoBehaviour
{
    public float sensorValue;
    [SerializeField]
    private float stagnationThreshold = 0.001f;
    [SerializeField]
    private float filterThreshold = 0.005f;
    [SerializeField, Range(0.0f, 1f)]
    private float filterValue = 0.75f;
    [SerializeField, Range(0.0f, 1f)]
    private float percentThreshold = 0.8f;

    private bool wasInc = false;
    private bool isMonitoringDecrease = false;

    //voraussichtliche maximale Abweichung der Werte
    private float maxDiff = 0.25f;

    [SerializeField]
    private int maxCount = 20;
    [SerializeField]
    private List<float> lastValues = new List<float>();

    // Update is called once per frame
    void Update()
    {
        //Ü Überbrüfung, ob der Sensor den Wert aktualisiert hat (gerade nur 100 mal in der Sekunde (HZ))
        if(lastValues.Count > 0 && Mathf.Abs(sensorValue - lastValues[lastValues.Count -1]) < 0.00001)
        {
            return;
        }

        // Bewegungsfilter anwenden
        sensorValue = MovementFilter(sensorValue);
        // Durschnittsflter anwenden
        //sensorValue = AverageFilter(sensorValue);

        //Debug.Log("SensorValue: " + sensorValue);

        // Liste der letzten paar Werte
        lastValues.Add(sensorValue);
        if (lastValues.Count > maxCount)
        {
            lastValues.RemoveAt(0);
        }
        if (lastValues.Count < maxCount)
        {
            return;
        }
        // Berechnung der Änderung

        int incCount = 0;
        int decCount = 0;

        for (int i = 1; i < lastValues.Count; i++)
        {
            if(lastValues[i] > lastValues[i-1])
            {
                incCount++;
            }
            else if (lastValues[i] < lastValues[i-1])
            {
                decCount++;
            }
        }

        float totalCounts = lastValues.Count - 1;
        float incPercent = incCount / totalCounts;
        float decPercent = decCount / totalCounts;


        float deltaValue = lastValues[lastValues.Count - 1] - lastValues[0];
        float rateOfChange = deltaValue / (lastValues.Count * Time.deltaTime);

        if (incPercent >= percentThreshold && !wasInc)
        {
            Debug.Log("Variable steigt überwiegend ( >= " + (incPercent * 100) + "%).");
            HandleInc();
        }
        else if(decPercent >= percentThreshold)
        {
            Debug.Log("Variable fällt überwiegend ( >= " + (decPercent * 100) + "%).");
            HandleDec();
        }
        else
        {
            //Debug.Log("Variable stagniert. Überwachung wird zurückgesetzt.");
            HandleNoSignificance(rateOfChange);
        }

    }

    void HandleInc()
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

    void HandleDec()
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
            Debug.Log("Variable fällt weiter.");
            // weitergeben an die Partikelmechanik
            float deltaValue = lastValues[lastValues.Count - 1] - lastValues[0];
            transform.GetComponent<sensorParticles>().sValue = -deltaValue * 10;

            // Überprüfung auf Stagnation
            if (Mathf.Abs(deltaValue/Time.deltaTime) < stagnationThreshold)
            {
                Debug.Log("Abfall stagniert. Überwachung wird zurückgesetzt.");
                transform.GetComponent<sensorParticles>().sValue = 0f;
                ResetMonitoring();
            }
        }
    }

    void HandleNoSignificance(float rateOfChange)
    {
        // Keine signifikante Änderung
        if (isMonitoringDecrease)
        {
            // Überprüfung auf Stagnation
            if (Mathf.Abs(rateOfChange) < stagnationThreshold)
            {
                Debug.Log("Variable stagniert. Überwachung wird zurückgesetzt.");
                //transform.GetComponent<sensorParticles>().sValue = 0f;
                ResetMonitoring();
            }
        }
    }

    void ResetMonitoring()
    {
        wasInc = false;
        isMonitoringDecrease = false;
    }

    // dynamischer Filter um Schwankungen auszugleichen
    private float MovementFilter(float value)
    {
        float previousValue = value;
        if(lastValues.Count > 0)
        {
            previousValue =  lastValues[lastValues.Count - 1];
        }
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
        if (lastValues.Count < maxCount || lastValues[0] == 0f)
        {
            return value;
        }
        float averageValue = 0f;
        foreach (float lV in lastValues)
        {
            averageValue += lV;
        }
        averageValue = (averageValue + value) / (lastValues.Count + 1);
        return averageValue;
    }
}
