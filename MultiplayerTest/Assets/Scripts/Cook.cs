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

        if (item.Info.Stage == ItemBaseInfo.CookStage.Raw)
        {
            CookObject();
            return;
        }
        if (item.Info.Stage == ItemBaseInfo.CookStage.Cooked)
        {
            BurnObject();
            return;
        }

        
    }

    void CookObject()
    {
        if (cookTime > 0 && cookTimer < cookTime) // can be cooked
        {
            CookObjectServer();
            return;
        }
        else if (cookTimer >= cookTime)
        {
            item.Info.Stage = ItemBaseInfo.CookStage.Cooked;
            item.Info.Name = "CookedBurger";
        }
    }
    void BurnObject()
    {
        if (burnTime > 0 && cookTimer >= burnTime) // can be burnt
        {
            item.Info.Stage = ItemBaseInfo.CookStage.Burnt;

            GameObject fire = GameObject.Instantiate(foodManager.PSFire, transform);
            fire.transform.localPosition = new Vector3();
            InstanceFinder.ServerManager.Spawn(fire);

            BurnObjectServer();
            this.enabled = false;
            return;
        }
    }

    #region ObserverRPCS
    [ObserversRpc]
    void CookObjectObserver()
    {
        rend.material.Lerp(startMat, cookedMat, 0.1f / cookTime);
    }

    // finish cooking (change state to cooked)
    [ObserversRpc]
    void FinishCookObjectObserver()
    {
        
    }

    [ObserversRpc]
    void BurnObjectObserver()
    {
        rend.material = foodManager.BurntMaterial;
    }
    #endregion
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
