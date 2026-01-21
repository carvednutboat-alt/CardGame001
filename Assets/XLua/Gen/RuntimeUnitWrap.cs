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
    public class RuntimeUnitWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(RuntimeUnit);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 25, 23);
			
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "Attack", _g_get_Attack);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsDead", _g_get_IsDead);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Id", _g_get_Id);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Name", _g_get_Name);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "SourceCard", _g_get_SourceCard);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "UI", _g_get_UI);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "EnemyUI", _g_get_EnemyUI);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "BaseMaxHp", _g_get_BaseMaxHp);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "MaxHp", _g_get_MaxHp);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "CurrentHp", _g_get_CurrentHp);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "BaseAtk", _g_get_BaseAtk);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "CurrentAtk", _g_get_CurrentAtk);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "PermAttackModifier", _g_get_PermAttackModifier);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "TempAttackModifier", _g_get_TempAttackModifier);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OverrideName", _g_get_OverrideName);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsFlying", _g_get_IsFlying);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "HasTaunt", _g_get_HasTaunt);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "CanAttack", _g_get_CanAttack);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsEvolved", _g_get_IsEvolved);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "EvolveTurnsLeft", _g_get_EvolveTurnsLeft);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Overload", _g_get_Overload);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsFatigued", _g_get_IsFatigued);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "RobotEvolved", _g_get_RobotEvolved);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "PendingOverloadSelfDamage", _g_get_PendingOverloadSelfDamage);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Equips", _g_get_Equips);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "Id", _s_set_Id);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Name", _s_set_Name);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "SourceCard", _s_set_SourceCard);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "UI", _s_set_UI);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "EnemyUI", _s_set_EnemyUI);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "BaseMaxHp", _s_set_BaseMaxHp);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "MaxHp", _s_set_MaxHp);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "CurrentHp", _s_set_CurrentHp);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "BaseAtk", _s_set_BaseAtk);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "CurrentAtk", _s_set_CurrentAtk);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "PermAttackModifier", _s_set_PermAttackModifier);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "TempAttackModifier", _s_set_TempAttackModifier);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OverrideName", _s_set_OverrideName);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "IsFlying", _s_set_IsFlying);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "HasTaunt", _s_set_HasTaunt);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "CanAttack", _s_set_CanAttack);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "IsEvolved", _s_set_IsEvolved);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "EvolveTurnsLeft", _s_set_EvolveTurnsLeft);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Overload", _s_set_Overload);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "IsFatigued", _s_set_IsFatigued);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "RobotEvolved", _s_set_RobotEvolved);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "PendingOverloadSelfDamage", _s_set_PendingOverloadSelfDamage);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Equips", _s_set_Equips);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 3 && LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2) && translator.Assignable<RuntimeCard>(L, 3))
				{
					int _id = LuaAPI.xlua_tointeger(L, 2);
					RuntimeCard _card = (RuntimeCard)translator.GetObject(L, 3, typeof(RuntimeCard));
					
					var gen_ret = new RuntimeUnit(_id, _card);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 2 && translator.Assignable<CardData>(L, 2))
				{
					CardData _data = (CardData)translator.GetObject(L, 2, typeof(CardData));
					
					var gen_ret = new RuntimeUnit(_data);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to RuntimeUnit constructor!");
            
        }
        
		
        
		
        
        
        
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Attack(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Attack);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsDead(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsDead);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Id(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Id);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Name(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.Name);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_SourceCard(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.SourceCard);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_UI(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.UI);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_EnemyUI(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.EnemyUI);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BaseMaxHp(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.BaseMaxHp);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MaxHp(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.MaxHp);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CurrentHp(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.CurrentHp);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BaseAtk(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.BaseAtk);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CurrentAtk(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.CurrentAtk);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_PermAttackModifier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.PermAttackModifier);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TempAttackModifier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.TempAttackModifier);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OverrideName(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.OverrideName);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsFlying(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsFlying);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_HasTaunt(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.HasTaunt);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CanAttack(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.CanAttack);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsEvolved(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsEvolved);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_EvolveTurnsLeft(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.EvolveTurnsLeft);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Overload(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Overload);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsFatigued(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsFatigued);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_RobotEvolved(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.RobotEvolved);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_PendingOverloadSelfDamage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.PendingOverloadSelfDamage);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Equips(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Equips);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Id(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Id = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Name(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Name = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_SourceCard(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.SourceCard = (RuntimeCard)translator.GetObject(L, 2, typeof(RuntimeCard));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_UI(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.UI = (FieldUnitUI)translator.GetObject(L, 2, typeof(FieldUnitUI));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_EnemyUI(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.EnemyUI = (EnemyUnitUI)translator.GetObject(L, 2, typeof(EnemyUnitUI));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BaseMaxHp(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.BaseMaxHp = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_MaxHp(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.MaxHp = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CurrentHp(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.CurrentHp = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BaseAtk(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.BaseAtk = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CurrentAtk(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.CurrentAtk = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_PermAttackModifier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.PermAttackModifier = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_TempAttackModifier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.TempAttackModifier = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OverrideName(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OverrideName = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_IsFlying(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.IsFlying = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_HasTaunt(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.HasTaunt = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CanAttack(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.CanAttack = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_IsEvolved(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.IsEvolved = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_EvolveTurnsLeft(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.EvolveTurnsLeft = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Overload(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Overload = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_IsFatigued(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.IsFatigued = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_RobotEvolved(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.RobotEvolved = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_PendingOverloadSelfDamage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.PendingOverloadSelfDamage = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Equips(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeUnit gen_to_be_invoked = (RuntimeUnit)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Equips = (System.Collections.Generic.List<CardData>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<CardData>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
