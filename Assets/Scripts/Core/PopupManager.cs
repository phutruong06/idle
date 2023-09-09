using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using IdleGunner.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TagHash
{
    public static string MAIN_CANVAS = "MainCanvas";
    public static string HIGH_CANVAS = "HighCanvas";
}

public class PopupManager : Singleton<PopupManager>
{
    [BoxGroup("REFERENCES"), SerializeField, Expandable] private PopupDataSO popupDataContainer = null;

    [BoxGroup("DEBUG"), SerializeField] private Transform parentTransform = null;
    [BoxGroup("DEBUG"), SerializeField, ReadOnly] private PopupSerializeCollection popupSerializeCollectionClone;
    [BoxGroup("DEBUG"), SerializeField, ReadOnly] private List<PopupBase> popupCount = new List<PopupBase>();
    public bool isOpened => popupCount.Count > 0;
    protected override void Awake()
    {
        base.Awake();

        popupSerializeCollectionClone = new PopupSerializeCollection();
    }

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        ResetIfError();
//    }
//#endif

//#if UNITY_EDITOR
//    [BoxGroup("EDITOR"), SerializeField] private EPopup selectToShow;
//    [Button(enabledMode: EButtonEnableMode.Playmode)]
//    private void ShowScreenEditor()
//    {
//        this.ShowPopup(selectToShow);
//    }
//    [Button(enabledMode: EButtonEnableMode.Playmode)]
//    private void HideScreenEditor()
//    {
//        HideCurrentPopup();
//    }

//    [Button]
//    private void ResetIfError()
//    {
//        try
//        {
//            for (int i = 0; i < parentTransform.childCount; i++)
//            {
//                GameObject.Destroy(parentTransform.GetChild(i).gameObject);
//            }
//        }
//        catch (Exception e)
//        {

//        }
//        popupSerializeCollectionClone.Clear();
//        popupCount.Clear();
//    }
//#endif

    // Show popup
    public void ShowPopup(EPopup popup)
    {
        var popupBase = GetPopup(popup);
        if (popupCount.Count > 0)
        {
            if (popupCount.Last() == popupBase)
                return;
        }

        if (popupBase == null)
            return;


        popupCount.Add(popupBase);
        popupBase.gameObject.SetActive(true);
        popupBase.ShowPopup();

        popupBase.OnDestroyGameObject.AddListener(() =>
        {
            var p = popupSerializeCollectionClone.First(e => e.Key == popup);
            popupSerializeCollectionClone.Remove(p.Key);
        });
    }
    public void ShowPopup<T>(T popup) where T : PopupBase
    {
        var popupBase = GetPopup(popup);
        if (popupCount.Count > 0)
        {
            if (popupCount.Last() == popupBase)
                return;
        }

        if (popupBase == null)
            return;


        popupCount.Add(popupBase);
        popupBase.gameObject.SetActive(true);
        popupBase.ShowPopup();

        popupBase.OnDestroyGameObject.AddListener(() =>
        {
            var p = popupSerializeCollectionClone.First(e => e.Value == popup);
            popupSerializeCollectionClone.Remove(p.Key);
        });
    }
    public void ShowPopup<T>() where T : PopupBase
    {
        var popupBase = GetPopup<T>();
        if (popupCount.Count > 0)
        {
            if (popupCount.Last() == popupBase)
                return;
        }

        if (popupBase == null)
            return;


        popupCount.Add(popupBase);
        popupBase.gameObject.SetActive(true);
        popupBase.ShowPopup();

        popupBase.OnDestroyGameObject.AddListener(() =>
        {
            var p = popupSerializeCollectionClone.First(e => e.Value.GetType() == typeof(T));
            popupSerializeCollectionClone.Remove(p.Key);
        });
    }

