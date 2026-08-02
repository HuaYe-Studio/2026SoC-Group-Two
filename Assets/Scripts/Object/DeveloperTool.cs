using awotr;
using UnityEngine;

public class DeveloperTool : MonoBehaviour
{
    public string id;
    public void Inspecting(string id)
    {

        AwotrObj obj = InitObj.GetObj(id);
        if (obj != null)
        {
            Debug.Log("成功创建"+id);
        }
        else if (obj == null)
        {
            Debug.Log("没有物品" + id);
        }
 
    }
        
    
}
