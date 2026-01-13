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
    public class LocationWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Location);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 11, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "HAND", Location.HAND);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MZONE", Location.MZONE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SZONE", Location.SZONE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "GRAVE", Location.GRAVE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "REMOVED", Location.REMOVED);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DECK", Location.DECK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EXTRA", Location.EXTRA);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "OVERLAY", Location.OVERLAY);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ONFIELD", Location.ONFIELD);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ALL", Location.ALL);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Location does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
