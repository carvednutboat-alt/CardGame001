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
    public class EffectFlagWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(EffectFlag);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 11, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SINGLE_RANGE", EffectFlag.SINGLE_RANGE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BOTH_SIDE", EffectFlag.BOTH_SIDE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_DISABLE", EffectFlag.CANNOT_DISABLE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "IMMEDIATELY_APPLY", EffectFlag.IMMEDIATELY_APPLY);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "COPY_INHERIT", EffectFlag.COPY_INHERIT);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "OWNER_RELATE", EffectFlag.OWNER_RELATE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "PLAYER_TARGET", EffectFlag.PLAYER_TARGET);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CARD_TARGET", EffectFlag.CARD_TARGET);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DELAY", EffectFlag.DELAY);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "INITIAL", EffectFlag.INITIAL);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "EffectFlag does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
