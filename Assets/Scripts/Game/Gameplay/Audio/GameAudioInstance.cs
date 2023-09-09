using Cysharp.Threading.Tasks;
using IdleGunner.Gameplay.Pooler;
using MackySoft.XPool;
using MackySoft.XPool.Unity.ObjectModel;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdleGunner.Gameplay.Pooler
{
    public class GameAudioInstance : SceneSingleton<GameAudioInstance>
    {
        [BoxGroup("SETTINGS"), SerializeField] private const string poolName = "AudioInstance_Pool";
        [BoxGroup("SETTINGS"), SerializeField] private bool m_initOnStart = true;
        [BoxGroup("SETTINGS"), SerializeField] private bool m_canShow = false;
        [BoxGroup("SETTINGS"), SerializeField] private AudioInstancePool m_pool = new AudioInstancePool();

        [BoxGroup("DEBUG"), SerializeField, ReadOnly] private GameObject poolerObject;
        private void Start()
        {
            poolerObject = new GameObject();
            poolerObject.name = poolName;

            m_canShow = true;

            if (m_initOnStart)
                m_pool.Create(poolerObject.transform);
        }

        public void Rent(Vector3 position, AudioClip audioClip, float pitch = 1, float volume = 1)
        {
            if (!m_canShow)
                return;

            var obj = m_pool.Rent();
            obj.transform.SetParent(poolerObject.transform);
            obj.Play(position, audioClip, pitch, volume);
        }

        public void SetCanRent(bool value) => m_canShow = value;

    }

    [System.Serializable]
    public class AudioInstancePool : ComponentPoolBase<AudioInstanceMono>
    {
        public AudioInstancePool()
        {
        }

        public AudioInstancePool(AudioInstanceMono original, int capacity) : base(original, capacity)
        {
        }

        protected override void OnCreate(AudioInstanceMono instance)
        {
            var trigger = instance.gameObject.AddComponent<AudioInstanceStopTrigger>();
            trigger.Initialize(instance, this);
        }

        protected override void OnRent(AudioInstanceMono instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected override void OnReturn(AudioInstanceMono instance)
        {
            instance.gameObject.SetActive(false);
        }

        protected override void OnRelease(AudioInstanceMono instance)
        {
            GameObject.Destroy(instance.gameObject);
        }

    }
}

public static class AudioInstance
{
    public static void Rent(Vector3 position, AudioClip audioClip, float pitch = 1, float volume = 1)
    {
        GameAudioInstance.Instance.Rent(position, audioClip, pitch, volume);
    }
}