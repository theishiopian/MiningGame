using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
    [CreateAssetMenu]
    public class BuildTile : GameTile
    {
        public new string name;
        public int price;
        public bool exitBuildMode = false;

        public override void OnBreak(Vector3 position, Vector3Int tilePos, Tilemap map)
        {
            base.OnBreak(position, tilePos, map);
            //TODO add to inventory
        }

        public override void OnPlace(Vector3 position, Vector3Int tilePos, Tilemap map)
        {
            base.OnPlace(position, tilePos, map);
            //TODO take from inventory
        }
    }
}