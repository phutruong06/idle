using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using IdleGunner.UI;

namespace IdleGunner.Gameplay.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(UpgradeSwipeView))]
    public class BottomPanel : MonoBehaviour
    {
        public enum EBottomTab { Attack, Stats, Miscs }

        [Header("BUTTON")]
        [SerializeField] private Button attackUpgradeTabButton;
        [SerializeField] private Button statsUpgradeTabButton;
        [SerializeField] private Button miscsUpgradeTabButton;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private Animator animator;
        [SerializeField, ReadOnly] private bool _isOpen = false;
        [SerializeField, ReadOnly] private UpgradeSwipeView upgradeSwipeView;
        [ReadOnly] public EBottomTab _currentViewIndex = EBottomTab.Attack;

        [SerializeField, ReadOnly] private RectTransform attackButtonRectTransform;
        [SerializeField, ReadOnly] private RectTransform statsButtonRectTransform;
        [SerializeField, ReadOnly] private RectTransform miscsButtonRectTransform;

        [SerializeField, ReadOnly] private Vector2 originWH;

        public bool isOpen
        {
            get => _isOpen;
            set
            {
                if (value)
                {
                    animator.Play("Open");
                }
                else
                {
                    animator.Play("Close");
                }

                _isOpen = value;
                attackButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                statsButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                miscsButtonRectTransform.DOSizeDelta(originWH, 0.3f);

                CameraController.Instance.OpenCamera(value);
            }
        }

        private void OnValidate()
        {
            TryGetComponent(out animator);
            TryGetComponent(out upgradeSwipeView);

            attackUpgradeTabButton.TryGetComponent(out attackButtonRectTransform);
            statsUpgradeTabButton.TryGetComponent(out statsButtonRectTransform);
            miscsUpgradeTabButton.TryGetComponent(out miscsButtonRectTransform);
        }

        protected void Start()
        {
            originWH = attackButtonRectTransform.sizeDelta;

            attackUpgradeTabButton.onClick.AddListener(() =>
            {
                if (!isOpen)
                {
                    animator.Play("Open");
                    OpenView(EBottomTab.Attack);
                }
                else
                {
                    if (_currentViewIndex != EBottomTab.Attack)
                        OpenView(EBottomTab.Attack);
                    else
                        isOpen = false;
                }
            });

            statsUpgradeTabButton.onClick.AddListener(() =>
            {
                if (!isOpen)
                {
                    animator.Play("Open");
                    OpenView(EBottomTab.Stats);
                }
                else
                {
                    if (_currentViewIndex != EBottomTab.Stats)
                        OpenView(EBottomTab.Stats);
                    else
                        isOpen = false;
                }
            });

            miscsUpgradeTabButton.onClick.AddListener(() =>
            {
                if (!isOpen)
                {
                    animator.Play("Open");
                    OpenView(EBottomTab.Miscs);
                }
                else
                {
                    if (_currentViewIndex != EBottomTab.Miscs)
                        OpenView(EBottomTab.Miscs);
                    else
                        isOpen = false;
                }
            });

            SwipeManager.OnSwipeDetected += HandleSwipe;
        }

        private void HandleSwipe(Swipe direction, Vector2 velocity)
        {
            if (!isOpen)
                return;

            if (direction == Swipe.Left)
            {
                OpenNextView();
                upgradeSwipeView.OpenNextView();
            }
            else if (direction == Swipe.Right)
            {
                OpenPreviousView();
                upgradeSwipeView.OpenPreviousView();
            }
        }

        private void OnDestroy()
        {
            SwipeManager.OnSwipeDetected -= HandleSwipe;
        }

        public bool OpenNextView()
        {
            if ((int)_currentViewIndex == 2)
                return false;
            else
            {
                OpenView(_currentViewIndex + 1);
                return true;
            }
        }

        public bool OpenPreviousView()
        {
            if ((int)_currentViewIndex == 0)
                return false;
            else
            {
                OpenView(_currentViewIndex - 1);
                return true;
            }
        }


        public void OpenView(EBottomTab tab)
        {
            isOpen = true;
            switch (tab)
            {
                case EBottomTab.Attack:
                    _currentViewIndex = EBottomTab.Attack;
                    attackButtonRectTransform.DOSizeDelta(originWH * 1.15f, 0.3f);

                    statsButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                    miscsButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                    break;
                case EBottomTab.Stats:
                    _currentViewIndex = EBottomTab.Stats;
                    statsButtonRectTransform.DOSizeDelta(originWH * 1.15f, 0.3f);

                    attackButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                    miscsButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                    break;
                case EBottomTab.Miscs:
                    _currentViewIndex = EBottomTab.Miscs;
                    miscsButtonRectTransform.DOSizeDelta(originWH * 1.15f, 0.3f);

                    attackButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                    statsButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                    break;
                default:
                    _currentViewIndex = EBottomTab.Attack;
                    attackButtonRectTransform.DOSizeDelta(originWH * 1.15f, 0.3f);
                    statsButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                    miscsButtonRectTransform.DOSizeDelta(originWH, 0.3f);
                    break;
            }
        }
    }
}