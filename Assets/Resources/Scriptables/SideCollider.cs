using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Controls changing the movement of the Aliens
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     Debug.Log("Collided with: " + Name);
    //     // If other is an Alien

    //     if (other.gameObject.GetType() == typeof(Alien))
    //     {
    //         // Cast the object as 
    //         Alien alien = other.gameObject.GetComponent<Alien>();

    //         // Calculate the new vector values
    //         switch (Name)
    //         {
    //             case "Top":
    //                 // Switch the y velocity from positive to negative
    //                 alien.dY = alien.dY * (-1);

    //                 break;
    //             case "Bottom":
    //                 // Switch the y velocity from positive to negative
    //                 alien.dY = alien.dY * (-1);
    //                 break;
    //             case "Left":
    //                 // Switch the x velocity from positive to negative
    //                 alien.dX = alien.dX * (-1);
    //                 break;
    //             case "Right":
    //                 // Switch the x velocity from positive to negative
    //                 alien.dX = alien.dX * (-1);
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }
}
