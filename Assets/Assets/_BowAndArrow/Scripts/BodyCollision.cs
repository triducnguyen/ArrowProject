using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollision : MonoBehaviour
{
    public Player player;
    public Transform head;
    public Transform feet;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "HitBox")
        {
            player.Health -= 10;
        }
        Debug.Log(collision.gameObject.tag);
            Debug.Log("being hit"); 
    }


    void FixedUpdate()
    {
        gameObject.transform.position = new Vector3(head.position.x, feet.position.y, head.position.z);
    }
}
