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
        //skillUpgradePanel.SetActive(false);
    }
    public void UpgradeSlow()
    {
        //digdi


        skillUpgradePanel.SetActive(false);
        PlayerController playerController = FindObjectOfType<PlayerController>();

        playerController.ExpTake(0);
    }
    public void UpgradeStun()
    {
        
    
        skillManager.NewStunValue(addStunValue);
        addStunValue++;
        skillUpgradePanel.SetActive(false);
        PlayerController playerController = FindObjectOfType<PlayerController>();

        playerController.ExpTake(0);

    }
    public void UpgradeHeal()
    {
        //digdi

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
