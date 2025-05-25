using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButtonHandler : MonoBehaviour
{
    // ボタンのOnClickイベントでこのメソッドを指定してください
    public void OnRetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}