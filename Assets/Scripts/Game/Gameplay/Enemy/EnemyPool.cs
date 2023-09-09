using MackySoft.XPool.Unity.ObjectModel;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{
    [System.Serializable]
    public class SerializeEnemyPool : SerializableDictionary<int, EnemyPooler> { }

    public class EnemyPool : SceneSingleton<EnemyPool>
    {
        [Header("SETTINGS")]
        [SerializeField] private const string poolName = "Enemy_Pool";
        [SerializeField] private SerializeEnemyPool m_pools = new SerializeEnemyPool();

        [BoxGroup("DEBUG"), SerializeField, ReadOnly] private GameObject poolerObject;
        [BoxGroup("DEBUG"), SerializeField, ReadOnly] private List<GameObject> poolObjectChild = new List<GameObject>();
        protected override void Awake()
        {
            base.Awake();

            poolerObject = new GameObject();
            poolerObject.name = poolName;
        }

        public void CreatePool(Enemy enemy, int capacity = 5)
        {
            if (m_pools.ContainsKey(enemy.GetInstanceID()))
                return;

            var poolParent = new GameObject();
            poolParent.name = $"Pool_{enemy.gameObject.name}";
            poolParent.transform.SetParent(poolerObject.transform);
            poolObjectChild.Add(poolParent);

            m_pools.Add(enemy.GetInstanceID(), new EnemyPooler(enemy, capacity, poolParent.transform));

            m_pools[enemy.GetInstanceID()].Create();
        }

        public Enemy Rent(int prefabInstanceId)
        {
            if (m_pools.ContainsKey(prefabInstanceId))
            {
                return m_pools[prefabInstanceId].Rent();
            }

            return null;
        }
    }

    [System.Serializable]
    public class EnemyPooler : ComponentPoolBase<Enemy>
    {
        [SerializeField] private Transform parent = null;

        public EnemyPooler()
        {
        }

        public EnemyPooler(Enemy original, int capacity, Transform parent = null) : base(original, capacity)
        {
            this.parent = parent;
        }

        protected override void OnCreate(Enemy instance)
        {
            instance.TryGetComponent<EnemyStopTrigger>(out var trigger);
            trigger.Initialize(instance, this);

            instance.transform.SetParent(parent);
            instance.gameObject.SetActive(false);
        }

        protected override void OnRent(Enemy instance)
        {
        }

        protected override void OnReturn(Enemy instance)
        {
            instance.gameObject.SetActive(false);
            //instance.isDead = false;
        }

        protected override void OnRelease(Enemy instance)
        {
            GameObject.Destroy(instance.gameObject);
        }

    }

}
