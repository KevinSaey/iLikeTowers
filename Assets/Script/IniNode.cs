using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IniNode
{
    public bool _hasForce;
    public bool _kinematic;
    public Vector3 _force;
    public int _xIndex, _yIndex;


    public IniNode()
    {

    }

    public IniNode(int xIndex, int yIndex, Vector3 force)
    {
        _xIndex = xIndex;
        _yIndex = yIndex;
        _force = force;

        _kinematic = false;
        _hasForce = true;
    }

    public IniNode(int xIndex, int yIndex, bool ground)
    {
        _xIndex = xIndex;
        _yIndex = yIndex;

        _kinematic = ground;
        _hasForce = false;
    }
}