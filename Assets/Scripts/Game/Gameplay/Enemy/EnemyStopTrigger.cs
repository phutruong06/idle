using MackySoft.XPool;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyStopTrigger : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField, ReadOnly] private Enemy enemyMono;
        [SerializeField] private IPool<Enemy> m_Pool;

        private void OnValidate()
        {
            TryGetComponent(out enemyMono);
        }

        public void ReturnToPool()
        {
            m_Pool?.Return(enemyMono);
        }

        internal void Initialize(Enemy ps, IPool<Enemy> pool)
        {
            enemyMono = ps;
            m_Pool = pool;
        }
    }
}
