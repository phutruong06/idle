using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IdleGunner.Gameplay.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PlusObjectUI : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private float moveOnY = 15f;

        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private Transform originPos;
        [SerializeField, ReadOnly] private CanvasGroup _canvasGroup;


        private void OnValidate()
        {
            TryGetComponent(out _canvasGroup);
        }

        private void Awake()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
        }

        public void AddGold(double amount)
        {
            transform.position = originPos.position;
            _canvasGroup.alpha = 0;
            goldText.text = $"+{CalcUtils.FormatNumber(amount)}";

            DOTween.Sequence().Append(_canvasGroup.DOFade(1, 0.2f))
                              .Append(transform.DOMoveY(originPos.position.y + moveOnY, 0.3f))
                              .AppendInterval(0.2f)
                              .Append(_canvasGroup.DOFade(0, 0.2f));
        }
    }
}