using deVoid.Utils;
using IdleGunner.Skills;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.UI.Skills
{
    [System.Serializable]
    public class SerializeSkillDataCollection : SerializableDictionary<SkillDataSO, SkillDataCollectionTemplate> { }

    public class SkillsPanel : SceneSingleton<SkillsPanel>
    {
        [Header("TRANSFORMS")]
        [SerializeField] private Transform activeSkilltransform;
        [SerializeField] private Transform collectionSkilltransform;

        [Header("REFERENCES")]
        [SerializeField] private Button addMoreSlotButton;
        [SerializeField] private TextMeshProUGUI addMoreSlotText;
        [SerializeField] private TextMeshProUGUI skillCardCountText;

        [Header("TEMPLATES")]
        [SerializeField] private ActiveSkillTemplate activeSkillTemplate;
        [SerializeField] private SkillDataCollectionTemplate collectionSkillTemplate;

        [Header("DEBUG")]
        [SerializeField] private List<ActiveSkillTemplate> activeSkillSlotList = new List<ActiveSkillTemplate>();
        [SerializeField, Expandable] private SkillDataCollectionSO skillDataCollectionSO;
        [SerializeField] private SerializeSkillDataCollection skillDataCollectionTemplateList;

        [SerializeField] PlayerData.Inventory inventory;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // Init UI
            InitSkillSlot();
            InitSkillCollection();
            
            // Refersh
            RefreshSkillSlot();
            RefreshSkillDataCollection();
        }

        public void ReloadSkillPanel()
        {
            RefreshSkillSlot();
            RefreshSkillDataCollection();
        }

        // Skill Slots
        private void InitSkillSlot()
        {
            skillDataCollectionSO = GameManager.Instance.skillDataCollection;
            inventory = GameManager.Instance.coreData.playerData.inventory;

            // Add more slot button
            addMoreSlotButton.gameObject.SetActive(inventory.skillSlot.Count < 5);
            float upgradePrice = 60 + inventory.skillSlot.Count * 60;
            addMoreSlotText.color = inventory.diamond >= upgradePrice ? Color.white : Color.red;
            addMoreSlotText.text = CalcUtils.FormatNumber(upgradePrice);

            // Skill slot
            skillCardCountText.text = $"{inventory.skillSlot.Count}/6";
            for (int i = 0; i < inventory.skillSlot.Count; i++)
            {
                ActiveSkillTemplate slot = GameObject.Instantiate<ActiveSkillTemplate>(activeSkillTemplate, activeSkilltransform);
                slot.Initialize(skillDataCollectionSO.GetSkill(inventory.skillSlot[i].id));
                activeSkillSlotList.Add(slot);
            }

            addMoreSlotButton.transform.SetAsLastSibling();
        }
        private void RefreshSkillSlot()
        {
            for (int i = 0; i < inventory.skillSlot.Count; i++)
            {
                activeSkillSlotList[i].Initialize(skillDataCollectionSO.GetSkill(inventory.skillSlot[i].id), () =>
                {
                    for (int k = 0; k < skillDataCollectionTemplateList.Count; k++)
                    {
                        if (skillDataCollectionTemplateList.ElementAt(k).Value.skillId == inventory.skillSlot[i].id)
                        {
                            SkillSelectPanel.Instance.Show(skillDataCollectionSO.GetSkill(inventory.skillSlot[i].id), 
                                                           skillDataCollectionTemplateList.ElementAt(k).Value.transform);
                            break;
                        }
                    }
                });
            }
        }

        // Skill in collection
        private void InitSkillCollection()
        {
            for (int i = 0; i < skillDataCollectionSO.skillData.Count; i++)
            {
                var skillTemplate = GameObject.Instantiate<SkillDataCollectionTemplate>(collectionSkillTemplate, collectionSkilltransform); ;
                var skillDataSO = GameObject.Instantiate(skillDataCollectionSO.skillData[i]);
                //skillDataSO.skillSlot.skillCard = inventory.skillDataCollection[i];

                skillTemplate.Initialize(skillDataSO);
                skillDataCollectionTemplateList.Add(skillDataSO, skillTemplate);
            }
            RefreshSkillDataCollection();
        }

        private void RefreshSkillDataCollection()
        {
            for (int i = 0; i < inventory.skillCardOwned.Count; i++)
            {
                for (int c = 0; c < skillDataCollectionTemplateList.Count; c++)
                {
                    if (inventory.skillCardOwned[i].id == skillDataCollectionTemplateList.ElementAt(c).Key.skillData.id)
                    {
                        skillDataCollectionTemplateList.ElementAt(c).Key.skillData.skillCard = inventory.skillCardOwned[i];
                        skillDataCollectionTemplateList.ElementAt(c).Value.Refresh();
                        break;
                    }
                }
            }
        }

        // Equip & Unequip
        public void EquipSkill(int id)
        {
            for (int i = 0; i < inventory.skillSlot.Count; i++)
            {
                if (inventory.skillSlot[i].id == -1) // check empty
                {
                    inventory.skillSlot[i].Equip(skillDataCollectionSO.GetSkill(id).skillData.skillCard);
                    activeSkillSlotList[i].Initialize(skillDataCollectionSO.GetSkill(id), () =>
                    {
                        for (int k = 0; k < skillDataCollectionTemplateList.Count; k++)
                        {
                            if (skillDataCollectionTemplateList.ElementAt(k).Value.skillId == inventory.skillSlot[i].id)
                            {
                                SkillSelectPanel.Instance.Show(skillDataCollectionSO.GetSkill(inventory.skillSlot[i].id),
                                                               skillDataCollectionTemplateList.ElementAt(k).Value.transform);
                                break;
                            }
                        }
                    });
                    SkillSelectPanel.Instance.Hide();
                    break;
                }
            }
        }
        public void UnEquipSkill(int id)
        {
            for (int i = 0; i < inventory.skillSlot.Count; i++)
            {
                if (inventory.skillSlot[i].id == id)
                {
                    inventory.skillSlot[i].UnEquip();
                    activeSkillSlotList[i].Initialize(null);
                    SkillSelectPanel.Instance.Hide();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// First T: Current Active Skills; Second T: Active Skill in Collections
    /// </summary>
    public class Menu_Refresh_SkillPanel_GameSignal : ASignal<List<SkillSlot>, List<SkillCard>> { }
}
