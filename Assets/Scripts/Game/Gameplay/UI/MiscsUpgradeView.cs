using deVoid.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI
{
    public class MiscsUpgradeView : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI expPerEnemyText;
        [SerializeField] private Button expPerEnemyButton;
        [SerializeField] private TextMeshProUGUI expPerEnemyUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI expPerLevelText;
        [SerializeField] private Button expPerLevelButton;
        [SerializeField] private TextMeshProUGUI expPerLevelUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI GoldPerEnemyText;
        [SerializeField] private Button GoldPerEnemyButton;
        [SerializeField] private TextMeshProUGUI GoldPerEnemyUpgradeCostText;
        [Space]

        [SerializeField] private TextMeshProUGUI GoldPerLevelText;
        [SerializeField] private Button GoldPerLevelButton;
        [SerializeField] private TextMeshProUGUI GoldPerLevelUpgradeCostText;

        private void Awake()
        {
            Signals.Get<Gameplay_Upgrade_GameSignal>().AddListener(UpgradeSignal);
            Signals.Get<Gameplay_Upgrade_GameSignal>().AddListener(UpgradeButtonEnableSignal);

            expPerEnemyButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.ExpPerEnemyMulti);
            });

            expPerLevelButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.ExpPerLevel);
            });

            GoldPerEnemyButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.GoldPerEnemyMulti);
            });

            GoldPerLevelButton.onClick.AddListener(() =>
            {
                PlayerController.Instance.Upgrade(PlayerGameplay.EStatType.GoldPerLevel);
            });
        }

        private void OnDestroy()
        {
            Signals.Get<Gameplay_Upgrade_GameSignal>().RemoveListener(UpgradeSignal);
            Signals.Get<Gameplay_Upgrade_GameSignal>().RemoveListener(UpgradeButtonEnableSignal);

            expPerEnemyButton.onClick.RemoveAllListeners();
            expPerLevelButton.onClick.RemoveAllListeners();
            GoldPerEnemyButton.onClick.RemoveAllListeners();
            GoldPerLevelButton.onClick.RemoveAllListeners();
        }

        private void UpgradeButtonEnableSignal(PlayerController playerController)
        {
            var exp = playerController.gameplayExp;

            expPerEnemyButton.interactable = exp >= playerController.playerGameplay.expPerEnemyMultiplier.nextUpgradeCost;
            expPerLevelButton.interactable = exp >= playerController.playerGameplay.expPerLevel.nextUpgradeCost;
            GoldPerEnemyButton.interactable = exp >= playerController.playerGameplay.goldPerEnemyMultiplier.nextUpgradeCost;
            GoldPerLevelButton.interactable = exp >= playerController.playerGameplay.goldPerLevel.nextUpgradeCost;
        }

        private void UpgradeSignal(PlayerController playerController)
        {
            expPerEnemyText.text = playerController.playerGameplay.expPerEnemyMultiplier.value.ToString();
            expPerEnemyUpgradeCostText.text = playerController.playerGameplay.expPerEnemyMultiplier.nextUpgradeCost.ToString();

            expPerLevelText.text = playerController.playerGameplay.expPerLevel.value.ToString();
            expPerLevelUpgradeCostText.text = playerController.playerGameplay.expPerLevel.nextUpgradeCost.ToString();

            GoldPerEnemyText.text = playerController.playerGameplay.goldPerEnemyMultiplier.value.ToString();
            GoldPerEnemyUpgradeCostText.text = playerController.playerGameplay.goldPerEnemyMultiplier.nextUpgradeCost.ToString();

            GoldPerLevelText.text = playerController.playerGameplay.goldPerLevel.value.ToString();
            GoldPerLevelUpgradeCostText.text = playerController.playerGameplay.goldPerLevel.nextUpgradeCost.ToString();
        }
    }
}
