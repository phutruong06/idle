using IdleGunner.Chapter;
using IdleGunner.Core;
using IdleGunner.Gameplay.Pooler;
using IdleGunner.Gameplay.UI;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleGunner.Gameplay
{
    public class StageSpawner : SceneSingleton<StageSpawner>
    {
        [Header("SETTINGS")]
        [SerializeField] private float spawnMinRadius = 1.5f;
        [SerializeField] private float spawnMaxRadius = 2f;

        [SerializeField] private Transform spherePointOrigin = null;

        [Header("REFERENCES")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private WaveDetailPanel waveDetailPanel;

        [Header("DEBUG")]
        [SerializeField, Expandable] private ChapterSettings chapterSettings = null;
        [SerializeField, ReadOnly] private List<Enemy> spawnedEnemy = new List<Enemy>();
        [SerializeField, ReadOnly] private float[] weights;
        [SerializeField, ReadOnly] private int Level = 0;

        private int EnemiesAlive = 0;
        private int SpawnedEnemies = 0;

        private Wave m_CurrentWave;
        public Wave CurrentWave { get { return m_CurrentWave; } }
        private float m_DelayFactor = 1.0f;

        private void OnValidate()
        {
            playerController = FindObjectOfType<PlayerController>();
            waveDetailPanel = FindObjectOfType<WaveDetailPanel>();
        }

        private void OnDrawGizmos()
        {
            if (!spherePointOrigin)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(spherePointOrigin.position, (float)(spawnMinRadius + playerController.playerGameplay.range.value));

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spherePointOrigin.position, (float)(spawnMaxRadius + playerController.playerGameplay.range.value));
        }

        public void SetupMap(ChapterSettings chapterSettings)
        {
            this.chapterSettings = ScriptableObject.Instantiate(chapterSettings);

            spherePointOrigin = GameObject.Instantiate(this.chapterSettings.Environment).transform;

            for (int i = 0; i < this.chapterSettings.enemyRollTable.rollTableObjects.Count; i++)
            {
                EnemyPool.Instance.CreatePool(this.chapterSettings.enemyRollTable.rollTableObjects[i] as Enemy, 10);
            }

            for (int i = 0; i < this.chapterSettings.BossRollTable.rollTableObjects.Count; i++)
            {
                EnemyPool.Instance.CreatePool(this.chapterSettings.BossRollTable.rollTableObjects[i] as Enemy, 2);
            }

        }

        public void SpawnLoop()
        {
            StartCoroutine(SpawnLoopCo());
        }

        public int currentStage = 1;
        public float currentStageTime = 0;
        public float stageTime = 0;
        public bool isBossStageNextStage = false;
        public bool isCurrentBossStage = false;

        //public class CurrentStageOption
        //{
        //    public float currentStageMaxTime = 0;
        //    public float currentTime = 0;
        //    public bool isBossStage = false;
        //}

        //public List<CurrentStageOption> stageOptions = new List<CurrentStageOption>();

        private IEnumerator SpawnLoopCo()
        {
            currentStage = 1;
            while (!playerController.isDead)
            {
                PopupManager.Instance.GetPopup<Popup_GameplayWave>().ShowNextWave(currentStage);
                int enemyPerWave = chapterSettings.GetEnemyPerWave(currentStage);
                int enemyPerWaveCount = 0;
                float[] delayRangeTime = new float[enemyPerWave];
                int bossSpawnIndex = 0;
                
                for (int i = 0; i < enemyPerWave; i++)
                {
                    delayRangeTime[i] = chapterSettings.spawnDelay;
                }

                currentStageTime = 0;
                stageTime = delayRangeTime.Sum();

                waveDetailPanel.InitNextStage(new WaveDetailPanel.Option()
                {
                    stageNumber = currentStage,
                    isCurrentBossStage = chapterSettings.GetIsBossStage(currentStage),
                    isNextBossStage = chapterSettings.GetIsBossStage(currentStage + 1),
                    stageTime = stageTime,
                    isEndGame = stageTime <= chapterSettings.chapterLenght,
                    enemyAverageAttack = chapterSettings.averageAttackDamage.baseValue,
                    enemyAverageHp = chapterSettings.averageHp.baseValue
                });

                isCurrentBossStage = false;
                if(chapterSettings.GetIsBossStage(currentStage))
                {
                    bossSpawnIndex = UnityEngine.Random.Range(1, enemyPerWave); // index of boss
                    isCurrentBossStage = true;
                }

                Enemy bossEnemy = null;
                while (true)
                {
                    if (enemyPerWaveCount >= enemyPerWave)
                        break;

                    Vector3 lastSpawnPoint = Vector3.zero;
                    yield return new WaitForSeconds(delayRangeTime[enemyPerWaveCount]);
                    currentStageTime += delayRangeTime[enemyPerWaveCount];

                    Enemy enemy = null;
                    if (chapterSettings.GetIsBossStage(currentStage) && enemyPerWaveCount == bossSpawnIndex)
                    {
                        bossEnemy = EnemyPool.Instance.Rent(((Enemy)chapterSettings.BossRollTable.Roll()).GetInstanceID());
                        bossEnemy.transform.position = RandomCircle();
                        bossEnemy.Initalize(playerController);
                        bossEnemy.gameObject.SetActive(true);
                        enemyPerWaveCount++;
                    }
                    else
                    {
                        enemy = EnemyPool.Instance.Rent(((Enemy)chapterSettings.enemyRollTable.Roll()).GetInstanceID());
                        enemy.transform.position = RandomCircle();
                        enemy.Initalize(playerController);
                        enemy.gameObject.SetActive(true);
                        enemyPerWaveCount++;
                    }
                }

                if (chapterSettings.GetIsBossStage(currentStage))
                    yield return new WaitUntil(() => bossEnemy.isDead);

                Debug.Log($"END Wave: {currentStage}");
                GoldPool.Instance.GetAllGold();
                currentStage++;
            }
        }

        private void SpawnWeightedRandomEnemy()
        {
            float Value = Random.value;

            for (int i = 0; i < weights.Length; i++)
            {
                if (Value < weights[i])
                {
                    DoSpawnEnemy(i, RandomCircle());
                    return;
                }

                Value -= weights[i];
            }

            Debug.LogError("Invalid configuration! Could not spawn a Weighted Random Enemy. Did you forget to call ResetSpawnWeights()?");
        }

        private void DoSpawnEnemy(int spawnIndex, Vector3 spawnPosition)
        {
            var enemy = EnemyPool.Instance.Rent(((Enemy)chapterSettings.enemyRollTable.Roll()).GetInstanceID());
        }

        //private IEnumerator SpawnLoop()
        //{
        //    m_DelayFactor = 1.0f;
        //    while (true)
        //    {
        //        foreach (Wave W in stageSetting.waves)
        //        {
        //            m_CurrentWave = W;
        //            Vector3 lastSpawnPoint = Vector3.zero;

        //            foreach (WaveAction A in W.actions)
        //            {
        //                if (A.delay > 0)
        //                    yield return new WaitForSeconds(A.delay * m_DelayFactor);

        //                if (A.message != "")
        //                {
        //                    // TODO: print ingame message
        //                }

        //                if (A.enemy != null && A.spawnCount > 0)
        //                {
        //                    for (int i = 0; i < A.spawnCount; i++)
        //                    {
        //                        // TODO: instantiate A.prefab
        //                        var enemy = EnemyPool.Instance.Rent(A.enemy.GetInstanceID());
        //                        enemy.transform.position = RandomCircle();

        //                        if(Vector3.Distance(lastSpawnPoint, enemy.transform.position) < 2f)
        //                        {
        //                            enemy.transform.position = RandomCircle();
        //                        }

        //                        enemy.gameObject.SetActive(true);
        //                        enemy.Initalize(playerController);
        //                    }
        //                }
        //            }
        //            yield return null;  // prevents crash if all delays are 0
        //        }
        //        m_DelayFactor *= stageSetting.difficultyFactor;
        //        yield return null;  // prevents crash if all delays are 0
        //    }
        //}

        private Vector3 RandomCircle()
        {
            Vector3 pos = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f).normalized * (Random.Range(spawnMinRadius, spawnMaxRadius) + (float)playerController.playerGameplay.range.value);
            pos.y = 0.65f;


            //float ang = Random.value * 360;
            //Vector3 pos;
            //pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            ////pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            //pos.y = 0;
            //pos.z = center.z + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            return pos;
        }
    }
}
