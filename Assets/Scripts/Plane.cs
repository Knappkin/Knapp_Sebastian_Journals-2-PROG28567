using UnityEngine;

public class Plane : MonoBehaviour
{

    public Rigidbody2D planeRB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planeRB = GetComponent<Rigidbody2D>();
       // planeRB.AddForce(Vector2.up *5f, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
       // planeRB.AddForce(-Vector2.up, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("FIRST CONTACT"); 
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("TOUCHING ME... TOUCHING YOU!!!");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("LIKE 2 SHIPS PASSING IN THE NIGHT");
    }
}
