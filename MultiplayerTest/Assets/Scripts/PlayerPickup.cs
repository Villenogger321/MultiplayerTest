using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using UnityEngine.InputSystem;

public class PlayerPickup : NetworkBehaviour
{
    [SerializeField] Transform pickupSlot;
    [SerializeField] float pickupDistance = 2f;
    [SerializeField] LayerMask pickupMask;

    bool chargeThrow;
    [SerializeField] float throwStrength = 0;
    [SerializeField] int throwIncreasement = 10;
    [SerializeField] int maxThrowStrength = 300;

    [SerializeField] int rotateAmt = 45;
    Transform cam;
    PlayerAction playerAction;
    public override void OnStartClient()    // make sure only owner can run script
    {
        base.OnStartClient();

        if (!base.IsOwner)
            this.enabled = false;

        cam = Camera.main.transform;

        if (pickupSlot == null)
            pickupSlot = transform.GetChild(1);
    }
    public override void OnStopClient()
    {
        base.OnStopClient();

        if (holdingObject())    // drop object on disconnect
        {
            DropObjectServer(getHeldObject());
        }
    }
    void OnInteract(InputAction.CallbackContext context)   // on interact key (e - default)
    {
        if (!base.IsOwner)
            return;

        if (holdingObject())    // holding an object
        {

            if (context.phase == InputActionPhase.Performed)    // on key press
            {
                // start charging throw
                chargeThrow = true;
            }
            else if (context.phase == InputActionPhase.Canceled)    // on key release
            {
                if (chargeThrow == false)
                    return;
                DropObjectServer(getHeldObject());
            }
        }
        else    // not holding an object, try picking one up
        {
            TryPickup();
        }
    }

    void Update()
    {
        if (chargeThrow)
        {
            throwStrength = Mathf.Clamp(throwStrength + (throwIncreasement * Time.deltaTime), 0, maxThrowStrength);
        }
    }
    public void OnScroll(InputAction.CallbackContext context)
    {
        float _rotAmt = context.ReadValue<float>();

        _rotAmt = (2 * (_rotAmt - -120) / (120 - -120)) - 1;
        pickupSlot.Rotate(Vector3.right, _rotAmt * rotateAmt);
    }
    bool holdingObject()
    {
        if (pickupSlot.childCount > 0)
            return true;
        else
            return false;
    }   // if holding object
    Transform getHeldObject()
    {
        return pickupSlot.GetChild(0);
    }   // get the held object

    void TryPickup()    // try to pickup an object (raycast forward)
    {
        if (Physics.Raycast(cam.position, cam.forward,
                            out RaycastHit hit, pickupDistance, pickupMask))
        {
            if (hit.transform.GetComponent<Item>() is Item _item)
            {
                if (!_item.IsHeld && _item.Info.PickupAble)
                {
                    PickupObjectServer(hit.transform, pickupSlot, gameObject);
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]   // tell server to pickup
    void PickupObjectServer(Transform _obj, Transform _pos, GameObject _player)
    {
        PickupObjectObserver(_obj, _pos, _player);
    }
    [ObserversRpc]  // tell all the observers that object was picked up
    void PickupObjectObserver(Transform _obj, Transform _pos, GameObject _player)
    {
        _obj.position = _pos.position;
        //_obj.rotation = _pos.rotation;

        _obj.transform.parent = _pos;
        if (_obj.GetComponent<Collider>() is Collider _col)
            _col.enabled = false;

        if (_obj.GetComponent<Rigidbody>() is Rigidbody _rb)
            _rb.isKinematic = true;

        if (_obj.GetComponent<Item>() is Item _item)
            _item.IsHeld = true;
    }
    [ServerRpc(RequireOwnership = false)]   
    void DropObjectServer(Transform _obj)
    {
        DropObjectObserver(_obj);
    }
    [ObserversRpc]  // tell all the observers that object was dropped
    void DropObjectObserver(Transform _obj)
    {
        _obj.parent = null;

        if (_obj.GetComponent<Collider>() is Collider _col)
            _col.enabled = true;

        if (_obj.GetComponent<Item>() is Item _item)
            _item.IsHeld = false;

        if (_obj.GetComponent<Rigidbody>() is Rigidbody _rb)
        {
            _rb.isKinematic = false;
            _rb.AddForce(cam.forward * throwStrength);
        }
        chargeThrow = false;
        throwStrength = 0;
    }
}
