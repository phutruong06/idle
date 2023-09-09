using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using deVoid.Utils;
using IdleGunner.UI.Skills;
using EnergySuite;

namespace IdleGunner.UI
{
    public class HomeMenuManager : SceneSingleton<HomeMenuManager>
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameManager gameManager;

        protected override void Awake()
        {
            base.Awake();

            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;

            gameManager = GameManager.Instance;
        }
        
        private void Start()
        {
            ReloadData();
        }

        public void ReloadData()
        {
            Signals.Get<Menu_RefreshCurrency_GameSignal>().Dispatch(EnergySuiteManager.GetAmount(TimeValue.Energy),
                                                                    gameManager.coreData.playerData.inventory.gold,
                                                                    gameManager.coreData.playerData.inventory.diamond);

            Signals.Get<Menu_Refresh_SkillPanel_GameSignal>().Dispatch(gameManager.coreData.playerData.inventory.skillSlot,
                                                                       gameManager.coreData.playerData.inventory.skillCardOwned);

            SkillsPanel.Instance.ReloadSkillPanel();
        }

        public void ShowMenu()
        {
            _canvasGroup.DOFade(1, 0.3f).OnComplete(() => _canvasGroup.interactable = true);
        }

        public void HideMenu()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.DOFade(0, 0.3f);
        }
    }
}
