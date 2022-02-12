using Player;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
    [CreateAssetMenu]
    public class ResourceTile : GameTile
    {
        public int value;
        public Sprite fillSprite;//TODO separate fill sprites for all resource tiles that arent basic stone/dirt
        public override void OnBreak(Vector3 position, Vector3Int tilePos, Tilemap map)
        {
            base.OnBreak(position, tilePos, map);

            PlayerController.Instance.AddMinerals(this);
        }
    }
}
