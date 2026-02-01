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
    public class EffectWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Effect);
			Utils.BeginObjectRegister(type, L, translator, 0, 27, 19, 19);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetCode", _m_SetCode);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetType", _m_SetType);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetRange", _m_SetRange);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetTargetRange", _m_SetTargetRange);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetValue", _m_SetValue);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetLabel", _m_SetLabel);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetDescription", _m_SetDescription);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetCondition", _m_SetCondition);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetCost", _m_SetCost);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetTarget", _m_SetTarget);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetOperation", _m_SetOperation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsAvailable", _m_IsAvailable);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CheckCondition", _m_CheckCondition);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CheckCost", _m_CheckCost);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CheckTarget", _m_CheckTarget);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ExecuteOperation", _m_ExecuteOperation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddTarget", _m_AddTarget);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetTargets", _m_GetTargets);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetUnitTargets", _m_GetUnitTargets);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ClearTargets", _m_ClearTargets);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetHandler", _m_GetHandler);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetHandlerUnit", _m_GetHandlerUnit);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetCode", _m_GetCode);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetType", _m_GetType);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetValue", _m_GetValue);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsHasType", _m_IsHasType);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Clone", _m_Clone);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "EffectCode", _g_get_EffectCode);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "EffectType", _g_get_EffectType);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "EffectFlag", _g_get_EffectFlag);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Range", _g_get_Range);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "TargetRange", _g_get_TargetRange);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "TargetRangePlayer", _g_get_TargetRangePlayer);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OwnerCard", _g_get_OwnerCard);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OwnerUnit", _g_get_OwnerUnit);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Value", _g_get_Value);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Label", _g_get_Label);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Description", _g_get_Description);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Condition", _g_get_Condition);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Cost", _g_get_Cost);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Target", _g_get_Target);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Operation", _g_get_Operation);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsActivated", _g_get_IsActivated);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsDisabled", _g_get_IsDisabled);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "ResetCount", _g_get_ResetCount);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "ResetFlag", _g_get_ResetFlag);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "EffectCode", _s_set_EffectCode);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "EffectType", _s_set_EffectType);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "EffectFlag", _s_set_EffectFlag);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Range", _s_set_Range);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "TargetRange", _s_set_TargetRange);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "TargetRangePlayer", _s_set_TargetRangePlayer);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OwnerCard", _s_set_OwnerCard);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OwnerUnit", _s_set_OwnerUnit);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Value", _s_set_Value);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Label", _s_set_Label);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Description", _s_set_Description);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Condition", _s_set_Condition);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Cost", _s_set_Cost);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Target", _s_set_Target);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Operation", _s_set_Operation);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "IsActivated", _s_set_IsActivated);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "IsDisabled", _s_set_IsDisabled);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "ResetCount", _s_set_ResetCount);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "ResetFlag", _s_set_ResetFlag);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 2, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateEffect", _m_CreateEffect_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new Effect();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Effect constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetCode(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _code = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.SetCode( _code );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetType(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _type = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.SetType( _type );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetRange(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _range = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.SetRange( _range );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetTargetRange(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _s = LuaAPI.xlua_tointeger(L, 2);
                    int _o = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.SetTargetRange( _s, _o );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetValue(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.SetValue( _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetLabel(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.SetLabel( _label );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetDescription(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _desc = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.SetDescription( _desc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetCondition(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    Effect.ConditionDelegate _func = translator.GetDelegate<Effect.ConditionDelegate>(L, 2);
                    
                    gen_to_be_invoked.SetCondition( _func );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetCost(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    Effect.CostDelegate _func = translator.GetDelegate<Effect.CostDelegate>(L, 2);
                    
                    gen_to_be_invoked.SetCost( _func );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetTarget(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    Effect.TargetDelegate _func = translator.GetDelegate<Effect.TargetDelegate>(L, 2);
                    
                    gen_to_be_invoked.SetTarget( _func );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetOperation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    Effect.OperationDelegate _func = translator.GetDelegate<Effect.OperationDelegate>(L, 2);
                    
                    gen_to_be_invoked.SetOperation( _func );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsAvailable(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.IsAvailable(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CheckCondition(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 9&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& translator.Assignable<object>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& translator.Assignable<Effect>(L, 6)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 7)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 8)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 9)) 
                {
                    int _tp = LuaAPI.xlua_tointeger(L, 2);
                    object _eg = translator.GetObject(L, 3, typeof(object));
                    int _ep = LuaAPI.xlua_tointeger(L, 4);
                    int _ev = LuaAPI.xlua_tointeger(L, 5);
                    Effect _re = (Effect)translator.GetObject(L, 6, typeof(Effect));
                    int _r = LuaAPI.xlua_tointeger(L, 7);
                    int _rp = LuaAPI.xlua_tointeger(L, 8);
                    int _chk = LuaAPI.xlua_tointeger(L, 9);
                    
                        var gen_ret = gen_to_be_invoked.CheckCondition( _tp, _eg, _ep, _ev, _re, _r, _rp, _chk );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 8&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& translator.Assignable<object>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& translator.Assignable<Effect>(L, 6)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 7)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 8)) 
                {
                    int _tp = LuaAPI.xlua_tointeger(L, 2);
                    object _eg = translator.GetObject(L, 3, typeof(object));
                    int _ep = LuaAPI.xlua_tointeger(L, 4);
                    int _ev = LuaAPI.xlua_tointeger(L, 5);
                    Effect _re = (Effect)translator.GetObject(L, 6, typeof(Effect));
                    int _r = LuaAPI.xlua_tointeger(L, 7);
                    int _rp = LuaAPI.xlua_tointeger(L, 8);
                    
                        var gen_ret = gen_to_be_invoked.CheckCondition( _tp, _eg, _ep, _ev, _re, _r, _rp );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Effect.CheckCondition!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CheckCost(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 9&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& translator.Assignable<object>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& translator.Assignable<Effect>(L, 6)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 7)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 8)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 9)) 
                {
                    int _tp = LuaAPI.xlua_tointeger(L, 2);
                    object _eg = translator.GetObject(L, 3, typeof(object));
                    int _ep = LuaAPI.xlua_tointeger(L, 4);
                    int _ev = LuaAPI.xlua_tointeger(L, 5);
                    Effect _re = (Effect)translator.GetObject(L, 6, typeof(Effect));
                    int _r = LuaAPI.xlua_tointeger(L, 7);
                    int _rp = LuaAPI.xlua_tointeger(L, 8);
                    int _chk = LuaAPI.xlua_tointeger(L, 9);
                    
                        var gen_ret = gen_to_be_invoked.CheckCost( _tp, _eg, _ep, _ev, _re, _r, _rp, _chk );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 8&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& translator.Assignable<object>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& translator.Assignable<Effect>(L, 6)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 7)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 8)) 
                {
                    int _tp = LuaAPI.xlua_tointeger(L, 2);
                    object _eg = translator.GetObject(L, 3, typeof(object));
                    int _ep = LuaAPI.xlua_tointeger(L, 4);
                    int _ev = LuaAPI.xlua_tointeger(L, 5);
                    Effect _re = (Effect)translator.GetObject(L, 6, typeof(Effect));
                    int _r = LuaAPI.xlua_tointeger(L, 7);
                    int _rp = LuaAPI.xlua_tointeger(L, 8);
                    
                        var gen_ret = gen_to_be_invoked.CheckCost( _tp, _eg, _ep, _ev, _re, _r, _rp );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Effect.CheckCost!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CheckTarget(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 9&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& translator.Assignable<object>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& translator.Assignable<Effect>(L, 6)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 7)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 8)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 9)) 
                {
                    int _tp = LuaAPI.xlua_tointeger(L, 2);
                    object _eg = translator.GetObject(L, 3, typeof(object));
                    int _ep = LuaAPI.xlua_tointeger(L, 4);
                    int _ev = LuaAPI.xlua_tointeger(L, 5);
                    Effect _re = (Effect)translator.GetObject(L, 6, typeof(Effect));
                    int _r = LuaAPI.xlua_tointeger(L, 7);
                    int _rp = LuaAPI.xlua_tointeger(L, 8);
                    int _chk = LuaAPI.xlua_tointeger(L, 9);
                    
                        var gen_ret = gen_to_be_invoked.CheckTarget( _tp, _eg, _ep, _ev, _re, _r, _rp, _chk );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 8&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& translator.Assignable<object>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& translator.Assignable<Effect>(L, 6)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 7)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 8)) 
                {
                    int _tp = LuaAPI.xlua_tointeger(L, 2);
                    object _eg = translator.GetObject(L, 3, typeof(object));
                    int _ep = LuaAPI.xlua_tointeger(L, 4);
                    int _ev = LuaAPI.xlua_tointeger(L, 5);
                    Effect _re = (Effect)translator.GetObject(L, 6, typeof(Effect));
                    int _r = LuaAPI.xlua_tointeger(L, 7);
                    int _rp = LuaAPI.xlua_tointeger(L, 8);
                    
                        var gen_ret = gen_to_be_invoked.CheckTarget( _tp, _eg, _ep, _ev, _re, _r, _rp );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Effect.CheckTarget!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ExecuteOperation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _tp = LuaAPI.xlua_tointeger(L, 2);
                    object _eg = translator.GetObject(L, 3, typeof(object));
                    int _ep = LuaAPI.xlua_tointeger(L, 4);
                    int _ev = LuaAPI.xlua_tointeger(L, 5);
                    Effect _re = (Effect)translator.GetObject(L, 6, typeof(Effect));
                    int _r = LuaAPI.xlua_tointeger(L, 7);
                    int _rp = LuaAPI.xlua_tointeger(L, 8);
                    
                    gen_to_be_invoked.ExecuteOperation( _tp, _eg, _ep, _ev, _re, _r, _rp );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddTarget(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<RuntimeCard>(L, 2)) 
                {
                    RuntimeCard _card = (RuntimeCard)translator.GetObject(L, 2, typeof(RuntimeCard));
                    
                    gen_to_be_invoked.AddTarget( _card );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<RuntimeUnit>(L, 2)) 
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 2, typeof(RuntimeUnit));
                    
                    gen_to_be_invoked.AddTarget( _unit );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Effect.AddTarget!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetTargets(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetTargets(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetUnitTargets(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetUnitTargets(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearTargets(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.ClearTargets(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetHandler(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetHandler(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetHandlerUnit(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetHandlerUnit(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCode(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetCode(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetType(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetType(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetValue(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetValue(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsHasType(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _type = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.IsHasType( _type );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Clone(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.Clone(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateEffect_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeCard _c = (RuntimeCard)translator.GetObject(L, 1, typeof(RuntimeCard));
                    
                        var gen_ret = Effect.CreateEffect( _c );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_EffectCode(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.EffectCode);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_EffectType(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.EffectType);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_EffectFlag(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.EffectFlag);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Range(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Range);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TargetRange(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.TargetRange);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TargetRangePlayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.TargetRangePlayer);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OwnerCard(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OwnerCard);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OwnerUnit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OwnerUnit);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Value(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Value);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Label(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.Label);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Description(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.Description);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Condition(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Condition);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Cost(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Cost);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Target(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Target);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Operation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Operation);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsActivated(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsActivated);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsDisabled(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsDisabled);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_ResetCount(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.ResetCount);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_ResetFlag(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.ResetFlag);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_EffectCode(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.EffectCode = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_EffectType(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.EffectType = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_EffectFlag(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.EffectFlag = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Range(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Range = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_TargetRange(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.TargetRange = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_TargetRangePlayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.TargetRangePlayer = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OwnerCard(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OwnerCard = (RuntimeCard)translator.GetObject(L, 2, typeof(RuntimeCard));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OwnerUnit(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OwnerUnit = (RuntimeUnit)translator.GetObject(L, 2, typeof(RuntimeUnit));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Value(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Value = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Label(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Label = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Description(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Description = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Condition(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Condition = translator.GetDelegate<Effect.ConditionDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Cost(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Cost = translator.GetDelegate<Effect.CostDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Target(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Target = translator.GetDelegate<Effect.TargetDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Operation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Operation = translator.GetDelegate<Effect.OperationDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_IsActivated(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.IsActivated = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_IsDisabled(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.IsDisabled = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_ResetCount(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.ResetCount = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_ResetFlag(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Effect gen_to_be_invoked = (Effect)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.ResetFlag = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
