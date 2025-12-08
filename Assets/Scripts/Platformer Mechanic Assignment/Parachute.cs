using UnityEngine;

public class Parachute : MonoBehaviour
{
    public LayerMask windLayer;
    private Rigidbody2D chuteRB;

    public 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chuteRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == windLayer)
        {
            Debug.Log("IN THE WIND");
            gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

}
