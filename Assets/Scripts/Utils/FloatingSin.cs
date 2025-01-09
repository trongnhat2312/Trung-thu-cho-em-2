using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using Koi.Common;

public class FloatingSin : MonoBehaviour
{
    public enum TypeSine
    {
        MoveY = 0,
        MoveX = 1,
        MoveZ = 2
    }
    [SerializeField] TypeSine type = TypeSine.MoveY;
    [SerializeField] Transform _target;
    [SerializeField] float neoPos = 0;
    [SerializeField] float amplitude = 0.1f;
    [SerializeField] float f = 0.5f;
    [SerializeField] float phi0;
    [SerializeField] bool randomPhi0 = true;

    void Start()
    {
        if (randomPhi0)
        {
            phi0 = Random.Range(-Mathf.PI, Mathf.PI);
        }
    }

    void Update()
    {
        float phi = phi0 + Time.time * f * 2 * Mathf.PI;
        float nextPos = neoPos + amplitude * Mathf.Sin(phi);
        switch (type)
        {
            case TypeSine.MoveY:
                UTransformUtils.SetLocalPosY(target, Mathf.Lerp(transform.localPosition.y, nextPos, 0.08f));
                break;

            case TypeSine.MoveX:
                UTransformUtils.SetLocalPosX(target, Mathf.Lerp(transform.localPosition.x, nextPos, 0.08f));
                break;

            case TypeSine.MoveZ:
                UTransformUtils.SetLocalPosZ(target, Mathf.Lerp(transform.localPosition.z, nextPos, 0.08f));
                break;
        }

    }


    Transform target
    {
        get
        {
            if (_target == null)
            {
                _target = transform;
            }
            return _target;
        }
    }
}
