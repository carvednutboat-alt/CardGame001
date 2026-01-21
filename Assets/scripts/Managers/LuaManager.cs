using UnityEngine;
using XLua;
using System.IO;

public class LuaManager : MonoBehaviour
{
    public static LuaManager Instance;
    public LuaEnv GlobalEnv { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitLuaEnv();
    }

    private void InitLuaEnv()
    {
        GlobalEnv = new LuaEnv();
        
        // 注册自定义加载器，用于从 Resources 文件夹加载 .lua.txt 文件
        GlobalEnv.AddLoader(CustomLoader);
        
        // 直接将 C# 常量类映射到 Lua 全局变量 (无需 CS. 前缀)
        // 使用 DoString 进行别名映射，确保 Lua 中能像访问静态类一样访问这些 Enum
        GlobalEnv.DoString(@"
            EffectType = CS.EffectType
            Location = CS.Location
            EffectCode = CS.EffectCode
            EventCode = CS.EventCode
            EffectFlag = CS.EffectFlag
            Reason = CS.Reason
            Phase = CS.Phase
            Position = CS.Position
            Player = CS.Player
            CardTag = CS.CardTag
            
            -- Map Core System Classes
            Duel = CS.Duel
            Effect = CS.Effect
            Card = CS.RuntimeCard -- Optional alias if needed
        ");
        
        Debug.Log("[LuaManager] LuaEnv Initialized. C# Constants mapped.");
    }

    private byte[] CustomLoader(ref string filepath)
    {
        // 1. Try standard path: "Lua/c1001"
        string scriptPath = "Lua/" + filepath.Replace('.', '/');
        TextAsset file = Resources.Load<TextAsset>(scriptPath);
        
        if (file == null)
        {
            // 2. Try with .lua suffix: "Lua/c1001.lua" (Common for .lua.txt files in Unity)
            file = Resources.Load<TextAsset>(scriptPath + ".lua");
        }

        if (file != null)
        {
            Debug.Log($"[LuaManager] Loaded script: {filepath}");
            return file.bytes;
        }
        
        Debug.LogError($"[LuaManager] Failed to load script: {filepath}. Searched paths: {scriptPath}, {scriptPath}.lua");
        return null;
    }

    public object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
    {
        return GlobalEnv.DoString(chunk, chunkName, env);
    }

    public LuaTable NewTable()
    {
        return GlobalEnv.NewTable();
    }

    private void OnDestroy()
    {
        if (GlobalEnv != null)
        {
            GlobalEnv.Dispose();
            GlobalEnv = null;
        }
    }
    
    // Helper to safely get function
    public LuaFunction GetFunction(LuaTable table, string funcName)
    {
        if (table == null) return null;
        return table.Get<LuaFunction>(funcName);
    }
}
