using EnergySuite;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IdleGunner.UI
{
    public class EnergyUI : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI eneryTimerCountdownText;

        private void Start()
        {
            if (EnergySuiteConfig.StoredInfo[TimeValue.Energy].IsFull())
            {
                eneryTimerCountdownText.fontSize = 48;
                eneryTimerCountdownText.text = "Full!";
            }

            EnergySuiteManager.OnAmountChanged += OnAmountChanged;
            EnergySuiteManager.OnTimeLeftChanged += OnTimeLeftChanged;
        }

        private void OnDestroy()
        {
            EnergySuiteManager.OnAmountChanged -= OnAmountChanged;
            EnergySuiteManager.OnTimeLeftChanged -= OnTimeLeftChanged;
        }

        private void OnAmountChanged(int amount, TimeBasedValue timeBasedValue)
        {
            if (EnergySuiteConfig.StoredInfo[TimeValue.Energy].IsFull())
            {
                eneryTimerCountdownText.fontSize = 48;
                eneryTimerCountdownText.text = "Full!";
            }
        }

        private void OnTimeLeftChanged(TimeSpan timeLeft, TimeBasedValue timeBasedValue)
        {
            eneryTimerCountdownText.fontSize = 38;
            eneryTimerCountdownText.text = $"Next energy in:\n{string.Format("{0:00}:{1:00}", timeLeft.Minutes, timeLeft.Seconds)}";
        }
    }
}
