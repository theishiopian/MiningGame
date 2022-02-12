using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
    public abstract class GameTile : Tile
    {
        public int hardness = 0;
        public Color particleColorA;
        public Color particleColorB;

        public virtual void OnBreak(Vector3 position, Vector3Int tilePos, Tilemap map)
        {
            BreakVFX.Instance.PlayVFX(position, this.particleColorA, this.particleColorB);
        }

        public virtual void OnPlace(Vector3 position, Vector3Int tilePos, Tilemap map)
        {
        
        }
    }
}
