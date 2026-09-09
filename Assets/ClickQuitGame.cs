using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickQuitGame : MonoBehaviour
{
    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
               if(hit.collider.gameObject==gameObject)
                {
                    QuitGame();
                }
            }
        }
    }
}
