using IdleGunner.Gameplay;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdleGunner.Skills
{
    public abstract class SkillDataSO : ScriptableObject
    {
        public SkillSlot skillData;

        public string skillName;
        public string skillDescription;
        [ShowAssetPreview(32, 32)] public Sprite sprite;
        
        public ECalculation calculation;
        public ESkillTpye skillTyye;
        public ESkillRare skillRare;

        public float skillActiveTimer = 2f;

        public float baseValue;
        public float increasementValue;
        public UnityEvent OnSkillExecute = new UnityEvent();
        public bool canUpgrade => skillData.skillCard.cardCount >= upgradeRequire;
        public int upgradeRequire
        {
            get
            {
                if (skillData.skillCard.skillLevel == 0)
                    return 1;

                switch (skillRare)
                {
                    case ESkillRare.Normal:
                        return skillData.skillCard.skillLevel * 2;
                    case ESkillRare.Rare:
                        return skillData.skillCard.skillLevel * 4;
                    case ESkillRare.Epic:
                        return skillData.skillCard.skillLevel * 6;
                    case ESkillRare.Legend:
                        return skillData.skillCard.skillLevel * 8;
                    default:
                        return skillData.skillCard.skillLevel * 10;
                }
            }
        }
        public abstract void Execute();
    }

    public enum ESkillTpye { Active, Passive }
    public enum ESkillRare { Empty, Normal, Rare, Epic, Legend }

    [System.Serializable]
    public class SkillSlot
    {
        public int id => skillCard.id;
        public bool isEmpty => skillCard == null;

        public SkillCard skillCard = null;
        public void Equip(SkillCard skillCard)
        {
            this.skillCard = skillCard;
        }

        public void UnEquip()
        {
            skillCard = new SkillCard();
        }
    }

    [System.Serializable]
    public class SkillCard
    {
        public int id = -1;
        public int skillLevel = 0;
        public int cardCount = 0;
        public bool isOwned => skillLevel >= 1 ? true : false;

        public SkillCard()
        {
            id = -1;
            skillLevel = 0;
            cardCount = 0;
        }
    }
}