    // Hide popup
    public void HidePopup(EPopup popup)
    {
        var popupBase = GetPopup(popup);
        if (popupBase == null)
            return;

        popupBase.HidePopup();
        popupCount.Remove(popupBase);
    }
    public void HidePopup<T>(T popup) where T : PopupBase
    {
        var popupBase = GetPopup(popup);
        if (popupBase == null)
            return;

        popupBase.HidePopup();
        popupCount.Remove(popupBase);
    }
    public void HidePopup<T>() where T : PopupBase
    {
        var popupBase = GetPopup<T>();

        if (popupBase == null)
            return;

        popupBase.HidePopup();
        popupCount.Remove(popupBase);

        if (popupBase.destroyWhenClose)
        {
            var p = popupSerializeCollectionClone.First(e => e.Value.GetType() == typeof(T));

            popupSerializeCollectionClone.Remove(p.Key);
            GameObject.Destroy(popupBase);
        }
    }
    public void HideCurrentPopup()
    {
        if (popupCount.Count > 0)
        {
            var p = popupCount.Last();

            p.HidePopup();

            popupCount.RemoveAt(popupCount.Count - 1);

            if (popupCount.Count > 0)
            {
                popupCount.Last().gameObject.SetActive(true);
            }
        }
    }

    // Get popup
    public PopupBase GetPopup(EPopup popup)
    {
        PopupBase popupBase = null;

        try
        {
            if (popupSerializeCollectionClone.ContainsKey(popup))
            {
                popupSerializeCollectionClone.TryGetValue(popup, out popupBase);
            }
            else
            {
                popupDataContainer.popupCollection.TryGetValue(popup, out popupBase);

                var obj = GameObject.Instantiate(popupBase, parentTransform);
                //obj.gameObject.SetActive(false);
                popupSerializeCollectionClone.Add(popup, obj);

                popupBase = obj;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }

        return popupBase;
    }
    public T GetPopup<T>(T popup) where T : PopupBase
    {
        T t = default;

        try
        {
            for (int i = 0; i < popupSerializeCollectionClone.Count; i++)
            {
                if (popupSerializeCollectionClone.ElementAt(i).Value.GetType() == popup.GetType())
                {
                    t = popupSerializeCollectionClone.ElementAt(i).Value as T;
                }
            }

            if (t == null)
            {
                for (int i = 0; i < popupDataContainer.popupCollection.Count; i++)
                {
                    if (popupDataContainer.popupCollection.ElementAt(i).Value.GetType() == typeof(T))
                    {
                        var key = popupDataContainer.popupCollection.ElementAt(i).Key;
                        var value = popupDataContainer.popupCollection.ElementAt(i).Value;

                        var obj = GameObject.Instantiate(popupDataContainer.popupCollection.ElementAt(i).Value, parentTransform);
                        //obj.gameObject.SetActive(false);
                        popupSerializeCollectionClone.Add(key, obj);

                        t = obj as T;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }

        return t;
    }
    public T GetPopup<T>() where T : PopupBase
    {
        T t = default;

        try
        {
            for (int i = 0; i < popupSerializeCollectionClone.Count; i++)
            {
                if (popupSerializeCollectionClone.ElementAt(i).Value.GetType() == typeof(T))
                {
                    t = popupSerializeCollectionClone.ElementAt(i).Value as T;
                }
            }

            if (t == null)
            {
                for (int i = 0; i < popupDataContainer.popupCollection.Count; i++)
                {
                    if (popupDataContainer.popupCollection.ElementAt(i).Value.GetType() == typeof(T))
                    {
                        var key = popupDataContainer.popupCollection.ElementAt(i).Key;
                        var value = popupDataContainer.popupCollection.ElementAt(i).Value;

                        var obj = GameObject.Instantiate(popupDataContainer.popupCollection.ElementAt(i).Value, parentTransform);
                        obj.gameObject.SetActive(false);
                        popupSerializeCollectionClone.Add(key, obj);

                        t = obj as T;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
        return t;
    }

    public PopupBase GetCurrentPopup => popupCount.Last();
}
