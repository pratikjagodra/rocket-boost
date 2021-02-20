using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = default ;
    [SerializeField] float period = 2f ;

    float movementFector;
    Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        if (period != 0)
        {
            float cycles = Time.time / period; //no of cycles depending on how much longer you play
            const float tau = Mathf.PI * 2f;
            float rawSinWave = Mathf.Sin(tau * cycles); //values -1 to 1 depending on sine wave

            movementFector = rawSinWave / 2f + 0.5f; //convert values -1 to 1 ----> 0 to 1
            Vector3 offset = movementVector * movementFector;
            transform.position = startingPos + offset;
        }
        
    }
}
