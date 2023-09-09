using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdleGunner.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SwipeView : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField, ReadOnly] private RectTransform _rect;

        public RectTransform rect => _rect;
        
        private void OnValidate()
        {
            TryGetComponent(out _rect);
        }

        private UnityEvent m_onHide = new UnityEvent();
        public UnityEvent OnHide
        {
            get => m_onHide; set => m_onHide = value;
        }

        private UnityEvent m_onShow = new UnityEvent();
        public UnityEvent OnShow
        {
            get => m_onShow; set => m_onShow = value;
        }

    }
}
