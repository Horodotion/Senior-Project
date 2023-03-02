using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamScript : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform endPoint;

    public void ChangeEndPosition(Vector3 newPos)
    {
        Vector3 endPosition = transform.InverseTransformPoint(newPos);

        lineRenderer.SetPosition(1, endPosition);
        endPoint.localPosition = endPosition;
    }
}
