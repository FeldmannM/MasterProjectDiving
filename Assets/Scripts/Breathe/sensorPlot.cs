using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensorPlot : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    //[SerializeField]
    //private useSensor sensor;
    private float sensorValue = 0f;
    [SerializeField]
    private int maxSize = 50;
    [SerializeField]
    private float graphWidth = 10f;
    [SerializeField]
    private float xpos = 0f;
    [SerializeField]
    private float ypos = 0f;
    [SerializeField]
    private float zpos = 0f;

    private List<Vector3> points = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        // Plot initalisieren (Gerade)
        for (int i = 0; i < maxSize; i++)
        {
            points.Add(new Vector3((i / (float)maxSize) * graphWidth + xpos, ypos, zpos));
        }
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float sensorValue = sensor.sensorValue;

        // Jeden Punkt um eins nach links schieben
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = new Vector3(points[i].x - (graphWidth / (float)maxSize), points[i].y, points[i].z);
        }

        // Neuer Sensorwert als Punkt am rechten Ende hinzufügen
        points.Add(new Vector3(graphWidth + xpos, sensorValue + ypos, zpos));
        // ältester Punkt entfernen
        if(points.Count >= maxSize)
        {
            points.RemoveAt(0);
        }
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    // aktuellen Sensorwert setzen
    public void setSensorValue(float value)
    {
        sensorValue = value;
    }
}
