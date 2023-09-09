using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using IdleGunner.Skills;

namespace IdleGunner.UI
{
    [System.Serializable]
    public class SerializeSwipeViewDictionary : SerializableDictionary<SwipeViewMainPanel.EMenuView, SwipeView> { }
    
    public class SwipeViewMainPanel : SceneSingleton<SwipeViewMainPanel>
    {
        public enum EMenuView { Shop = 0, Skills = 1, Play = 2, Upgrade = 3, Heroes = 4 }
        [Header("SETTINGS")]
        [SerializeField] private SerializeSwipeViewDictionary swipeViewDictionary = new SerializeSwipeViewDictionary();
        [SerializeField] private RectTransform canvasRectTransform;

        [Header("BUTTONS")]
        [SerializeField] private Button shopButton;
        [SerializeField] private Button skillsButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button heroesButton;

        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI shopText;
        [SerializeField] private TextMeshProUGUI skillsText;
        [SerializeField] private TextMeshProUGUI playText;
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private TextMeshProUGUI heroText;

        [Header("IMAGES")]
        [SerializeField] private Image shopImage;
        [SerializeField] private Image skillsImage;
        [SerializeField] private Image playImage;
        [SerializeField] private Image upgradeImage;
        [SerializeField] private Image heroesImage;

        [Header("AUDIO")]
        [SerializeField] private AudioSource audioSource;

        [Header("COLOR")]
        [SerializeField] private Color selectedColor;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private SwipeView _currentView;
        [SerializeField, ReadOnly] private Vector2 screenSize = Vector2.zero;
        [SerializeField, ReadOnly] private SwipeView[] swipeViewIndex;
        [SerializeField, ReadOnly] private Vector2 originButtonSizeDelta;
        [SerializeField, ReadOnly] private EMenuView _currentMenuViewEnum;
        [Space]
        [SerializeField, ReadOnly] private RectTransform shopButtonRect;
        [SerializeField, ReadOnly] private RectTransform skillsButtonRect;
        [SerializeField, ReadOnly] private RectTransform playButtonRect;
        [SerializeField, ReadOnly] private RectTransform upgradeButtonRect;
        [SerializeField, ReadOnly] private RectTransform heroesButtonRect;
        [SerializeField, ReadOnly] private Color originButtonColor;
        [SerializeField, ReadOnly] private Vector2 originImageSizeDelta;
        [SerializeField, ReadOnly] private Vector2 originImagePivot;

        public SwipeView currentView => _currentView;
       
