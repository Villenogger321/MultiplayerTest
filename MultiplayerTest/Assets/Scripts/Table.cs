using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Table : MonoBehaviour
{
    public int seatCount;
    public Seat[] seats;
    public Vector3 destination;

    void Start()
    {
        for (int i = 0; i < seats.Length; i++)
        {
            seats[i].location = transform.GetChild(1).GetChild(i).position;
            seats[i].rotation = transform.GetChild(1).GetChild(i).rotation;
            seats[i].delivery = transform.GetChild(2).GetChild(i);
            seats[i].table = this;
        }

        destination = transform.GetChild(0).transform.position;
        seatCount = seats.Length;
    }
}
[Serializable]
public class Seat
{
    public bool taken;
    public Vector3 location;
    public Quaternion rotation;
    public Transform delivery;
    public Table table;
}