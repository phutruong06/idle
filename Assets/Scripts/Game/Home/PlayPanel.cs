using EnergySuite;
using IdleGunner.Chapter;
using IdleGunner.Core;
using IdleGunner.Gameplay;
using IdleGunner.UI.Popup;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IdleGunner.UI
{
    public class PlayPanel : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField, Scene] private int gameplayScene;
        [SerializeField] private List<ChapterSettings> chapterSettingCollection = new List<ChapterSettings>();

        [Header("BUTTONS")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button chapterSelectButton;
        [Space]
        [SerializeField] private Button blessingButton;
        [SerializeField] private Button adventurePackButton;
        [SerializeField] private Button piggyBankButton;
        [SerializeField] private Button freeRewardButton;
        [SerializeField] private Button questButton;

        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI lastStageText;
        [SerializeField] private TextMeshProUGUI chapterNumberText;
        [SerializeField] private TextMeshProUGUI chapterNameText;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private int currentChapterSelected = 0;


        private void Start()
        {
            playButton?.onClick.AddListener(() =>
            {
                if (EnergySuiteManager.Use(TimeValue.Energy, 1))
                {
                    StartCoroutine(PlayCo());
                }
                else
                {
                    // TO DO: Show refill energy ads popup
                    //PopupManager.Instance.ShowPopup<Popup_RefillEnergy>();
                }
            });

            questButton?.onClick.AddListener(() =>
            {
            });
        }

        private IEnumerator PlayCo()
        {
            playButton.interactable = false;
            chapterSelectButton.interactable = false;
            blessingButton.interactable = false;
            adventurePackButton.interactable = false;
            piggyBankButton.interactable = false;
            freeRewardButton.interactable = false;
            questButton.interactable = false;

            PopupManager.Instance.ShowPopup<Popup_Loading>();

            var homeScene = SceneManager.GetActiveScene();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameplayScene);
            while (asyncLoad.progress <= 0.9f)
            {
                yield return null;
            }
        }
    }
}