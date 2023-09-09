using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Skills
{
    [System.Serializable]
    public class SerializeSkillBorder : SerializableDictionary<ESkillRare, BorderSprite> { }

    [CreateAssetMenu(fileName = "Skill Border Data", menuName = "Game/Skills/Assets/Skill Border UI")]
    public class SkillBorderSO : ScriptableObject
    {
        [SerializeField] private SerializeSkillBorder skillsBorderCollection = new SerializeSkillBorder();

        private void OnValidate()
        {
            if (!skillsBorderCollection.ContainsKey(ESkillRare.Empty))
                skillsBorderCollection.Add(ESkillRare.Empty, null);
            if (!skillsBorderCollection.ContainsKey(ESkillRare.Normal))
                skillsBorderCollection.Add(ESkillRare.Normal, null);
            if (!skillsBorderCollection.ContainsKey(ESkillRare.Rare))
                skillsBorderCollection.Add(ESkillRare.Rare, null);
            if (!skillsBorderCollection.ContainsKey(ESkillRare.Epic))
                skillsBorderCollection.Add(ESkillRare.Epic, null);
            if (!skillsBorderCollection.ContainsKey(ESkillRare.Legend))
                skillsBorderCollection.Add(ESkillRare.Legend, null);
        }

        public BorderSprite GetBorderSprite(ESkillRare eSkillRare) => skillsBorderCollection[eSkillRare];
    }

    [System.Serializable]
    public class BorderSprite
    {
        [Header("ACTIVE BORDER")]
        public Sprite activeSkill;
        public Sprite activeSkillUpper;
        [Header("COLLECTION BORDER")]
        public Sprite collectionFrame;
        [Header("CCLORS")]
        public Color borderColor = Color.white;
    }
}