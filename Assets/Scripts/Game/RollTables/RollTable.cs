using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleGunner.RollTables
{
    [CreateAssetMenu(fileName = "New RollTable Data", menuName = "Game/RollTable Data")]
    public class RollTable : ScriptableObject
    {
        [SerializeReference] public List<RollTableObject> rollTableObjects = new List<RollTableObject>();

        public RollTableObject Roll()
        {
            var random = new System.Random();

            return rollTableObjects[random.Next(0, rollTableObjects.Count)];
        }
    }

    public class RollTableObject : MonoBehaviour, IRollTableObject
    {
        [Header("ROLL SETTINGS")]
        [SerializeField, MinMaxSlider(0f, 1f)] private Vector2 rollWeight;

        public float GetMinWeight() => rollWeight.x;
        public float GetMaxWeight() => rollWeight.y;
        public float GetWeight() => Random.Range(rollWeight.x, rollWeight.y);
    }

    public class RollTableObjectMono : RollTableObject
    {
    }

    public class RollTableObjectSCriptable : RollTableObject
    {
    }

    public interface IRollTableObject
    {
        public float GetMinWeight();
        public float GetMaxWeight();
        public float GetWeight();
    }
}