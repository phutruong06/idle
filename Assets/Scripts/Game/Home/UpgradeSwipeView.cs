using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IdleGunner.UI
{
    public class UpgradeSwipeView : SwipeView
    {
        [System.Serializable]
        public class SerializeSwipeViewDictionary : SerializableDictionary<EMenuView, SwipeView> { }
        public enum EMenuView { Attack = 0, Stats = 1, Miscs = 2 }

        [Header("SETTINGS")]
        [SerializeField] private SerializeSwipeViewDictionary swipeViewDictionary = new SerializeSwipeViewDictionary();

        [Header("BUTTONS")]
        [SerializeField] private Button attackUpgradeButton;
        [SerializeField] private Button statsUpgradeButton;
        [SerializeField] private Button miscsUpgradeButton;

        [Header("AUDIO")]
        [SerializeField] private AudioSource audioSource;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private SwipeView _currentView;
        [SerializeField, ReadOnly] private Vector2 screenSize = Vector2.zero;
        [SerializeField, ReadOnly] private SwipeView[] swipeViewIndex;
        [SerializeField, ReadOnly] private Vector2 originButtonSizeDelta;
        [SerializeField, ReadOnly] private EMenuView _currentMenuViewEnum;

        [SerializeField, ReadOnly] private RectTransform attackUpgradeButtonRect;
        [SerializeField, ReadOnly] private RectTransform statsUpgradeButtonRect;
        [SerializeField, ReadOnly] private RectTransform miscsUpgradeButtonRect;

        public SwipeView currentView => _currentView;

        private void OnValidate()
        {
            attackUpgradeButton.TryGetComponent(out attackUpgradeButtonRect);
            statsUpgradeButton.TryGetComponent(out statsUpgradeButtonRect);
            miscsUpgradeButton.TryGetComponent(out miscsUpgradeButtonRect);

            swipeViewIndex = new SwipeView[swipeViewDictionary.Count];
            for (int i = 0; i < swipeViewDictionary.Count; i++)
            {
                swipeViewIndex[i] = swipeViewDictionary.ElementAt(i).Value;
            }
        }

        private void Awake()
        {
            screenSize.x = Screen.currentResolution.width;
            screenSize.y = Screen.currentResolution.height;

            originButtonSizeDelta = attackUpgradeButtonRect.sizeDelta;
        }

        private void Start()
        {
            attackUpgradeButton.onClick.AddListener(() =>
            {
                OpenView(EMenuView.Attack);
            });

            statsUpgradeButton.onClick.AddListener(() =>
            {
                OpenView(EMenuView.Stats);
            });

            miscsUpgradeButton.onClick.AddListener(() =>
            {
                OpenView(EMenuView.Miscs);
            });

            if (_currentView == null)
            {
                for (int i = 0; i < swipeViewIndex.Length; i++)
                {
                    swipeViewIndex[i].gameObject.SetActive(false);
                }

                _currentView = swipeViewDictionary[EMenuView.Attack];
                _currentView.gameObject.SetActive(true);
                
                OpenView(EMenuView.Attack);
                _currentMenuViewEnum = EMenuView.Attack;
            }
        }

        private void OnDestroy()
        {
            OnShow.RemoveAllListeners();
        }

        public bool OpenNextView()
        {
            if ((int)_currentMenuViewEnum == 2)
                return false;
            else
            {
                OpenView(_currentMenuViewEnum + 1);
                return true;
            }
        }

        public bool OpenPreviousView()
        {
            if ((int)_currentMenuViewEnum == 0)
                return false;
            else
            {
                OpenView(_currentMenuViewEnum - 1);
                return true;
            }
        }

        public void OpenView(EMenuView view)
        {
            switch (view)
            {
                case EMenuView.Attack:
                    attackUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta * 1.1f, 0.3f);

                    statsUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    miscsUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    break;
                case EMenuView.Stats:
                    statsUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta * 1.1f, 0.3f);

                    attackUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    miscsUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    break;
                case EMenuView.Miscs:
                    miscsUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta * 1.1f, 0.3f);

                    attackUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    statsUpgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    break;
                default:
                    break;
            }

            if (_currentView == swipeViewDictionary[view])
                return;

            audioSource.Play();
            _currentMenuViewEnum = view;

            var posX = screenSize.x;
            if (Array.IndexOf(swipeViewIndex, _currentView) < Array.IndexOf(swipeViewIndex, swipeViewDictionary[view]))
            {
                posX *= -1;
            }
            _currentView.rect.DOAnchorPosX(posX, 0.3f).SetEase(Ease.OutQuint).OnComplete(() => _currentView.OnHide?.Invoke());

            _currentView = swipeViewDictionary[view];
            _currentView.gameObject.SetActive(true);

            _currentView.rect.anchoredPosition = new Vector2(-posX, 0);
            _currentView.rect.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutQuint).OnComplete(() => _currentView.OnShow?.Invoke());

        }

    }
}
