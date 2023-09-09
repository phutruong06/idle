using MackySoft.XPool;
using MackySoft.XPool.Unity.ObjectModel;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{

    [System.Serializable]
    public class SerializeBulletPool : SerializableDictionary<int, BulletPooler> { }

    public class BulletPool : SceneSingleton<BulletPool>
    {
        [Header("SETTINGS")]
        [SerializeField] private const string poolName = "Bullet_Pool";
        [SerializeField] private SerializeBulletPool m_pools = new SerializeBulletPool();

        [BoxGroup("DEBUG"), SerializeField, ReadOnly] private GameObject poolerObject;
        [BoxGroup("DEBUG"), SerializeField, ReadOnly] private List<GameObject> poolObjectChild = new List<GameObject>();
        protected override void Awake()
        {
            base.Awake();

            poolerObject = new GameObject();
            poolerObject.name = poolName;
        }

        public void CreatePool(Bullet bullet, int capacity = 5)
        {
            if (m_pools.ContainsKey(bullet.GetInstanceID()))
                return;

            var poolParent = new GameObject();
            poolParent.name = $"Pool_{bullet.gameObject.name}";
            poolParent.transform.SetParent(poolerObject.transform);
            poolObjectChild.Add(poolParent);

            m_pools.Add(bullet.GetInstanceID(), new BulletPooler(bullet, capacity, poolParent.transform));

            m_pools[bullet.GetInstanceID()].Create();
        }

        public Bullet Rent(int prefabInstanceId)
        {
            if (m_pools.ContainsKey(prefabInstanceId))
            {
                return m_pools[prefabInstanceId].Rent();
            }

            return null;
        }
    }

    public class BulletPooler : ComponentPoolBase<Bullet>
    {
        Transform parent = null;

        public BulletPooler()
        {
        }

        public BulletPooler(Bullet original, int capacity, Transform parent = null) : base(original, capacity)
        {
            this.parent = parent;
        }

        protected override void OnCreate(Bullet instance)
        {
            instance.TryGetComponent<BulletStopTrigger>(out var trigger);
            trigger.Initialize(instance, this);

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(parent);
        }

        protected override void OnRent(Bullet instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected override void OnReturn(Bullet instance)
        {
            //instance.isDead = false;
            instance.gameObject.SetActive(false);
        }

        protected override void OnRelease(Bullet instance)
        {
            GameObject.Destroy(instance.gameObject);
        }
    }
}