using Player;
using UnityEngine;

[CreateAssetMenu]
public class ResourceTile : GameTile
{
    public int value;
    public Sprite fillSprite;//TODO separate fill sprites for all resource tiles that arent basic stone/dirt
    public override void OnBreak(Vector3 position)
    {
        base.OnBreak(position);

        BucketRenderer.Instance.fill += 1;
        
        PlayerController.Instance.AddMinerals(this);
    }
}
