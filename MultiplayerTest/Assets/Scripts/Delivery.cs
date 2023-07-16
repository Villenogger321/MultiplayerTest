using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    Customer customer;
    void Start()
    {
        customer = GetComponent<Customer>();    // temp
    }

    void Update()
    {
        
    }
    void OnTriggerEnter(Collider _col)
    {
        if (_col.GetComponent<Tray>() is Tray tray)
        {
            Item[] content = tray.GetContent();
            print(content.Length);
            print(customer.Order.TrayNameOrder.Length);
            if (content.Length != customer.Order.TrayNameOrder.Length)
                return;

            for (int i = 0; i < content.Length; i++)
            {
                print(i);
                // check if name doesn't match
                if (content[i].Info.Name != customer.Order.TrayNameOrder[i])
                {
                    print(content[i].Info.Name);
                    print(customer.Order.TrayNameOrder[i]);
                    print("wrong order");
                    return;
                }
                if (i == customer.Order.TrayNameOrder.Length - 1)
                    print("is correct borger");
            }
        }
    }
}