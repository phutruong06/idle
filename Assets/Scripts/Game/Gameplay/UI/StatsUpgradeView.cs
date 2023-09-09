using deVoid.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI
{
    public class StatsUpgradeView : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Button hpButton;
        [SerializeField] private TextMeshProUGUI hpUpgradeCostText;
        [Space]
        
        [SerializeField] private TextMeshProUGUI hpRegenText;
        [SerializeField] private Button hpRegenButton;
        [SerializeField] private TextMeshProUGUI hpRegenUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI mpText;
        [SerializeField] private Button mpButton;
        [SerializeField] private TextMeshProUGUI mpUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI mpRegenText;
        [SerializeField] private Button mpRegenButton;
        [SerializeField] private TextMeshProUGUI mpRegenUpgradeCostText;
        private void Awake()
        {
            Signals.Get<Gameplay_Upgrade_GameSignal>().AddListener(UpgradeSignal);
            Signals.Get<Gameplay_Upgrade_GameSignal>().AddListener(UpgradeButtonEnableSignal);

            hpButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.MaxHealth);
            });

            hpRegenButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.HeathRegen);
            });

            mpButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.MaxMana);
            });

            mpRegenButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.ManaRegen);
            });
        }

        private void OnDestroy()
        {
            Signals.Get<Gameplay_Upgrade_GameSignal>().RemoveListener(UpgradeSignal);
            Signals.Get<Gameplay_Upgrade_GameSignal>().RemoveListener(UpgradeButtonEnableSignal);

            hpButton.onClick.RemoveAllListeners();
            hpRegenButton.onClick.RemoveAllListeners();
            mpButton.onClick.RemoveAllListeners();
            mpRegenButton.onClick.RemoveAllListeners();
        }

        private void UpgradeButtonEnableSignal(PlayerController playerController)
        {
            var exp = playerController.gameplayExp;

            hpButton.interactable = exp >= playerController.playerGameplay.maxHP.nextUpgradeCost;
            hpRegenButton.interactable = exp >= playerController.playerGameplay.HPRegen.nextUpgradeCost;
            mpButton.interactable = exp >= playerController.playerGameplay.maxMP.nextUpgradeCost;
            mpRegenButton.interactable = exp >= playerController.playerGameplay.MPRegen.nextUpgradeCost;
        }

        private void UpgradeSignal(PlayerController playerController)
        {
            hpText.text = CalcUtils.FormatNumber(playerController.playerGameplay.maxHP.value);
            hpUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.maxHP.nextUpgradeCost);

            hpRegenText.text = CalcUtils.FormatNumber(playerController.playerGameplay.HPRegen.value) + " /s";
            hpRegenUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.HPRegen.nextUpgradeCost);

            mpText.text = CalcUtils.FormatNumber(playerController.playerGameplay.maxMP.value);
            mpUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.maxMP.nextUpgradeCost);

            mpRegenText.text = CalcUtils.FormatNumber(playerController.playerGameplay.MPRegen.value) + "/s";
            mpRegenUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.MPRegen.nextUpgradeCost);
        }
    }
}