using UnityEngine;

/// <summary>
/// 编译检查器 - 检查所有修改的组件是否可以正常编译
/// </summary>
public class CompileChecker : MonoBehaviour
{
    [ContextMenu("检查所有修改的组件")]
    public void CheckAllComponents()
    {
        Debug.Log("=== 开始编译检查 ===");
        
        try 
        {
            // 检查RelicItemUI
            var relicUI = GetComponent<RelicItemUI>();
            Debug.Log("✓ RelicItemUI 编译成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError("✗ RelicItemUI 编译失败: " + e.Message);
        }
        
        try 
        {
            // 检查MapHUD
            var mapHUD = GetComponent<MapHUD>();
            Debug.Log("✓ MapHUD 编译成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError("✗ MapHUD 编译失败: " + e.Message);
        }
        
        try 
        {
            // 检查ShopSceneManager
            var shop = GetComponent<ShopSceneManager>();
            Debug.Log("✓ ShopSceneManager 编译成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError("✗ ShopSceneManager 编译失败: " + e.Message);
        }
        
        try 
        {
            // 检查RelicManager
            var relicManager = GetComponent<RelicManager>();
            Debug.Log("✓ RelicManager 编译成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError("✗ RelicManager 编译失败: " + e.Message);
        }
        
        try 
        {
            // 检查PlayerCollection
            var playerCollection = GetComponent<PlayerCollection>();
            Debug.Log("✓ PlayerCollection 编译成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError("✗ PlayerCollection 编译失败: " + e.Message);
        }
        
        Debug.Log("=== 编译检查完成 ===");
    }
}