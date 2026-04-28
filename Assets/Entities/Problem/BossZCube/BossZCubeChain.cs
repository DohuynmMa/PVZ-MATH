using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZCubeChain : MonoBehaviour
{
    public Entity aim;
    public Entity parent;
    public LineRenderer line;
    public bool enableChain = false;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        if (!enableChain) return;
        if (aim == null || parent == null || line == null || aim.hitpoint <= 0 || parent.hitpoint <= 0)
        {
            Destroy(gameObject);
        }
        line.SetPosition(0, aim.getEntityBoxColliderPos());
        line.SetPosition(1,parent.getEntityBoxColliderPos());
    }
}
