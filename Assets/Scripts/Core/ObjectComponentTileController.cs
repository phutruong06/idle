using IdleGunner.Core;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace IdleGunner.Core
{
    [System.Serializable]
    public class SerializeTitleDict : SerializableDictionary<int, Component> { }

    public static class ObjectTileComponent
    {
        private static SerializeTitleDict tileCollection = new SerializeTitleDict();

        /// <summary>
        /// Get title component cached
        /// </summary>
        /// <param name="instanceId">return as monobehaviour, you will need to parse this object as other inheritance</param>
        /// <returns></returns>
        public static T GetTileComponent<T>(int instanceId) where T : Component
        {
            return tileCollection[instanceId] as T;
        }

        /// <summary>
        /// Get title component cached
        /// </summary>
        /// <param name="instanceId">return as monobehaviour, you will need to parse this object as other inheritance</param>
        public static bool TryGetTileComponent<T>(int instanceId, out T component) where T : Component
        {
            if (tileCollection.ContainsKey(instanceId))
            {
                component = tileCollection[instanceId] as T;
                return true;
            }
            component = null;
            return false;
        }

        public static bool TryRegisterTileComponent<T>(T instance) where T : Component
        {
            if (tileCollection.ContainsKey(instance.GetInstanceID()))
            {
                return false;
            }
            else
            {
                tileCollection.Add(instance.gameObject.GetInstanceID(), instance);
                return true;
            }
        }

        /// <summary>
        /// Add the class that inherit from 'MonoBehaviour' to storage for other use, this will storage the object instanceId as key
        /// </summary>
        /// <param name="instance">Object inherited from 'MonoBehaviour'</param>
        public static void RegisterTileComponent(Component instance)
        {
            tileCollection.Add(instance.GetInstanceID(), instance);
            //Debug.Log($"Added object with instance Id: '{instance.GetInstanceID()}'");
        }

        /// <summary>
        /// Clear the object that cached previously *Note that this will have some error*
        /// </summary>
        /// <param name="instance">This will take the object and automatically get instance Id of the object</param>
        public static void RemoveTileComponent<T>(T instance) where T : Component
        {
            tileCollection.Remove(instance.GetInstanceID());
        }

        /// <summary>
        /// Clear the object that cached
        /// </summary>
        /// <param name="instanceId">this will take the object id as key to remove it</param>
        public static void RemoveTileComponent(int instanceId)
        {
            tileCollection.Remove(instanceId);
        }
    }
}