using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryViewController : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryViewObject;

    [SerializeField] private PlayerController _playerController;

    [SerializeField] private List<ItemSlot> _slots;

    [SerializeField] private ScreenFader _fader;

    [SerializeField] private TMP_Text _itemNameText;

    [SerializeField] private TMP_Text _itemDescriptionText;

    private enum State
    {
        menuClosed,
        menuOpen,
    }

    private State _state;

    public void OnSlotSelected(ItemSlot selectedSlot)
    {
        if (selectedSlot.itemData == null)
        {
            _itemNameText.ClearMesh();
            _itemDescriptionText.ClearMesh();
            return;
        }

        _itemNameText.SetText(selectedSlot.itemData.itemName);
        _itemDescriptionText.SetText(selectedSlot.itemData.description[0]);
    }

    private void OnEnable() => EventBus.Instance.onPickUpItem += OnItemPickedUp;

    private void OnDisable()
    {
        EventBus.Instance.onPickUpItem -= OnItemPickedUp;
    }

    private void OnItemPickedUp(Item itemData)
    {
        foreach (var slot in _slots)
        {
            if (slot.IsEmpty())
            {
                slot.itemData = itemData;
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_state == State.menuClosed)
            {
                EventBus.Instance.PauseGameplay();
                _fader.FadeToBlack(0.3f, FadeToMenuCallback);

                _state = State.menuOpen;
            }
            else if (_state == State.menuOpen)
            {
                _fader.FadeToBlack(0.3f, FadeFromMenuCallback);

                _state = State.menuClosed;
            }
        }
    }

    private void FadeToMenuCallback()
    {
        _inventoryViewObject.SetActive(true);
        _fader.FadeFromBlack(0.3f, null);
    }

    private void FadeFromMenuCallback()
    {
        _inventoryViewObject.SetActive(false);
        _fader.FadeFromBlack(0.3f, EventBus.Instance.ResumeGameplay);
    }
}
