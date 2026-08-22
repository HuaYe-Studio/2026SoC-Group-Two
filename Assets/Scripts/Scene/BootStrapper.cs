using UnityEngine;

namespace Scene
{
    public class Bootstrapper:MonoBehaviour
    {
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            await GameSceneManager.Instance.ChangeWorldSceneAsync(SceneType.MainMenu);
        }
    }
}