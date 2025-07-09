using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSkillManager : MonoBehaviour
{
    private SkillManager skillManager;
    public GameObject skillUpgradePanel;
    public float addStunValue = 0.5f;
    public float addSlowValue = 0.5f;
    public int addHealValue = 4;

    private void Start()
    {
        skillManager = FindObjectOfType<SkillManager>();

        addStunValue = 0.5f;
        addSlowValue = 0.5f;
        addHealValue = 4;
        //skillUpgradePanel.SetActive(false);
    }
    public void UpgradeSlow()
    {
        skillManager.NewSlowValue(addSlowValue);
        addSlowValue++;
        addSlowValue = 0.5f;

        skillUpgradePanel.SetActive(false);
        PlayerController playerController = FindObjectOfType<PlayerController>();

        playerController.ExpTake(0);
    }
    public void UpgradeStun()
    {
        skillManager.NewStunValue(addStunValue);
        addStunValue++;
        addStunValue = 0.5f;

        skillUpgradePanel.SetActive(false);
        PlayerController playerController = FindObjectOfType<PlayerController>();

        playerController.ExpTake(0);

    }
    public void UpgradeHeal()
    {
        skillManager.NewHealValue(addHealValue);
        addHealValue++;
        addHealValue = 4;

        skillUpgradePanel.SetActive(false);
        PlayerController playerController = FindObjectOfType<PlayerController>();

        playerController.ExpTake(0);
    }
    public void UpgradeClone()
    {
        //digdi

        skillUpgradePanel.SetActive(false);
        PlayerController playerController = FindObjectOfType<PlayerController>();

        playerController.ExpTake(0);
    }
}
