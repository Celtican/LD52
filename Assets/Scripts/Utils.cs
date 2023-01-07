using UnityEngine;

public static class Utils
{
    public static void RotateTowards(GameObject gameObject, Vector3 targetPosition, float lerp)
    {
        Vector3 difference = targetPosition - gameObject.transform.position;
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), lerp);
    }
    
    public static void RotateTowards(GameObject gameObject, Vector3 targetPosition)
    {
        Vector3 difference = targetPosition - gameObject.transform.position;
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
