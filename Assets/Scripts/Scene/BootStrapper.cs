using UnityEngine;

namespace Scene
{
    public class BootStrapper:MonoBehaviour
    {
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            await GameSceneManager.Instance.ChangeWorldSceneAsync(SceneType.MainMenu);
        }
    }
}