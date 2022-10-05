using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    // we dont require x and y, its just to see the position on Inspector while develping and for debugging purpose
    [SerializeField] private int _x;
    [SerializeField] private int _y;   

    public eGridType GridType = eGridType.None;

    private Image Image => gameObject.GetComponent<Image>();

    public void Init(int x, int y)
    {
        _x = x;
        _y = y;
        transform.GetChild(0).GetComponent<Text>().text = $"({_x},{_y})";
    }

    public void Set(eGridType gridType, Sprite sprite)
    {
        GridType = gridType;
        Image.sprite = sprite;
    }
}
