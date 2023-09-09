using IdleGunner.Gameplay;
using IdleGunner.RollTables;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Chapter
{
    [CreateAssetMenu(fileName = "New Chapter Setting", menuName = "Game/Chapter Setting")]
    public class ChapterSettings : ScriptableObject
    {
        public enum EGameplayType { Static }

        // Fields
        public ChapterContent chapterContent;
        public int chapterLenght = 20;
        public int rewardEachLevels = 5;
        private int index;
        private bool chapterUnlocked;
        private bool chapterCleared;

        public EGameplayType gameplayType;
        public AdvancedStat averageAttackDamage;
        public AdvancedStat averageHp;

        public ChapterEnvironment Environment;
        [Expandable] public RollTable enemyRollTable;
        [Expandable] public RollTable BossRollTable;
        public int BossEveryWave;
        public Vector2Int _enemiesPerWave;
        [SerializeField] private Vector2 _spawnDelay;
        [Expandable] public ChapterRewardData[] ChapterRewards;

        public bool SetChapterUnlocked(bool value) => chapterUnlocked = value;
        public float spawnDelay => Random.Range(_spawnDelay.x, _spawnDelay.y);
        public bool GetIsBossStage(int currentWave) => (currentWave % BossEveryWave) == 0;
        public int GetEnemyPerWave(int currentWave)
        {
            int range = _enemiesPerWave.y - _enemiesPerWave.x; // 30 - 20 = 10
            int average = chapterLenght / range; // 2 - each 2 stage, enemy spawn will increase

            int waveFactor = currentWave / average;

            return _enemiesPerWave.x + waveFactor;
        }
    }

    [System.Serializable]
    public class ChapterContent
    {
        public string levelNumber;
        public string levelName;
        public string description;
        [ShowAssetPreview(32, 32), AllowNesting] public Sprite sprite;
        public Color levelNummberTextColor;
        public Color levelNameTextColor;
        public Color levelBackgroundColor;

    }
}