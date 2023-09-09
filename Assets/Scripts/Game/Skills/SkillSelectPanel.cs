using DG.Tweening;
using IdleGunner.UI.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static IdleGunner.PlayerData;

namespace IdleGunner.Skills
{
    public class SkillSelectPanel : SceneSingleton<SkillSelectPanel>
    {
        [Header("BUTTONS")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button InfoButton;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button unEquipButton;

        [Header("REFERENCES")]
        [SerializeField] private SkillDataCollectionTemplate skillDataTemplate;
        [Space]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("DEBUG")]
        [SerializeField] private RectTransform skillDataTemplateRectTransform;
        [SerializeField] private bool isShowing = false;
        [SerializeField] private Inventory inventory;

        private void OnValidate()
        {
            skillDataTemplate.TryGetComponent(out skillDataTemplateRectTransform);
        }

        private void Start()
        {
            inventory = GameManager.Instance.coreData.playerData.inventory;

            isShowing = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            closeButton.onClick.AddListener(() =>
            {
                isShowing = false;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.DOFade(0, 0.02f);
            });

            InfoButton.onClick.AddListener(() =>
            {

            });

            equipButton.onClick.AddListener(() =>
            {
                SkillsPanel.Instance.EquipSkill(skillDataTemplate.skillId);
                Hide();
            });

            unEquipButton.onClick.AddListener(() =>
            {
                SkillsPanel.Instance.UnEquipSkill(skillDataTemplate.skillId);
                Hide();
            });
        }

        public void Hide()
        {
            if (!isShowing)
                return;

            isShowing = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            skillDataTemplate.transform.DOScale(Vector3.one, 0.05f);
            canvasGroup.DOFade(0, 0.02f);
        }

        public void Show(SkillDataSO skillDataSO, Transform transform)
        {
            bool equiped = false;
            for (int i = 0; i < inventory.skillSlot.Count; i++)
            {
                if (inventory.skillSlot[i].id == skillDataSO.skillData.id)
                {
                    equiped = true;
                    break;
                }
            }

            unEquipButton.gameObject.SetActive(equiped);

            skillDataTemplate.InitializeSelect(skillDataSO);
            skillDataTemplateRectTransform.transform.position = transform.position;
            skillDataTemplate.transform.localScale = Vector3.one;

            if (isShowing)
            {
                skillDataTemplate.transform.DOScale(1.2f, 0.05f);
            }
            else
            {
                DOTween.Sequence().Append(canvasGroup.DOFade(1f, 0.02f))
                  .Append(skillDataTemplate.transform.DOScale(1.2f, 0.05f))
                  .OnComplete(() =>
                  {
                      canvasGroup.interactable = true;
                      canvasGroup.blocksRaycasts = true;
                  });
            }

            isShowing = true;
        }
    }

}