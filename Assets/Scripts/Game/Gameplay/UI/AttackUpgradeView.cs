using deVoid.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI
{
    public class AttackUpgradeView : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private Button damageButton;
        [SerializeField] private TextMeshProUGUI damageUpgradeCostText;
        [Space]
        
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private Button attackSpeedButton;
        [SerializeField] private TextMeshProUGUI attackSpeedUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI rangeText;
        [SerializeField] private Button rangeButton;
        [SerializeField] private TextMeshProUGUI rangeUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI damageMeterText;
        [SerializeField] private Button damageMeterButton;
        [SerializeField] private TextMeshProUGUI damageMeterUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI critChanceText;
        [SerializeField] private Button critChanceButton;
        [SerializeField] private TextMeshProUGUI critChanceUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI critMultiText;
        [SerializeField] private Button critMultiButton;
        [SerializeField] private TextMeshProUGUI critMultiUpgradeCostText;

        private void Awake()
        {
            Signals.Get<Gameplay_Upgrade_GameSignal>().AddListener(UpgradeSignal);
            Signals.Get<Gameplay_Upgrade_GameSignal>().AddListener(UpgradeButtonEnableSignal);

            damageButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.Damage);
            });

            attackSpeedButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.AttackSpeed);
            });

            rangeButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.Range);
            });

            damageMeterButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.DamageMeter);
            });

            critChanceButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.CritChance);
            });

            critMultiButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.CritMulti);
            });
        }

        private void OnDestroy()
        {
            Signals.Get<Gameplay_Upgrade_GameSignal>().RemoveListener(UpgradeSignal);
            Signals.Get<Gameplay_Upgrade_GameSignal>().RemoveListener(UpgradeButtonEnableSignal);

            damageButton.onClick.RemoveAllListeners();
            attackSpeedButton.onClick.RemoveAllListeners();
            rangeButton.onClick.RemoveAllListeners();
            damageMeterButton.onClick.RemoveAllListeners();
            critChanceButton.onClick.RemoveAllListeners();
            critMultiButton.onClick.RemoveAllListeners();
        }

        private void UpgradeButtonEnableSignal(PlayerController playerController)
        {
            var exp = playerController.gameplayExp;

            damageButton.interactable = exp >= playerController.playerGameplay.damage.currentLevelUpgradeCost;
            attackSpeedButton.interactable = exp >= playerController.playerGameplay.attackSpeed.currentLevelUpgradeCost;
            rangeButton.interactable = exp >= playerController.playerGameplay.range.currentLevelUpgradeCost;
            damageMeterButton.interactable = exp >= playerController.playerGameplay.damageMeter.currentLevelUpgradeCost;
            critChanceButton.interactable = exp >= playerController.playerGameplay.criticalChance.currentLevelUpgradeCost;
            critMultiButton.interactable = exp >= playerController.playerGameplay.criticalMultiplier.currentLevelUpgradeCost;
        }

        private void UpgradeSignal(PlayerController playerController)
        {
            damageText.text = CalcUtils.FormatNumber(playerController.playerGameplay.damage.value);
            damageUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.damage.currentLevelUpgradeCost);

            attackSpeedText.text = playerController.playerGameplay.attackSpeed.value.ToString("0.##");
            attackSpeedUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.attackSpeed.currentLevelUpgradeCost);

            rangeText.text = CalcUtils.FormatNumber(playerController.playerGameplay.range.value) + "m";
            rangeUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.range.currentLevelUpgradeCost);

            damageMeterText.text = playerController.playerGameplay.damageMeter.value.ToString("0.##") + "%/m";
            damageMeterUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.damageMeter.currentLevelUpgradeCost);

            critChanceText.text = playerController.playerGameplay.criticalChance.value.ToString("0.##") + "%";
            critChanceUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.criticalChance.currentLevelUpgradeCost);

            critMultiText.text = playerController.playerGameplay.criticalMultiplier.value.ToString("0.##") + "x";
            critMultiUpgradeCostText.text = CalcUtils.FormatNumber(playerController.playerGameplay.criticalMultiplier.currentLevelUpgradeCost);
        }
    }
}