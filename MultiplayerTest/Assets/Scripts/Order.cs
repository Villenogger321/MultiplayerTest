using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "MultiplayerTest/New Order")]
public class Order : ScriptableObject
{
    public string OrderName;
    public string[] TrayNameOrder;  // the order of items that are on the tray
    public int OrderDuration = 120;   // how long the order will last until failed
    public int BaseBurgerPrice;
    // add stuff like drink & non tray stuff
}
