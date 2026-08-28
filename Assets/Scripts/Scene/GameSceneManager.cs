using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scene
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        public SceneType CurrentWorldScene { get; private set; }

        public event Action<SceneType> OnWorldChanged;
        
        //为了防止改名造成的bug,增加了映射
        [System.Serializable]
        public struct SceneMap
        {
            public SceneType sceneType;
            [Tooltip("填写 Unity 场景文件的真实名称，无后缀")]public string sceneName;
        }
        [Header("场景映射配置")]
        [SerializeField]private List<SceneMap> sceneMaps = new List<SceneMap>();
        private Dictionary<SceneType, string> sceneMapDict = new Dictionary<SceneType, string>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            foreach (SceneMap sceneMap in sceneMaps)
            {
                if (!sceneMapDict.ContainsKey(sceneMap.sceneType))
                {
                    sceneMapDict.Add(sceneMap.sceneType, sceneMap.sceneName);
                }
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        private string GetSceneName(SceneType sceneType)
        {
            if (sceneMapDict.TryGetValue(sceneType, out string sceneName))
            {
                return sceneName;
            }
            Debug.LogError($"{sceneType}的映射没配置,返回该sceneType名称");
            return sceneType.ToString();
        }

        //同步，不推荐
        public void ChangeScene(SceneType targetScene)
        {
            if (CurrentWorldScene == targetScene)
                return;
            
            Debug.Log($"准备从{CurrentWorldScene}切换到{targetScene}");
            SceneManager.LoadScene(GetSceneName(targetScene));
            CurrentWorldScene = targetScene;
        }

        public async Task ChangeWorldSceneAsync(SceneType targetWorldScene)
        {
            if (CurrentWorldScene == targetWorldScene)
                return;
            
            string sceneName = GetSceneName(targetWorldScene);
            CurrentWorldScene = targetWorldScene;
            Debug.Log($"准备从{CurrentWorldScene}异步加载到{sceneName}");
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }

        public async Task ChangeUISceneAddictiveAsync(SceneType targetUIScene)
        {
            string sceneName = GetSceneName(targetUIScene);
            Debug.Log($"准备叠加UI{sceneName}");
            
            for (int i = 0; i < SceneManager.sceneCount; i++)                                                                 
            {                                                                                                                 
                if (SceneManager.GetSceneAt(i).name == sceneName)                                                             
                {                                                                                                             
                    Debug.Log($"{sceneName} 已叠加，跳过");                                                                   
                    return;                                                                                                   
                }                                                                                                             
            }
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }
        
        private void HandleSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Additive)
            {
                Debug.Log($"UI{scene.name}叠加完毕");
            }
            else if (mode == LoadSceneMode.Single)
            {
                SceneManager.SetActiveScene(scene);
                OnWorldChanged?.Invoke(CurrentWorldScene);
                Debug.Log($"成功切换到{CurrentWorldScene}");
            }
        }
    }
}