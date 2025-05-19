using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    public void Init(Transform playerObject)
    {
        _cinemachineCamera.Follow = playerObject;
    }
}