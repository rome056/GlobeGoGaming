using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public SlowField slowFieldSkill;
    public StunField stunFieldSkill;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateSlowField();
        }
    }

    public void ActivateSlowField()
    {
        if (slowFieldSkill != null)
            slowFieldSkill.Activate();
    }
    public void ActivateStunField()
    {
        if (stunFieldSkill != null)
            stunFieldSkill.Activate();
    }
}
