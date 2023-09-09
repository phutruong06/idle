using deVoid.Utils;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI
{

    public class PlayerInfo : MonoBehaviour
    {
        [Header("PLUS OBJECT")]
        [SerializeField] private PlusObjectUI plusObjectUI;

        [Header("SLIDERS")]
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Slider mpSlider;

        [Header("TEXT")]
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI expText;

        private Vector3 originGoldTextScale;
        private Vector3 originExpTextScale;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private double _gold = 0;
        private double gold
        {
            set
            {

                if (value != _gold)
                {
                    plusObjectUI.AddGold(value - _gold);
                    _gold = value;
                    DOTween.Sequence().Append(goldText.transform.DOScale(originGoldTextScale * 1.2f, 0.2f))
                                      .Append(goldText.transform.DOScale(originGoldTextScale, 0.2f));
                }
            }
        }

        [SerializeField, ReadOnly] private double _exp = 0;
        private double exp
        {
            set
            {
                if (value != _exp)
                {
                    _exp = value;
                    DOTween.Sequence().Append(expText.transform.DOScale(originExpTextScale * 1.2f, 0.2f))
                                      .Append(expText.transform.DOScale(originExpTextScale, 0.2f));
                }
            }
        }


        private void Awake()
        {
            Signals.Get<Gameplay_PlayerInfo_GameSignal>().AddListener(UpdatePlayerInfo);
            originGoldTextScale = goldText.transform.localScale;
            originExpTextScale = expText.transform.localScale;
        }

        private void OnDestroy()
        {
            Signals.Get<Gameplay_PlayerInfo_GameSignal>().RemoveListener(UpdatePlayerInfo);
        }

        private void UpdatePlayerInfo(PlayerController playerController)
        {
            // HP Slider
            hpSlider.minValue = 0;
            hpSlider.maxValue = 1;
            hpSlider.value = (float)(playerController.playerGameplay.currentHP / playerController.playerGameplay.maxHP.value);

            // MP Slider
            mpSlider.minValue = 0;
            mpSlider.maxValue = 1;
            mpSlider.value = (float)(playerController.playerGameplay.currentMP / playerController.playerGameplay.maxMP.value);

            // DOTWEEN
            gold = playerController.gammeplayGold;
            exp = playerController.gameplayExp;

            // Texts
            hpText.text = $"{CalcUtils.FormatNumber(playerController.playerGameplay.currentHP)}/{CalcUtils.FormatNumber(playerController.playerGameplay.maxHP.value)}";
            goldText.text = CalcUtils.FormatNumber(playerController.gammeplayGold);
            expText.text = CalcUtils.FormatNumber(playerController.gameplayExp);
            
            Signals.Get<Gameplay_Upgrade_GameSignal>().Dispatch(playerController);
        }
    }

    public class Gameplay_PlayerInfo_GameSignal : ASignal<PlayerController> { }
}