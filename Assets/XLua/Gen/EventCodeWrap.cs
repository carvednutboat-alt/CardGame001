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
    public class EventCodeWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(EventCode);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 32, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SUMMON", EventCode.SUMMON);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FLIP_SUMMON", EventCode.FLIP_SUMMON);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SPECIAL_SUMMON", EventCode.SPECIAL_SUMMON);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SUMMON_SUCCESS", EventCode.SUMMON_SUCCESS);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DESTROYED", EventCode.DESTROYED);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BATTLE_DESTROYED", EventCode.BATTLE_DESTROYED);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DESTROYED_BY_EFFECT", EventCode.DESTROYED_BY_EFFECT);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BATTLE_START", EventCode.BATTLE_START);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BATTLE_END", EventCode.BATTLE_END);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ATTACK_ANNOUNCE", EventCode.ATTACK_ANNOUNCE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "BE_BATTLE_TARGET", EventCode.BE_BATTLE_TARGET);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DAMAGE_STEP_START", EventCode.DAMAGE_STEP_START);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DAMAGE_CALCULATING", EventCode.DAMAGE_CALCULATING);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DAMAGE_STEP_END", EventCode.DAMAGE_STEP_END);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "TO_HAND", EventCode.TO_HAND);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "TO_DECK", EventCode.TO_DECK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "TO_GRAVE", EventCode.TO_GRAVE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "REMOVE", EventCode.REMOVE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DRAW", EventCode.DRAW);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DAMAGE", EventCode.DAMAGE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "RECOVER", EventCode.RECOVER);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "PHASE_START", EventCode.PHASE_START);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "PHASE_END", EventCode.PHASE_END);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "TURN_END", EventCode.TURN_END);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHAIN_SOLVING", EventCode.CHAIN_SOLVING);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHAIN_SOLVED", EventCode.CHAIN_SOLVED);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHAIN_END", EventCode.CHAIN_END);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EQUIP", EventCode.EQUIP);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EQUIPPED", EventCode.EQUIPPED);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "LEAVE_FIELD", EventCode.LEAVE_FIELD);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FREE_CHAIN", EventCode.FREE_CHAIN);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "EventCode does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
