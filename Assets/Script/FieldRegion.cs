using UnityEngine;
using System.Collections;

public class FieldRegion {

    public Vector3 leftTop;
    public float width;
    public float height;

    public Vector3 Center()
    {
        return new Vector3(leftTop.x+ width/2, leftTop.y - height / 2, 0);
    }
}
