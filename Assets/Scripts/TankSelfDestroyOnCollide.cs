using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.A3Practical.TankVS
{
    public class TankSelfDestroyOnCollide : MonoBehaviourPun
    {
        // destroy tank if it is hit by a bullet that is not its own
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Pellet")
            {
                GameObject pellet = collision.gameObject;
                if(pellet.GetComponent<Pellet>().origin != gameObject)
                {
                    photonView.RPC("SelfDestruct", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        private void SelfDestruct()
        {
            gameObject.SetActive(false);
        }
    }
}
