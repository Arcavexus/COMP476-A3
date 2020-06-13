using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace Com.A3Practical.TankVS
{
    public class ZombieController : MonoBehaviourPun, IPunObservable
    {
        private NavMeshAgent _navAgent;
        
        [SerializeField]
        private Vector3[] _patrolPoints;
        private GameObject[] playerTanks;
        private int counter;
        [SerializeField]
        private GameObject pellet;
        private Vector3 initialPosition;

        public enum State {PATROL, CHASING, RETURNING};
        public State currentState;
        
        private GameObject chaseTarget;
        private float detectRadius = 5.0f;
        private float firingTimer = 0.0f;
        public float fireRate = 0.5f;
    

        void Awake()
        {
            // stop zombies from being destroyed on new scene load
            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            _navAgent = GetComponent<NavMeshAgent>();
            _navAgent.enabled = true;
            counter = 0;
            playerTanks = GameObject.FindGameObjectsWithTag("Tank");
            currentState = State.PATROL;
            initialPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            // state machine for zombie AI
            if(currentState == State.PATROL)
            {
                Patrol();
            }
            else if (currentState == State.CHASING)
            {
                ChaseAndShoot();
            }
            else if (currentState == State.RETURNING)
            {
                GoBack();
            }

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(currentState);
            }
            else
            {
                // Network player, receive data
                this.currentState = (State) stream.ReceiveNext();
            }
        }

        void Patrol()
        {
            //allows the zombie to patrol between certain points
            while(counter < _patrolPoints.Length)
            {
                if(_navAgent.remainingDistance <= _navAgent.stoppingDistance)
                {
                    if(_navAgent.hasPath || _navAgent.velocity.sqrMagnitude == 0f)
                    {
                        _navAgent.SetDestination(_patrolPoints[counter]);
                    }
                }
                counter++;
                if(counter == _patrolPoints.Length)
                {
                    counter = 0;
                }
            }
            // if it sees a target (player) it will start chasing it
            foreach(GameObject tank in playerTanks)
            {
                if(tank.activeSelf)
                {
                    Vector3 castDirection = tank.transform.position - transform.position;
                    RaycastHit hit;
                    if(Physics.Raycast(transform.position, castDirection, out hit, detectRadius))
                    {
                        if(hit.transform.tag == "Tank")
                        {
                            chaseTarget = tank;
                            currentState = State.CHASING;
                        }
                    }
                }
            }
        }

        // chase and shoot towards the player target
        void ChaseAndShoot()
        {
            _navAgent.SetDestination(chaseTarget.transform.position);
            Vector3 castDirection = chaseTarget.transform.position - transform.position;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, castDirection, out hit, detectRadius))
            {
                if(hit.transform.tag == "Tank")
                {
                    // fire every 0.5 seconds
                    if(Time.time > firingTimer)
                    {
                        transform.rotation = Quaternion.LookRotation(castDirection);
                        firingTimer = Time.time;
                        firingTimer += fireRate;
                        GameObject firedPellet = PhotonNetwork.Instantiate(pellet.name, transform.position + transform.forward, Quaternion.identity);

                        // add force to the bullets to propel them forward and add their origin to prevent them from killing their owner
                        firedPellet.GetComponent<Rigidbody>().AddForce(transform.forward * 200f);
                        firedPellet.GetComponent<Pellet>().origin = gameObject;
                    }
                }
            }
            // if zombie goes out of a certain tether range, it will return to its initial position
            if(Vector3.Distance(initialPosition, transform.position) > detectRadius)
            {
                currentState = State.RETURNING;
            }
        }

        // returning
        void GoBack()
        {
            _navAgent.SetDestination(initialPosition);
            if(Vector3.Distance(initialPosition, transform.position) == 0f)
            {
                currentState = State.PATROL;
            }
        }

    }

}

