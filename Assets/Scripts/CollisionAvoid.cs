using UnityEngine;
using UnityEngine.UI;

public class CollisionAvoid : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private BoxCollider _collider;
    private RectTransform _panel;

    private Vector3 _bottom;
    private bool _isSizeChanged;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        _panel = transform.Find("Panel").GetComponent<RectTransform>();
        _bottom = default;
        _isSizeChanged = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ignore collision") || other.CompareTag(gameObject.tag))
            Physics.IgnoreCollision(_collider, other, true);


        Debug.Log("On Trigger enter");

        
        //Debug.Log("Current pos: " + transform.position.ToString());
        //Debug.Log("Current local pos: " + transform.localPosition.ToString());

        //Debug.Log("Local size: " + transform.l)

        //transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
        //Debug.Log("New position: " + transform.position.ToString());
        //Debug.Log("New local pos: " + transform.localPosition.ToString());
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.CompareTag("Ignore collision") || other.CompareTag(gameObject.tag))
        //    Physics.IgnoreCollision(_collider, other, true);

        //Debug.Log("On trigger: " + other.name);
        //transform.Translate(-Vector3.forward);

        //var curPos = transform.position;
        //if (curPos.y > 125f)
        //    transform.position = new Vector3(curPos.x, 125f, curPos.z);
        
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit();");
        //Debug.Log("Pos: " + transform.position.ToString());
        //if (_bottom == default)
        //    _bottom = transform.position;

        //if (transform.position.y < _bottom.y)
        //{
        //    transform.position = _bottom;
        //}
        //else _bottom = transform.position;
    }
    private void OnGUI()
    {
        if (_isSizeChanged)
        {
            _isSizeChanged = false;
            var panelSize = _panel.rect.size;
            Debug.Log($"Control size: {panelSize}; ");
            _collider.size = new Vector3(panelSize.x, panelSize.y, 1f);
        }
        
    }
}
