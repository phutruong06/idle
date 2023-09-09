using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFaceCamera : MonoBehaviour
{
    [BoxGroup("DEBUG"), SerializeField, ReadOnly] protected UnityEngine.Camera p_camera;

    protected virtual void OnValidate()
    {
        p_camera = UnityEngine.Camera.main;

    }
    protected virtual void Awake()
    {
        p_camera = UnityEngine.Camera.main;

    }
    private void Update()
    {
        transform.rotation = Quaternion.Euler(p_camera.transform.eulerAngles.x, p_camera.transform.eulerAngles.y, 0);
    }
}
