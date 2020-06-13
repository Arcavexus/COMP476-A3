using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace Com.A3Practical.TankVS
{
    public class CollisionHandler : MonoBehaviourPun
    {

        public bool selfDestruct;
        public bool destroyWall;

        private void Awake()
        {
            selfDestruct = true;
            destroyWall = true;
        }

        // handles the collisions for each pellet
        // destroyWall is on by default, selfDestruct can be toggle with the powerup
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Wall")
            {
                if (selfDestruct)
                {
                    photonView.RPC("SelfDestruct", RpcTarget.All);
                }
                if(destroyWall)
                {
                    collision.gameObject.GetComponent<WallManager>().IsEnabled = false;
                }
            }

        }

        [PunRPC]
        private void SelfDestruct()
        {
            Destroy(gameObject);
        }

    }
}

