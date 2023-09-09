using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RotaryHeart.Lib.SerializableDictionary;
using IdleGunner.UI;

[CreateAssetMenu(fileName = "Popup Data", menuName = "Popup Data")]
public class PopupDataSO : ScriptableObject
{
    [SerializeField] public PopupSerializeCollection popupCollection = new PopupSerializeCollection();
}

[System.Serializable]
public class PopupSerializeCollection : SerializableDictionaryBase<EPopup, PopupBase> { }
