using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class useSensor : MonoBehaviour
{
    public float sensorValue;
    [SerializeField]
    private GameObject sensorPlotGO;
    [SerializeField]
    private float dynamicArmMovementCompensator = 0.2f;
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
    [SerializeField]
    private List<float> mountains = new List<float>();
    private float maxMountain = float.MinValue;
    [SerializeField]
    private List<float> mountainTimings = new List<float>();
    private float lastMountainTiming = -1f;
    [SerializeField]
    private List<float> valleys = new List<float>();
    private float minValley = float.MaxValue;
    private bool startBreathing = false;
    private float lastBreathingTime = -1f;

    public int currentLocomotionState = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Überprüfung, ob der Sensor den Wert aktualisiert hat (gerade nur 100 mal in der Sekunde (HZ))
        if(lastValues.Count > 0 && Mathf.Approximately(sensorValue, lastValues[lastValues.Count-1]))
        {
            return;
        }

        // auf Bewegung mit den Haenden reagieren
        // Wert sinkt normalerweise beim Erhöhen der Distanz zu einem
        if(currentLocomotionState == 1)
        {
            sensorValue += dynamicArmMovementCompensator;
        }
        // Wert steigt normalerweise leicht beim Verringern der Distanz zu einem
        else if (currentLocomotionState == 2)
        {
            sensorValue -= dynamicArmMovementCompensator * 0.1f;
        }

        // Bewegungsfilter anwenden
        sensorValue = MovementFilter(sensorValue);
        // Durschnittsflter anwenden
        sensorValue = AverageFilter(sensorValue);

        //Debug.Log("SensorValue: " + sensorValue);

        // Wert dem Plot übergeben
        if (sensorPlotGO.activeSelf)
        {
            sensorPlotGO.GetComponent<sensorPlot>().setSensorValue(sensorValue);
        }

        // höchster Gesamtwert
        if(sensorValue > maxMountain)
        {
            maxMountain = sensorValue;
        }
        // niedrigster Gesamtwert
        if(sensorValue < minValley && Time.realtimeSinceStartup >= 20f)
        {
            minValley = sensorValue;
        }

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

            // Täler speichern in den ersten 2 Minuten
            if (Time.realtimeSinceStartup < 120f)
            {
                float minV = float.MaxValue;
                foreach (float v in lastValues)
                {
                    if (v < minV)
                    {
                        minV = v;
                    }
                }
                valleys.Add(minV);
            }
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

            // Berge speichern in den ersten 2 Minuten
            if(Time.realtimeSinceStartup < 120f)
            {
                float maxV = float.MinValue;
                foreach (float v in lastValues)
                {
                    if (v > maxV)
                    {
                        maxV = v;
                    }
                }
                mountains.Add(maxV);
                if (lastMountainTiming > 0)
                {
                    mountainTimings.Add(Time.time - lastMountainTiming);
                }
                lastMountainTiming = Time.time;
            }
        }

        if (isMonitoringDecrease)
        {
            // Kontinuierliche Ausgabe des Abfalls
            Debug.Log("Variable fällt weiter.");
            // weitergeben an die Partikelmechanik
            float deltaValue = lastValues[lastValues.Count - 1] - lastValues[0];
            if(Time.realtimeSinceStartup < 120f || startBreathing)
            {
                transform.GetComponent<sensorParticles>().sValue = -deltaValue * 10;
            }
            else
            {
                // Ähnlichkeit prüfen zu den in den ersten 2 Minuten gemessenen Werten
                // Höhe des Werts
                float minMountain = Mathf.Min(mountains.ToArray());
                float maxMountain = Mathf.Max(mountains.ToArray());
                bool similarValue = sensorValue >= minMountain - 0.1f && sensorValue <= maxMountain + 0.1f;
                // Zeitabstand zur letzten Ausatmung
                int index = mountainTimings.Count / 2;
                float medianTime;
                if(mountainTimings.Count % 2 == 0)
                {
                    medianTime = (mountainTimings[index - 1] + mountainTimings[index]) / 2f;
                }
                else
                {
                    medianTime = mountainTimings[index];
                }
                bool similarTime;
                if (lastBreathingTime <= 0f)
                {
                    similarTime = true;
                }
                else
                {
                    if (lastBreathingTime >= medianTime - 0.5f)
                    {
                        similarTime = true;
                    }
                    else
                    {
                        similarTime = false;
                    }
                }
                // Falls beides erfüllt dann Start der Ausatmungsblassen
                if (similarValue && similarTime)
                {
                    transform.GetComponent<sensorParticles>().sValue = -deltaValue * 10;
                    startBreathing = true;
                    lastBreathingTime = Time.time;
                }
            }

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
        startBreathing = false;
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
        if (lastValues.Count < maxCount || Mathf.Approximately(lastValues[0], 0f))
        {
            return value;
        }
        float averageValue = 0f;
        foreach (float lV in lastValues)
        {
            averageValue += lV;
        }
        averageValue = (averageValue / lastValues.Count) * (1f - filterValue) + value * filterValue;
        return averageValue;
    }
}
