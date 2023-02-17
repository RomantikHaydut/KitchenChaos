using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        LookForward,
        lookForwardInverted
    }

    [SerializeField] private Mode mode;

    void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.LookForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.lookForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}
