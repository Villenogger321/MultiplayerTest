using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class Cook : NetworkBehaviour
{
    [SerializeField] Material cookedMat;
    float cookTimer, cookTime, burnTime;

    Material startMat;
    Renderer rend;
    Item item;
    FoodManager foodManager;

    void Start()
    {
        foodManager = FoodManager.Instance;
        rend = GetComponent<Renderer>();
        startMat = rend.material;
        item = GetComponent<Item>();
        cookTime = item.Info.CookTime;
        burnTime = item.Info.BurnTime;
        item.Info.Stage = ItemBaseInfo.CookStage.Raw;
    }
    void OnTriggerStay(Collider col)
    {
        if (!base.IsHost)
            return;
        if (!col.CompareTag("Stove"))
            return;

        cookTimer += Time.fixedDeltaTime;

        CookObject();
        BurnObject();

        
    }

    void CookObject()
    {
        if (cookTime > 0 && cookTimer < cookTime) // can be cooked
        {
            CookObjectServer();
            return;
        }
        else if (cookTimer >= cookTime)
            FinishCookObjectServer();
    }
    void BurnObject()
    {
        if (burnTime > 0 && cookTimer >= burnTime) // can be burnt
        {
            BurnObjectServer();
            return;
        }
    }

    
    [ObserversRpc]
    void CookObjectObserver()
    {
        rend.material.Lerp(startMat, cookedMat, 0.1f / cookTime);
    }

    // finish cooking (change state to cooked)
    [ObserversRpc]
    void FinishCookObjectObserver()
    {
        item.Info.Stage = ItemBaseInfo.CookStage.Cooked;
    }

    [ObserversRpc]
    void BurnObjectObserver()
    {
        rend.material = foodManager.BurntMaterial;
        item.Info.Stage = ItemBaseInfo.CookStage.Burnt;
        GameObject fire = GameObject.Instantiate(foodManager.PSFire, transform);
        InstanceFinder.ServerManager.Spawn(fire);
        this.enabled = false;   
    }

    #region ServerRPCS
    [ServerRpc(RequireOwnership = false)]
    void CookObjectServer()
    {
        CookObjectObserver();
    }

    [ServerRpc(RequireOwnership = false)]
    void FinishCookObjectServer()
    {
        FinishCookObjectObserver();
    }

    [ServerRpc(RequireOwnership = false)]
    void BurnObjectServer()
    {
        BurnObjectObserver();
    }
    #endregion
}
