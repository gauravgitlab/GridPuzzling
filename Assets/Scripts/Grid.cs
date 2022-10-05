using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public int x;
    public int y;   
    public eGridType GridType = eGridType.None;

    private Image Image => gameObject.GetComponent<Image>();

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Set(eGridType gridType, Sprite sprite)
    {
        GridType = gridType;
        Image.sprite = sprite;
    }
}
