using MackySoft.XPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{
    [RequireComponent(typeof(Gold))]
    public class GoldStopTrigger : MonoBehaviour
    {
        [SerializeField] private Gold m_gold;
        [SerializeField] private IPool<Gold> m_Pool;

        private void OnValidate()
        {
            TryGetComponent(out m_gold);
        }

        internal void Initialize(Gold ps, IPool<Gold> pool)
        {
            m_gold = ps;
            m_Pool = pool;

            m_gold.OnEnd.AddListener(() =>
            {
                m_Pool?.Return(m_gold);
            });
        }
    }
}
