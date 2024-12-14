using UnityEngine;

public class MousePositionManager : MonoBehaviourSingleton<MousePositionManager>
{
    public Vector3 GetWorldPosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        
        if (plane.Raycast(mouseCameraRay, out float distance))
        {
            return mouseCameraRay.GetPoint(distance);
        }

        return Vector3.zero;
    }
}
