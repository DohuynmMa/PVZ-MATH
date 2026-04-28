using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraMask : MonoBehaviour
{
    private readonly Vector3[] frustumCorners = new Vector3[2];

    void Update()
    {
        frustumCorners[0] = Camera.main.ViewportToWorldPoint(Vector3.zero);
        frustumCorners[1] = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        var newV3 = frustumCorners[0] + frustumCorners[1];
        newV3.z = 10;

        transform.position = newV3 * 0.5f;
        transform.localScale = frustumCorners[1] - frustumCorners[0];
    }
}
