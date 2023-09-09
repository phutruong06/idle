using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay
{
    [CreateAssetMenu(fileName = "New Stage Setting Data", menuName = "Game/New Stage Data")]
    public class StageSettingSO : ScriptableObject
    {
        public float difficultyFactor = 0.9f;
        public List<Wave> waves = new List<Wave>();

        private void OnValidate()
        {
            for (int i = 0; i < waves.Count; i++)
            {
                //// Validate if boss spawn time is not higher than stage time
                //if (stageList[i].isBossStage)
                //{
                //    Debug.Assert(stageList[i].bossInStage == null, "No boss set in this stage!", this);

                //    if (stageList[i].bossInStage.spawnTimeAt > stageList[i].stageTime)
                //        stageList[i].bossInStage.spawnTimeAt = stageList[i].stageTime;
                //}

                // Validate if has any enemy set in stage
                if (waves[i].actions.Count <= 0)
                {
                    Debug.Assert(waves[i].actions.Count <= 0, $"There no enemy set in stage number '{i}'.", this);
                    continue;
                }

                //if (wavss[i].stageTime <= 0)
                //{
                //    Debug.Assert(wavss[i].stageTime <= 0, $"Stage time is cannot equal or below 0.", this);
                //    continue;
                //}
            }
        }
    }

    [System.Serializable]
    public class Wave
    {
        [Range(1f, 30f)] public float waveTime = 15f;
        public List<WaveAction> actions;
        //[Range(1f, 30f)] public float stageTime = 15f;
    }

    [System.Serializable]
    public class WaveAction
    {
        public string name;
        public float delay; 
        public Enemy enemy;
        public bool isBoss;
        public int spawnCount;
        public string message;
    }
}
