using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;
using FishNet.Object;

public class Customer : NetworkBehaviour
{
    [SerializeField] Order Order;


    [Header("AI")]
    [SerializeField] CustomerAIState state;
    [SerializeField] float movementSpeed;
    [SerializeField] Vector3 destination;
    NavMeshAgent agent;
    bool leaving;

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
            return;
        }        
        
        if (Vector3.Distance(transform.position, destination) <= 0.25f)
        {
            if (leaving)
            {
                Destroy(gameObject);     // TEMP destroy, not synced multiplayer
                OrderManager.Instance.UpdateAvailableSeats();
                return;
            }
            else
            {
                SitDown();
                return;
            }
        }
        
        agent.SetDestination(destination);
        agent.speed = movementSpeed;
    }
    void SitDown()
    {
        agent.isStopped = true;
        agent.enabled = false;

        transform.position = seat.location;
        transform.localEulerAngles = new Vector3(seat.rotation.x, seat.rotation.y + 90, seat.rotation.z);
        
        seat.delivery.GetComponent<Delivery>().SetOrderText(Order.OrderName);
        state = CustomerAIState.sitting;
    }
    void HandleSittingState()
    {
        // NEED TO SYNC SITTING POSITION AND ROTATION
        // add timer until leaves + angry
        
        // SET TEXT orderText.text = Order.OrderName;
    }
    public void GetUp()
    {
        leaving = true;
        agent.enabled = true;
        agent.isStopped = false;
        seat.taken = false;

        transform.position = destination;
        SetDestination(new Vector3());  // TEMP destination, 0,0,0

        state = CustomerAIState.walking;
    }
    public void SetSeat(Seat _seat)
    {
        seat = _seat;

        seat.delivery.GetComponent<Delivery>().SetCustomer(this);
        SetDestination(seat.table.destination);
    }
    public void SetDestination(Vector3 _dest)
    {
        destination = _dest;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();   // "temp" solution...

        agent.enabled = true;
    }
    public void SetOrder(Order _order)
    {
        Order = _order;
    }
    public Order GetOrder()
    {
        return Order;
    }
    // after finishing food/not getting food set new destination and state to walking
    enum CustomerAIState
    {
        walking,
        sitting
    }
}