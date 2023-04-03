using UnityEngine;

public static class Utils {
    //Properly Lerp between two angles
    public static Vector3 AngleLerp(Vector3 startAngle, Vector3 finishAngle, float t)
    {        
        float xLerp = Mathf.LerpAngle(startAngle.x, finishAngle.x, t);
        float yLerp = Mathf.LerpAngle(startAngle.y, finishAngle.y, t);
        float zLerp = Mathf.LerpAngle(startAngle.z, finishAngle.z, t);
        Vector3 lerped = new Vector3(xLerp, yLerp, zLerp);
        return lerped;
    }
}