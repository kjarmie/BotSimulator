using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public float dX;   // speed in x direction
    public float dY;   // speed in y direction

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, move the alien in the specified x and y direction;
        if (Time.timeScale == 0)    // unless the time scale is 0
        {
            return;
        }

        // transform.position.Set(transform.position.x + (dX * Time.timeScale), transform.position.y + (dY * Time.timeScale), transform.position.z);
        transform.position = new Vector3(transform.position.x + (dX * Time.timeScale), transform.position.y + (dY * Time.timeScale), transform.position.z);
        // transform.position = new Vector3(transform.position.x + dX, transform.position.y + dY, transform.position.z);
    }
    // Method will assign the vectors that define the movement of the alien as well as its location
    public void Init()
    {
        // Randomize the x and y vectors
        dX = Random.Range(-1f, 1f);
        dY = Random.Range(-1f, 1f);


        // Randomize the location
        float location = Random.Range(0f, 1f);

        float x, y;
        if (0.75 < location && location <= 1)
        {
            // Spawn on the right
            y = Random.Range(-100f, -980f);
            x = 1820;
        }
        else if (0.5 < location && location <= 0.75)
        {
            // Spawn on the left
            y = Random.Range(-100f, -980f);
            x = 100;
        }
        else if (0.25 < location && location <= 0.5)
        {
            // Spawn at the top
            y = -100;
            x = Random.Range(100f, 1820f);
        }
        else
        {  // 0 < location && location <= 0.25
           // Spawn at the bottom
            y = -980;
            x = Random.Range(100f, 1820f);
        }

        // Set the transform position
        // transform.position.Set(x, y, transform.position.z);
        transform.position = new Vector3(x, y, transform.position.z);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Calculate the new vector values
        string name = other.gameObject.name.ToLower().Replace(" side", "");
        switch (name)
        {
            case "top":
                // Switch the y velocity from positive to negative
                // dY = dY * (-1) + Random.Range(0f, 0.5f);
                dY = dY * (-1);
                break;
            case "bottom":
                // Switch the y velocity from positive to negative
                // dY = dY * (-1) + Random.Range(0f, 0.5f);
                dY = dY * (-1);
                break;
            case "left":
                // Switch the x velocity from positive to negative
                // dX = dX * (-1) + Random.Range(0f, 0.5f);
                dX = dX * (-1);
                break;
            case "right":
                // Switch the x velocity from positive to negative
                // dX = dX * (-1) + Random.Range(0f, 0.5f);
                dX = dX * (-1);
                break;
            default:
                break;
        }

        // Cap the value of the movement
        float max = 0.4f;
        if (dX < 0)
            dX = (dX < (-1 * max)) ? (-1 * max) : dX;
        else
            dX = (dX > max) ? max : dX;

        if (dY < 0)
            dY = (dY < (-1 * max)) ? (-1 * max) : dY;
        else
            dY = (dY > max) ? max : dY;

    }


    // private void OnTriggerEnter2D(Collider2D other)
    // {



    //     if (other.gameObject.GetType() == typeof(SideCollider))
    //     {
    //         // Cast the object as 
    //         SideCollider collider = other.gameObject.GetComponent<SideCollider>();

    //         // Calculate the new vector values
    //         switch (collider.name)
    //         {
    //             case "TOP":
    //                 // Switch the y velocity from positive to negative
    //                 dY = dY * (-1);

    //                 break;
    //             case "Bottom":
    //                 // Switch the y velocity from positive to negative
    //                 dY = dY * (-1);
    //                 break;
    //             case "Left":
    //                 // Switch the x velocity from positive to negative
    //                 dX = dX * (-1);
    //                 break;
    //             case "Right":
    //                 // Switch the x velocity from positive to negative
    //                 dX = dX * (-1);
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }
}

