using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Delivery : MonoBehaviour
{
    Customer customer;

    TextMeshProUGUI orderText;

    void Start()
    {
        orderText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void OnTriggerEnter(Collider _col)
    {
        if (customer == null)
            return;

        if (_col.GetComponent<Tray>() is Tray tray)
        {
            Item[] content = tray.GetContent();
            if (content.Length != customer.GetOrder().TrayNameOrder.Length)
                return;

            for (int i = 0; i < content.Length; i++)
            {
                // check if name doesn't match
                if (content[i].Info.Name != customer.GetOrder().TrayNameOrder[i])
                {
                    print("wrong order");
                    return;
                }
                if (i == customer.GetOrder().TrayNameOrder.Length - 1)  // correct order
                    FinishOrder();
            }
        }
    }
    void FinishOrder()
    {
        SetOrderText("");
        customer.GetUp();
    }
    public void SetCustomer(Customer _customer)
    {
        customer = _customer;
    }
    public void SetOrderText(string _text)
    {
        orderText.text = _text;
    }
}