using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill
{
    private List<Effect> _EffectList { get; } = new();
    protected string[] _skill_Info = null;

    public Skill(string[] skill_Info)
    {
        _skill_Info = skill_Info;
    }

    public void AddEffects(List<Effect> effects)
    {
        _EffectList.AddRange(effects);
    }

    protected void ApplyEffectsTargetNoneTarget(Actor source, Actor target)
    {
        for (int i = 0; i < 2; i++)
            _EffectList[i].Apply(source, target);
    }

    protected void ApplyEffectsTarget(Actor source, Actor target)
    {
        for (int i = 2; i < 4; i++)
            _EffectList[i].Apply(source, target);
    }

    public string[] GetSkillInfo()
    {
        return _skill_Info;
    }

    public abstract bool ApplySkill(Actor source, Actor target);
}

#region Skills
public class RocketGrab : Skill
{
    public RocketGrab(string[] skill_Info) : base(skill_Info) { }
    int range = 0;

    public override bool ApplySkill(Actor source, Actor target)
    {
        ApplyEffectsTargetNoneTarget(source, target);

        range = int.Parse(_skill_Info[6]);

        SoundManager.Instance.PlaySFX(SoundManager.Instance._AC_sfx_qBefore);

        Projectile projectile = source.GetComponentInChildren<Projectile>(true);
        projectile.Settings(source, this, range);
        projectile.gameObject.SetActive(true);

        return true;
    }

    public void OnRocketGrabSuccess(Actor source, Actor target)
    {
        ApplyEffectsTarget(source, target);

        SoundManager.Instance.PlaySFX(SoundManager.Instance._AC_sfx_qAfter);

        Debug.Log($"{source.name} used Rocket Grab on Target {target.name}");
    }
}

public class blitzcrank_w : Skill
{
    public blitzcrank_w(string[] skill_Info) : base(skill_Info) { }

    public override bool ApplySkill(Actor source, Actor target)
    {
        return false;
    }
}

public class blitzcrank_e : Skill
{
    public blitzcrank_e(string[] skill_Info) : base(skill_Info) { }

    public override bool ApplySkill(Actor source, Actor target)
    {
        return false;
    }
}

public class blitzcrank_r : Skill
{
    public blitzcrank_r(string[] skill_Info) : base(skill_Info) { }

    public override bool ApplySkill(Actor source, Actor target)
    {
        return false;
    }
}


public class MysticShot : Skill
{
    public MysticShot(string[] skill_Info) : base(skill_Info) { }

    public override bool ApplySkill(Actor source, Actor target)
    {
        return false;
    }
}

public class ezreal_w : Skill
{
    public ezreal_w(string[] skill_Info) : base(skill_Info) { }

    public override bool ApplySkill(Actor source, Actor target)
    {
        return false;
    }
}

public class ezreal_e : Skill
{
    public ezreal_e(string[] skill_Info) : base(skill_Info) { }

    public override bool ApplySkill(Actor source, Actor target)
    {
        return false;
    }
}

public class ezreal_r : Skill
{
    public ezreal_r(string[] skill_Info) : base(skill_Info) { }

    public override bool ApplySkill(Actor source, Actor target)
    {
        return false;
    }
}
#endregion