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
    public class ReasonWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Reason);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 21, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DESTROY", Reason.DESTROY);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "RELEASE", Reason.RELEASE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "TEMPORARY", Reason.TEMPORARY);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MATERIAL", Reason.MATERIAL);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SUMMON", Reason.SUMMON);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BATTLE", Reason.BATTLE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EFFECT", Reason.EFFECT);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "COST", Reason.COST);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ADJUST", Reason.ADJUST);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "LOST_TARGET", Reason.LOST_TARGET);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "RULE", Reason.RULE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SPSUMMON", Reason.SPSUMMON);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "RETURN", Reason.RETURN);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FUSION", Reason.FUSION);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SYNCHRO", Reason.SYNCHRO);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "RITUAL", Reason.RITUAL);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "XYZ", Reason.XYZ);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "REPLACE", Reason.REPLACE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DRAW", Reason.DRAW);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "REDIRECT", Reason.REDIRECT);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Reason does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
