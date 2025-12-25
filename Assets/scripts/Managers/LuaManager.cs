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
        
        // Add a custom loader to load .lua files from Resources
        GlobalEnv.AddLoader(CustomLoader);

        // Load initialization scripts if needed (e.g. requires, enums)
        // GlobalEnv.DoString("require 'c_base'"); 
        Debug.Log("[LuaManager] LuaEnv Initialized.");
    }

    private byte[] CustomLoader(ref string filepath)
    {
        // Redirect "require 'xxx'" to load "Assets/Resources/Lua/xxx.lua.txt"
        // Unity Resources.Load works with "Lua/xxx" if the file is "Assets/Resources/Lua/xxx.lua.txt"
        // Note: xLua usually requires adding a .txt extension to .lua files in Resources.
        
        string scriptPath = "Lua/" + filepath.Replace('.', '/');
        TextAsset file = Resources.Load<TextAsset>(scriptPath);
        if (file != null)
        {
            return file.bytes;
        }
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
