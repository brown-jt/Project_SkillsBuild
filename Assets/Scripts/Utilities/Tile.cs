using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int correctPosition;
    public Vector2Int currentPosition;

    private TileManager manager;

    private void Start()
    {
        manager = FindFirstObjectByType<TileManager>();
    }

    public void OnClick()
    {
        if (manager != null) manager.TryMoveTile(this);
    }
}
