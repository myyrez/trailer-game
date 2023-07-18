using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewItemPickup : MonoBehaviour
{
    [SerializeField] private Item _itemData;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKey(KeyCode.F))
        {
            EventBus.Instance.PickUpItem(_itemData);
            Destroy(gameObject);
        }
    }
}
