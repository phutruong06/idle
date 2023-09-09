using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Data", menuName = "Scene Data")]
public class SceneData : ScriptableObject
{
    [SerializeField] public SceneDataSerializeCollection scenedataCollection = new SceneDataSerializeCollection();
}

[System.Serializable]
public class SceneDataSerializeCollection : SerializableDictionaryBase<EScene, int> { }

public enum EScene { Splash, Login, Admin, Worker, Manager }