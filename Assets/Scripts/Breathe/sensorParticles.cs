using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensorParticles : MonoBehaviour
{
    public float sValue = 0f;
    [SerializeField]
    private ParticleSystem particles;
    [SerializeField]
    private AudioSource audioS;
    [SerializeField]
    private float freqTime;
    private float factor;
    private float passedTime = 0f;
    private ParticleSystem.EmissionModule emission;
    // Start is called before the first frame update
    void Start()
    {
        emission = particles.emission;
        freqTime *= 2;
    }

    // Update is called once per frame
    void Update()
    {
        // eingelesener Sensorwert für Anzahl der Partikel verarbeiten
        //particles.Play();
        factor = sValue * 40;
        factor = Mathf.Clamp(factor, 0.01f, 40);
        //Debug.Log("Breathe Factor: " + factor);
        emission.rateOverTime = factor/5f;

        // Bei einigen Luftblassen Sound abspielen
        if (factor > 5 && !audioS.isPlaying)
        {
            audioS.time = 0.2f;
            audioS.Play();
        }
        else if (factor <= 5 && audioS.isPlaying)
        {
            audioS.Stop();
        }
        // Partikel über die Zeit verringern
        sValue -= Time.deltaTime*0.75f;
        sValue = Mathf.Max(sValue, 0f);

        /*
        if (factor == 0)
        {
            particles.Stop();
            if (audioS.isPlaying)
            {
                audioS.Stop();
            }
        }*/
    }
}
