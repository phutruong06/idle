using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class LayoutGroupStartStop : MonoBehaviour
{
    [SerializeField, ReadOnly] private HorizontalLayoutGroup horizontalLayoutGroup;

    private void OnValidate()
    {
        TryGetComponent(out horizontalLayoutGroup);
    }
    private void Awake()
    {
        horizontalLayoutGroup.childControlWidth = true;
        horizontalLayoutGroup.childControlWidth = false;

        Destroy(this);
    }
}
