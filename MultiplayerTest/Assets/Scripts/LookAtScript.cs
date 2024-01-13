using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class LookAtScript : NetworkBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] bool lookAt = true;
    [SerializeField] bool flip;
    [SerializeField] bool targetPlayer;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (targetPlayer)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void Update()
    {
        // if it should currently look at and if target is available
        if (!lookAt || target == null)
            return;


        if (!flip)
            transform.rotation = Quaternion.LookRotation(transform.position -
                                                         target.position);
        else
            transform.rotation = Quaternion.LookRotation(target.position -
                                                         transform.position);
    }
    // why not
    public void SetTarget(Transform _target)
    {
        target = _target;
    }
}
