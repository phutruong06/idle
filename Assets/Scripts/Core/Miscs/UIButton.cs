using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIButton : Button
{
    #region Sub-Classes
    [System.Serializable]
    public class UIButtonEvent : UnityEvent<PointerEventData.InputButton> { }
    #endregion

    private TextMeshProUGUI _text;
    private UISelectableExtension _uiSelectableExtension;

    public UISelectableExtension uiSelectableExtension => _uiSelectableExtension;

    #region Events
    [Tooltip("Event that fires when a button is initially pressed down")]
    public UIButtonEvent OnButtonPress;
    [Tooltip("Event that fires when a button is released")]
    public UIButtonEvent OnButtonRelease;
    [Tooltip("Event that continually fires while a button is held down")]
    public UIButtonEvent OnButtonHeld;
    #endregion

    private bool _pressed;
    private PointerEventData _heldEventData;


    private Vector3 vec3Default;
    private Vector2 buttonSize;

    protected override void Awake()
    {
        vec3Default = transform.localScale;
        buttonSize = transform.GetComponent<RectTransform>().rect.size;

        TryGetComponent(out _uiSelectableExtension);
    }

    protected virtual void Update()
    {
        if (!_pressed)
            return;

        if (OnButtonHeld != null)
        {
            OnButtonHeld.Invoke(_heldEventData.button);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _pressed = false;
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        Color color;
        switch (state)
        {
            case Selectable.SelectionState.Normal:
                color = this.colors.normalColor;
                break;
            case Selectable.SelectionState.Highlighted:
                color = this.colors.highlightedColor;
                break;
            case Selectable.SelectionState.Pressed:
                color = this.colors.pressedColor;
                break;
            case Selectable.SelectionState.Disabled:
                color = this.colors.disabledColor;
                break;
            default:
                color = this.colors.normalColor;
                break;
        }
        if (base.gameObject.activeInHierarchy)
        {
            switch (this.transition)
            {
                case Selectable.Transition.ColorTint:
                    ColorTween(color * this.colors.colorMultiplier, instant); 
                    break;
            }
        }
    }

    private void ColorTween(Color targetColor, bool instant)
    {
        if (this.targetGraphic == null)
        {
            this.targetGraphic = this.image;
        }
        if (_text == null)
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        base.image.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
        _text.CrossFadeColor(targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (!interactable)
            return;

        transform.DOScale(new Vector3(1.05f, 1.05f, 1), 0.1f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (!interactable)
            return;

        transform.DOScale(vec3Default, 0.1f);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (!interactable)
            return;

        transform.DOScale(new Vector3(0.97f, 0.97f, 1), 0.1f);
        if (OnButtonPress != null)
        {
            OnButtonPress.Invoke(eventData.button);
        }
        _pressed = true;
        _heldEventData = eventData;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (!interactable)
            return;

        transform.DOScale(vec3Default, 0.1f);
        if (OnButtonRelease != null)
        {
            OnButtonRelease.Invoke(eventData.button);
        }
        _pressed = false;
        _heldEventData = null;

    }
}
