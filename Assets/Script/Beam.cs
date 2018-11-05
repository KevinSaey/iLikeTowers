using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Beam
{

    public int _rotIndex { get; }
    public Node _startNode, _endNode;
    public int _index;

    public HingeJoint _startHinge, _endHinge;
    public Vector3 _position { get; private set; }
    public Quaternion _rotation { get; private set; }
    public Rigidbody _rb;
    public Collider _col;
    public Vector3 _appliedForce;


    // index of the beams
    //      3  2
    //    4 _\/_ 1
    //       /\
    //      5  0


    TriGrid _grid;
    public Node[] _connectedNodes;
    public GameObject _beam;
    public bool _exists;

    public Beam()
    {
    }

    public Beam(TriGrid grid, Node startNode, Node endNode, int rotIndex, int index)
    {
        _grid = grid;
        _rotIndex = rotIndex;
        _startNode = startNode;
        _endNode = endNode;
        _index = index;

        InstantiateBeam();
        SetColor();

    }

    private void InstantiateBeam()
    {
        float length = _grid._length;
        _position = Vector3.Lerp(_startNode._position, _endNode._position, 0.5f);

        //Create beam object
        _beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _beam.transform.localScale = new Vector3(0.1f, length, 0.1f);
        _beam.name = $"Beam_|{_startNode._xIndex.ToString()}-{_startNode._yIndex.ToString()}" +
            $"|_|{_endNode._xIndex.ToString()}-{_endNode._yIndex.ToString()}|";
        _beam.layer = 9;
        _beam.GetComponent<MeshRenderer>().material = _grid._matBeam;

        //Place beam object into 3D space
        _beam.transform.position = _position;
        /*var bol = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bol.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        bol.transform.position = _position;*/
        _rotation = Quaternion.Euler(new Vector3(0, 0, 30 + 60 * _rotIndex));
        _beam.transform.rotation = _rotation;

        //set physics components
        _rb = _beam.AddComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezePositionZ;
        _col = _beam.GetComponent<BoxCollider>();
        CollisionSwitch(true);

        //Create hinges
        _startHinge = CreateHinges(_startNode, 1);
        _endHinge = CreateHinges(_endNode, -1);
    }

    public bool CheckBroken()=>!_startHinge || !_endHinge;

    public void ShowForces()
    {
        if (Physics.autoSimulation)
        {
            //GOOGLE Task.Run()

            float colVal = 0;
            bool compression = true;
            if (_startHinge && _endHinge)
            {
                //Check which hinge has the highest force. Check if the beam is under compression or tension 
                float distance = _startHinge.currentForce.magnitude > _endHinge.currentForce.magnitude ?
                    Vector3.Distance(_startHinge.currentForce.normalized + _startNode._position, _position) :
                    Vector3.Distance(_endHinge.currentForce.normalized + _endNode._position, _position);
                compression = distance < _grid._length / 2;
                colVal = _startHinge.currentForce.magnitude + _endHinge.currentForce.magnitude / 2;
            }
            else if (_startHinge)
            {
                colVal = _startHinge.currentForce.magnitude;
                float distance = Vector3.Distance(_startHinge.currentForce.normalized + _startNode._position, _position);
                compression = distance < _grid._length / 2;
            }
            else if (_endHinge)
            {
                colVal = _endHinge.currentForce.magnitude;
                float distance = Vector3.Distance(_endHinge.currentForce.normalized + _endNode._position, _position);
                compression = distance < _grid._length / 2;
            }
            

            colVal = Remap(colVal, 0f, _grid._mass, 0f, 1f);

            Debug.Log(colVal);
            Color col;
            if (compression) col = new Color(colVal, 0f, 0f, 1f);
            else col = new Color(0f, 0f, colVal, 1f);

            Renderer rend = _beam.GetComponent<Renderer>();
            rend.material.color = col;
        }
    }

    public void ExcludeCollision(Beam conBeam)
    {
        Physics.IgnoreCollision(_col, conBeam._col, true);
    }

    public void CollisionSwitch(bool colEnabled)
    {
        _col.enabled = colEnabled;
    }

    public HingeJoint CreateHinges(Node node, int dir)
    {
        //Create and set the hingejoint
        HingeJoint hJoint = _beam.AddComponent<HingeJoint>();
        hJoint.connectedBody = node._node.GetComponent<Rigidbody>();
        hJoint.axis = new Vector3(0, 0, 1);
        hJoint.autoConfigureConnectedAnchor = false;
        hJoint.connectedAnchor = Vector3.zero;
        hJoint.anchor = new Vector3(0, dir * _grid._length / 4, 0);
        hJoint.breakForce = _grid._mass;

        return hJoint;
    }

    public void SetColor()
    {
        float alpha = _exists ? 1f : 0.1f;

        Color col = new Color(1f, 1f, 1f, alpha);
        Renderer rend = _beam.GetComponent<Renderer>();
        rend.material.color = col;

    }


    public void Destruct()
    {
        GameObject.Destroy(_beam);
    }

    public float Remap(float x, float min, float max, float newMin, float newMax)
    {
        return newMin + (x - min) * (newMax - newMin) / (max - min);
    }


}
