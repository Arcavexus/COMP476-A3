using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.A3Practical.TankVS
{
    public class Pellet : MonoBehaviourPun
    {

        public GameObject origin;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            Invoke("SelfDestruct", 5);
            
        }

        void SelfDestruct()
        {
            if(photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

    }
}


