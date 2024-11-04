using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static SkillManager instance;
    public static SkillManager Instance { get { return instance; } }

    [SerializeField] private TextAsset _skillDataCSV;
    [Tooltip("actor, skill[]")]
    private Dictionary<string, Skill[]> _skillDictionary = new Dictionary<string, Skill[]>();

    private void Awake()
    {
        Init();
    }

    #region Functions
    private void Init()
    {
        instance = this;
        DontDestroyOnLoad(instance);

        LoadSkillData();
    }

    private void LoadSkillData()
    {
        string actor = string.Empty;
        Skill[] skill = new Skill[4];
        short skill_index = 0;

        string[] skillData = _skillDataCSV.text.Split('\n');

        for (int i = 0; ++i < skillData.Length;)
        {
            string data = skillData[i];

            if (string.IsNullOrWhiteSpace(data))
                continue;

            string[] values = data.Split(',');

            if (!actor.Equals(values[0]))
                actor = values[0];

            string skillName = values[1];
            float cooldown = float.Parse(values[2]);
            int mpCost = int.Parse(values[3]);
            int damage = int.Parse(values[4]);
            float stunnedOrImmobile = float.Parse(values[7]);

            Skill newSkill = CreateSkill(skillName, values);

            newSkill.AddEffects(new List<Effect>
            {
                new Cooldown(cooldown, skillName),
                new MpCost(mpCost),
                new Damage(damage),
                new StunnedOrImmobile(stunnedOrImmobile)
            }
            );

            skill[skill_index] = newSkill;

            if (++skill_index >= 4)
            {
                _skillDictionary.Add(actor, skill);
                skill = new Skill[4];
                skill_index = 0;
            }
        }
    }

    private Skill CreateSkill(string skillName, string[] values)
    {
        switch (skillName)
        {
            case Define._roketGrab:
                return new RocketGrab(values);
            case Define._blitzcrank_w:
                return new blitzcrank_w(values);
            case Define._blitzcrank_e:
                return new blitzcrank_e(values);
            case Define._blitzcrank_r:
                return new blitzcrank_r(values);
            case Define._mysticShot:
                return new MysticShot(values);
            case Define._ezreal_w:
                return new ezreal_w(values);
            case Define._ezreal_e:
                return new ezreal_e(values);
            case Define._ezreal_r:
                return new ezreal_r(values);
            default:
                Debug.LogError($"{skillName} 이 존재하지 않습니다.");
                return null;
        }
    }

    public Skill[] GetSkill(string actor)
    {
        if (_skillDictionary.TryGetValue(actor, out Skill[] skill))
        {
            return skill;
        }

        Debug.LogError($"{actor} 이 존재하지 않습니다.");
        return null;
    }
    #endregion
}