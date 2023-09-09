using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Chapter
{

    [CreateAssetMenu(fileName = "New Chapter Reward Data", menuName = "Game/Chapter Reward Data")]
    public class ChapterRewardData : ScriptableObject
    {
        public bool isClaimed = false;
        public Reward[] rewards;
    }


    public class Reward : ScriptableObject
    {
    }
}