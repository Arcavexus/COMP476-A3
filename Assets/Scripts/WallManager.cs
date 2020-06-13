using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.A3Practical.TankVS
{
    public class WallManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public bool IsEnabled;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        
        void Start()
        {
            IsEnabled = true;
        }

        void Update()
        {
            if (IsEnabled)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
                photonView.RPC("Deactivate", RpcTarget.All);
            }   
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsEnabled);
            }
            else
            {
                // Network player, receive data
                this.IsEnabled = (bool)stream.ReceiveNext();
            }
        }

        // Disable the wall and network its state
        [PunRPC]
        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
