using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningLine : MonoBehaviour {

    // Use this for initialization
    public Transform endPointA;
    public Transform endPointB;
    public int vertexCount = 1;
    public int lineCount = 1;
    public float lineWidth;
    public float randomOffset;
    public float timeBetweenDraws;
    private float drawTimer;
    private LineRenderer lineRenderer;

    private Vector3 point;
    private Vector3 orthoVector;
    private Vector3 vectorFromA;
    private float stepSize;         //in percent

	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = vertexCount;
        stepSize = 1f / vertexCount;

    }

    // Update is called once per frame
    void LateUpdate () {
        if (drawTimer > 0)
        {
            drawTimer -= Time.deltaTime;
        }
        else
        {
            for (int j = 0; j < lineCount; j++)
            {
                lineRenderer.SetPosition(0, endPointA.position);
                for (int i = 1; i < vertexCount - 1; i++)
                {

                    int rngSign = Random.value > 0.5f ? 1 : -1;
                    point = Vector3.Lerp(endPointA.position, endPointB.position, i * stepSize + (randomOffset * Random.value * rngSign));
                    vectorFromA = point - endPointA.position;
                    orthoVector = new Vector3(-vectorFromA.y, vectorFromA.x, 0).normalized;
                    rngSign = Random.value > 0.5f ? 1 : -1;
                    point += orthoVector * (lineWidth / 2 * Random.value) * rngSign;
                    lineRenderer.SetPosition(i, point);

                }
                lineRenderer.SetPosition(vertexCount - 1, endPointB.position);
                drawTimer = timeBetweenDraws;
            }
        }
    }
}
