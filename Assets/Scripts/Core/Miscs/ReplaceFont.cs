using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReplaceFont : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textMeshProUGUIArray;

    [SerializeField] private TMP_FontAsset TMP_FontAsset;

    private void OnValidate()
    {
        textMeshProUGUIArray = FindObjectsOfType<TextMeshProUGUI>();

        for (int i = 0; i < textMeshProUGUIArray.Length; i++)
        {
            textMeshProUGUIArray[i].font = TMP_FontAsset;
        }
    }
}
