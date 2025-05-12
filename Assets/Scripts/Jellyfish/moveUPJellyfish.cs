using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveUPJellyfish : MonoBehaviour
{
    private float speed;
    private float maxTime = 90f;
    private float passedTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        // zufällige Geschwindigkeit
        speed = Random.Range(0.25f, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        // Qualle steigt über eine gewisse Zeit nach oben bevor sie verschwindet
        if(passedTime < maxTime)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            passedTime += Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
