using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public Item itemData;

    private InventoryViewController _viewController;

    private Image _spawnedItemSprite;

    public void OnSelect(BaseEventData eventData)
    {
        _viewController.OnSlotSelected(this);
    }

    public bool IsEmpty()
    {
        return itemData == null;
    }

    private void OnEnable()
    {
        _viewController = FindObjectOfType<InventoryViewController>();

        if (itemData == null) return;

        _spawnedItemSprite = Instantiate<Image>(itemData.sprite, transform.position, Quaternion.identity, transform);
    }

    private void OnDisable()
    {
        if (_spawnedItemSprite != null) 
        {
            Destroy(_spawnedItemSprite);
        }
    }
}
