using IdleGunner.Gameplay;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Skills
{
    [CreateAssetMenu(fileName = "Skill Settings Data", menuName = "Game/Skill Settings Data")]
    public class SkillDataCollectionSO : ScriptableObject
    {
        [Expandable, AllowNesting] public List<SkillDataSO> skillData = new List<SkillDataSO>();

        public SkillDataSO GetSkill(int id)
        {
            if (id == -1)
                return null;

            SkillDataSO skillSO = null;
            for (int i = 0; i < skillData.Count; i++)
            {
                if (skillData[i].skillData.id == id)
                {
                    skillSO = Instantiate(skillData[i]);
                    skillSO.skillData.skillCard = GameManager.Instance.coreData.playerData.inventory.GetSkillCard(id);
                    break;
                }
            }
            return skillSO;
        }

        private void OnValidate()
        {
            for (int i = 0; i < skillData.Count; i++)
            {
                //skillData[i].skillSlot.id = i;
            }
        }
    }
}
