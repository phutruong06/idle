using deVoid.Utils;
using IdleGunner.UI.Popup;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Numerics;
using EnergySuite;

namespace IdleGunner.UI
{
    public class TopBar : MonoBehaviour
    {
        [Header("BUTTONS")]
        [SerializeField] private Button settingButton;
        [SerializeField] private Button energyButton;
        [SerializeField] private Button goldButton;
        [SerializeField] private Button diamondButton;

        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI diamondText;

        private UnityEngine.Vector3 originTextScale;

        private int _energy = 0;
        private int energy
        {
            set
            {
                if (value != _energy)
                {
                    _energy = value;
                    energyText.text = $"{_energy}/10";
                    DOTween.Sequence().Append(energyText.transform.DOScale(originTextScale * 1.2f, 0.2f))
                                      .Append(energyText.transform.DOScale(originTextScale, 0.2f));
                }
            }
        }

        private double _gold = 0;
        private double gold
        {
            set
            {
                if (value != _gold)
                {
                    _gold = value;
                    goldText.text = CalcUtils.FormatNumber(_gold);
                    DOTween.Sequence().Append(goldText.transform.DOScale(originTextScale * 1.2f, 0.2f))
                                      .Append(goldText.transform.DOScale(originTextScale, 0.2f))
                                      .SetAutoKill();
                }
            }
        }

        private double _diamond = 0;
        private double diamond
        {
            set
            {
                if (value != _diamond)
                {
                    _diamond = value;
                    diamondText.text = CalcUtils.FormatNumber(_diamond);
                    DOTween.Sequence().Append(diamondText.transform.DOScale(originTextScale * 1.2f, 0.2f))
                                      .Append(diamondText.transform.DOScale(originTextScale, 0.2f))
                                      .SetAutoKill();
                }
            }
        }

        [Space]
        [SerializeField] private int goldTapTime = 0;
        private void Start()
        {
            Signals.Get<Menu_RefreshCurrency_GameSignal>().AddListener(OnSingalRefreshCurrency);

            originTextScale = goldText.transform.localScale;

            settingButton.onClick.AddListener(() =>
            {
                PopupManager.Instance.ShowPopup<Popup_Settings>();
            });

            energyButton.onClick.AddListener(() =>
            {
                SwipeViewMainPanel.Instance.OpenView(SwipeViewMainPanel.EMenuView.Shop);
            });

            goldButton.onClick.AddListener(() =>
            {
                SwipeViewMainPanel.Instance.OpenView(SwipeViewMainPanel.EMenuView.Shop);

                goldTapTime++;
                if (goldTapTime == 5)
                {
                    EnergySuiteManager.Add(TimeValue.Energy, 5);
                    GameManager.Instance.coreData.playerData.inventory.gold += 500;
                    GameManager.Instance.coreData.playerData.inventory.diamond += 500;
                    goldTapTime = 0;
                    HomeMenuManager.Instance.ReloadData();
                }
            });

            diamondButton.onClick.AddListener(() => 
            {
                SwipeViewMainPanel.Instance.OpenView(SwipeViewMainPanel.EMenuView.Shop);
            });
        }

        private void OnDestroy()
        {
            Signals.Get<Menu_RefreshCurrency_GameSignal>().RemoveListener(OnSingalRefreshCurrency);
        }

        private void OnSingalRefreshCurrency(int energy, double gold, double diamond)
        {
            this.energy = energy;
            this.gold = gold;
            this.diamond = diamond;
        }
    }

    public class Menu_RefreshCurrency_GameSignal : ASignal<int, double, double> { }
}