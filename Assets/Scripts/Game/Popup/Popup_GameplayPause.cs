using IdleGunner.UI;
using IdleGunner.UI.Popup;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI.Popup
{

    public class Popup_GameplayPause : PopupBase
    {
        [Header("SETTINGS")]
        [SerializeField, Scene] private int homeScene;

        [Header("BUTTONS")]
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button resumeButton;

        protected override void Awake()
        {
            base.Awake();

            OnShow.AddListener(() =>
            {
                Time.timeScale = 0;
            });
        }

        private void Start()
        {
            leaveButton.onClick.AddListener(() =>
            {
                var yesNoPopup = PopupManager.Instance.GetPopup<Popup_YesNo>();

                yesNoPopup.Initialize(new Popup_YesNo.Options()
                {
                    titleText = "Are you sure?",
                    descriptionText = "You will lose all your current process and loot.",
                    noButtonText = "Leave",
                    yesButtonText = "Stay",
                    // Swap yes no because of the color
                    yesAction = () =>
                    {
                        PopupManager.Instance.HidePopup<Popup_YesNo>();
                    },
                    noAction = async () =>
                    {
                        // Hide yes no popup and pause popup
                        PopupManager.Instance.HidePopup(this);
                        PopupManager.Instance.HidePopup<Popup_YesNo>();

                        // Show loading popup
                        PopupManager.Instance.ShowPopup<Popup_Loading>();

                        // Return game speed to normal
                        Time.timeScale = 1;

                        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(homeScene);
                        while (!asyncLoad.isDone)
                        {
                            await Task.Yield();
                        }

                        await Task.Delay(500);

                        PopupManager.Instance.HidePopup<Popup_Loading>();
                        HomeMenuManager.Instance.ShowMenu();
                    }
                });

                PopupManager.Instance.ShowPopup<Popup_YesNo>();
            });

            resumeButton.onClick.AddListener(() =>
            {
                PopupManager.Instance.HidePopup(this);

                Time.timeScale = PlayerPrefs.HasKey("GameSpeed") ? PlayerPrefs.GetInt("GameSpeed") : 1;
            });
        }
    }

}