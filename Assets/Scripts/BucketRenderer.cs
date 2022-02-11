using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class BucketRenderer : MonoBehaviour
{
    #region SINGLETON
    public static BucketRenderer Instance
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
    public GameObject layerPrefab;
    public float width = 14;
    public float vertOffset = 12;
    public float hMult = 4;
    public Text percent;
    public int fill;
    
    private readonly Dictionary<ResourceTile, Image> _layers = new Dictionary<ResourceTile, Image>();

    private int _offset;
    private int _size;
    
    private void Update()
    {
        _offset = 0;
        
        percent.text = fill < 100 ? fill + "%" : "Full";
        
        foreach (var pair in _layers)
        {
            if (PlayerController.Instance.minerals[pair.Key] <= 0)
            {
                pair.Value.enabled = false;
                continue;
            }
            
            pair.Value.enabled = true;
            _size = Mathf.FloorToInt(Mathf.Max(((PlayerController.Instance.minerals[pair.Key] * hMult) / 16f), 1));
            pair.Value.rectTransform.sizeDelta = new Vector2(width, _size);//TODO cache
            pair.Value.rectTransform.localPosition = new Vector3(0, (_size/2f) - vertOffset + _offset);
            _offset += _size;
        }
    }

    public void AddMineralLayerIfNeeded(ResourceTile tile)
    {
        if (_layers.ContainsKey(tile)) return;
        var layer = Instantiate(layerPrefab, transform).GetComponent<Image>();
        layer.sprite = tile.sprite;
        _layers.Add(tile, layer);
        percent.transform.SetSiblingIndex(percent.transform.parent.childCount - 1);
    }
}
