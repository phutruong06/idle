using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using NaughtyAttributes;
using System;
using UnityEngine.Events;

namespace IdleGunner.Gameplay.Pooler
{
    [RequireComponent(typeof(GoldStopTrigger))]
    public class Gold : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private double _amount = 1;
        [SerializeField] private float rotationSpeed = 100f;

        [Header("AUDIOS")]
        [SerializeField] private AudioClip[] audioClips;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private MeshRenderer _meshRenderer;
        [SerializeField, ReadOnly] private Collider _collider;
        [SerializeField, ReadOnly] private GoldStopTrigger _goldStopTrigger;
        [SerializeField, ReadOnly] private GoldPool _goldPool;
        public double amount => Math.Ceiling(_amount * PlayerController.Instance.playerGameplay.goldPerEnemyMultiplier.value);

        [SerializeField]
        private UnityEvent m_onEnd = new UnityEvent();
        public UnityEvent OnEnd
        {
            get
            {
                return m_onEnd;
            }
            set
            {
                m_onEnd = value;
            }
        }


        public GoldStopTrigger goldStopTrigger => _goldStopTrigger;
        public MeshRenderer meshRenderer => _meshRenderer;
        public GoldPool goldPool;

        private void OnValidate()
        {
            TryGetComponent(out _meshRenderer);
            TryGetComponent(out _collider);
            TryGetComponent(out _goldStopTrigger);
        }

        private void OnEnable()
        {
            _meshRenderer.enabled = true;
            _collider.enabled = true;
        }

        private void Update()
        {
            transform.Rotate(Vector3.left * (rotationSpeed * Time.deltaTime));
        }

        public void Initialize(double amount)
        {
            _amount = amount;
        }

        public void MoveToPlayer(PlayerController playerController)
        {
            goldPool.RemoveOnScreen(this);
            AudioInstance.Rent(transform.position, audioClips[UnityEngine.Random.Range(0, audioClips.Length)], volume: 0.2f);

            DOTween.Sequence(this.gameObject).Append(transform.DOMoveY(transform.position.y + 2f, 0.5f))
                                             .Append(transform.DOMove(playerController.transform.position, 0.5f))
                                             .OnComplete(() => 
                                             { 
                                                 _meshRenderer.enabled = false;
                                                 _collider.enabled = false;
                                                 OnEnd?.Invoke();
                                             });
        }
    }
}
