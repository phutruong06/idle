﻿using System;
using UnityEngine;

namespace MackySoft.XPool.Unity.ObjectModel {

	/// <summary>
	/// Base of pool for <see cref="Component"/>.
	/// </summary>
	public abstract class ComponentPoolBase<T> : UnityObjectPoolBase<T>, IHierarchicalUnityObjectPool<T> where T : Component {

		protected ComponentPoolBase () {
		}

		/// <param name="original"> The original object from which the pool will instantiate a new instance. </param>
		/// <param name="capacity"> The pool capacity. If less than 0, <see cref="ArgumentOutOfRangeException"/> will be thrown. </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		protected ComponentPoolBase (T original,int capacity) : base(original,capacity) {
		}

		public void Create(Transform parent = null)
		{
			for (int i = 0; i < Capacity; i++)
			{
				var instance = UnityEngine.Object.Instantiate(m_Original);
				instance.gameObject.SetActive(false);

				if (parent)
				{
					instance.transform.SetParent(parent);

				}
				m_Pool.Enqueue(instance);
				OnCreate(instance);
			}
		}

		public T Rent (Vector3 position,Quaternion rotation,Transform parent = null) {
			T instance = GetPooledInstance();
			if (instance != null) {
				Transform transform = parent.transform;
				transform.SetParent(parent);
				transform.SetPositionAndRotation(position,rotation);
			}
			else {
				instance = UnityEngine.Object.Instantiate(m_Original,position,rotation,parent);
				OnCreate(instance);
			}
			OnRent(instance);
			return instance;
		}

		public T Rent (Transform parent,bool worldPositionStays) {
			T instance = GetPooledInstance();
			if (instance != null) {
				instance.transform.SetParent(parent,worldPositionStays);
			}
			else {
				instance = UnityEngine.Object.Instantiate(m_Original,parent,worldPositionStays);
				OnCreate(instance);
			}
			OnRent(instance);
			return instance;
		}

	}
}
