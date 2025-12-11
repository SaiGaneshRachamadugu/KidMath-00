using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] numbers = { 10, 24, 22, 72, 1 };

        int min = numbers[0];
        int max = numbers[0];

        for (int i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] < min) min = numbers[i];
            if (numbers[i] > max) max = numbers[i];
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
