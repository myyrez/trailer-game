using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryViewController : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryViewObject;
    [SerializeField] private GameObject _contextMenuObject;
    [SerializeField] private List<GameObject> _contextMenuOptions;
    [SerializeField] private List<Button> _contextMenuIgnore;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private List<ItemSlot> _slots;
    [SerializeField] private ItemSlot _currentSelectedSlot;
    [SerializeField] private ScreenFader _fader;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _itemDescriptionText;
    private GameObject _firstContextOption;

    public void OnSlotSelected(ItemSlot selectedSlot)
    {
        _currentSelectedSlot = selectedSlot;
        if (selectedSlot.itemData == null)
        {
            _itemNameText.SetText("");
            _itemDescriptionText.SetText("");

            // loop to set all options false
            foreach (var option in _contextMenuOptions)
            {
                option.SetActive(false);
            }
            
            return;
        }

        _itemNameText.SetText(selectedSlot.itemData.itemName);
        _itemDescriptionText.SetText(selectedSlot.itemData.description[0]);

        switch (selectedSlot.itemData.itemClass)
        {
            case Item.ItemClass.weapon:
                // loops all options and set them active/inactive.
                foreach (var option in _contextMenuOptions) option.SetActive(true);
                //set options you don't want to appear false/true.
                _contextMenuOptions[2].SetActive(false);
                _contextMenuOptions[5].SetActive(false);
                break;

            case Item.ItemClass.secondaryEquipment:
                foreach (var option in _contextMenuOptions) option.SetActive(false);
                _contextMenuOptions[0].SetActive(true);
                _contextMenuOptions[4].SetActive(true);
                break;

            case Item.ItemClass.consumable:
                foreach (var option in _contextMenuOptions) option.SetActive(true);
                _contextMenuOptions[0].SetActive(false);
                _contextMenuOptions[1].SetActive(false);
                break;

            case Item.ItemClass.key:
                foreach (var option in _contextMenuOptions) option.SetActive(true);
                _contextMenuOptions[0].SetActive(false);
                _contextMenuOptions[1].SetActive(false);
                _contextMenuOptions[5].SetActive(false);
                break;

            case Item.ItemClass.ammo:
                foreach (var option in _contextMenuOptions) option.SetActive(true);
                _contextMenuOptions[0].SetActive(false);
                _contextMenuOptions[2].SetActive(false);
                break;
        }
    }

    private enum State
    {
        inventoryClosed,
        navigatingInventory,
        contextMenuOpen,
    }

    private State _state;

    private void OnEnable()
    {
        EventBus.Instance.onPickUpItem += OnItemPickedUp;
    }

    private void OnDisable()
    {
        EventBus.Instance.onPickUpItem -= OnItemPickedUp;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_state == State.inventoryClosed)
            {
                EventBus.Instance.PauseGameplay();

                _fader.FadeToBlack(0.3f, FadeToMenuCallback);
                _state = State.navigatingInventory;
            }

            else if (_state == State.navigatingInventory)
            {
                _fader.FadeToBlack(0.3f, FadeFromMenuCallback);
                _state = State.inventoryClosed;
            }

            else if (_state == State.contextMenuOpen)
            {
                _contextMenuObject.SetActive(false);

                foreach (var button in _contextMenuIgnore)
                {
                    button.interactable = true;
                }
                EventSystem.current.SetSelectedGameObject(_currentSelectedSlot.gameObject);
                _state = State.navigatingInventory;
            }
        }

        // open context menu
        if (Input.GetKeyDown(KeyCode.E) && _currentSelectedSlot.itemData != null) 
        {
            if (_state == State.navigatingInventory)
            {
                if (EventSystem.current.currentSelectedGameObject.TryGetComponent<ItemSlot>(out var slot))
                {
                    _state = State.contextMenuOpen;
                    _contextMenuObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(_firstContextOption);

                    foreach (var button in _contextMenuIgnore)
                    {
                        button.interactable = false;
                    }
                }
            }
        }

        // checks the first active option so Selector can jump to it.
        foreach (var option in _contextMenuOptions)
        {
            if (option.activeSelf)
            {
                _firstContextOption = option;
                return;
            }
        }
    }

    public void OnItemPickedUp(Item itemData)
    {
        foreach (var slot in _slots)
        {
            if (slot.IsEmpty())
            {
                slot.itemData = itemData;
                if (slot == _currentSelectedSlot)
                {
                    // I need to repeat this code here because the context menu wouldn't update if you
                    // dind't move the Selector. An item can appear inside the selector after picking it
                    // up, therefore causing problems when selecting.
                    switch (itemData.itemClass)
                    {
                        case Item.ItemClass.weapon:
                            foreach (var option in _contextMenuOptions) option.SetActive(true);
                            _contextMenuOptions[2].SetActive(false);
                            _contextMenuOptions[5].SetActive(false);
                            break;

                        case Item.ItemClass.secondaryEquipment:
                            foreach (var option in _contextMenuOptions) option.SetActive(false);
                            _contextMenuOptions[0].SetActive(true);
                            _contextMenuOptions[4].SetActive(true);
                            break;

                        case Item.ItemClass.consumable:
                            foreach (var option in _contextMenuOptions) option.SetActive(true);
                            _contextMenuOptions[0].SetActive(false);
                            _contextMenuOptions[1].SetActive(false);
                            break;

                        case Item.ItemClass.key:
                            foreach (var option in _contextMenuOptions) option.SetActive(true);
                            _contextMenuOptions[0].SetActive(false);
                            _contextMenuOptions[1].SetActive(false);
                            _contextMenuOptions[5].SetActive(false);
                            break;

                        case Item.ItemClass.ammo:
                            foreach (var option in _contextMenuOptions) option.SetActive(true);
                            _contextMenuOptions[0].SetActive(false);
                            _contextMenuOptions[2].SetActive(false);
                            break;
                    }

                    _itemNameText.SetText(_currentSelectedSlot.itemData.itemName);
                    _itemDescriptionText.SetText(_currentSelectedSlot.itemData.description[0]);
                }
                break;
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
