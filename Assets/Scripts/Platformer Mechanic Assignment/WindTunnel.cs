using UnityEngine;

public class WindTunnel : MonoBehaviour
{
    public bool windLeft;
    private float windDirection;
    public float multiplier;
    public float windPower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    // Update is called once per frame
    void Update()
    {
        if (windLeft)
        {
            windDirection = -1;
        }
        else
        {
            windDirection = 1;
        }
        windPower = windDirection * multiplier;

    }

    }
