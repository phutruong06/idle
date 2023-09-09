using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using MackySoft.XPool.Unity.ObjectModel;
using MackySoft.XPool;
using Cysharp.Threading.Tasks;

namespace IdleGunner.Gameplay.Pooler
{
    public class DamagePopup : SceneSingleton<DamagePopup>
    {
        [BoxGroup("SETTINGS"), SerializeField] private bool m_initOnStart = false;
        [BoxGroup("SETTINGS"), SerializeField] private bool m_canShow = false;
        [BoxGroup("SETTINGS"), SerializeField] private Canvas m_canvas;
        [BoxGroup("SETTINGS"), SerializeField] private DamagePopupPool m_pool = new DamagePopupPool();

        private void Start()
        {
            if(m_initOnStart)
                m_pool.Create(m_canvas.transform);
        }

        public void Rent(double value, bool isCrit, Vector3 position, Quaternion rotation, float endValue = 0.5f, float duration = 0.8f)
        {
            if (!m_canShow)
                return;

            var obj = m_pool.Rent(m_canvas.transform, false);
            obj.rectTransform.anchoredPosition = m_canvas.WorldToCanvasPosition(position);
            obj.transform.rotation = rotation;

            obj.Play(value, isCrit , endValue, duration);
        }

        public void SetCanShow(bool value) => m_canShow = value;
    }

    [System.Serializable]
    public class DamagePopupPool : ComponentPoolBase<DamagePopupText>
    {
        public DamagePopupPool()
        {
        }

        public DamagePopupPool(DamagePopupText original, int capacity) : base(original, capacity)
        {
        }

        protected override void OnCreate(DamagePopupText instance)
        {
            instance.TryGetComponent<DamagePopupStopTrigger>(out var trigger);
            trigger.Initialize(instance, this);
        }

        protected override void OnRent(DamagePopupText instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected override void OnReturn(DamagePopupText instance)
        {
            instance.gameObject.SetActive(false);
        }

        protected override void OnRelease(DamagePopupText instance)
        {
            GameObject.Destroy(instance.gameObject);
        }

    }
}