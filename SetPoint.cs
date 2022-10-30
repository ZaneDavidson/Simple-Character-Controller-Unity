using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPoint : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        SetRotation();
    }

    void SetRotation()
    {
        var lookAtPos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(lookAtPos);
        if(Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.layer == 6)
        {
            transform.position = hit.point;
        }
    }
}
