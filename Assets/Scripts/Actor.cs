#pragma warning disable CS0414
// orgHP, MP 아직 미사용

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Actor : MonoBehaviour
{
    [SerializeField] Define.actor _actor;
    [SerializeField] NavMeshAgent _navAgent = null;

    // 추후 원형 사거리 등을 위해 따로 cs로 빼서 사용해야 함
    [SerializeField] LineRenderer _lineRenderer = null;

    [SerializeField] private bool isAlly = true;

    [SerializeField] private int _orgHp = 100;
    [SerializeField] private int _currentHp = 100;
    [SerializeField] private int _orgMp = 100;
    [SerializeField] private int _currentMp = 100;
    [SerializeField] private float[] _cooldownTimers = new float[4];
    [SerializeField] private float _stunnedOrImmobileTimer = 0f;

    private Skill[] _skills = new Skill[4];
    private bool[] _preparingSkill = new bool[4];
    private (string, string[])[] _skillValues = new (string, string[])[4];
    private bool isPreparingSkill = false;
    private float _skillRange = 0f;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        Timer();

        if (!isAlly || _currentHp <= 0)
            return;

        if (_stunnedOrImmobileTimer > 0)
            return;

        SkillControl();
        DrawSkillRange();
    }

    #region Functions
    /// <summary>
    /// skill, enemy setting
    /// </summary>
    private void Init()
    {
        gameObject.layer = isAlly ? LayerMask.NameToLayer("ally") : LayerMask.NameToLayer("enemy");

        _skills = SkillManager.Instance.GetSkill(_actor.ToString());

        for (int i = -1; ++i < _skills.Length;)
        {
            string[] values = _skills[i].GetSkillInfo();
            _skillValues[i] = (values[1], values);

            _preparingSkill[i] = false;
        }
    }

    /// <summary>
    /// cooldown, StunnedOrImmobile
    /// </summary>
    private void Timer()
    {
        for (int i = -1; ++i < _cooldownTimers.Length;)
        {
            if (_cooldownTimers[i] > 0f)
                _cooldownTimers[i] -= Time.deltaTime;
            else if (_cooldownTimers[i] < 0f)
                _cooldownTimers[i] = 0f;
        }

        if (_stunnedOrImmobileTimer > 0f)
            _stunnedOrImmobileTimer -= Time.deltaTime;
        else if (_stunnedOrImmobileTimer < 0f)
            _stunnedOrImmobileTimer = 0f;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ResetSkillQuickCast();

                _navAgent.SetDestination(hit.point);
            }
        }
    }

    #region Skill
    private void SkillControl()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            SetSkill(0);
            Debug.Log("set Q");
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            SetSkill(1);
            Debug.Log("set W");
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            SetSkill(2);
            Debug.Log("set E");
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SetSkill(3);
            Debug.Log("set R");
        }
    }

    private void SetSkill(short key)
    {
        string[] values = _skillValues[key].Item2;
        int mpCost = int.Parse(values[3]);

        if (!CheckQuickCast(values, key))
            return;

        if (_skills[key] == null || _cooldownTimers[key] > 0f || _currentMp - mpCost < 0)
        {
            // skill이 사용불가할 떄 사용하는 UI Effect, sound
            return;
        }

        LookCursor();

        Actor target = null;

        bool isTargetable = values[5].Equals("TRUE") ? true : false;
        target = isTargetable ? FindTarget(int.Parse(values[6])) : null;

        ApplySkill(target, _skills[key]);
    }

    /// <summary>
    /// 사거리 표기를 해야 할 경우
    /// 사거리가 0 or 100이상일 경우 nontarget으로 간주 -> 사거리 표기 없이 즉시 시전
    /// 그 외 사거리가 있을 경우 표기
    /// </summary>
    /// <param name="values"></param>
    private bool CheckQuickCast(string[] values, int key)
    {
        if (!SettingManager.Instance.GetQuickCastSetting())
        {
            int range = int.Parse(values[6]);
            if (range == 0 || range >= 100 || _preparingSkill[key] == true || _cooldownTimers[key] > 0f)
            {
                ResetSkillQuickCast();
                return true;
            }

            for (int i = -1; ++i < _preparingSkill.Length;)
                _preparingSkill[i] = false;

            _preparingSkill[key] = true;

            _lineRenderer.enabled = true;
            _skillRange = range;
            isPreparingSkill = true;

            return false;
        }

        return true;
    }

    private void ResetSkillQuickCast()
    {
        _lineRenderer.enabled = false;
        isPreparingSkill = false;

        for (int i = -1; ++i < _preparingSkill.Length;)
            _preparingSkill[i] = false;
    }

    private void LookCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 mouseDirection = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);

            transform.rotation = Quaternion.LookRotation((mouseDirection - transform.position).normalized);
        }
    }

    Actor FindTarget(int range)
    {
        Actor target = null;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if ((hitInfo.collider.CompareTag("Player") && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("enemy"))
                || hitInfo.collider.CompareTag("Monster"))
            {
                if (Vector3.Distance(hitInfo.collider.gameObject.transform.position, gameObject.transform.position) <= range)
                    target = hitInfo.collider.GetComponent<Actor>();
            }
        }

        return target;
    }

    public void ApplySkill(Actor target, Skill skill)
    {
        if (skill.ApplySkill(this, target))
        {
            Debug.Log($"{name} applied {skill.GetType().Name}");
        }
        else
        {
            Debug.Log($"{name} failed to apply {skill.GetType().Name}");
        }
    }

    /// <summary>
    /// 현재는 RocketPunch(일직선 표기)를 위해서만 있는 메서드
    /// </summary>
    private void DrawSkillRange()
    {
        if (!isPreparingSkill || _skillRange == 0f)
            return;

        _lineRenderer.SetPosition(0, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 mouseDirection = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);

            _lineRenderer.SetPosition(1, transform.position + (mouseDirection - transform.position).normalized * _skillRange);
        }
    }
    #endregion

    #region Effects
    public void ApplyCooldown(float cooldown, string skillName)
    {
        for (int i = -1; ++i < _skillValues.Length;)
        {
            if (skillName.Equals(_skillValues[i].Item1))
            {
                _cooldownTimers[i] = cooldown;
                break;
            }
        }
    }

    public void UseMp(int mpCost)
    {
        _currentMp -= mpCost;
    }

    public void GetDamage(int damage)
    {
        _currentHp -= damage;
    }

    public void StunnedOrImmobile(float time)
    {
        _stunnedOrImmobileTimer = time;
    }
    #endregion

    #endregion
}