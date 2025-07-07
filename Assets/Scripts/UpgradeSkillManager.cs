using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSkillManager : MonoBehaviour
{
    private SkillManager skillManager;
    public GameObject skillUpgradePanel;
    public float addStunValue = 0.5f;

    private void Start()
    {
        skillManager = FindObjectOfType<SkillManager>();

        addStunValue = 0.5f;
        skillUpgradePanel.SetActive(false);
    }
    public void UpgradeSlow()
    {
        
    }
    public void UpgradeStun()
    {
        skillManager.NewStunValue(addStunValue);
        addStunValue++;
        skillUpgradePanel.SetActive(false);
    }
    public void UpgradeHeal()
    {

    }
    public void UpgradeClone()
    {

    }
}
