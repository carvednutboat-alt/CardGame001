#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class PhaseWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Phase);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 11, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DRAW", Phase.DRAW);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "STANDBY", Phase.STANDBY);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MAIN1", Phase.MAIN1);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BATTLE_START", Phase.BATTLE_START);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BATTLE_STEP", Phase.BATTLE_STEP);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DAMAGE", Phase.DAMAGE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DAMAGE_CAL", Phase.DAMAGE_CAL);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BATTLE", Phase.BATTLE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MAIN2", Phase.MAIN2);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "END", Phase.END);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Phase does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
