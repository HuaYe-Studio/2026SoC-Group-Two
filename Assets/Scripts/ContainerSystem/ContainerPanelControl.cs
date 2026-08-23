using UnityEngine.UI;
using UnityEngine;

public class ContainerKeyboradInput : MonoBehaviour
{
    [Header("容器面板画布")]
    public Canvas containerPanel;

    #region 关闭按钮事件
    public void onCloseBtnClick()
    {
        containerPanel.gameObject.SetActive(false);
        GetComponent<Container_ItemManager>().HideItemInContainer();
    }
    #endregion
}