        private void OnValidate()
        {
            swipeViewIndex = new SwipeView[swipeViewDictionary.Count];
            for (int i = 0; i < swipeViewDictionary.Count; i++)
            {
                swipeViewIndex[i] = swipeViewDictionary.ElementAt(i).Value;
            }

            shopButton.TryGetComponent(out shopButtonRect);
            skillsButton.TryGetComponent(out skillsButtonRect);
            playButton.TryGetComponent(out playButtonRect);
            upgradeButton.TryGetComponent(out upgradeButtonRect);
            heroesButton.TryGetComponent(out heroesButtonRect);

            shopText = shopButton.GetComponentInChildren<TextMeshProUGUI>();
            skillsText = skillsButton.GetComponentInChildren<TextMeshProUGUI>();
            playText = playButton.GetComponentInChildren<TextMeshProUGUI>();
            upgradeText = upgradeButton.GetComponentInChildren<TextMeshProUGUI>();
            heroText = heroesButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        protected override void Awake()
        {
            base.Awake();

            screenSize.x = canvasRectTransform.rect.width;
            screenSize.y = canvasRectTransform.rect.height;

            originButtonSizeDelta = playButtonRect.sizeDelta;

            originButtonColor = playButton.colors.normalColor;
            originImageSizeDelta = playImage.rectTransform.sizeDelta;
            originImagePivot = playImage.rectTransform.pivot;
        }
#if UNITY_EDITOR
        private void Update()
        {
            screenSize.x = canvasRectTransform.rect.width;
            screenSize.y = canvasRectTransform.rect.height;
        }
#endif

        private void Start()
        {
            // Sub for swipe
            SwipeManager.OnSwipeDetected += HandleSwipe;

            // Handle tab buttons
            shopButton.onClick.AddListener(() =>
            {
                OpenView(EMenuView.Shop);
            });

            skillsButton.onClick.AddListener(() =>
            {
                OpenView(EMenuView.Skills);
            });

            playButton.onClick.AddListener(() =>
            {
                OpenView(EMenuView.Play);
            });

            upgradeButton.onClick.AddListener(() =>
            {
                OpenView(EMenuView.Upgrade);
            });

            heroesButton.onClick.AddListener(() =>
            {
                OpenView(EMenuView.Heroes);
            });

            // Initialize
            if (_currentView == null)
            {
                for (int i = 0; i < swipeViewIndex.Length; i++)
                {
                    swipeViewIndex[i].gameObject.SetActive(false);
                }

                _currentView = swipeViewDictionary[EMenuView.Play];
                _currentView.gameObject.SetActive(true);

                OpenView(EMenuView.Play);
                _currentMenuViewEnum = EMenuView.Play;
            }
        }

        private void OnDestroy()
        {
            SwipeManager.OnSwipeDetected -= HandleSwipe;
        }

        private void HandleSwipe(Swipe direction, Vector2 velocity)
        {
            if (PopupManager.Instance.isOpened)
                return;

            SkillSelectPanel.Instance.Hide();

            if (_currentView is UpgradeSwipeView)
            {
                _currentView.TryGetComponent(out UpgradeSwipeView upgradeView);
                if (direction == Swipe.Left)
                {
                    if (upgradeView.OpenNextView())
                        return;
                }
                else if (direction == Swipe.Right)
                {
                    if (upgradeView.OpenPreviousView())
                        return;
                }
            }

            if (direction == Swipe.Left)
            {
                if ((int)_currentMenuViewEnum == 4)
                    return;
                OpenView(_currentMenuViewEnum + 1);
            }
            else if (direction == Swipe.Right)
            {
                if ((int)_currentMenuViewEnum == 0)
                    return;
                OpenView(_currentMenuViewEnum - 1);
            }
        }


        public void OpenView(EMenuView menuView)
        {
            switch (menuView)
            {
                case EMenuView.Shop:
                    shopButtonRect.DOSizeDelta(originButtonSizeDelta * 1.1f, 0.2f);

                    skillsButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    playButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    upgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    heroesButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);

                    // Color
                    shopButton.targetGraphic.DOColor(selectedColor, 0.2f);

                    skillsButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    playButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    upgradeButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    heroesButton.targetGraphic.DOColor(originButtonColor, 0.2f);

                    // Text
                    shopText.transform.DOScale(1, 0.2f);

                    skillsText.transform.DOScale(0, 0.2f);
                    playText.transform.DOScale(0, 0.2f);
                    upgradeText.transform.DOScale(0, 0.2f);
                    heroText.transform.DOScale(0, 0.2f);

                    // Image Size
                    shopImage.rectTransform.DOSizeDelta(originImageSizeDelta * 1.2f, 0.2f);

                    skillsImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    playImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    upgradeImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    heroesImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);

                    // Image Pivot
                    shopImage.rectTransform.DOPivotY(0.25f, 0.2f);

                    skillsImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    playImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    upgradeImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    heroesImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);

                    break;
                case EMenuView.Skills:
                    skillsButtonRect.DOSizeDelta(originButtonSizeDelta * 1.2f, 0.2f);

                    shopButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    playButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    upgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    heroesButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);

                    // Color
                    skillsButton.targetGraphic.DOColor(selectedColor, 0.2f);

                    shopButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    playButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    upgradeButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    heroesButton.targetGraphic.DOColor(originButtonColor, 0.2f);

                    // Text
                    skillsText.transform.DOScale(1, 0.2f);

                    shopText.transform.DOScale(0, 0.2f);
                    playText.transform.DOScale(0, 0.2f);
                    upgradeText.transform.DOScale(0, 0.2f);
                    heroText.transform.DOScale(0, 0.2f);

                    // Image Size
                    skillsImage.rectTransform.DOSizeDelta(originImageSizeDelta * 1.2f, 0.2f);

