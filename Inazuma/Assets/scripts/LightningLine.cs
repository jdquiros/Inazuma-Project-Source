using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningLine : MonoBehaviour {

    // Use this for initialization
    public bool playOnAwake = true;

    public Transform endPointA;
    public Transform endPointB;
    public bool capEndpoints = true;
    public int vertexCount = 1;
    public float lineWidth;
    public float randomOffset;
    public float timeBetweenDraws;
    private float drawTimer;
    private LineRenderer lineRenderer;

    private Vector3 point;
    private Vector3 orthoVector;
    private Vector3 vectorFromA;
    private float stepSize;         //in percent
    private bool isDrawing;

	void Awake () {
        lineRenderer = GetComponent<LineRenderer>();
        stepSize = 1f / vertexCount;
        lineRenderer.positionCount = vertexCount;
        isDrawing = playOnAwake;

    }

    // Update is called once per frame
    void LateUpdate () {
        if (isDrawing)
        {
            if (drawTimer > 0)
            {
                drawTimer -= Time.deltaTime;
            }
            else
            {
                if (capEndpoints)
                {
                    lineRenderer.SetPosition(0, endPointA.position);
                    drawLine(1, vertexCount - 1, stepSize);
                    lineRenderer.SetPosition(vertexCount - 1, endPointB.position);
                }
                else
                {
                    drawLine(0, vertexCount, stepSize);
                }
                drawTimer = timeBetweenDraws;

            }
        } else
        {
            lineRenderer.positionCount = 0;
        }
    }
    private void drawLine(int minIndex, int maxIndex, float step)
    {
        for (int i = minIndex; i < maxIndex; i++)
        {

            int rngSign = Random.value > 0.5f ? 1 : -1;
            point = Vector3.Lerp(endPointA.position, endPointB.position, i * step + (randomOffset * Random.value * rngSign));
            vectorFromA = point - endPointA.position;
            orthoVector = new Vector3(-vectorFromA.y, vectorFromA.x, 0).normalized;
            rngSign = Random.value > 0.5f ? 1 : -1;
            point += orthoVector * (lineWidth / 2 * Random.value) * rngSign;
            lineRenderer.SetPosition(i, point);

        }
    }
    public void setIsDrawing(bool x)
    {
        isDrawing = x;
    }
}
