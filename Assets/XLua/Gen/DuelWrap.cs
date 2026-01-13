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
    public class DuelWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Duel);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 35, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateToken", _m_CreateToken_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SendtoDeck", _m_SendtoDeck_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ShuffleDeck", _m_ShuffleDeck_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddToHand", _m_AddToHand_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SendToGrave", _m_SendToGrave_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SearchDeckAndAddToHand", _m_SearchDeckAndAddToHand_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetFieldUnitCount", _m_GetFieldUnitCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetFieldUnits", _m_GetFieldUnits_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetUnitCount", _m_GetUnitCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetDeckCount", _m_GetDeckCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetHandCount", _m_GetHandCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ModifyATK", _m_ModifyATK_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ConsolidateTempATK", _m_ConsolidateTempATK_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetBaseMaxHP", _m_SetBaseMaxHP_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetCurrentHP", _m_SetCurrentHP_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Heal", _m_Heal_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Damage", _m_Damage_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DamageAll", _m_DamageAll_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DestroyUnit", _m_DestroyUnit_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SwapUnitPositions", _m_SwapUnitPositions_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetLastAttacker", _m_GetLastAttacker_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetLastAttacker", _m_SetLastAttacker_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DoubleBattleDamage", _m_DoubleBattleDamage_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "EvolveUnit", _m_EvolveUnit_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddOverload", _m_AddOverload_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RemoveOverload", _m_RemoveOverload_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GrantPlayerImmuneToEffects", _m_GrantPlayerImmuneToEffects_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetSelection", _m_SetSelection_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetFirstTarget", _m_GetFirstTarget_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ClearSelection", _m_ClearSelection_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SelectTarget", _m_SelectTarget_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Log", _m_Log_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "TriggerEffect", _m_TriggerEffect_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterEffect", _m_RegisterEffect_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Duel does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateToken_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _player = LuaAPI.xlua_tointeger(L, 1);
                    int _cardId = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = Duel.CreateToken( _player, _cardId );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendtoDeck_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeCard _card = (RuntimeCard)translator.GetObject(L, 1, typeof(RuntimeCard));
                    int _player = LuaAPI.xlua_tointeger(L, 2);
                    int _position = LuaAPI.xlua_tointeger(L, 3);
                    int _reason = LuaAPI.xlua_tointeger(L, 4);
                    
                    Duel.SendtoDeck( _card, _player, _position, _reason );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ShuffleDeck_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _player = LuaAPI.xlua_tointeger(L, 1);
                    
                    Duel.ShuffleDeck( _player );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddToHand_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeCard _card = (RuntimeCard)translator.GetObject(L, 1, typeof(RuntimeCard));
                    
                    Duel.AddToHand( _card );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendToGrave_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeCard _card = (RuntimeCard)translator.GetObject(L, 1, typeof(RuntimeCard));
                    
                    Duel.SendToGrave( _card );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SearchDeckAndAddToHand_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeCardFilter _filter = translator.GetDelegate<RuntimeCardFilter>(L, 1);
                    
                        var gen_ret = Duel.SearchDeckAndAddToHand( _filter );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetFieldUnitCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _player = LuaAPI.xlua_tointeger(L, 1);
                    int _location = LuaAPI.xlua_tointeger(L, 2);
                    int _range = LuaAPI.xlua_tointeger(L, 3);
                    
                        var gen_ret = Duel.GetFieldUnitCount( _player, _location, _range );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetFieldUnits_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _player = LuaAPI.xlua_tointeger(L, 1);
                    int _location = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = Duel.GetFieldUnits( _player, _location );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetUnitCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool _player = LuaAPI.lua_toboolean(L, 1);
                    
                        var gen_ret = Duel.GetUnitCount( _player );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 0) 
                {
                    
                        var gen_ret = Duel.GetUnitCount(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Duel.GetUnitCount!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetDeckCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = Duel.GetDeckCount(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetHandCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        var gen_ret = Duel.GetHandCount(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ModifyATK_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    bool _permanent = LuaAPI.lua_toboolean(L, 3);
                    
                    Duel.ModifyATK( _unit, _value, _permanent );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ConsolidateTempATK_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    
                    Duel.ConsolidateTempATK( _unit );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetBaseMaxHP_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    Duel.SetBaseMaxHP( _unit, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetCurrentHP_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    int _value = LuaAPI.xlua_tointeger(L, 2);
                    
                    Duel.SetCurrentHP( _unit, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Heal_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    int _amount = LuaAPI.xlua_tointeger(L, 2);
                    
                    Duel.Heal( _unit, _amount );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Damage_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _target = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    int _amount = LuaAPI.xlua_tointeger(L, 2);
                    
                    Duel.Damage( _target, _amount );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DamageAll_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    int _amount = LuaAPI.xlua_tointeger(L, 1);
                    bool _playerUnits = LuaAPI.lua_toboolean(L, 2);
                    bool _enemyUnits = LuaAPI.lua_toboolean(L, 3);
                    
                    Duel.DamageAll( _amount, _playerUnits, _enemyUnits );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)) 
                {
                    int _amount = LuaAPI.xlua_tointeger(L, 1);
                    bool _playerUnits = LuaAPI.lua_toboolean(L, 2);
                    
                    Duel.DamageAll( _amount, _playerUnits );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int _amount = LuaAPI.xlua_tointeger(L, 1);
                    
                    Duel.DamageAll( _amount );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Duel.DamageAll!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DestroyUnit_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    
                    Duel.DestroyUnit( _unit );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SwapUnitPositions_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit1 = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    RuntimeUnit _unit2 = (RuntimeUnit)translator.GetObject(L, 2, typeof(RuntimeUnit));
                    
                    Duel.SwapUnitPositions( _unit1, _unit2 );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetLastAttacker_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        var gen_ret = Duel.GetLastAttacker(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetLastAttacker_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    
                    Duel.SetLastAttacker( _unit );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DoubleBattleDamage_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    
                    Duel.DoubleBattleDamage( _unit );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EvolveUnit_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    string _newName = LuaAPI.lua_tostring(L, 2);
                    string _newNameEn = LuaAPI.lua_tostring(L, 3);
                    
                    Duel.EvolveUnit( _unit, _newName, _newNameEn );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddOverload_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    int _amount = LuaAPI.xlua_tointeger(L, 2);
                    
                    Duel.AddOverload( _unit, _amount );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveOverload_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    int _amount = LuaAPI.xlua_tointeger(L, 2);
                    
                    Duel.RemoveOverload( _unit, _amount );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GrantPlayerImmuneToEffects_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _player = LuaAPI.xlua_tointeger(L, 1);
                    bool _immune = LuaAPI.lua_toboolean(L, 2);
                    
                    Duel.GrantPlayerImmuneToEffects( _player, _immune );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetSelection_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    RuntimeUnit _unit = (RuntimeUnit)translator.GetObject(L, 1, typeof(RuntimeUnit));
                    
                    Duel.SetSelection( _unit );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetFirstTarget_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        var gen_ret = Duel.GetFirstTarget(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearSelection_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    Duel.ClearSelection(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SelectTarget_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    int _player = LuaAPI.xlua_tointeger(L, 1);
                    int _location = LuaAPI.xlua_tointeger(L, 2);
                    int _min = LuaAPI.xlua_tointeger(L, 3);
                    int _max = LuaAPI.xlua_tointeger(L, 4);
                    
                    Duel.SelectTarget( _player, _location, _min, _max );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<RuntimeCard>(L, 1)&& translator.Assignable<CardTargetType>(L, 2)) 
                {
                    RuntimeCard _c = (RuntimeCard)translator.GetObject(L, 1, typeof(RuntimeCard));
                    CardTargetType _targetType;translator.Get(L, 2, out _targetType);
                    
                    Duel.SelectTarget( _c, _targetType );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Duel.SelectTarget!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Log_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _msg = LuaAPI.lua_tostring(L, 1);
                    
                    Duel.Log( _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TriggerEffect_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _effectKey = LuaAPI.lua_tostring(L, 1);
                    EffectContext _context = (EffectContext)translator.GetObject(L, 2, typeof(EffectContext));
                    
                    Duel.TriggerEffect( _effectKey, _context );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterEffect_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _effectKey = LuaAPI.lua_tostring(L, 1);
                    System.Action<EffectContext> _handler = translator.GetDelegate<System.Action<EffectContext>>(L, 2);
                    
                    Duel.RegisterEffect( _effectKey, _handler );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
