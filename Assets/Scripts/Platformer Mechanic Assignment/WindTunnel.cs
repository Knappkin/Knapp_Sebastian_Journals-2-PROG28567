using UnityEngine;

public class WindTunnel : MonoBehaviour
{
    //Boolean to get whether the wind tunnel is blowing left or right - true for left, false for right
    public bool windLeft;
    // Actual direction value - -1 if blowing left, +1 if right
    private float windDirection;
    //multiplier, * direction, allows to make so wind tunnels push harder
    public float multiplier;
    //Final value to be used by player controller script
    public float windPower;
   

    // Update is called once per frame
    void Update()
    {
        //Get direction of the tunnel
        if (windLeft)
        {
            windDirection = -1;
        }
        else
        {
            windDirection = 1;
        }
        //Set the wind power to the direction times multiplier, both set in the inspector. wind power is accessed from player script
        windPower = windDirection * multiplier;

    }

    }
