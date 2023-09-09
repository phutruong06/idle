using DG.Tweening;
using IdleGunner.RollTables;
using IdleGunner.Core;
using IdleGunner.Gameplay.Pooler;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace IdleGunner.Gameplay
{
    [RequireComponent(typeof(EnemyStopTrigger))]
    public class Enemy : RollTableObjectMono
    {
        [Header("CHARACTER SETTINGS")]
        [SerializeField] private float movementSpeed = 2f;
        [SerializeField] private float maxDistanceFromPlayer = 3.5f;
        [SerializeField] private double health = 5;
        [SerializeField] private double baseDamage = 1;
        [SerializeField] private float attackSpeed = 0.5f;
        [SerializeField] private AudioClip[] hitAudios;
        [SerializeField] private AudioClip[] deadAudios;
        [SerializeField] private Animator animator;

        [Header("DROP SETTINGS")]
        [SerializeField] private Vector2Int coinDrop = new Vector2Int(1, 5);
        [SerializeField] private int expPointDrop = 1;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private PlayerController playerController;
        [SerializeField, ReadOnly] private bool _isDead = false;
        [SerializeField, ReadOnly] private float distanceBetweenPlayer = 100f;
        [SerializeField, ReadOnly] private EnemyStopTrigger enemyStopTrigger;

        public bool isDead => _isDead;
        private UnityEvent m_onDead = new UnityEvent();
        public UnityEvent OnDead
        {
            get
            {
                return m_onDead;
            }
            set
            {
                m_onDead = value;
            }
        }

        private void OnValidate()
        {
            TryGetComponent(out enemyStopTrigger);
        }

        private void OnEnable()
        {
            if (playerController == null)
                return;
            distanceBetweenPlayer = 100f;
            StartCoroutine(AttackPlayerCo());
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            if (playerController != null)
                Gizmos.DrawLine(transform.position, playerController.transform.position);

            Gizmos.color = Color.yellow;
            var coinDropPos = transform.position - transform.forward * 2f;
            coinDropPos.y = 0.3f;
            Gizmos.DrawSphere(coinDropPos, 0.3f);
        }

        private void Awake()
        {
            ObjectTileComponent.TryRegisterTileComponent(this);

            // Dead drop gold
            OnDead.AddListener(() =>
            {
                var gold = GoldPool.Instance.Rent();
                gold.transform.position = this.transform.position;

                gold.Initialize(Math.Ceiling(playerController.playerGameplay.goldPerEnemyMultiplier.value * UnityEngine.Random.Range(coinDrop.x, coinDrop.y)));

                var waypoint1 = transform.position - transform.forward * 1.5f;
                waypoint1.y += 2f;

                var waypoint2 = transform.position - transform.forward * 2f;
                waypoint2.y = 0;

                Vector3[] waypoints = new Vector3[2] { waypoint1, waypoint2 };

                gold.transform.DOPath(waypoints, 0.4f);

                StartCoroutine(DeadCo());
            });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
        private void Update()
        {
            MoveTowardPlayer();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ObjectTileComponent.TryGetTileComponent(other.GetInstanceID(), out Bullet bullet))
            {

            }
        }

        private void MoveTowardPlayer()
        {
            if (_isDead)
                return;

            distanceBetweenPlayer = Vector3.Distance(transform.position, playerController.transform.position);
            // Check if the position of the cube and sphere are approximately equal.
            if (distanceBetweenPlayer > maxDistanceFromPlayer)
            {
                var step = movementSpeed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, playerController.transform.position, step);
            }
        }

        private IEnumerator AttackPlayerCo()
        {
            while (!isDead)
            {
                if (playerController.isDead)
                    break;

                yield return new WaitUntil(() => distanceBetweenPlayer <= maxDistanceFromPlayer);

                // shake cam
                CameraController.Instance.Shake(0.05f, 0.05f, 1);

                // send damage
                playerController.TakeDamage(baseDamage);

                // delay
                yield return new WaitForSeconds(attackSpeed);
            }
        }

        private IEnumerator DeadCo()
        {
            var exp = Math.Ceiling(expPointDrop * playerController.playerGameplay.expPerEnemyMultiplier.value);

            playerController.AddExp(exp);
            animator.Play("Dead");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);

            var lastPos = transform.position;
            AudioInstance.Rent(transform.position, deadAudios[UnityEngine.Random.Range(0, deadAudios.Length)], volume: 0.02f);

            //yield return new WaitForSeconds(0.6f);

            ExpPopup.Instance.Rent(exp, lastPos, Quaternion.identity);
            enemyStopTrigger.ReturnToPool();
        }

        public void GetHit(double damage)
        {
            health -= damage;
            AudioInstance.Rent(transform.position, hitAudios[UnityEngine.Random.Range(0, hitAudios.Length)], volume: 0.02f);

            if (health <= 0)
            {
                _isDead = true;
                OnDead?.Invoke();
            }
            else
            {
                // TO DO: SHOW HEALTH BAR
            }
        }

        public void Initalize(PlayerController playerController)
        {
            this.playerController = playerController;

            transform.rotation = Quaternion.LookRotation(playerController.transform.position - transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            _isDead = false;
        }
    }
}
