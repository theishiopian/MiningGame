using System;
using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[Serializable]
public class CaveLayer
{
    public string name;
    public int depth = 25;
    public float caveWidth = 10;
    public float caveHeight = 15;
    public ResourceTile startTile;
    public ResourceTile mainTile;
    public AnimationCurve densityCurve;
    public AnimationCurve fadeCurve;

    public OreDeposit[] ores;
}

[Serializable]
public class OreDeposit
{
    public string name;
    public ResourceTile oreTile;
    public AnimationCurve oreCurve;
    public float xScale = 2;
    public float yScale = 2;
}
public class Map : MonoBehaviour
{
    //TODO: make surface layer
    //TODO: make second pass for structures
    //TODO: make third pass for decorators
    //TODO: make fourth pass for monster spawn points
    //TODO: convert all this to assets, so cave stuff can be mixed and matched via asset packs
    //TODO: make a frontend for this in menus
    #region SINGLETON

    public static Map Instance
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

    public int width;
    public int seed = 0;
    public bool generateNewSeed = true;

    public CaveLayer[] layers;

    private Tilemap _tilemap;
    private Grid _grid;
    
    // Start is called before the first frame update
    private void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _grid = _tilemap.layoutGrid;
        Generate();
    }

    // Start is called before the first frame update
    private void Generate()
    {
        if (generateNewSeed)
        {
            seed = Random.Range(-99999, 99999);
        }
        
        var xExtant = width / 2;
        var depthModifier = 0;

        foreach (var layer in layers)
        {
            var yExtant = layer.depth / 2;

            for (var y = 0; y > -layer.depth; y--)
            {
                var percent = 1 - ((-layer.depth - y) / (float)-layer.depth);
                var altBlockChance = layer.fadeCurve.Evaluate(percent);

                for (var x = -xExtant; x < xExtant; x++)
                {
                    var caveNoise = Mathf.PerlinNoise((x + xExtant + seed) / layer.caveWidth, (y + yExtant + seed) / layer.caveHeight);
                    var totalDensity = layer.densityCurve.Evaluate((y) / (float) -layer.depth);
                    var die = Random.Range(0f, 1f);//TODO use seed
                    GameTile toPlace = die < altBlockChance ? layer.startTile : layer.mainTile;

                    if (!(caveNoise < totalDensity)) continue;//create a cave TODO maybe put something interesting here?
                    
                    foreach (var deposit in layer.ores)
                    {
                        var t = (float)y / -layer.depth;
                        var oreDensity = 1f - deposit.oreCurve.Evaluate(t);
                        var oreNoise = Mathf.PerlinNoise((x + xExtant + seed) / deposit.xScale, (y + yExtant + seed) / deposit.yScale);
                        if (oreNoise > oreDensity)
                        {
                            toPlace = deposit.oreTile;
                        }
                    }
                    _tilemap.SetTile(new Vector3Int(x, y - depthModifier - 5, 0), toPlace);
                }
            }

            depthModifier += layer.depth;
        }
    }

    /**
     * This method attempts to break a tile with a specific pickaxe power, and returns true if it succeeds.
     * Also provides information about the tile tested via brokenTile, or provides null if no tile was found.
     */
    public bool TryBreakTile(Vector3 position, int power, out GameTile brokenTile)
    {
        var tilePos = _grid.WorldToCell(position);
        brokenTile = _tilemap.GetTile(tilePos) as GameTile;
        
        if (!brokenTile) return false;
        if (power < brokenTile.hardness) return false;
        
        _tilemap.SetTile(tilePos, null);
        brokenTile.OnBreak(position, tilePos, _tilemap);
        return true;
    }

    public bool TryPlaceTile(Vector3 position, GameTile toPlace)
    {
        var tilePos = _grid.WorldToCell(position);

        if (_tilemap.GetTile(tilePos)) return false;
        _tilemap.SetTile(tilePos, toPlace);
        toPlace.OnPlace(position, tilePos, _tilemap);
        return true;
    }
}