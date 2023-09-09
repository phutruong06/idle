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
    public class ExpPopup : SceneSingleton<ExpPopup>
    {
        [BoxGroup("SETTINGS"), SerializeField] private bool m_initOnStart = false;
        [BoxGroup("SETTINGS"), SerializeField] private bool m_canShow = false;
        [BoxGroup("SETTINGS"), SerializeField] private Canvas m_canvas;
        [BoxGroup("SETTINGS"), SerializeField] private ExpPopupPool m_pool = new ExpPopupPool();

        private void Start()
        {
            if(m_initOnStart)
                m_pool.Create(m_canvas.transform);
        }

        public void Rent(double value, Vector3 position, Quaternion rotation, float endValue = 0.5f, float duration = 0.3f)
        {
            if (!m_canShow)
                return;

            var obj = m_pool.Rent(m_canvas.transform, false);
            obj.rectTransform.anchoredPosition = m_canvas.WorldToCanvasPosition(position);

            obj.transform.rotation = rotation;

            obj.Play(value, endValue, duration);
        }

        public void SetCanShow(bool value) => m_canShow = value;
    }

    [System.Serializable]
    public class ExpPopupPool : ComponentPoolBase<ExpPopupText>
    {
        public ExpPopupPool()
        {
        }

        public ExpPopupPool(ExpPopupText original, int capacity) : base(original, capacity)
        {
        }

        protected override void OnCreate(ExpPopupText instance)
        {
            instance.TryGetComponent<ExpPopupStopTrigger>(out var trigger);
            trigger.Initialize(instance, this);
        }

        protected override void OnRent(ExpPopupText instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected override void OnReturn(ExpPopupText instance)
        {
            instance.gameObject.SetActive(false);
        }

        protected override void OnRelease(ExpPopupText instance)
        {
            GameObject.Destroy(instance.gameObject);
        }

    }
}