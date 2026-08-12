using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newtrip : MonoBehaviour
{
    [Header("相机设置")]
    public Camera targetCamera;
    [Header("移动设置")]
    public float moveDuration =1.5f;
    [Header("聚焦距离")]
    public float focusDistance =2f;
    private bool isMoving = false;
    void Start()
    {

    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.collider.gameObject == gameObject && !isMoving)
                {
                    StartCoroutine(MoveCameraToTarget());
                                    }
            }
        }
    }
    IEnumerator MoveCameraToTarget()
    {
        Vector3 targetPosition = transform.position - targetCamera.transform.forward * focusDistance;
        Vector3 startPosition = targetCamera.transform.position;
        float elapsedTime = 0f;
        isMoving = true;
        while (elapsedTime < moveDuration)
        {
            targetCamera.transform.position = Vector3.Lerp(startPosition,targetPosition,elapsedTime/moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetCamera.transform.position = targetPosition;
        isMoving = false;
        StartNewGame();
    }
    public void StartNewGame()
    {

    }
}
