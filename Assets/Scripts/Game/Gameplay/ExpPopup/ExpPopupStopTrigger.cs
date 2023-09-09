using MackySoft.XPool;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{
    [RequireComponent(typeof(ExpPopupText))]
    public class ExpPopupStopTrigger : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField, ReadOnly] private ExpPopupText m_popupText;
        [SerializeField] private IPool<ExpPopupText> m_Pool;

        private void OnValidate()
        {
            TryGetComponent(out m_popupText);
        }

        internal void Initialize(ExpPopupText ps, IPool<ExpPopupText> pool)
        {
            m_popupText = ps;
            m_Pool = pool;

            m_popupText.onEnd.AddListener(() =>
            {
                m_Pool?.Return(m_popupText);
            });
        }
    }
}