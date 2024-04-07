using UnityEngine;



/// Store the integer <summary>
/// Integer to joystick value for drone
/// Drone Joystick to Vector3 Movement
/// Drone Joystick to Rotation Movement
/// Drone Joystick to Tilt Lerp 
/// Apply Integer to Texture2D
/// Move Mesh Triangle Vertices
/// </summary>

public class IID_NativeArrayValue : MonoBehaviour
{

}
public class IID_NativeArrayDrone : MonoBehaviour
{

    public struct GamePadXbox
    {
        public float m_joystickLeftHorizontal;
        public float m_joystickLeftVertical;
        public float m_joystickRightHorizontal;
        public float m_joystickRightVertical;
        public float m_joystickTiggerLeft;
        public float m_joystickTiggerRight;

    }
    public struct GamePadOpenXR
    {
        public float m_joystickLeftHorizontal;
        public float m_joystickLeftVertical;
        public float m_joystickRightHorizontal;
        public float m_joystickRightVertical;
        public float m_joystickTiggerLeft;
        public float m_joystickTiggerRight;

    }
    public struct KeyboardButtonState {/*104 keys*/ }
    public struct SteamdeckButtonState {/*32 keys*/ }


    public struct DroneJoystick
    {
        public float m_joystickX;
        public float m_joystickY;
        public float m_joystickZ;
        public float m_joystickW;
    }
    public struct DronePosition
    {
        public Vector3 m_localPosition;
        public float m_rotationHorizontal;
    }
    public struct DroneTiltLerp
    {
        public float m_tiltAngleCurrent;
        public float m_rollAngleCurrent;
        public float m_tiltAngleTarget;
        public float m_rollAngleTarget;
    }


    public struct DronePositionToTransform {
        public Vector3 m_worldPosition;
        public Quaternion m_worldRotation;
    }

    public struct DroneBulletStartPoint
    {
        public Vector3 m_startPosition;
        public Vector3 m_startDirection;
        public float m_speed;
    }

  
    public struct DroneBullet
    {
        public Vector3 m_bulletPosition;
        public Vector3 m_bulletDirection;
    }
}



public class IID_NativeArrayDroneMove : MonoBehaviour
{
   
}
