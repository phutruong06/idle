using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatEffect
{
    public double value;
    public ECalculation calculation;
}

[System.Serializable]
public enum ECalculation
{
    Percentage, Add, Exponetional
}

[System.Serializable]
public class AdvancedStat
{

    /// <summary>
    /// Ly do ko xai Icloneable vi no nhu nhau :v
    /// Icloneable no cung la copy, nhung hamm copy co san cua no la shallow copy.
    /// No copy reference vao object goc, chu ko tao ra 1 variable moi nen ko dung
    /// </summary>

    public double baseValue;
    public int maxLevel;
    public bool isRounded = false;
    public ECalculation calculation;

    [SerializeField, ShowIf("calculation", ECalculation.Percentage), AllowNesting] public double perLevelPercentageBaseValue = 30;
    [SerializeField, ShowIf("calculation", ECalculation.Add), AllowNesting] public double perLevelAddBaseValue;
    [SerializeField, ShowIf("calculation", ECalculation.Exponetional), AllowNesting] public double perLevelExponentBaseValue;
    [SerializeField] private double m_baseUpgradeCost = 10;
    [SerializeField] private double m_currentLevelUpgradeCost = 10;

    public List<StatEffect> statEffects = new List<StatEffect>();

    public int level => m_level == 0 ? 1 : m_level;
    public bool HasStatEffect => statEffects.Count > 0;
    public double value => isRounded ? Math.Ceiling(baseValue) : baseValue;
    public bool isMaxLevel => maxLevel == 0 ? false : m_level == maxLevel ? true : false;

    private int m_level = 1;
    public double currentLevelUpgradeCost => m_currentLevelUpgradeCost;

    public double nextUpgradeCost
    {
        get
        {
            if (level == 1) return m_baseUpgradeCost;

            return Math.Ceiling(m_currentLevelUpgradeCost + (m_currentLevelUpgradeCost  * perLevelPercentageBaseValue / 100));
            //switch (calculation)
            //{
            //    case ECalculation.Percentage:
            //        return Mathf.RoundToInt(10 + (perLevelPercentageBaseValue / 100 * m_currentLevelUpgradeCost));
            //    case ECalculation.Add:
            //        return Mathf.RoundToInt(m_currentLevelUpgradeCost + perLevelAddBaseValue);
            //    case ECalculation.Exponetional:
            //        return Mathf.RoundToInt((float)Math.Pow(m_currentLevelUpgradeCost, perLevelExponentBaseValue));
            //    default:
            //        return 0;
            //}
        }
    }
    public void LevelUp()
    {
        m_level++;
        m_currentLevelUpgradeCost = nextUpgradeCost;
        switch (calculation)
        {
            case ECalculation.Percentage:
                baseValue += baseValue * perLevelPercentageBaseValue / 100;
                break;
            case ECalculation.Add:
                baseValue += perLevelAddBaseValue;
                break;
            case ECalculation.Exponetional:
                baseValue = (float)Math.Pow(baseValue, perLevelExponentBaseValue);
                break;
            default:
                break;
        }
    }

    public AdvancedStat()
    {
        m_baseUpgradeCost = 10;
        m_currentLevelUpgradeCost = 10;
        perLevelPercentageBaseValue = 30;
    }

    public AdvancedStat Clone()
    {
        AdvancedStat clone = new AdvancedStat();
        clone.perLevelPercentageBaseValue = 30;
        clone.baseValue = baseValue;
        clone.maxLevel = maxLevel;
        clone.isRounded = isRounded;
        clone.calculation = calculation;
        clone.perLevelPercentageBaseValue = perLevelPercentageBaseValue;
        clone.perLevelAddBaseValue = perLevelAddBaseValue;
        clone.perLevelExponentBaseValue = perLevelExponentBaseValue;

        return clone;
    }
}