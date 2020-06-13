using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.A3Practical.TankVS
{
    public class Pickup : MonoBehaviourPunCallbacks
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);   
        }
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Tank")
            {
                photonView.RPC("SelfDestruct", RpcTarget.All);
                collision.gameObject.GetComponent<TankManager>().Powerup(); // powerup on pickup
            }
        }

        [PunRPC]
        void SelfDestruct()
        {
            Destroy(gameObject);
        }

    }

    
}

