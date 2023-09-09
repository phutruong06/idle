using MackySoft.XPool;
using MackySoft.XPool.Unity.ObjectModel;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{
    public class GoldPool : SceneSingleton<GoldPool>
    {
        [BoxGroup("SETTINGS"), SerializeField] private GoldPooler m_pool = new GoldPooler();
        [BoxGroup("SETTINGS"), SerializeField] private bool isCanShow = true;

        [BoxGroup("REFERENCES"), SerializeField] private PlayerController playerController;

        [BoxGroup("DEBUG"), SerializeField, ReadOnly] private List<Gold> onScreenGoldSpawn = new List<Gold>();

        public void SetCanShow(bool value) => isCanShow = value;

        private void OnValidate()
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        private void Start()
        {
            m_pool.Create(this.transform);
        }

        public Gold Rent()
        {
            var gold = m_pool.Rent(this.transform, false);
            gold.goldPool = this;
            gold.meshRenderer.enabled = isCanShow;
            onScreenGoldSpawn.Add(gold);
            return gold;
        }

        public void RemoveOnScreen(Gold gold)
        {
            onScreenGoldSpawn.Remove(gold);
        }

        public void GetAllGold()
        {
            double amount = 0;
            List<Gold> goldGet = new List<Gold>();
            for (int i = 0; i < onScreenGoldSpawn.Count; i++)
            {
                amount += onScreenGoldSpawn[i].amount;
                goldGet.Add(onScreenGoldSpawn[i]);
            }
            for (int i = 0; i < goldGet.Count; i++)
            {
                goldGet[i].MoveToPlayer(playerController);
            }
            playerController.AddGold(amount);
        }
    }

    [System.Serializable]
    public class GoldPooler : ComponentPoolBase<Gold>
    {
        public GoldPooler()
        {
        }

        public GoldPooler(Gold original, int capacity) : base(original, capacity)
        {
        }

        protected override void OnCreate(Gold instance)
        {
            instance.gameObject.SetActive(false);
            instance.goldStopTrigger.Initialize(instance, this);
        }

        protected override void OnRent(Gold instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected override void OnReturn(Gold instance)
        {
            instance.gameObject.SetActive(false);
        }

        protected override void OnRelease(Gold instance)
        {
            GameObject.Destroy(instance.gameObject);
        }

    }
}