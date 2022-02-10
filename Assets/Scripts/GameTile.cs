using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class GameTile : Tile
{
    public int hardness = 0;
    public Color particleColorA;
    public Color particleColorB;

    public virtual void OnBreak(Vector3 position)
    {
        BreakVFX.Instance.PlayVFX(position, this.particleColorA, this.particleColorB);
    }
}
