using MackySoft.XPool;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{
    [RequireComponent(typeof(Bullet))]
    public class BulletStopTrigger : MonoBehaviour
    {
        [Header("DEBUG")]
        [SerializeField, ReadOnly] private Bullet bullet;
        [SerializeField] private IPool<Bullet> m_Pool;

        private void OnValidate()
        {
            TryGetComponent(out bullet);
        }

        private void OnBecameInvisible()
        {
            m_Pool?.Return(bullet);
        }

        internal void Initialize(Bullet ps, IPool<Bullet> pool)
        {
            bullet = ps;
            m_Pool = pool;

            bullet.OnHit.AddListener(() =>
            {
                m_Pool?.Return(bullet);
            });
        }
    }
}