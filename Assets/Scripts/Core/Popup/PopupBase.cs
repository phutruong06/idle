using DG.Tweening;
using NativeWebSocket;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace IdleGunner.UI
{
    [System.Serializable]
    public enum EPopup
    {
        Settings, IAPPurchase, Blessings, SkillDetail, FreeRewards, Missions, PiggyBank, GameplayPause,
        GameLoading, BuyGems, BuyGolds, Revive, QuitGame, EndGame, YesNo,
        GameplayWave
    }

    public interface IPopup
    {
        void ShowPopup();
        void HidePopup();
    }
}

[RequireComponent(typeof(CanvasGroup))]
public class PopupBase : MonoBehaviour, IdleGunner.UI.IPopup
{
    [BoxGroup("SETTINGS"), SerializeField] private Transform childPanel;
    [BoxGroup("SETTINGS")] public bool destroyWhenClose = false;

    [BoxGroup("DEBUG"), SerializeField, ReadOnly] protected bool _isShowing = false;
    [BoxGroup("DEBUG"), SerializeField, ReadOnly] protected CanvasGroup _canvasGroup;

    /// <summary>
    /// Subcribe on Start
    /// </summary>
    protected UnityEvent OnShow = new UnityEvent();
    protected UnityEvent OnShowCompleted = new UnityEvent();

    /// <summary>
    /// Subcribe on Start
    /// </summary>
    protected UnityEvent OnHide = new UnityEvent();
    protected UnityEvent OnHideCompleted = new UnityEvent();

    [HideInInspector]
    public UnityEvent OnDestroyGameObject = new UnityEvent();

    public bool isShowing => _isShowing;


    protected virtual void OnValidate()
    {
        TryGetComponent(out _canvasGroup);
        _isShowing = false;
    }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        TryGetComponent(out _canvasGroup);
    }

    protected virtual void OnDestroy()
    {
        OnShow.RemoveAllListeners();
        OnShowCompleted.RemoveAllListeners();

        OnHide.RemoveAllListeners();
        OnHideCompleted.RemoveAllListeners();

        DOTween.Kill(this.gameObject);
    }

    /// <summary>
    /// Show base data
    /// </summary>
    public virtual void ShowPopup()
    {
        if (_isShowing) return;

        OnShow?.Invoke();

        this.gameObject.SetActive(true);

        this.transform.localScale = Vector3.one;
        _canvasGroup.alpha = 0f;
        this.transform.SetAsLastSibling();

        if (childPanel != null)
        {
            childPanel.transform.localScale = Vector3.zero;
            DOTween.Sequence().Append(childPanel.transform.DOScale(Vector3.one * 1.2f, 0.1f))
                              .Append(childPanel.transform.DOScale(Vector3.one, 0.1f)).SetUpdate(true);

            Debug.Log("OPEN");
        }
        this._canvasGroup.DOFade(1f, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            OnShowCompleted?.Invoke();
        }).SetUpdate(true);
        _isShowing = true;

    }

    /// <summary>
    /// Hide base data
    /// </summary>
    public virtual void HidePopup()
    {
        if (!_isShowing) return;

        OnHide.Invoke();

        this.transform.localScale = Vector3.one;
        _canvasGroup.alpha = 1f;

        if (childPanel != null)
        {
            childPanel.transform.DOScale(Vector3.one * 0.5f, 0.2f);
        }

        this._canvasGroup.DOFade(0f, 0.2f).SetUpdate(true).OnComplete((TweenCallback)(() =>
        {
            this.transform.localScale = Vector3.one;
            _canvasGroup.alpha = 0f;
            this.gameObject.SetActive(false);
            OnHideCompleted?.Invoke();

            if (destroyWhenClose)
            {
                OnDestroyGameObject.Invoke();
                GameObject.Destroy(this.gameObject);
            }
        })).SetUpdate(true);
        _isShowing = false;
    }

}