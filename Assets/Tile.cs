using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Tile : MonoBehaviour
{
    [SerializeField] public Color Color { get; private set; }

    public static Tile Create(Color color)
    {
        var go = new GameObject();
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = Resources.Load<Sprite>("Tile");
        renderer.color = color;
        go.AddComponent<BoxCollider2D>();
        go.layer = LayerMask.NameToLayer("Ground");

        var tile = go.AddComponent<Tile>();
        tile.Color = color;

        return tile;
    }
}
