using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    Customer customer;

    void OnTriggerEnter(Collider _col)
    {
        if (customer == null)
            return;

        if (_col.GetComponent<Tray>() is Tray tray)
        {
            Item[] content = tray.GetContent();
            if (content.Length != customer.Order.TrayNameOrder.Length)
                return;

            for (int i = 0; i < content.Length; i++)
            {
                // check if name doesn't match
                if (content[i].Info.Name != customer.Order.TrayNameOrder[i])
                {
                    print("wrong order");
                    return;
                }
                if (i == customer.Order.TrayNameOrder.Length - 1)
                    print("is correct borger");
            }
        }
    }
    public void SetCustomer(Customer _customer)
    {
        customer = _customer;
    }
}