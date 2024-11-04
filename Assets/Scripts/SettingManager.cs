using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private static SettingManager instance;
    public static SettingManager Instance { get { return instance; } }

    [SerializeField] private bool isQuickCast = false;

    private void Awake()
    {
        Init();
    }

    #region Functions
    private void Init()
    {
        instance = this;
        DontDestroyOnLoad(instance);
    }

    public void SetQuickCast(bool setQuickCast)
    {
        isQuickCast = setQuickCast;
    }

    public bool GetQuickCastSetting()
    {
        return isQuickCast;
    }
    #endregion
}
