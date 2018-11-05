using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;
using System.Linq;

// Class is a reference type
// ... a.x = 10;
// b = a
// b.x = 20
// ==> a.x = 20
// b = reference to a

// Struct is a value type
// ... a.x = 10;
// b = a
// b.x = 20
// ==> a.x = 10
// b = copy of a


public class TriGrid
{
    public GameObject _goNode;
    public float _length;
    
    public Material _matBeam, _matLine;
    public float _breakForce;

    //Variables for gamestate --> JSON
    public int _xAmount, _yAmount;// JSON
    public List<IniNode> _iniNodes = new List<IniNode>();// JSON
    public List<bool> _beamExists = new List<bool>();// JSON
    public Vector3 _position; //JSON

    public List<List<Node>> _nodes = new List<List<Node>>();
    public List<Beam> _beams = new List<Beam>();

    public int _turn;
    public int _numberOfBeams;
    public bool _broken;

    // Constructor Grid
    public TriGrid(GameObject goNode, float length, Material matBeam, Material matLine, float mass, Level level)
    {
        _goNode = goNode;
        _length = length;
        _matBeam = matBeam;
        _matLine = matLine;
        _breakForce = mass;
        _xAmount = level._xAmount;
        _yAmount = level._yAmount;
        _position = level._position;
        _iniNodes = level._iniNodes;

        MakeGrid();
    }

    //     Generate points for a grid for perfect triangles
    public void MakeGrid()
    {
        _broken = false;
        for (int j = 0; j < _yAmount; j++)
        {
            _nodes.Add(new List<Node>());
            for (int i = 0; i < _xAmount; i++)
            {
                float x = i * _length - (_xAmount - 0.5f) * _length / 2;

                // Simplified pitahoric equation of rectangle with equal members
                float y = j * Sqrt(Sq(_length) - Sq(_length / 2));

                //shift odd list to make triangle grid
                if (j % 2 == 0)
                {
                    x += _length / 2;
                }

                // check if the node is on the bottom layer, bottom layer is kinematic

                Vector3 pos = new Vector3(x, y, 0);

                _nodes[j].Add(new Node(this, pos, this._length, i, j));
            }
        }

        ApplyForces();
        PopulateGrid();


    }

    private void ApplyForces()
    {
        foreach (var iniNode in _iniNodes)
        {
            int x = iniNode._xIndex;
            int y = iniNode._yIndex;
            if (iniNode._kinematic)
            {
                _nodes[y][x].SetKinematic();
            }
            if (iniNode._hasForce)
            {
                var rb = _nodes[y][x]._rb;
                rb.AddForce(iniNode._force,ForceMode.Force);                
               _nodes[y][x].Setforce(iniNode._force);
            }
        }
    }

    private void PopulateGrid()
    {
        // index of the beams
        //      3  2
        //    4 _\/_ 1
        //       /\
        //      5  0
        // Very convoluted way to create a diagid
        for (int y = 0; y < _yAmount; y++)
        {
            for (int x = 0; x < _xAmount; x++)
            {
                Beam beam;
                List<Beam> beams = new List<Beam>();
                if (x < _xAmount - 1)
                {
                    beam = new Beam(this, _nodes[y][x], _nodes[y][x + 1], 1, _beams.Count);//Horizontal connections
                    _beams.Add(beam);
                    beams.Add(beam);
                }
                if (y < _yAmount - 1)
                {
                    if (y % 2 == 0)
                    {
                        beam = new Beam(this, _nodes[y][x], _nodes[y + 1][x], 3, _beams.Count);
                        _beams.Add(beam);
                        beams.Add(beam);
                        if (x < _xAmount - 1)
                        {
                            beam = new Beam(this, _nodes[y][x], _nodes[y + 1][x + 1], 2, _beams.Count);
                            _beams.Add(beam);
                            beams.Add(beam);
                        }
                    }
                    else
                    {
                        beam = new Beam(this, _nodes[y][x], _nodes[y + 1][x], 2, _beams.Count);
                        _beams.Add(beam);
                        beams.Add(beam);
                        if (x < _xAmount - 1)
                        {
                            beam = new Beam(this, _nodes[y][x + 1], _nodes[y + 1][x], 3, _beams.Count);
                            _beams.Add(beam);
                            beams.Add(beam);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < _beams.Count; i++)
        {
            if (_turn == 0)
            {
                _beamExists.Add(false);
            }
            else
            {
                _beams[i]._exists = _beamExists[i];
                if (_beams[i]._exists)
                {
                    // this number will be used to detemine if a beam should exist
                    _beams[i]._startNode._connectedBeams++;
                    _beams[i]._endNode._connectedBeams++;
                }
                _beams[i].SwitchBeams();
            }
        }

        //Crossreference elements from _beams connected to the same node
        for (int i = 0; i < _beams.Count - 1; i++)
        {
            for (int j = i + 1; j < _beams.Count; j++)
            {
                if (_beams[i]._startNode == _beams[j]._startNode || _beams[i]._endNode == _beams[j]._endNode
                    || _beams[i]._startNode == _beams[j]._endNode || _beams[i]._endNode == _beams[j]._startNode)
                {
                    //Dissable collision between beams connected to the same node
                    Physics.IgnoreCollision(_beams[i]._col, _beams[j]._col, true);
                }
            }
        }
        _turn++;
    }

    public void Update()
    {
        foreach (var beam in _beams.Where(s => s._exists))
        {
            beam.ShowForces();
            if (beam.CheckBroken()) beam.SwitchBeams();
            if (!_broken) _broken = beam.CheckBroken();
        }

        foreach (var iniNode in _iniNodes.Where(s => s._hasForce))
        {
            _nodes[iniNode._yIndex][iniNode._xIndex].DrawForce();
        }


    }

    public Beam GetBeam(GameObject goBeam)
    {
        var reBeam = new Beam();
        foreach (var beam in _beams.Where(s => s._beam == goBeam))
        {
            reBeam = beam;
        }

        return reBeam;
    }

    public void Clear()
    {
        _beams.Clear();
        _nodes.Clear();
    }

    float Sq(float num) => num * num;
}
