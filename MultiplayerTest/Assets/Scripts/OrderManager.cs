using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : NetworkBehaviour
{
    [Header("Order Variables")]
    [SerializeField] bool readyForOrder;
    [SerializeField] float nextOrderTimer;   // timer until next order
    [SerializeField] float orderFrequently;  // how often new orders should come

    [SerializeField] Order[] availableOrders;

    [SerializeField] List<Table> tables;

    [SerializeField] List<Seat> allSeats;
    [SerializeField] List<Seat> availableSeats;
    [SerializeField] GameObject CustomerGO;

    [SerializeField] Transform[] spawnpoints;

    public static OrderManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsHost)
            this.enabled = false;
    }

    void Start()
    {
        GetAllSeats();
        UpdateAvailableSeats();
        spawnpoints = transform.GetChild(0).GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (!base.IsHost)
            return;
        if (!readyForOrder)
            return;

        if (nextOrderTimer > 0)
        {
            nextOrderTimer -= Time.deltaTime;
            return;
        }

        UpdateAvailableSeats();
        
        if (availableSeats.Count <= 0)
        {
            readyForOrder = false;
            return;
        }
        nextOrderTimer = orderFrequently;
        NewOrder();
    }
    void NewOrder()
    {
        Order order = GetRandomOrder();

        Customer customer = Instantiate(CustomerGO).GetComponent<Customer>();
        InstanceFinder.ServerManager.Spawn(customer.gameObject);

        customer.SetOrder(order);
        Seat tempSeat = GetRandomAvailableSeat();

        customer.SetSeat(tempSeat);
        tempSeat.taken = true;
    }
    Order GetRandomOrder()
    {
        return availableOrders[Random.Range(0, availableOrders.Length)];
    }
    Seat GetRandomAvailableSeat()
    {
        return availableSeats[Random.Range(0, availableSeats.Count)];
    }
    void GetAllSeats()
    {
        allSeats.Clear();

        for (int i = 0; i < tables.Count; i++)
        {
            for (int e = 0; e < tables[i].seatCount; e++)
            {
                allSeats.Add(tables[i].seats[e]);
            }   // i = table
        }       // e = seat
        UpdateAvailableSeats();
    }
    public void UpdateAvailableSeats()
    {
        availableSeats.Clear();

        for (int i = 0; i < allSeats.Count; i++)
        {
            if (!allSeats[i].taken)
                availableSeats.Add(allSeats[i]);
        }
        if (availableSeats.Count > 0)
            readyForOrder = true;
    }
    public void AddTable(Table _table)
    {
        tables.Add(_table);
        GetAllSeats();
    }
    public void RemoveTable(Table _table)
    {
        tables.Remove(_table);  // will probably need to refresh the list ?
        GetAllSeats();
    }
}
