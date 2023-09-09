using DG.Tweening;
using IdleGunner.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI
{
    public class Popup_GameplayWave : PopupBase
    {
        public class Option
        {
            public string chapterNumber = "";
            public string chapterName = "";

            public Color chapterNumberTextColor;
            public Color chapterNameTextColor;

            public Color chapterBackgroundColor;
        }

        [Header("SETTINGS")]
        [SerializeField] private float waveTextMoveOnY = 3f;

        [Header("IMAGE")]
        [SerializeField] private Image chapterBackgroundImage;

        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI chapterNumberText;
        [SerializeField] private TextMeshProUGUI chapterNameText;
        [SerializeField] private TextMeshProUGUI waveNumberText;

        [Header("CANVAS GROUP")]
        [SerializeField] private CanvasGroup chapterTitleCanvasGroup;

        [Header("DEBUG")]
        [SerializeField] private Vector3 originWaveTextPosition;

        private void Start()
        {
            originWaveTextPosition = waveNumberText.transform.position;
        }

        public void Initialize(Option option)
        {
            // Chapter Number
            chapterNumberText.text = $"Chapter {option.chapterNumber}";
            chapterNumberText.color = option.chapterNumberTextColor;

            // Chapter Name
            chapterNameText.text = option.chapterName;
            chapterNameText.color = option.chapterNameTextColor;

            // Chapter Background
            chapterBackgroundImage.color = option.chapterBackgroundColor;

            DoAnimationInit();
        }

        public void ShowNextWave(int waveIndex)
        {
            waveNumberText.text = $"Wave {waveIndex}";
            PopupManager.Instance.ShowPopup(this);
            DOAnimationWaveChange();
        }

        private void DoAnimationInit()
        {
            chapterTitleCanvasGroup.alpha = 0;
            DOTween.Sequence().Append(chapterTitleCanvasGroup.DOFade(1, 0.8f))
                              .Append(chapterTitleCanvasGroup.DOFade(0, 0.8f))
                              .OnComplete(() =>
                              {
                                  PopupManager.Instance.HidePopup(this);
                              });
        }

        private void DOAnimationWaveChange()
        {
            // Setup
            waveNumberText.transform.position = originWaveTextPosition;
            waveNumberText.alpha = 0;

            waveNumberText.TryGetComponent(out RectTransform rectTransform);

            DOTween.Sequence().Append(waveNumberText.DOFade(1, 0.3f))
                              .Append(rectTransform.DOPivotY(waveTextMoveOnY, 1f))
                              .Append(waveNumberText.DOFade(0, 0.3f))
                              .OnComplete(() =>
                              {
                                  PopupManager.Instance.HidePopup(this);
                              });
        }
    }
}