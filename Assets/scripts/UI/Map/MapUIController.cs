using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUIController : MonoBehaviour
{
    // 供按钮 OnClick 调用
    public void OpenDeckBuilder()
    {
        // 确保这里的字符串和你的构筑场景名字完全一致
        SceneManager.LoadScene("DeckBuilderScene");
    }
}
