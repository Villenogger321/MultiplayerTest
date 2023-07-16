using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsHost)
            this.enabled = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (!base.IsHost)
            return;

        if (col.GetComponent<Tray>() is Tray _tray)
        {
            _tray.CleanTray();
            return;
        }
        if (col.GetComponent<Item>() is Item _item)
        {
            if (_item.Info.Stage == ItemBaseInfo.CookStage.Burnt &&
                _item.transform.childCount > 0)
            {
                base.Despawn(_item.transform.GetChild(0).gameObject);
                // sfx putting out fire 
                // vfx smoke && not abruptly ending fire
            }
            return;
        }
    }
}
