using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerControll : MonoBehaviour
{
    [SerializeField] float margin = 100f;
    [SerializeField] float CamSpeed;
    [SerializeField] Transform tablet;
    [SerializeField] Transform tabletPosAct;
    [SerializeField] Transform tabletPosPas;
    [SerializeField] float tabletSpeed = .4f;
    [SerializeField] float maxRotation = 0f;
    [SerializeField] float minRotation = -180f;

    bool inCams = false;

    private void Awake()
    {
        tablet.transform.SetLocalPositionAndRotation(tabletPosPas.position, tabletPosPas.rotation);
    }

    private void Start()
    {
        deactivateCam();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        handleRotation();
    }

    void handleRotation() 
    {
        if (inCams) return;

        Vector2 mouse_position = Input.mousePosition;

        if (mouse_position.x < margin)
        {
            transform.Rotate(Vector3.up * -CamSpeed * Time.fixedDeltaTime);
        }
        if (mouse_position.x > Screen.width - margin)
        {
            transform.Rotate(Vector3.up * CamSpeed * Time.fixedDeltaTime);
        }
        //print(transform.rotation.eulerAngles.y);
    }

    public void onCam(InputAction.CallbackContext context) 
    {
        if (context.started) 
        { 
            if(inCams) deactivateCam();
            else activateCam();
        }
        
    }

    void activateCam() 
    {
        inCams = true;
        LeanTween.cancel(tablet.gameObject);
        tablet.LeanMove(tabletPosAct.position, tabletSpeed);
        tablet.LeanRotate(tabletPosAct.rotation.eulerAngles, tabletSpeed);
        gameManager.instance.onCamOpen();
    }

    void deactivateCam()
    {
        inCams = false;
        LeanTween.cancel(tablet.gameObject);
        tablet.LeanMove(tabletPosPas.position, tabletSpeed);
        tablet.LeanRotate(tabletPosPas.rotation.eulerAngles, tabletSpeed);
        gameManager.instance.onCamClose();
    }
}
