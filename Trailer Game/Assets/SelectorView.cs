using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectorView : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private float speed = 25f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        var selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null) return;

        transform.position = Vector3.Lerp(transform.position, selected.transform.position, speed * Time.deltaTime);

        var otherRect = selected.GetComponent<RectTransform>();

        var horizontalLerp = Mathf.Lerp(rectTransform.rect.size.x, otherRect.rect.size.x, speed * Time.deltaTime);
        var verticalLerp = Mathf.Lerp(rectTransform.rect.size.y, otherRect.rect.size.y, speed * Time.deltaTime);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalLerp);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalLerp);

    }
}
