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
    public bool PickupAble = true;
    public bool TrayAble;   // can be placed on tray,      great variable name :-)
    public float heightOffset;
    [Header("Cooking Info")]
    public float CookTime;
    public float BurnTime;
    public CookStage Stage;


    public enum CookStage
    {
        Raw,
        Cooked,
        Burnt
    }
}
