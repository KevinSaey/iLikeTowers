using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlPan : MonoBehaviour
{
    public float _bottom, _top, _left, _right;
    private float _currentX;
    private float _currentY;

    private void Start()
    {
        _currentX = this.transform.position.x;
        _currentY = this.transform.position.y;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            _currentX += Input.GetAxis("Mouse X");
            _currentY += Input.GetAxis("Mouse Y");

            _currentX = Mathf.Clamp(_currentX, _left, _right);
            _currentY = Mathf.Clamp(_currentY, _bottom, _top);
            
            this.transform.position = new Vector3(_currentX, _currentY, this.transform.position.z);
        }

        /*var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0)
        {   // scroll up
            Debug.Log(d);

            Vector3 scroll = this.transform.position;
            this.transform.position -= new Vector3(scroll.x, scroll.y, Mathf.Clamp(scroll.z + d * 0.01f, _minZ, _maxZ));
        }
        else if (d < 0)
        {
            // scroll down
            Debug.Log(d);

            Vector3 scroll = this.transform.position;
            this.transform.position -= new Vector3(scroll.x, scroll.y, Mathf.Clamp(scroll.z - d * 0.01f, _minZ, _maxZ));
        }*/
        
    }

}
