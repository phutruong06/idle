using MackySoft.XPool;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{
    public class DamagePopupStopTrigger : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField, ReadOnly] private DamagePopupText m_popupText;
        [SerializeField] private IPool<DamagePopupText> m_Pool;

        private void OnValidate()
        {
            TryGetComponent(out m_popupText);
        }

        internal void Initialize(DamagePopupText ps, IPool<DamagePopupText> pool)
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