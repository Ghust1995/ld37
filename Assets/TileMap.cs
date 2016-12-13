using System;
using UnityEngine;
using System.Collections;

public enum TileType
{
    None,
    Default
}

public class TileMap : MonoBehaviour
{
    [SerializeField]
    private Tile[,] _map;
    
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    [SerializeField]
    private int _tileSize = 32;

    public int BlockCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    if (_map[i, j] != null) count++;
                }
            }
            return count;
        }
    }

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Tile>())
                Destroy(transform.GetChild(i).gameObject);
        }
    }

    // Use this for initialization
    
    void Start () {
        _map = new Tile[_width, _height];
        var collider = GetComponent<BoxCollider2D>();
        if (!collider)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.size = new Vector2(_width, _height);
        for (int i = 0; i < _width; i++)
        {
            if (i > _width/4 && i < 3*_width / 4) continue;
            AddTile(i, _height - 1, Color.white);
            AddTile(i, 0, Color.white);
        }
        for (int i = 0; i < _height; i++)
        {
            AddTile(0, i, Color.white);
            AddTile(_width - 1, i, Color.white);
        }
    }

    public void AddTile(Vector2 point)
    {
        AddTile(point, Color.white);
    }

    public void AddTile(Vector2 point, Color color)
    {
        var pos = point - (Vector2)transform.position;
        var x = Mathf.FloorToInt(pos.x + (float)_width / 2) ;
        var y = Mathf.FloorToInt(pos.y + (float)_height / 2) ;
        AddTile(x, y, color);
    }

    public void RemoveTile(Vector2 point)
    {
        var pos = point - (Vector2)transform.position;
        var x = Mathf.FloorToInt(pos.x + (float)_width / 2);
        var y = Mathf.FloorToInt(pos.y + (float)_height / 2);
        if (_map[x, y] != null)
        {
            Destroy(_map[x, y].gameObject);
            _map[x, y] = null;
        }
    }

    private void AddTile(int x, int y, Color color)
    {
        var tile = Tile.Create(color);
        if (x >= _width || y >= _height || x < 0 || y < 0)
        {
            return;
        }
        if (_map[x, y] != null)
        {
            Destroy(_map[x, y].gameObject);
        }
        _map[x , y] = tile;
        tile.transform.position = (Vector2)transform.position + new Vector2(x - _width / 2, y - _height / 2 + 0.5f);
        tile.transform.parent = transform;
    }


    
    [SerializeField]
    private SpriteRenderer _previewTile;
    public void PreviewTile(Vector2 point, Color color)
    {
        var pos = point - (Vector2)transform.position;
        var x = Mathf.FloorToInt(pos.x + (float)_width / 2);
        var y = Mathf.FloorToInt(pos.y + (float)_height / 2);
        if (x >= _width || y >= _height || x < 0 || y < 0)
        {
            return;
        }
        _previewTile.color = new Color(color.r, color.g, color.b, 0.2f);
        _previewTile.transform.position = (Vector2)transform.position + new Vector2(x - _width / 2, y - _height / 2 + 0.5f);
    }
}
