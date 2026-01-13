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
    public class RuntimeCardWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(RuntimeCard);
			Utils.BeginObjectRegister(type, L, translator, 0, 5, 10, 8);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadScript", _m_LoadScript);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RegisterEffect", _m_RegisterEffect);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UpdateLocation", _m_UpdateLocation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsLocation", _m_IsLocation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "IsOnField", _m_IsOnField);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "UniqueId", _g_get_UniqueId);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Data", _g_get_Data);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "CurrentLocation", _g_get_CurrentLocation);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "CurrentSequence", _g_get_CurrentSequence);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Owner", _g_get_Owner);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Controller", _g_get_Controller);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "PreviousLocation", _g_get_PreviousLocation);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "PreviousSequence", _g_get_PreviousSequence);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Script", _g_get_Script);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Effects", _g_get_Effects);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "CurrentLocation", _s_set_CurrentLocation);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "CurrentSequence", _s_set_CurrentSequence);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Owner", _s_set_Owner);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Controller", _s_set_Controller);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "PreviousLocation", _s_set_PreviousLocation);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "PreviousSequence", _s_set_PreviousSequence);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Script", _s_set_Script);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "Effects", _s_set_Effects);
            
			
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
				if(LuaAPI.lua_gettop(L) == 2 && translator.Assignable<CardData>(L, 2))
				{
					CardData _data = (CardData)translator.GetObject(L, 2, typeof(CardData));
					
					var gen_ret = new RuntimeCard(_data);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to RuntimeCard constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadScript(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.LoadScript(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterEffect(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    Effect _e = (Effect)translator.GetObject(L, 2, typeof(Effect));
                    
                    gen_to_be_invoked.RegisterEffect( _e );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UpdateLocation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _newLocation = LuaAPI.xlua_tointeger(L, 2);
                    int _newSequence = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.UpdateLocation( _newLocation, _newSequence );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsLocation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _location = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.IsLocation( _location );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsOnField(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.IsOnField(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_UniqueId(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.UniqueId);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Data(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Data);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CurrentLocation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.CurrentLocation);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_CurrentSequence(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.CurrentSequence);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Owner(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Owner);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Controller(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.Controller);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_PreviousLocation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.PreviousLocation);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_PreviousSequence(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.PreviousSequence);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Script(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Script);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Effects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Effects);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CurrentLocation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.CurrentLocation = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_CurrentSequence(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.CurrentSequence = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Owner(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Owner = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Controller(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Controller = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_PreviousLocation(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.PreviousLocation = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_PreviousSequence(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.PreviousSequence = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Script(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Script = (XLua.LuaTable)translator.GetObject(L, 2, typeof(XLua.LuaTable));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Effects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                RuntimeCard gen_to_be_invoked = (RuntimeCard)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.Effects = (System.Collections.Generic.List<Effect>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<Effect>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
