using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.UI.Popup
{
    public class Popup_Settings : PopupBase
    {
        [Header("BUTTONS")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button closeButton2;
        [SerializeField] private Button privacyPolicyButton;
        [SerializeField] private Button facebookButton;
        [SerializeField] private Button loadCloudSaveButton;
        [SerializeField] private Button changeLanguageButton;

        [Header("TEXT")]
        [SerializeField] private TextMeshProUGUI gameDetailText;

        protected override void Awake()
        {
            base.Awake();

            OnShow.AddListener(() =>
            {
                gameDetailText.text = $"Game Version: {Application.version} \nUser: " + (GooglePlayManager.Instance.isAuthenticated ? PlayGamesPlatform.Instance.GetUserId() : "Not Logged In");
            });
        }

        private void Start()
        {

            closeButton?.onClick.AddListener(() =>
            {
                PopupManager.Instance.HidePopup(this);
            });

            closeButton2?.onClick.AddListener(() =>
            {
                PopupManager.Instance.HidePopup(this);
            });

            privacyPolicyButton?.onClick.AddListener(() =>
            {

            });

            loadCloudSaveButton?.onClick.AddListener(() =>
            {
                loadCloudSaveButton.interactable = false;

                if (GameManager.HasNetworkConnection())
                    GooglePlayManager.Instance.Authorize();
                else
                {
                    loadCloudSaveButton.interactable = true;
                    return;
                }
            });

            changeLanguageButton?.onClick.AddListener(() =>
            {

            });
        }
    }
}