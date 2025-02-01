using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Class Variables
    private PluxDeviceManager pluxDevManager;

    // Class constants (CAN BE EDITED BY IN ACCORDANCE TO THE DESIRED DEVICE CONFIGURATIONS)
    [System.NonSerialized]
    public List<string> domains = new List<string>() { "BTH" };

    public int samplingRate = 100;

    private int Hybrid8PID = 517;
    private int BiosignalspluxPID = 513;
    private int BitalinoPID = 1538;
    private int MuscleBanPID = 1282;
    private int MuscleBanNewPID = 2049;
    private int CardioBanPID = 2050;
    private int BiosignalspluxSoloPID = 532;
    private int MaxLedIntensity = 255;


    // Start is called before the first frame update
    private void Start()
    {
        // Initialise object
        pluxDevManager = new PluxDeviceManager(ScanResults, ConnectionDone, AcquisitionStarted, OnDataReceived, OnEventDetected, OnExceptionRaised);

        pluxDevManager.GetDetectableDevicesUnity(domains);
    }

    // Update function, being constantly invoked by Unity.
    private void Update()
    { }

    // Method invoked when the application was closed.
    private void OnApplicationQuit()
    {
        try
        {
            // Disconnect from device.
            if (pluxDevManager != null)
            {
                pluxDevManager.DisconnectPluxDev();
                Console.WriteLine("Application ending after " + Time.time + " seconds");
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine("Device already disconnected when the Application Quit.");
        }
    }


    // Method called when the "Disconnect Device" button is pressed.
    public void DisconnectButtonFunction()
    {
        // Disconnect from the device.
        pluxDevManager.DisconnectPluxDev();
    }

    // Method called when the "Start Acquisition" button is pressed.
    public void StartFunction()
    {
        // Get the Sampling Rate and Resolution values.
        samplingRate = 100;     // in HZ
        int resolution = 16;    // in bit

        // Initializing the sources array.
        List<PluxDeviceManager.PluxSource> pluxSources = new List<PluxDeviceManager.PluxSource>();

        // biosignalsplux Hybrid-8 device (3 sensors >>> 1 Analog + 2 Digital SpO2/fNIRS)
        if (pluxDevManager.GetProductIdUnity() == Hybrid8PID)
        {
            // Add the sources of the digital channels (CH1 and CH2).
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x03));
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x03));

            // Define the LED Intensities of both sensors (CH1 and CH2) as: {RED, INFRARED}
            int redLedIntensity = (int)(80 * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            int infraredLedIntensity = (int)(40 * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            int[] ledIntensities = new int[2] { redLedIntensity, infraredLedIntensity };
            pluxDevManager.SetParameter(1, 0x03, ledIntensities);
            pluxDevManager.SetParameter(2, 0x03, ledIntensities);

            // Add the source of the analog channel (CH8).
            pluxSources.Add(new PluxDeviceManager.PluxSource(8, 1, resolution, 0x01));

            // Add the sources of the internal IMU channels (CH11 with 9 derivations [3xACC | 3xGYRO | 3xMAG] defined by the 0x01FF chMask).
            int imuPort = 11;
            pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x01FF));

            // Alternatively only some of the derivations can be activated.
            // >>> 3xACC (channel mask 0x0007)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x0007));
            // >>> 3xGYR (channel mask 0x0038)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x0038));
            // >>> 3xMAG (channel mask 0x01C0)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x01C0));
        }
        // biosignalsplux (2 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == BiosignalspluxPID)
        {
            // Starting a real-time acquisition from:
            // >>> biosignalsplux [CH1 and CH2 active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01)); // CH1 | EDA
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x01)); // CH2 | ECG

            // Add the sources of the digital channel (CH9 | fNIRS/SpO2).
            // pluxSources.Add(new PluxDeviceManager.PluxSource(9, 1, resolution, 0x03));

            // Define the LED Intensities of the CH9 sensor as: {RED, INFRARED}
            // int redLedIntensity = (int)(int.Parse(RedIntensityDropdown.options[RedIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            // int infraredLedIntensity = (int)(int.Parse(InfraredIntensityDropdown.options[InfraredIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            // int[] ledIntensities = new int[2] { redLedIntensity, infraredLedIntensity };
            // pluxDevManager.SetParameter(9, 0x03, ledIntensities);
        }
        // muscleBAN (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == MuscleBanPID)
        {
            // Starting a real-time acquisition from:
            // >>> muscleBAN [CH1 > EMG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> muscleBAN [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x3F));
        }
        // muscleBAN v2 (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == MuscleBanNewPID)
        {
            // Starting a real-time acquisition from:
            // >>> muscleBAN [CH1 > EMG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> muscleBAN Virtual Port [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
        }
        // cardioBAN (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == CardioBanPID)
        {
            // Starting a real-time acquisition from:
            // >>> cardioBAN [CH1 > ECG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> cardioBAN Virtual Port [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
        }
        // biosignalsplux Solo (8 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == BiosignalspluxSoloPID)
        {
            // Starting a real-time acquisition from:
            // >>> biosignalsplux Solo [CH1 > MICRO]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> biosignalsplux Solo [CH2 > CUSTOM]
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x01));
            // >>> biosignalsplux Solo Virtual Port [CH3-CH5 > ACC | CH6-CH8 > MAG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
        }

        // BITalino (2 Analog sensors)
        if (pluxDevManager.GetProductIdUnity() == BitalinoPID)
        {
            // Starting a real-time acquisition from:
            // >>> BITalino [Channels A2 and A5 active]
            pluxDevManager.StartAcquisitionUnity(samplingRate, new List<int> { 2, 5 }, 10);
        }
        else
        {
            // Start a real-time acquisition with the created sources.
            pluxDevManager.StartAcquisitionBySourcesUnity(samplingRate, pluxSources.ToArray());
        }
    }

    // Method called when the "Stop Acquisition" button is pressed.
    public void StopButtonFunction()
    {
        // Stop the real-time acquisition.
        pluxDevManager.StopAcquisitionUnity();
    }

    /**
     * =================================================================================
     * ============================= Callbacks =========================================
     * =================================================================================
     */

    // Callback that receives the list of PLUX devices found during the Bluetooth scan.
    public void ScanResults(List<string> listDevices)
    {
        if (listDevices.Count > 0)
        {
            // Show an informative message about the number of detected devices.
            Debug.Log("Scan completed.\nNumber of devices found: " + listDevices.Count);
            pluxDevManager.PluxDev(listDevices[0]);
        }
        else
        {
            // Show an informative message stating the none devices were found.
            Debug.Log("Bluetooth device scan didn't found any valid devices.");
        }
    }

    // Callback invoked once the connection with a PLUX device was established.
    // connectionStatus -> A boolean flag stating if the connection was established with success (true) or not (false).
    public void ConnectionDone(bool connectionStatus)
    {
        if (connectionStatus)
        {
            Debug.Log("Connected, Bähm!");
            StartFunction();
        }
        else
        {
            // Show an informative message stating the connection with the device was not established with success.
            Debug.Log("It was not possible to establish a connection with the device. Please, try to repeat the connection procedure.");
        }
    }

    // Callback invoked once the data streaming between the PLUX device and the computer is started.
    // acquisitionStatus -> A boolean flag stating if the acquisition was started with success (true) or not (false).
    // exceptionRaised -> A boolean flag that identifies if an exception was raised and should be presented in the GUI (true) or not (false).
    public void AcquisitionStarted(bool acquisitionStatus, bool exceptionRaised = false, string exceptionMessage = "")
    {
        if (acquisitionStatus)
        {

        }
        else
        {
            // Present an informative message about the error.
            Debug.Log(!exceptionRaised ? "It was not possible to start a real-time data acquisition. Please, try to repeat the scan/connect/start workflow." : exceptionMessage);
        }
    }

    // Callback invoked every time an exception is raised in the PLUX API Plugin.
    // exceptionCode -> ID number of the exception to be raised.
    // exceptionDescription -> Descriptive message about the exception.
    public void OnExceptionRaised(int exceptionCode, string exceptionDescription)
    {
        if (pluxDevManager.IsAcquisitionInProgress())
        {
            // Present an informative message about the error.
            Debug.Log(exceptionDescription);
        }
    }

    // Callback that receives the data acquired from the PLUX devices that are streaming real-time data.
    // nSeq -> Number of sequence identifying the number of the current package of data.
    // data -> Package of data containing the RAW data samples collected from each active channel ([sample_first_active_channel, sample_second_active_channel,...]).
    public void OnDataReceived(int nSeq, int[] data)
    {
        // Show samples with a 1s interval.
        //if (nSeq % samplingRate == 0)
        //{
        double datapoint = (((data[1] / (Math.Pow(2, 16) - 1.0)) - 0.5) * 3.0);
        //Debug.Log(datapoint);
        datapoint += 1;
        //transform.localScale = Vector3.one * (float)datapoint;
        transform.GetComponent<useSensor>().sensorValue = (float)datapoint;
    }

    // Callback that receives the events raised from the PLUX devices that are streaming real-time data.
    // pluxEvent -> Event object raised by the PLUX API.
    public void OnEventDetected(PluxDeviceManager.PluxEvent pluxEvent)
    {
        if (pluxEvent is PluxDeviceManager.PluxDisconnectEvent)
        {
            // Present an error message.
            Console.WriteLine(
                 "The connection between the computer and the PLUX device was interrupted due to the following event: " +
                (pluxEvent as PluxDeviceManager.PluxDisconnectEvent).reason);

            // Securely stop the real-time acquisition.
            pluxDevManager.StopAcquisitionUnity(-1);
        }
        else if (pluxEvent is PluxDeviceManager.PluxDigInUpdateEvent)
        {
            PluxDeviceManager.PluxDigInUpdateEvent digInEvent = (pluxEvent as PluxDeviceManager.PluxDigInUpdateEvent);
            Console.WriteLine("Digital Input Update Event Detected on channel " + digInEvent.channel + ". Current state: " + digInEvent.state);
        }
    }
}