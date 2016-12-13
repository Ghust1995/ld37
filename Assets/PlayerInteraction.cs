using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public enum PlayerState
{
    editingCode,
    editingLevel
}
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private int _selectedColorIndex = 0;

    [SerializeField]
    private PlayerState _state = PlayerState.editingLevel;

    public List<Color> SelectableColors;

    public LayerMask LayerMask;

    [Serializable]
    public struct KeyEvent
    {
        public KeyCode Key;
        public PlayerState state;
        public UnityEvent Callback;
    }

    public KeyEvent[] KeyEvents;

    void Start()
    {
    }

    public void NextColor()
    {
        _selectedColorIndex = (_selectedColorIndex + 1)%SelectableColors.Count;
    }

    public void PrevColor()
    {
        _selectedColorIndex = (_selectedColorIndex -1 + SelectableColors.Count) % SelectableColors.Count;
    }

    public void EnterEditMode()
    {
        _state = PlayerState.editingCode;
    }

    public void ExitEditMode()
    {
        _state = PlayerState.editingLevel;
    }

    public void DeselectEditor()
    {
        EventSystem.current.SetSelectedGameObject(null, null);
    }


    // Update is called once per frame
    void FixedUpdate () {
        foreach (var keyEvent in KeyEvents)
        {
            if (Input.GetKeyDown(keyEvent.Key) && _state == keyEvent.state)
            {
                if (keyEvent.Callback != null)
                {
                    keyEvent.Callback.Invoke();
                }
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask);
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<TileMap>() != null)
            {
                if (Input.GetMouseButtonDown(0) && _state == PlayerState.editingLevel)
                {
                    hit.collider.GetComponent<TileMap>().AddTile(
                        hit.point,
                        SelectableColors[_selectedColorIndex]
                );
                }
                else if (Input.GetMouseButtonDown(1) && _state == PlayerState.editingLevel)
                {
                    hit.collider.GetComponent<TileMap>().RemoveTile(
                        hit.point
                );
                }
                else
                {
                    hit.collider.GetComponent<TileMap>().PreviewTile(
                        hit.point,
                        _state == PlayerState.editingLevel ? SelectableColors[_selectedColorIndex] : Color.clear
                );
                }

                
            }
        }

        
    }
        
}
