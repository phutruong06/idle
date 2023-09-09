using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Skills
{
    public class ActiveSkillTemplate : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField, Expandable] private SkillBorderSO skillBorderSO;

        [Header("REFERENCES")]
        [SerializeField] private Button skillButton;
        [Space]
        [SerializeField] private Image skillImage;
        [SerializeField] private Image borderImage;
        [SerializeField] private Image upperBorderImage;
        [SerializeField] private TextMeshProUGUI levelText;

        [Header("GAME OBJECTS")]
        [SerializeField] private GameObject EmptyActiveGO;
        [SerializeField] private GameObject activeSkillGO;
        [SerializeField] private GameObject canUpgradeGO;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private SkillDataSO skillDataSO = null;
        [SerializeField, ReadOnly] private bool isEmpty = true;

        public bool empty => isEmpty;

        private void OnDestroy()
        {
            skillButton.onClick.RemoveAllListeners();
        }

        public void Initialize(SkillDataSO skillDataSO, Action callback = null)
        {
            this.skillDataSO = skillDataSO;
            isEmpty = skillDataSO == null;

            EmptyActiveGO.SetActive(isEmpty);
            if (isEmpty) 
                return;

            skillImage.sprite = skillDataSO.sprite;
            borderImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).activeSkill;
            upperBorderImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).activeSkillUpper;

            activeSkillGO.SetActive(skillDataSO.skillTyye == ESkillTpye.Active);
            canUpgradeGO.SetActive(skillDataSO.canUpgrade);
            levelText.text = skillDataSO.skillData.skillCard.skillLevel.ToString();
            skillButton.onClick.RemoveAllListeners();
            skillButton.onClick.AddListener(() =>
            {
                callback?.Invoke();
            });
        }
    }
}