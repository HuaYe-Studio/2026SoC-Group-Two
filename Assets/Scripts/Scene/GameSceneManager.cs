using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Scene
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        public SceneType CurrentScene { get; private set; } = SceneType.MainMenu;

        public event Action<SceneType> OnSceneChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        public void ChangeScene(SceneType targetScene)
        {
            if (CurrentScene == targetScene)
                return;
            
            Debug.Log($"准备从{CurrentScene}切换到{targetScene}");
            SceneManager.LoadScene(targetScene.ToString());
            CurrentScene = targetScene;
        }

        private void HandleSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            OnSceneChanged?.Invoke(CurrentScene);
            Debug.Log($"成功切换到{CurrentScene}");
        }
    }
}