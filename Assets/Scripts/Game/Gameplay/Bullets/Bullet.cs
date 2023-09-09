using IdleGunner.Core;
using IdleGunner.Gameplay.Pooler;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdleGunner.Gameplay
{
    [RequireComponent(typeof(BulletStopTrigger))]
    public class Bullet : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private float bulletSpeed = 5f;

        [Header("REFERENCES")]
        [SerializeField] private AudioClip _audioClip;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private Rigidbody _rigidbody;

        private UnityEvent m_onHit = new UnityEvent();
        public UnityEvent OnHit
        {
            get
            {
                return m_onHit;
            }
            set
            {
                m_onHit = value;
            }
        }


        [System.Serializable]
        public class Option
        {
            public double damage;
            public double range;
            public double critChance;
            public double critMultiplier;
            public double damageMeter;
        }

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private Option option;

        private void OnValidate()
        {
            TryGetComponent(out _rigidbody);
        }

        private void Awake()
        {
            ObjectTileComponent.RegisterTileComponent(this);
        }

        private void OnEnable()
        {
            Fire();
        }

        private void OnDisable()
        {
            _rigidbody.velocity = Vector3.zero;
        }

        private void Update()
        {
            transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform.position, transform.forward + transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                if (enemy.isDead)
                    return;

                double damage = Math.Round(option.damage + (option.damage * option.damageMeter / 100 * option.range));

                if (UnityEngine.Random.Range(0f, 100f) < option.critChance)
                {
                    damage *= option.critMultiplier;

                    DamagePopup.Instance.Rent(damage, true, transform.position, Quaternion.identity, 0.5f, 0.3f);
                }
                else
                {
                    DamagePopup.Instance.Rent(damage, false, transform.position, Quaternion.identity, 0.5f, 0.3f);
                }

                enemy.GetHit(damage);

                OnHit.Invoke();
            }
        }


        public void Fire()
        {
            //_rigidbody.velocity = transform.forward + transform.position;
            //_rigidbody.AddForce((transform.TransformDirection(transform.forward)) * bulletSpeed);
        }

        public void Shoot(Transform target, Option option)
        {
            Vector3 targetDirection = target.position - transform.position;
            var lookRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = lookRotation;

            this.option = option;
            AudioInstance.Rent(transform.position, _audioClip, 0.8f, 0.2f);
        }
    }
}