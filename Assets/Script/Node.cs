using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 _position { get; private set; }
    public bool _kinematic { get; private set; }
    public bool _hasForce { get; private set; }
    public int _xIndex { get; private set; }
    public int _yIndex { get; private set; }
    public float _length;

    public string _type;
    public Vector3 _force;

    private ConstantForce _cForce;

    public int _connectedBeams = 0;
    public GameObject _node;
    TriGrid _grid;
    public Rigidbody _rb;
    LineRenderer _lineRenderer;


    // index of the beams
    //      3  2
    //    4 _\/_ 1
    //       /\
    //      5  0



    public Node(TriGrid grid, Vector3 position, float length, int xIndex, int yIndex)
    {
        _grid = grid;
        _position = position;
        _length = length;
        _xIndex = xIndex;
        _yIndex = yIndex;


        CreateNode();

    }

    private void CreateNode()
    {
        _node = GameObject.Instantiate(_grid._goNode, _position, Quaternion.identity);
        _node.name = $"Node_|{_xIndex.ToString()}-{_yIndex.ToString()}|";
    }

    public void SetKinematic()
    {
        _rb = _node.GetComponent<Rigidbody>();
        _kinematic = true;
        _rb.isKinematic = true;
        _node.GetComponent<MeshRenderer>().enabled = true;
        Renderer rend = _node.GetComponent<Renderer>();
        rend.material.color = new Color(1f, 0f, 0f, 1f);
    }

    public void SwitchLineRenderer()
    {
        _lineRenderer.enabled = !_lineRenderer.enabled;
    }

    public void Setforce(Vector3 force)
    {
        _force = force;
        _hasForce = true;
        _cForce = _node.AddComponent<ConstantForce>();
        _cForce.force = _force;
        _node.GetComponent<MeshRenderer>().enabled = true;
        //DrawForce();
    }

    public void DrawForce()
    {
        if (!_lineRenderer)
        {
            _lineRenderer = _node.AddComponent<LineRenderer>();
        }
        _lineRenderer.SetPosition(0, _node.transform.position);
        _lineRenderer.SetPosition(1, _node.transform.position + _cForce.force * 0.1f);
        _lineRenderer.startWidth = 0.5f;
        _lineRenderer.endWidth = 0.05f;
        _lineRenderer.material = _grid._matLine;
        _lineRenderer.alignment = LineAlignment.View;
    }

    public void CreateHinges(Rigidbody rb)
    {
        HingeJoint hJoint = _node.AddComponent<HingeJoint>();
        hJoint.connectedBody = rb;
        hJoint.axis = new Vector3(0, 0, 1);
        //hJoint.autoConfigureConnectedAnchor = false;
        hJoint.connectedAnchor = new Vector3(0, -_length / 2, 0);
        hJoint.anchor = Vector3.zero;
    }

    public void Destruct()
    {
        GameObject.Destroy(_node);

    }



}
