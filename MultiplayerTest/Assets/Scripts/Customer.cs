using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;
using FishNet.Object;
using UnityEditor.Experimental.GraphView;

public class Customer : NetworkBehaviour
{
    public Order Order;


    [Header("AI")]
    [SerializeField] CustomerAIState state;
    [SerializeField] float movementSpeed;
    [SerializeField] Vector3 destination;
    NavMeshAgent agent;

    [SerializeField] Seat seat;

    void Start()
    {
        if (!base.IsHost)
            return;

        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (!base.IsHost)
            return;

        switch(state)
        {
            case CustomerAIState.walking:
                HandleWalkingState();
                break;

            case CustomerAIState.sitting:
                HandleSittingState();
                break;
        }
    }

    void HandleWalkingState()
    {
        if (destination == null)
        {
            Debug.LogWarning("Desination not set " + transform.name);
        }
        if (Vector3.Distance(transform.position, destination) <= 0.25f)
        {
            state = CustomerAIState.sitting;
            return;
        }
        
        agent.SetDestination(destination);
        agent.speed = movementSpeed;
    }
    void HandleSittingState()
    {
        agent.isStopped = true;
        transform.position = seat.location;
        transform.eulerAngles = new Vector3(seat.rotation.x, (seat.rotation.y * 180) + 90, seat.rotation.z) ;
    }
    public void SetSeat(Seat _seat)
    {
        seat = _seat;
        
        seat.delivery.GetComponent<Delivery>().SetCustomer(this);
        destination = seat.table.destination;
    }
    // after finishing food/not getting food set new destination and state to walking
    enum CustomerAIState
    {
        walking,
        sitting
    }
}