using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCanCube : MonoBehaviour
{
    public int width = 6;
    public int height = 3;
    public float range = 3f;
    [HideInInspector] public Inventory trashInv;

    Transform player;

    void Awake()
    {
        trashInv = new Inventory(width, height);
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float dis = Vector3.Distance(transform.position, player.position);
        if (dis < range && Input.GetKeyDown(KeyCode.E))
        {
            var uiMgr = InventoryUIManager.Instance;
            if (uiMgr.windowRoot.activeSelf)
            {
                if (uiMgr.currentTrash == this.trashInv)
                {
                    uiMgr.Close();
                }
                else
                {
                    uiMgr.Open(trashInv);
                }
            }
            else
            {
                uiMgr.Open(trashInv);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}