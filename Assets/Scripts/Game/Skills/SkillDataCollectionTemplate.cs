using IdleGunner.UI;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Skills
{
    public class SkillDataCollectionTemplate : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField, Expandable] private SkillBorderSO skillBorderSO;

        [Header("CANVAS GROUP")]
        [SerializeField] private CanvasGroup canvasGroup;
        [Header("BUTTONS")]
        [SerializeField] private Button skillButton;
        [Header("IMAGES")]
        [SerializeField] private Image skillImage;
        [SerializeField] private Image frameImage;
        [SerializeField] private Image borderImage;
        [SerializeField] private Image upperBorderImage;
        [SerializeField] private Image isEquipedImage;
        [SerializeField] private Image iscanUpgradeInfoImage;
        [Header("GAME OBJECTS")]
        [SerializeField] private GameObject activeSkillGO;
        [SerializeField] private GameObject canUpgradeGO;
        [SerializeField] private GameObject isOwnedGO;
        [SerializeField] private GameObject isNotOwnedGO;
        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillCardCountText;
        [SerializeField] private TextMeshProUGUI skillLevelText;
        [Header("SLIDERS")]
        [SerializeField] private Slider skillCardSlider;

        [Header("DEBUG")]
        [SerializeField, ReadOnly, Expandable] private SkillDataSO skillDataSO = null;

        public int skillId => skillDataSO.skillData.id;

        public void Refresh()
        {
            skillImage.sprite = skillDataSO.sprite;
            frameImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).collectionFrame;
            borderImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).activeSkill;
            upperBorderImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).activeSkillUpper;

            activeSkillGO.SetActive(skillDataSO.skillTyye == ESkillTpye.Active);
            canUpgradeGO.SetActive(skillDataSO.canUpgrade);
            isEquipedImage.gameObject.SetActive(skillDataSO.skillData.isEmpty);

            isOwnedGO.SetActive(skillDataSO.skillData.skillCard.isOwned);
            isNotOwnedGO.SetActive(!skillDataSO.skillData.skillCard.isOwned);
            canvasGroup.alpha = skillDataSO.skillData.skillCard.isOwned ? 1f : 0.25f;

            skillButton.interactable = skillDataSO.skillData.skillCard.isOwned;
            skillButton.onClick.RemoveAllListeners();
            if (!skillDataSO.skillData.skillCard.isOwned)
            {
                //skillButton.onClick.AddListener(() =>
                //{
                //    SwipeViewMainPanel.Instance.OpenView(SwipeViewMainPanel.EMenuView.Shop);
                //});
                return;
            }

            skillButton.onClick.AddListener(() =>
            {
                SkillSelectPanel.Instance.Show(skillDataSO, this.transform);
            });

            skillNameText.text = skillDataSO.skillName;
            skillCardCountText.text = $"{skillDataSO.skillData.skillCard.cardCount}/{skillDataSO.upgradeRequire}";

            skillCardSlider.minValue = 0;
            skillCardSlider.maxValue = skillDataSO.upgradeRequire;
            skillCardSlider.value = skillDataSO.skillData.skillCard.cardCount;
        }

        public void Initialize(SkillDataSO skillDataSO)
        {
            this.skillDataSO = skillDataSO;

            skillImage.sprite = skillDataSO.sprite;
            frameImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).collectionFrame;
            borderImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).activeSkill;
            upperBorderImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).activeSkillUpper;

            activeSkillGO.SetActive(skillDataSO.skillTyye == ESkillTpye.Active);
            canUpgradeGO.SetActive(skillDataSO.canUpgrade);
            isEquipedImage.gameObject.SetActive(skillDataSO.skillData.isEmpty);

            isOwnedGO.SetActive(skillDataSO.skillData.skillCard.isOwned);
            isNotOwnedGO.SetActive(!skillDataSO.skillData.skillCard.isOwned);
            canvasGroup.alpha = skillDataSO.skillData.skillCard.isOwned ? 1f : 0.25f;

            skillButton?.onClick.RemoveAllListeners();
            
            if (!skillDataSO.skillData.skillCard.isOwned)
                return;

            skillNameText.text = skillDataSO.skillName;
            skillCardCountText.text = $"{skillDataSO.skillData.skillCard.cardCount}/{skillDataSO.upgradeRequire}";

            skillLevelText.text = skillDataSO.skillData.skillCard.skillLevel.ToString();

            skillCardSlider.minValue = 0;
            skillCardSlider.maxValue = skillDataSO.upgradeRequire;
            skillCardSlider.value = skillDataSO.skillData.skillCard.cardCount;
        }

        public void InitializeSelect(SkillDataSO skillDataSO)
        {
            this.skillDataSO = skillDataSO;

            skillImage.sprite = skillDataSO.sprite;
            frameImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).collectionFrame;
            borderImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).activeSkill;
            upperBorderImage.sprite = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).activeSkillUpper;

            activeSkillGO.SetActive(skillDataSO.skillTyye == ESkillTpye.Active);
            canUpgradeGO.SetActive(skillDataSO.canUpgrade);
            iscanUpgradeInfoImage.gameObject.SetActive(skillDataSO.canUpgrade);
            isEquipedImage.color = skillBorderSO.GetBorderSprite(skillDataSO.skillRare).borderColor;

            isOwnedGO.SetActive(skillDataSO.skillData.skillCard.isOwned);
            isNotOwnedGO.SetActive(!skillDataSO.skillData.skillCard.isOwned);
            canvasGroup.alpha = skillDataSO.skillData.skillCard.isOwned ? 1f : 0.25f;

            skillNameText.text = skillDataSO.skillName;
            skillCardCountText.text = $"{skillDataSO.skillData.skillCard.cardCount}/{skillDataSO.upgradeRequire}";

            skillCardSlider.minValue = 0;
            skillCardSlider.maxValue = skillDataSO.upgradeRequire;
            skillCardSlider.value = skillDataSO.skillData.skillCard.cardCount;
        }
    }
}