using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using UnityEngine;

namespace Com.A3Practical.TankVS
{
    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class TankManager : MonoBehaviourPunCallbacks, IPunObservable
    {

        #region IPunObservable implementation

        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsFiring);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
            }
        }


        #endregion

        #region Public Fields

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion


        #region Private Fields

        [Tooltip("The Pellet GameObject to fire")]
        [SerializeField]
        private GameObject pellet;
        //True, when the user is firing
        bool IsFiring;
        public bool isPoweredUp;

        float firingTimer = 0.0f;
        float fireRate = 0.5f;
        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized q
            if (photonView.IsMine)
            {
                TankManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);

        }



        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Start()
        {
            isPoweredUp = false;

            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            if(photonView.IsMine)
            {
                ProcessInputs();
            }
        }

        #endregion

        #region Custom

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButton("Fire1"))
            {
                // fire every 0.5 seconds
                
                if(Time.time > firingTimer)
                {
                    firingTimer = Time.time;
                    firingTimer += fireRate;
                    GameObject firedPellet = PhotonNetwork.Instantiate(pellet.name, transform.position + transform.forward, Quaternion.identity);

                    // propel the bullet forward and identify its origin to avoid killing its owner
                    firedPellet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);
                    firedPellet.GetComponent<Pellet>().origin = gameObject;

                    // bullets will bounce off walls if the tank is powered up
                    if(isPoweredUp)
                    {
                        firedPellet.GetComponent<CollisionHandler>().selfDestruct = false;
                    }
                }
            }
        }

        public void Powerup()
        {
            isPoweredUp = true;
            Invoke("Powerdown", 5.0f);
        }

        void Powerdown()
        {
            isPoweredUp = false;
        }


        #endregion

    }
}
