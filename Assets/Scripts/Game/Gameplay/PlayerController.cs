using deVoid.Utils;
using IdleGunner.Core;
using IdleGunner.Gameplay.Pooler;
using IdleGunner.Gameplay.UI;
using IdleGunner.Gameplay.UI.Popup;
using IdleGunner.Skills;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace IdleGunner.Gameplay
{
    public class PlayerController : SceneSingleton<PlayerController>
    {
        [Header("SETTINGS")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask goldLayer;
        [SerializeField] private float getGoldSphereRadius = 5f;

        [Header("REFERENCES")]
        [SerializeField] private PlayerShoot playerShoot = null;
        [SerializeField] private PlayerShootingRange playerShootingRange = null;
        [SerializeField] private Animator animator = null;

        [Header("PREFABS")]
        [SerializeField] private Bullet bulletPrefab;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private bool _isDead = false;
        [SerializeField, ReadOnly] private bool _isAttacking = false;

        [SerializeField, ReadOnly] private Enemy _currentLockOnEnemy;
        public Enemy currentLockOnEnemy
        {
            get => _currentLockOnEnemy;
            set
            {
                if (value != _currentLockOnEnemy)
                    _currentLockOnEnemy = value;

                _canAttack = false;
                RotateTowardEnemy(_currentLockOnEnemy.transform);
                if (_currentLockOnEnemy.isDead)
                {
                    _canAttack = false;
                    _currentLockOnEnemy = null;
                }
            }
        }
        [SerializeField, ReadOnly] private double _gameplayGold = 0;
        [SerializeField, ReadOnly] private double _gameplayExp = 0;
        [SerializeField, ReadOnly] private bool _isRevive = false;
        [SerializeField, ReadOnly] private bool _canAttack = false;

        public PlayerGameplay playerGameplay = new PlayerGameplay();

        public double gammeplayGold => _gameplayGold;
        public double gameplayExp { get => _gameplayExp; set => _gameplayExp = value; }
        public bool isDead => _isDead;

        private CancellationTokenSource ctk = new CancellationTokenSource();

        private void OnValidate()
        {
            playerShootingRange = GetComponentInChildren<PlayerShootingRange>();
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            BulletPool.Instance.CreatePool(bulletPrefab, 5);
            GameManager.Instance.InitializeGameplay();
            playerShootingRange.UpdateScaleRangeGameObject((float)playerGameplay.range.value);

            AddExp(500);
        }

        private void Update()
        {
            Signals.Get<Gameplay_PlayerInfo_GameSignal>().Dispatch(this);

            if (_isDead)
                return;

            GetGold();
            
            if (playerShootingRange.CheckForEnemy(out int numberOfHit))
            {
                float rangeBetweenEnemy = 100f;
                int index = 0;

                for (int i = 0; i < numberOfHit; i++)
                {
                    float distance = Vector3.Distance(playerShootingRange.enemyColliders[i].transform.position, playerShootingRange.transform.position);
                    if (distance < rangeBetweenEnemy)
                    {
                        rangeBetweenEnemy = distance;
                        index = i;
                    }
                }
                currentLockOnEnemy = playerShootingRange.enemyColliders[index].GetComponent<Enemy>();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(mouseToWorldPosition, 0.4f);

            Gizmos.DrawWireSphere(hit.point, getGoldSphereRadius);
        }

        Vector3 mouseToWorldPosition = Vector3.zero;
        RaycastHit hit;
        private void GetGold()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
                {
                    mouseToWorldPosition = hit.point;
                    Debug.DrawLine(ray.origin, hit.point, Color.red);

                    var goldInSideRadius = Physics.OverlapSphere(hit.point, getGoldSphereRadius, goldLayer);
                    double goldValue = 0;
                    for (int i = 0; i < goldInSideRadius.Length; i++)
                    {
                        goldInSideRadius[i].TryGetComponent(out Gold gold);
                        Debug.Log(gold.gameObject.name);
                        gold.MoveToPlayer(this);
                        goldValue += gold.amount;
                    }
                    AddGold(goldValue);
                }
            }
        }
        public void AddGold(double amount)
        {
            _gameplayGold += Math.Ceiling(amount * playerGameplay.goldPerEnemyMultiplier.baseValue);
        }
        public void AddExp(double amount)
        {
            _gameplayExp += amount;
        }

        public void SetupPlayer(PlayerGameplay playerGameplay, List<SkillSlot> skillDataList)
        {
            // Player gameplay data
            this.playerGameplay = playerGameplay.Clone();

            // TO DO: Player skill data
            if (skillDataList.Count > 0)
            {

            }

            playerGameplay.Initialize();
            Signals.Get<Gameplay_Upgrade_GameSignal>().Dispatch(this);
            StartPlayer();
        }

        public void TakeDamage(double amount)
        {
            playerGameplay.currentHP -= amount;

            if (playerGameplay.currentHP <= 0)
            {
                Death();
            }
        }

        private void Death()
        {
            _isDead = true;

            StopAllCoroutines();

            // Handle revive popup
            if (!_isRevive)
            {
                PopupManager.Instance.ShowPopup<Popup_Revive>();
            }
            else
            {
                PopupManager.Instance.ShowPopup<Popup_EndGame>();
            }
        }

        public void Upgrade(PlayerGameplay.EStatType statType)
        {
            switch (statType)
            {
                case PlayerGameplay.EStatType.Damage:
                    if (_gameplayExp < playerGameplay.damage.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.damage.nextUpgradeCost;
                    playerGameplay.damage.LevelUp();
                    break;
                case PlayerGameplay.EStatType.AttackSpeed:
                    if (_gameplayExp < playerGameplay.attackSpeed.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.attackSpeed.nextUpgradeCost;
                    playerGameplay.attackSpeed.LevelUp();
                    break;
                case PlayerGameplay.EStatType.Range:
                    if (_gameplayExp < playerGameplay.range.nextUpgradeCost)
                        return; 
                    _gameplayExp -= playerGameplay.range.nextUpgradeCost;
                    playerGameplay.range.LevelUp();
                    CameraController.Instance.IncreaseOrthSize(playerGameplay.range.value);
                    playerShootingRange.UpdateScaleRangeGameObject((float)playerGameplay.range.value);
                    break;
                case PlayerGameplay.EStatType.DamageMeter:
                    if (_gameplayExp < playerGameplay.damageMeter.nextUpgradeCost)
                        return; 
                    _gameplayExp -= playerGameplay.damageMeter.nextUpgradeCost;
                    playerGameplay.damageMeter.LevelUp();
                    break;
                case PlayerGameplay.EStatType.CritChance:
                    if (_gameplayExp < playerGameplay.criticalChance.nextUpgradeCost)
                        return; 
                    _gameplayExp -= playerGameplay.criticalChance.nextUpgradeCost;
                    playerGameplay.criticalChance.LevelUp();
                    break;
                case PlayerGameplay.EStatType.CritMulti:
                    if (_gameplayExp < playerGameplay.criticalMultiplier.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.criticalMultiplier.nextUpgradeCost;
                    playerGameplay.criticalMultiplier.LevelUp();
                    break;
                case PlayerGameplay.EStatType.MaxHealth:
                    if (_gameplayExp < playerGameplay.maxHP.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.maxHP.nextUpgradeCost;
                    playerGameplay.maxHP.LevelUp();
                    playerGameplay.HPRegen.maxLevel = Mathf.RoundToInt((float)(playerGameplay.maxHP.value / playerGameplay.HPRegen.perLevelAddBaseValue));
                    break;
                case PlayerGameplay.EStatType.HeathRegen:
                    if (_gameplayExp < playerGameplay.HPRegen.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.HPRegen.nextUpgradeCost;
                    playerGameplay.HPRegen.LevelUp();
                    break;
                case PlayerGameplay.EStatType.MaxMana:
                    if (_gameplayExp < playerGameplay.maxMP.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.maxMP.nextUpgradeCost;
                    playerGameplay.maxMP.LevelUp();
                    break;
                case PlayerGameplay.EStatType.ManaRegen:
                    if (_gameplayExp < playerGameplay.MPRegen.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.MPRegen.nextUpgradeCost;
                    playerGameplay.MPRegen.LevelUp();
                    playerGameplay.MPRegen.maxLevel = Mathf.RoundToInt((float)(playerGameplay.maxMP.value / playerGameplay.MPRegen.perLevelAddBaseValue));
                    break;
                case PlayerGameplay.EStatType.ExpPerEnemyMulti:
                    if (_gameplayExp < playerGameplay.expPerEnemyMultiplier.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.expPerEnemyMultiplier.nextUpgradeCost;
                    playerGameplay.expPerEnemyMultiplier.LevelUp();
                    break;
                case PlayerGameplay.EStatType.ExpPerLevel:
                    if (_gameplayExp < playerGameplay.expPerLevel.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.expPerLevel.nextUpgradeCost;
                    playerGameplay.expPerLevel.LevelUp();
                    break;
                case PlayerGameplay.EStatType.GoldPerEnemyMulti:
                    if (_gameplayExp < playerGameplay.goldPerEnemyMultiplier.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.goldPerEnemyMultiplier.nextUpgradeCost;
                    playerGameplay.goldPerEnemyMultiplier.LevelUp();
                    break;
                case PlayerGameplay.EStatType.GoldPerLevel:
                    if (_gameplayExp < playerGameplay.goldPerLevel.nextUpgradeCost)
                        return;
                    _gameplayExp -= playerGameplay.goldPerLevel.nextUpgradeCost;
                    playerGameplay.goldPerLevel.LevelUp();
                    break;
                default:
                    break;
            }

            Signals.Get<Gameplay_Upgrade_GameSignal>().Dispatch(this);
        }

        // Player Execute
        public void StartPlayer()
        {
            StartCoroutine(AutoShootCo());
            StartCoroutine(HPRegenCo());
            StartCoroutine(MPRegenCo());
        }
        private IEnumerator AutoShootCo()
        {
            while (!_isDead)
            {
                if (!_canAttack)
                    yield return null;

                if (currentLockOnEnemy == null)
                    yield return new WaitUntil(() => currentLockOnEnemy != null);

                _isAttacking = true;
                animator.Play("Shoot");
                Bullet bullet = BulletPool.Instance.Rent(bulletPrefab.GetInstanceID());
                bullet.transform.position = playerShoot.transform.position;
                bullet.Shoot(currentLockOnEnemy.transform, new Bullet.Option()
                {
                    damage = playerGameplay.damage.value,
                    range = playerGameplay.range.value,
                    critChance = playerGameplay.criticalChance.baseValue,
                    critMultiplier = playerGameplay.criticalMultiplier.baseValue,
                    damageMeter = playerGameplay.damageMeter.baseValue
                });

                float delay = 1 / (float)playerGameplay.attackSpeed.baseValue;
                yield return new WaitForSeconds(delay);
                _isAttacking = false;
            }
        }

        private IEnumerator HPRegenCo()
        {
            while (!_isDead)
            {
                playerGameplay.currentHP += playerGameplay.HPRegen.value;
                yield return new WaitForSeconds(1);
            }
        }

        private IEnumerator MPRegenCo()
        {
            while (!_isDead)
            {
                playerGameplay.currentMP += playerGameplay.MPRegen.value;
                yield return new WaitForSeconds(1);
            }
        }

        private void RotateTowardEnemy(Transform target)
        {
            Vector3 targetDirection = target.position - transform.position;
            targetDirection.y = 0;
            var lookRotation = Quaternion.LookRotation(targetDirection);

            transform.DORotateQuaternion(lookRotation, 0.8f / (float)playerGameplay.attackSpeed.value).OnComplete(() =>
            {
                _canAttack = true;
            });

            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, singleStep);
        }
    }


    [System.Serializable]
    public class PlayerGameplay
    {
        public enum EStatType
        {
            Damage, AttackSpeed, Range, DamageMeter, CritChance, CritMulti,
            MaxHealth, HeathRegen, MaxMana, ManaRegen,
            ExpPerEnemyMulti, ExpPerLevel, GoldPerEnemyMulti, GoldPerLevel
        }

        [SerializeField] private double _currentHP = 10;
        [SerializeField] private double _currentMP = 100;

        public double currentHP
        {
            get => _currentHP;
            set
            {
                double next = value;
                if (next > maxHP.value)
                    _currentHP = maxHP.value;
                else
                    _currentHP = next;
            }
        }
        public double currentMP
        {
            get => _currentMP;
            set
            {
                double next = value;
                if (next > maxMP.value)
                    _currentMP = maxMP.value;
                else
                    _currentMP = next;
            }
        }


        public bool unlockAttackRound2 = false;
        public bool unlockAttackRound3 = false;
        public bool unlockStatsRound2 = false;

        // Attacks
        public AdvancedStat damage = new AdvancedStat() { baseValue = 10 , isRounded = true, calculation = ECalculation.Percentage, perLevelPercentageBaseValue = 30 };
        public AdvancedStat attackSpeed = new AdvancedStat() { baseValue = 0.5f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.09f, perLevelPercentageBaseValue = 30 };
        public AdvancedStat range = new AdvancedStat() { baseValue = 5f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.1f, perLevelPercentageBaseValue = 30 };
        public AdvancedStat damageMeter = new AdvancedStat() { baseValue = 0.5f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.5f, perLevelPercentageBaseValue = 30 };
        public AdvancedStat criticalChance = new AdvancedStat() { baseValue = 1, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 1, perLevelPercentageBaseValue = 30 };
        public AdvancedStat criticalMultiplier = new AdvancedStat() { baseValue = 2f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.1f, perLevelPercentageBaseValue = 30 };

        // Stats
        public AdvancedStat maxHP = new AdvancedStat() { baseValue = 10, isRounded = true, calculation = ECalculation.Add, perLevelAddBaseValue = 3, perLevelPercentageBaseValue = 30 };
        public AdvancedStat HPRegen = new AdvancedStat() { baseValue = 0.1f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.09f, perLevelPercentageBaseValue = 30 };
        public AdvancedStat maxMP = new AdvancedStat() { baseValue = 100, isRounded = true, calculation = ECalculation.Add, perLevelAddBaseValue = 20, perLevelPercentageBaseValue = 30 };
        public AdvancedStat MPRegen = new AdvancedStat() { baseValue = 2f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 1f, perLevelPercentageBaseValue = 30 };


        // Miscs
        public AdvancedStat expPerEnemyMultiplier = new AdvancedStat() { baseValue = 1f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.1f, perLevelPercentageBaseValue = 30 };
        public AdvancedStat expPerLevel = new AdvancedStat() { baseValue = 2f , isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.1f, perLevelPercentageBaseValue = 30 };
        public AdvancedStat goldPerEnemyMultiplier = new AdvancedStat() { baseValue = 1f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.1f, perLevelPercentageBaseValue = 30 };
        public AdvancedStat goldPerLevel = new AdvancedStat() { baseValue = 2f, isRounded = false, calculation = ECalculation.Add, perLevelAddBaseValue = 0.1f, perLevelPercentageBaseValue = 30 };

        public void Initialize()
        {
            currentHP = maxHP.baseValue;
            currentMP = maxMP.baseValue;
        }

        public PlayerGameplay Clone()
        {
            PlayerGameplay pg = new PlayerGameplay();
            pg.damage = damage.Clone();
            pg.attackSpeed = attackSpeed.Clone();
            pg.range = range.Clone();
            pg.damageMeter = damageMeter.Clone();
            pg.criticalChance = criticalChance.Clone();
            pg.criticalMultiplier = criticalMultiplier.Clone();

            pg.maxHP = maxHP.Clone();
            pg.HPRegen = HPRegen.Clone();
            pg.maxMP = maxMP.Clone();
            pg.MPRegen = MPRegen.Clone();

            pg.expPerEnemyMultiplier = expPerEnemyMultiplier.Clone();
            pg.expPerLevel = expPerLevel.Clone();
            pg.goldPerEnemyMultiplier = goldPerEnemyMultiplier.Clone();
            pg.goldPerLevel = goldPerLevel.Clone();

            return pg;
        }
    }

    public class Gameplay_Upgrade_GameSignal : ASignal<PlayerController> { }

}
