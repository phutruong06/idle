using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IdleGunner.UI.Popup
{
    public class Popup_YesNo : PopupBase
    {
        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [Space]
        [SerializeField] private TextMeshProUGUI yesButtonText;
        [SerializeField] private TextMeshProUGUI noButtonText;


        [Header("BUTTON")]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        public class Options
        {
            public string titleText = "[No Title!]";
            public string descriptionText = "";
            public string yesButtonText = "";
            public string noButtonText = "";

            public UnityAction yesAction = null;
            public UnityAction noAction = null;
        }

        protected override void Awake()
        {
            base.Awake();
            OnHide.AddListener(() =>
            {
                // Remove last use
                yesButton.onClick.RemoveAllListeners();
                noButton.onClick.RemoveAllListeners();
            });
        }

        public void Initialize(Options options)
        {
            // Remove all listener first
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();

            // Set up
            titleText.text = options.titleText;
            descriptionText.text = options.descriptionText;

            yesButtonText.text = options.yesButtonText;
            noButtonText.text = options.noButtonText;

            yesButton.onClick.AddListener(options.yesAction);
            noButton.onClick.AddListener(options.noAction);
        }
    }

}