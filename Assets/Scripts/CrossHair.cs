using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    #region SINGLETON

    public static CrossHair Instance
    {
        get;
        private set;
    }
    
    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Duplicate Singleton: "+gameObject.name+"! Removing...");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    public Color defaultColor;
    public Color invalidColor;
    public Color attackColor;

    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    public void SetVisible(bool visible)
    {
        if (!visible)
        {
            transform.position = Vector2.zero;
            _image.color = defaultColor;
        }

        _image.enabled = visible;
    }

    public void SetColor(string color)
    {
        _image.color = color switch
        {
            "invalid" => invalidColor,
            "attack" => attackColor,
            _ => defaultColor
        };
    }
}
