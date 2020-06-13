using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.A3Practical.TankVS
{
    public class DestroyWallOnCollide : MonoBehaviour
    {

        private void Start()
        {
            
        }
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Wall")
            {
                collision.gameObject.GetComponent<WallManager>().IsEnabled = false;
            }
        
        }

    }
}


