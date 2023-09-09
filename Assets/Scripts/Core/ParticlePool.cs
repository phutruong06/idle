using IdleGunner.Core;
using MackySoft.XPool.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Core
{
    public class ParticlePool : Singleton<ParticlePool>
    {

        [SerializeField] private Dictionary<string, ParticleSystemPool> _pools = new Dictionary<string, ParticleSystemPool>();

        public void RentObject(string originObjectId, Transform spawnPosition, Transform parent = null)
        {
            _pools.TryGetValue(originObjectId, out var pool);

            pool.Rent(spawnPosition.position, spawnPosition.rotation, parent);
        }

        public void AddObject(string originObjectId, ParticleSystemPool particleType)
        {
            _pools.Add(originObjectId, particleType);
        }

        public void ReleaseObject(string tag)
        {
            _pools.TryGetValue(tag, out var pool);
            pool.ReleaseInstances(0);
            _pools.Remove(tag);
        }
    }
}

