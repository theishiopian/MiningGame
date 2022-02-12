using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
    [CreateAssetMenu]
    public class ClimbTile : BuildTile
    {
        public float climbSpeedMultiplier = 1;
        public int range = 25;
        public Vector2Int buildDir;
        
        private void Build(Vector3Int tilePos, Vector3Int dir, Tilemap map)
        {
            for (var i = 0; i < range - 1; i++)
            {
                var nextTile = map.GetTile(tilePos + dir);

                if (nextTile) break;
                map.SetTile(tilePos + dir, this);
                tilePos += dir;
            }
        }

        public override void OnPlace(Vector3 position, Vector3Int tilePos, Tilemap map)
        {
            base.OnPlace(position, tilePos, map);
            
            Build(tilePos, (Vector3Int) buildDir, map);
        }
    }
}