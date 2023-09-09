using Cysharp.Threading.Tasks;
using DG.Tweening;
using IdleGunner.Gameplay.Pooler;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace IdleGunner.Gameplay
{
    [RequireComponent(typeof(ExpPopupStopTrigger))]
    public class ExpPopupText : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_onEnd = new UnityEvent();
        public UnityEvent onEnd
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

        [Header("DEBUG")]
        [SerializeField, ReadOnly] public RectTransform rectTransform;
        [SerializeField, ReadOnly] private TextMeshProUGUI textMeshPro;
        [SerializeField, ReadOnly] private Color originColor;
        [SerializeField, ReadOnly] private float originFontSize;
        [SerializeField, ReadOnly] private Vector3 originScale;

        private void OnValidate()
        {
            TryGetComponent(out rectTransform);
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        protected void Awake()
        {
            originColor = textMeshPro.color;
            originFontSize = textMeshPro.fontSize;
            originScale = transform.localScale;
        }

        private void OnDestroy()
        {
            DOTween.Kill(this.GetInstanceID());
        }

        public void Play(double value, float endValue = 0.5f, float duration = 0.3f)
        {
            textMeshPro.text = value.ToString();

            transform.localScale = Vector3.zero;

            DOTween.Sequence().Append(transform.DOScale(originScale * 1.2f, duration))
                              .AppendInterval(duration)
                              .OnComplete(() =>
                              {
                                  transform.DOScale(0, duration);
                                  DOTween.Sequence().Append(transform.DOMoveY(transform.position.y + endValue, duration))
                                                    .AppendInterval(duration).OnComplete(() => onEnd?.Invoke());
                              });
            //await Task.Delay((int)(duration * 1000));
            //transform.DOScale(0, duration);
            //transform.DOMoveY(transform.position.y + endValue, duration);

            //await Task.Delay((int)(duration * 1000));

            //onEnd.Invoke();
        }
    }

}