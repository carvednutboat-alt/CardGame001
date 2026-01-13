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
    public class EffectTypeWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(EffectType);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 8, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SINGLE", EffectType.SINGLE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FIELD", EffectType.FIELD);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "IGNITION", EffectType.IGNITION);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "TRIGGER", EffectType.TRIGGER);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "QUICK", EffectType.QUICK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CONTINUOUS", EffectType.CONTINUOUS);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EQUIP", EffectType.EQUIP);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "EffectType does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
