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
    public class PositionWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Position);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 9, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FACEUP_ATTACK", Position.FACEUP_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FACEDOWN_ATTACK", Position.FACEDOWN_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FACEUP_DEFENSE", Position.FACEUP_DEFENSE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FACEDOWN_DEFENSE", Position.FACEDOWN_DEFENSE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FACEUP", Position.FACEUP);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FACEDOWN", Position.FACEDOWN);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ATTACK", Position.ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DEFENSE", Position.DEFENSE);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Position does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