                    shopImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    playImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    upgradeImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    heroesImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);

                    // Image Pivot
                    skillsImage.rectTransform.DOPivotY(0.25f, 0.2f);

                    shopImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    playImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    upgradeImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    heroesImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    break;
                case EMenuView.Play:
                    playButtonRect.DOSizeDelta(originButtonSizeDelta * 1.2f, 0.2f);

                    shopButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    skillsButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    upgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);
                    heroesButtonRect.DOSizeDelta(originButtonSizeDelta, 0.3f);

                    // Color
                    playButton.targetGraphic.DOColor(selectedColor, 0.2f);

                    shopButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    skillsButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    upgradeButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    heroesButton.targetGraphic.DOColor(originButtonColor, 0.2f);

                    // Text
                    playText.transform.DOScale(1, 0.2f);

                    skillsText.transform.DOScale(0, 0.2f);
                    shopText.transform.DOScale(0, 0.2f);
                    upgradeText.transform.DOScale(0, 0.2f);
                    heroText.transform.DOScale(0, 0.2f);

                    // Image Size
                    playImage.rectTransform.DOSizeDelta(originImageSizeDelta * 1.2f, 0.2f);

                    shopImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    skillsImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    upgradeImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    heroesImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);

                    // Image Pivot
                    playImage.rectTransform.DOPivotY(0.25f, 0.2f);

                    shopImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    skillsImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    upgradeImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    heroesImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f); 
                    break;
                case EMenuView.Upgrade:
                    upgradeButtonRect.DOSizeDelta(originButtonSizeDelta * 1.2f, 0.2f);

                    shopButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    skillsButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    playButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    heroesButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);

                    // Color
                    upgradeButton.targetGraphic.DOColor(selectedColor, 0.2f);

                    shopButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    skillsButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    playButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    heroesButton.targetGraphic.DOColor(originButtonColor, 0.2f);

                    // Text
                    upgradeText.transform.DOScale(1, 0.2f);

                    skillsText.transform.DOScale(0, 0.2f);
                    shopText.transform.DOScale(0, 0.2f);
                    playText.transform.DOScale(0, 0.2f);
                    heroText.transform.DOScale(0, 0.2f);

                    // Image Size
                    upgradeImage.rectTransform.DOSizeDelta(originImageSizeDelta * 1.2f, 0.2f);

                    shopImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    skillsImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    playImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    heroesImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);

                    // Image Pivot
                    upgradeImage.rectTransform.DOPivotY(0.25f, 0.2f);

                    shopImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    skillsImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    playImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    heroesImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    break;
                case EMenuView.Heroes:
                    heroesButtonRect.DOSizeDelta(originButtonSizeDelta * 1.2f, 0.2f);

                    shopButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    skillsButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    playButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);
                    upgradeButtonRect.DOSizeDelta(originButtonSizeDelta, 0.2f);

                    // Color
                    heroesButton.targetGraphic.DOColor(selectedColor, 0.2f);

                    shopButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    skillsButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    playButton.targetGraphic.DOColor(originButtonColor, 0.2f);
                    upgradeButton.targetGraphic.DOColor(originButtonColor, 0.2f);

                    // Text
                    heroText.transform.DOScale(1, 0.2f);

                    skillsText.transform.DOScale(0, 0.2f);
                    shopText.transform.DOScale(0, 0.2f);
                    playText.transform.DOScale(0, 0.2f);
                    upgradeText.transform.DOScale(0, 0.2f);

                    // Image Size
                    heroesImage.rectTransform.DOSizeDelta(originImageSizeDelta * 1.2f, 0.2f);

                    shopImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    skillsImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    playImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);
                    upgradeImage.rectTransform.DOSizeDelta(originImageSizeDelta, 0.2f);

                    // Image Pivot
                    heroesImage.rectTransform.DOPivotY(0.25f, 0.2f);

                    shopImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    skillsImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    playImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    upgradeImage.rectTransform.DOPivotY(originImagePivot.y, 0.2f);
                    break;
                default:
                    break;
            }

            if (_currentView == swipeViewDictionary[menuView])
                return;

            audioSource.Play();
            _currentMenuViewEnum = menuView;

            var posX = screenSize.x;
            if (Array.IndexOf(swipeViewIndex, _currentView) < Array.IndexOf(swipeViewIndex, swipeViewDictionary[menuView]))
            {
                posX *= -1;
            }
            _currentView.rect.DOAnchorPosX(posX, 0.3f).SetEase(Ease.OutQuint).OnComplete(() => _currentView.OnHide?.Invoke());

            _currentView = swipeViewDictionary[menuView];
            _currentView.gameObject.SetActive(true);

            _currentView.rect.anchoredPosition = new Vector2(-posX, 0);
            _currentView.rect.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutQuint).OnComplete(() => _currentView.OnShow?.Invoke());
        }
    }

}