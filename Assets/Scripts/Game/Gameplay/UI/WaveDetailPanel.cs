using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace IdleGunner.Gameplay.UI
{
    public class WaveDetailPanel : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private float textJumpScale;

        [Header("REFERENCES - TEXTS")]
        [SerializeField] private TextMeshProUGUI currentWaveText;
        [SerializeField] private TextMeshProUGUI nextWaveText;

        [SerializeField] private TextMeshProUGUI averageEnemyAttackText;
        [SerializeField] private TextMeshProUGUI averageEnemyHPText;

        [Header("REFERENCES - SLIDERS")]
        [SerializeField] private Slider waveTimerSlider;

        [Header("REFERENFCES - IMAGE")]
        [SerializeField] private Image currentWaveBossImage;
        [SerializeField] private Image nextWaveBossImage;

        [Header("REFERENCES - GAME OBJECTS")]
        [SerializeField] private GameObject endChapterGameObject;
        [SerializeField] private GameObject nextLevelPanelGameObject;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private StageSpawner stageSpawner;
        [SerializeField, ReadOnly] private PlayerController playerController;
        [SerializeField, ReadOnly] private bool isNextBossStage;
        [SerializeField, ReadOnly] private bool isBossStage;
        private void Start()
        {
            stageSpawner = StageSpawner.Instance;
            playerController = PlayerController.Instance;
        }

        public class Option
        {
            public int stageNumber = 0;
            public bool isCurrentBossStage = false;
            public bool isNextBossStage = false;
            public float stageTime = 0;
            public bool isEndGame =false;
            public double enemyAverageAttack = 0;
            public double enemyAverageHp = 0;
        }

        public void InitNextStage(Option option)
        {
            // Game Object
            endChapterGameObject.SetActive(option.isEndGame);
            nextLevelPanelGameObject.SetActive(!option.isEndGame);

            // Text
            currentWaveText.text = option.stageNumber.ToString();
            nextWaveText.text = $"{++option.stageNumber}";
            Debug.Log(++option.stageNumber);

            // Current Boss?
            currentWaveBossImage.gameObject.SetActive(option.isCurrentBossStage);
            currentWaveText.gameObject.SetActive(!option.isCurrentBossStage);

            // Next Boss?
            nextWaveBossImage.gameObject.SetActive(option.isNextBossStage);
            nextWaveText.gameObject.SetActive(!option.isNextBossStage);
            
            // Average Numbers
            averageEnemyAttackText.text = option.enemyAverageAttack.ToString();
            averageEnemyHPText.text = option.enemyAverageHp.ToString();

            // Slider
            waveTimerSlider.minValue = 0;
            waveTimerSlider.maxValue = stageSpawner.stageTime;
            waveTimerSlider.value = 0;

            SliderMove();
        }

        private void SliderMove()
        {
            waveTimerSlider.DOValue(waveTimerSlider.maxValue, waveTimerSlider.maxValue);
        }
    }
}