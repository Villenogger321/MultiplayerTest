using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemBaseInfo Info;
    public bool IsHeld;
}
[Serializable]
public class ItemBaseInfo
{
    public string Name;
    public bool PickupAble;
    
}