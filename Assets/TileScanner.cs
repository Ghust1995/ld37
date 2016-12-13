using UnityEngine;
using System.Collections;
using System.Diagnostics;

[RequireComponent(typeof(SpriteRenderer), typeof(LineRenderer))]
public class TileScanner : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    public Color Value { get; private set; }

    private Color _lastValue;

    public Color NewValue
    {
        get { return _lastValue != Value ? Value : Color.black; }
    }

    [SerializeField]
    private float _scanDistance;

    [SerializeField]
    private LayerMask _layerMask;

    private bool _enabled = true;

    // Use this for initialization
    void Awake ()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetEnabled(bool enabled)
    {
        // UNCOMMENT TO HAVE SENSOR ONLY IN MOVING DIRECTION
        
        _lineRenderer.enabled = enabled;
        _enabled = enabled;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    _lastValue = Value;

        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + Vector3.down * _scanDistance);

	    if (_enabled)
	    {
	        var hit = Physics2D.Raycast(transform.position, Vector2.down, _scanDistance, _layerMask);
	        if (hit && hit.collider.GetComponent<Tile>())
	        {
	            var color = hit.collider.GetComponent<Tile>().Color;
	            Value = color;
	            _lineRenderer.SetColors(Color.green, Color.green);
	        }
	        else
	        {
	            _lineRenderer.SetColors(Color.red, Color.red);
	            Value = Color.black;
	        }
	    }
	    else
	    {
            Value = Color.black;
        }


        GetComponent<SpriteRenderer>().color = Value;
    }
}
