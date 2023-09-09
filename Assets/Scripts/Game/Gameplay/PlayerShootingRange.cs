using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

namespace IdleGunner.Gameplay
{
    public class PlayerShootingRange : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private float shootingRadius = 5f;
        [SerializeField] private LayerMask enemyLayer;

        [Header("DEBUG")]
        [ReadOnly] public Collider[] enemyColliders;

        private void Start()
        {
            enemyColliders = new Collider[32];
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, shootingRadius);
        }

        public void UpdateScaleRangeGameObject(float shootingRadius)
        {
            this.shootingRadius = shootingRadius;
            var scale = transform.localScale;
            scale.x = shootingRadius * 1.9f;
            scale.z = shootingRadius * 1.9f;

            transform.DOScale(scale, 0.5f);
        }

        public bool CheckForEnemy(out int numberOfHit)
        {
            numberOfHit = Physics.OverlapSphereNonAlloc(transform.position, shootingRadius, enemyColliders, enemyLayer);

            return numberOfHit > 0 ? true : false;
        }
    }
}