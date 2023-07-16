using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : NetworkBehaviour
{
    [SerializeField] List<Item> content;
    [SerializeField] float offset;
    [SerializeField] bool isClean = true;
    [SerializeField] Material cleanMat, dirtyMat;
   
    public Item[] GetContent()
    {
        return content.ToArray();
    }

    void OnTriggerEnter(Collider col)
    {
        if (!base.IsHost)
            return;

        if (col.GetComponent<Item>() is Item _item)
        {
            if (_item.Info.TrayAble)
            {
                PlaceOnTrayServer(_item.transform);
            }
        }
    }
    public void CleanTray()
    {
        if (!isClean)
            CleanTrayServer(transform);
    }

    #region ObserverRPCS
    [ObserversRpc]
    void PlaceOnTrayObserver(Transform _obj)
    {
        // set item variable
        Item _item = _obj.GetComponent<Item>();
        
        // set parent & add item to content (currently added items)
        _obj.transform.parent = transform;
        content.Add(_item);
        
        // disable components
        if (_obj.GetComponent<Collider>() is Collider _col)
            _col.enabled = false;

        if (_obj.GetComponent<Rigidbody>() is Rigidbody _rb)
            _rb.isKinematic = true;
        
        // set pos and rot
        _obj.rotation = transform.rotation;
        _obj.localPosition = new Vector3(
                            0,
                            0 + offset,
                            0);
        
        // set offset so next item won't be overlapping
        offset += _item.Info.heightOffset;
        _item.Info.PickupAble = false;  // failsafe not pickupable

    }

    [ObserversRpc]
    void CleanTrayObserver(Transform _obj)
    {
        _obj.GetComponent<MeshRenderer>().material = cleanMat;
        // add soap particles :)
    }
    #endregion


    #region ServerRPCS
    [ServerRpc(RequireOwnership = false)]
    void PlaceOnTrayServer(Transform _obj)
    {
        PlaceOnTrayObserver(_obj);
    }
    [ServerRpc(RequireOwnership = false)]
    void CleanTrayServer(Transform _obj)
    {
        CleanTrayObserver(_obj);
    }
    #endregion
}
