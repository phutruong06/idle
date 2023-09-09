using deVoid.Utils;
using IdleGunner.Core;
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
    public class Popup_EndGame : PopupBase
    {
        [Header("SETTINGS")]
        [SerializeField, Scene] private int homeScene;

        [Header("BUTTONS")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button claimMoreGoldButton;

        protected override void Awake()
        {
            base.Awake();

            OnShow.AddListener(() =>
            {
            });
        }

        private void Start()
        {
            continueButton.onClick.AddListener(async () =>
            {
                // Hide end game popup
                PopupManager.Instance.HidePopup(this);

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
            });

            claimMoreGoldButton.onClick.AddListener(() =>
            {
                var ad = AdsManager.CreateAndLoadRewardedAd(AdsManager.AdsUnitIdAndroid.Reward_ClaimMoreGold);

                // TO DO: Increase gold claimed and continue to home menu
            });
        }
    }
}