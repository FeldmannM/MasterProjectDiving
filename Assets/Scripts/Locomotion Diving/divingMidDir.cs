using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class divingMidDir : MonoBehaviour
{
    [SerializeField]
    private GameObject locomotion;
    [SerializeField]
    private GameObject neckAnchor;
    [SerializeField]
    private GameObject breathe;
    [SerializeField]
    private GameObject xrOrigin;
    [SerializeField]
    private GameObject turnManipulator;
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
    private int posListLimit = 10;
    [SerializeField]
    private InputActionReference leftConPos;
    [SerializeField]
    private InputActionReference rightConPos;
    [SerializeField]
    private GameObject lCon;
    [SerializeField]
    private GameObject rCon;

    private Vibration vibration;

    private Vector3 lastLeftConPos;
    private Vector3 lastRightConPos;
    private Vector3 currentLeftConPos;
    private Vector3 currentRightConPos;
    private float currentSpeed;

    private float lastLeftDistRefPoint;
    private float lastRightDistRefPoint;
    private float leftDistRefPoint;
    private float rightDistRefPoint;
    private float conSeparation;
    private float refPointPenalty;

    [SerializeField]
    private List<Vector3> lastLeftPosList = new List<Vector3>();
    [SerializeField]
    private List<Vector3> lastRightPosList = new List<Vector3>();
    private Vector3 currentLeftConLocalPos;
    private Vector3 currentRightConLocalPos;

    private useSensor sensor;

    // Start is called before the first frame update
    void Start()
    {
        if (leftConPos != null && rightConPos != null)
        {
            lastLeftConPos = lCon.transform.position;
            lastRightConPos = rCon.transform.position;
            lastLeftDistRefPoint = Vector3.Distance(lastLeftConPos, neckAnchor.transform.position);
            lastRightDistRefPoint = Vector3.Distance(lastRightConPos, neckAnchor.transform.position);
            vibration = locomotion.GetComponent<Vibration>();
        }
        sensor = breathe.GetComponent<useSensor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (leftConPos != null && rightConPos != null)
        {
            currentLeftConLocalPos = leftConPos.action.ReadValue<Vector3>();
            currentRightConLocalPos = rightConPos.action.ReadValue<Vector3>();
            currentLeftConPos = lCon.transform.position;
            currentRightConPos = rCon.transform.position;

            // Distanzberechnung
            leftDistRefPoint = Vector3.Distance(currentLeftConPos, neckAnchor.transform.position);
            rightDistRefPoint = Vector3.Distance(currentRightConPos, neckAnchor.transform.position);
            conSeparation = Vector3.Distance(currentLeftConPos, currentRightConPos);
            refPointPenalty = Mathf.Abs(leftDistRefPoint - rightDistRefPoint);

            /* ehemals:
            // Mittlerer Vektor
            Vector3 midCons = (lCon.transform.position + rCon.transform.position) / 2;
            Vector3 underCam = neckAnchor.transform.position;
            Vector3 middleDirection = midCons - underCam;
            Vector3 middleDirection = (lCon.transform.forward + rCon.transform.forward);
            //middleDirection.y = 0f;
            middleDirection = middleDirection.normalized;

            Debug.DrawLine(underCam, midCons, Color.red, 0.2f);
            Debug.DrawRay(locomotion.transform.position, middleDirection * 2, Color.blue, 0.1f);
            */

            /* erste Idee
            Vector3 middleDirection = (lCon.transform.forward + rCon.transform.forward);
            //middleDirection.y = 0f;
            middleDirection = middleDirection.normalized;
            Debug.DrawRay(locomotion.transform.position, middleDirection * 2, Color.blue, 0.1f);
            */

            Vector3 leftDirection = CalculateMidDir(lastLeftPosList);
            Vector3 rightDirection = CalculateMidDir(lastRightPosList);

            Quaternion turnRot = Quaternion.Euler(0, xrOrigin.transform.localEulerAngles.y, 0);
            if (turnManipulator.transform.localEulerAngles.z != 0)
            {
                turnRot = Quaternion.Euler(0, turnManipulator.transform.localEulerAngles.z, 0);
            }

            if(lastLeftPosList.Count > 1 && lastRightPosList.Count > 1)
            {
                Vector3 expectedLeftDir = (lastLeftPosList[0] - lastLeftPosList[lastLeftPosList.Count - 1]).normalized;
                Vector3 expectedRightDir = (lastRightPosList[0] - lastRightPosList[lastLeftPosList.Count - 1]).normalized;
                Debug.DrawRay(currentLeftConPos, turnRot * expectedLeftDir, Color.green, 0.1f);
                Debug.DrawRay(currentRightConPos, turnRot * expectedRightDir, Color.green, 0.1f);
                if (Vector3.Dot(leftDirection, expectedLeftDir) < 0)
                {
                    leftDirection *= -1;
                }
                if(Vector3.Dot(rightDirection, expectedRightDir) < 0)
                {
                    rightDirection *= -1;
                }
            }
            //Flippen
            //leftDirection *= -1;
            //rightDirection *= -1;
            leftDirection.y *= 0.01f;
            rightDirection.y *= 0.01f;
            leftDirection = turnRot * leftDirection;
            rightDirection = turnRot * rightDirection;
            leftDirection = leftDirection.normalized;
            rightDirection = rightDirection.normalized;
            Debug.DrawRay(currentLeftConPos, leftDirection * 2, Color.red, 0.1f);
            Debug.DrawRay(currentRightConPos, rightDirection * 2, Color.red, 0.1f);


            Vector3 middleDirection = (leftDirection + rightDirection).normalized;
            if(Vector3.Dot(middleDirection, neckAnchor.transform.forward) < 0)
            {
                middleDirection *= -1;
            }
            Debug.DrawRay(locomotion.transform.position, middleDirection * 2, Color.blue, 0.1f);
            Debug.Log("MidDir: " + middleDirection);
            

            if (leftDistRefPoint < lastLeftDistRefPoint - movementThreshold && rightDistRefPoint < lastRightDistRefPoint - movementThreshold)
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

                Vector3 movement = middleDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
                sensor.currentLocomotionState = 2;
            }
            else if (leftDistRefPoint > lastLeftDistRefPoint + movementThreshold && rightDistRefPoint > lastRightDistRefPoint + movementThreshold)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);

                Vector3 movement = middleDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
                sensor.currentLocomotionState = 1;
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);

                Vector3 movement = middleDirection * currentSpeed * Time.deltaTime;
                locomotion.transform.position += movement;
                sensor.currentLocomotionState = 0;
            }

            lastLeftConPos = currentLeftConPos;
            lastRightConPos = currentRightConPos;
            lastLeftDistRefPoint = leftDistRefPoint;
            lastRightDistRefPoint = rightDistRefPoint;
            //Debug.Log(currentSpeed);

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

    // per kleinste Quadrate (Least Squares)
    private Vector3 CalculateMidDir(List<Vector3> posList)
    {
        if(posList.Count < 2)
        {
            return Vector3.forward;
        }

        float avgX = 0;
        float avgY = 0;
        float avgZ = 0;

        foreach (Vector3 pos in posList)
        {
            avgX += pos.x;
            avgY += pos.y;
            avgZ += pos.z;
        }
        avgX /= posList.Count;
        avgY /= posList.Count;
        avgZ /= posList.Count;
        Vector3 avgPosition = new Vector3(avgX, avgY, avgZ);

        float sumXX = 0;
        float sumXY = 0;
        float sumXZ = 0;
        float sumYY = 0;
        float sumYZ = 0;
        float sumZZ = 0;

        // Matrix berechnen
        foreach(Vector3 pos in posList)
        {
            Vector3 diff = pos - avgPosition;
            sumXX += diff.x * diff.x;
            sumXY += diff.x * diff.y;
            sumXZ += diff.x * diff.z;
            sumYY += diff.y * diff.y;
            sumYZ += diff.y * diff.z;
            sumZZ += diff.z * diff.z;
        }

        Matrix4x4 covarianceMatrix = new Matrix4x4(
            new Vector4(sumXX, sumXY, sumXZ, 0),
            new Vector4(sumXY, sumYY, sumYZ, 0),
            new Vector4(sumXZ, sumYZ, sumZZ, 0),
            new Vector4(0,0,0,1)
        );

        Vector3 direction = Eigenvector(covarianceMatrix);
        Debug.Log("Dir: " + direction);
        return direction.normalized;
    }

    // Eigenvektor berechnen
    private static Vector3 Eigenvector(Matrix4x4 matrix, int iterations = 10)
    {
        // Startvektor
        Vector3 eigenvector = Vector3.forward;
        // Iterative Methode
        for (int i = 0; i < iterations; i++)
        {
            Vector3 newEigenvector = new Vector3(
                matrix.m00 * eigenvector.x + matrix.m01 * eigenvector.y + matrix.m02 * eigenvector.z,
                matrix.m10 * eigenvector.x + matrix.m11 * eigenvector.y + matrix.m12 * eigenvector.z,
                matrix.m20 * eigenvector.x + matrix.m21 * eigenvector.y + matrix.m22 * eigenvector.z
            );

            eigenvector = Vector3.Normalize(newEigenvector);
        }
        return eigenvector;
    }

    private void UpdateList(List<Vector3> posList, Vector3 pos)
    {
        posList.Add(pos);
        if (posList.Count > posListLimit)
        {
            posList.RemoveAt(0);
        }
    }
    private bool IsPosNearPosInList(List<Vector3> posList, Vector3 currentPos, float threshold)
    {
        foreach (Vector3 pos in posList)
        {
            if (Vector3.Distance(pos, currentPos) >= threshold)
            {
                return false;
            }
        }
        return true;
    }

}
