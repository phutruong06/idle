using IdleGunner.Core;
using IdleGunner.UI;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI.Popup
{
    public class Popup_Revive : PopupBase
    {
        [Header("SETTINGS")]
        [SerializeField] private float countTime = 15f;
        [SerializeField] private int revivePrice = 20;

        [Header("BUTTONS")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button closeButton2;
        [Space]
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button superReviveButton;

        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI reviveExpText;
        [SerializeField] private TextMeshProUGUI superReviveExpText;
        [SerializeField] private TextMeshProUGUI priceText;

        [Header("SLIDERS")]
        [SerializeField] private Slider timeRemainSlider;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private float pauseTimer = 0f;

        protected override void Awake()
        {
            base.Awake();

            OnShow.AddListener(() =>
            {
                pauseTimer = countTime;

                StartCoroutine(Countdown(pauseTimer));

                // Sliders
                timeRemainSlider.minValue = 0;
                timeRemainSlider.maxValue = countTime;
                timeRemainSlider.value = countTime;

                // Texts
                priceText.color = GameManager.Instance.coreData.playerData.inventory.diamond > revivePrice ? priceText.color = Color.white : priceText.color = Color.red;

                reviveExpText.text = $"{PlayerController.Instance.gameplayExp} Exp";
                superReviveExpText.text = $"{PlayerController.Instance.gameplayExp * 2} Exp";

                // Button
                superReviveButton.interactable = GameManager.Instance.coreData.playerData.inventory.diamond > revivePrice;
            });
        }

        private void Start()
        {
            closeButton?.onClick.AddListener(() =>
            {
                PopupManager.Instance.HidePopup(this);

                PopupManager.Instance.ShowPopup<Popup_EndGame>();
            });

            closeButton2?.onClick.AddListener(() =>
            {
                PopupManager.Instance.HidePopup(this);

                PopupManager.Instance.ShowPopup<Popup_EndGame>();
            });

            reviveButton?.onClick.AddListener(() =>
            {
                // Pause coroutine
                StopAllCoroutines();

                // Load ad
                var rewardAd = AdsManager.CreateAndLoadRewardedAd(AdsManager.AdsUnitIdAndroid.Reward_GameRevive,
                OnAdClosedCallback: (sender, args) =>
                {
                    PopupManager.Instance.HidePopup(this);

                    PlayerController.Instance.playerGameplay.currentHP = PlayerController.Instance.playerGameplay.maxHP.value / 2;

                    // TO DO: Restart wave
                },
                OnAdDidRecordImpressionCallback: (sender, args) =>
                {
                    // Continue Timer
                    StartCoroutine(Countdown(pauseTimer));

                    // TO DO: Continue count
                });
            });

            superReviveButton?.onClick.AddListener(() =>
            {
                StopAllCoroutines();

                PopupManager.Instance.HidePopup(this);

                // TO DO: Restart wave
                PlayerController.Instance.playerGameplay.currentHP = PlayerController.Instance.playerGameplay.maxHP.value;
                PlayerController.Instance.gameplayExp *= 2;
            });
        }

        private IEnumerator Countdown(float duration)
        {
            float normalizedTime = duration;
            while (normalizedTime >= duration)
            {
                pauseTimer = normalizedTime;
                timeRemainSlider.value = normalizedTime;
                normalizedTime -= Time.deltaTime / duration;
                yield return null;
            }
        }
    }
}