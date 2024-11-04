/// <summary>
/// 자원 소모, 스킬의 기능, 기타 효과 등 필요
/// </summary>
public abstract class Effect
{
    public abstract void Apply(Actor source, Actor target);
}

public class Cooldown : Effect
{
    private float cooldown;
    private string skillName;

    public Cooldown(float cooldown, string skillName)
    {
        this.cooldown = cooldown;
        this.skillName = skillName;
    }

    public override void Apply(Actor source, Actor target)
    {
        source.ApplyCooldown(cooldown, skillName);
    }
}

public class MpCost : Effect
{
    private int mpCost;

    public MpCost(int mpCost)
    {
        this.mpCost = mpCost;
    }

    public override void Apply(Actor source, Actor target)
    {
        source.UseMp(mpCost);
    }
}

public class Damage : Effect
{
    private int damage;

    public Damage(int damage)
    {
        this.damage = damage;
    }

    public override void Apply(Actor source, Actor target)
    {
        target.GetDamage(damage);
    }
}

public class StunnedOrImmobile : Effect
{
    private float time;

    public StunnedOrImmobile(float time)
    {
        this.time = time;
    }

    public override void Apply(Actor source, Actor target)
    {
        target.StunnedOrImmobile(time);
    }
}