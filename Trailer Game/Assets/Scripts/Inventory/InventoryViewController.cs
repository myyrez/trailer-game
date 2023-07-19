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
            _itemNameText.ClearMesh();
            _itemDescriptionText.ClearMesh();

            // loop to set all options false
            foreach (var option in _contextMenuOptions)
            {
                option.SetActive(false);
            }
            
            return;
        }

        _itemNameText.SetText(selectedSlot.itemData.itemName);
        _itemDescriptionText.SetText(selectedSlot.itemData.description[0]);

        if (selectedSlot.itemData.itemClass == Item.ItemClass.weapon) 
        {
            // loops all options and set them active/inactive.
            foreach (var option in _contextMenuOptions) option.SetActive(true);

            //set options you don't want to appear false/true.
            _contextMenuOptions[2].SetActive(false);
            _contextMenuOptions[5].SetActive(false);
        }

        if (selectedSlot.itemData.itemClass == Item.ItemClass.secondaryEquipment) 
        {
            foreach (var option in _contextMenuOptions) option.SetActive(false);

            _contextMenuOptions[0].SetActive(true);
            _contextMenuOptions[4].SetActive(true);
        }

        if (selectedSlot.itemData.itemClass == Item.ItemClass.key) 
        {
            foreach (var option in _contextMenuOptions) option.SetActive(true);

            _contextMenuOptions[0].SetActive(false);
            _contextMenuOptions[1].SetActive(false);
            _contextMenuOptions[5].SetActive(false);
        }

        if (selectedSlot.itemData.itemClass == Item.ItemClass.ammo) 
        {
            foreach (var option in _contextMenuOptions) option.SetActive(true);

            _contextMenuOptions[0].SetActive(false);
            _contextMenuOptions[2].SetActive(false);
        }

        if (selectedSlot.itemData.itemClass == Item.ItemClass.consumable) 
        {
            foreach (var option in _contextMenuOptions) option.SetActive(true);

            _contextMenuOptions[0].SetActive(false);
            _contextMenuOptions[1].SetActive(false);
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

    private enum State
    {
        inventoryClosed,
        navigatingInventory,
        contextMenuOpen,
    }

    private State _state;

    private void OnEnable() => EventBus.Instance.onPickUpItem += OnItemPickedUp;

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
        if (Input.GetKeyDown(KeyCode.E)) 
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
