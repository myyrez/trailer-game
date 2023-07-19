using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public string uniqueId;
    public string itemName;
    public List<string> description;
    public Image sprite;
    public ItemClass itemClass;
    public enum ItemClass
    {
        weapon,
        secondaryEquipment,
        consumable,
        key,
        ammo,
    }
}
