using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }

    public event Action onOpenInventory;

    public event Action onCloseInventory;

    public event Action<Item> onPickUpItem;

    public event Action onGameplayPaused;

    public event Action onGameplayResumed;

    public void OpenInventory()
    {
        onOpenInventory?.Invoke();
    }

    public void CloseInventory()
    {
        onCloseInventory?.Invoke();
    }

    public void PickUpItem(Item itemData)
    {
        onPickUpItem?.Invoke(itemData);
    }

    public void PauseGameplay()
    {
        onGameplayPaused?.Invoke();
    }
    
    public void ResumeGameplay()
    {
        onGameplayResumed?.Invoke();
    }

    private void Awake()
    {
        Instance = this;
    }
}
