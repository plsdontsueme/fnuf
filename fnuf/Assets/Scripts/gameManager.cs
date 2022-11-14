using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class gameManager : MonoBehaviour
{
    [SerializeField] float doorTime = .2f;
    [SerializeField] Transform doorL;
    [SerializeField] Transform doorLOpen;
    [SerializeField] Transform doorLClosed;
    [SerializeField] Transform doorR;
    [SerializeField] Transform doorROpen;
    [SerializeField] Transform doorRClosed;
    [SerializeField] float awaketransitionTime = 1.5f;
    [SerializeField] CanvasGroup transition;
    bool lOpen = false;
    bool rOpen = false;

    [SerializeField] Transform camFolder;
    Transform[] cams;
    [SerializeField] Transform securityCam;
    [SerializeField] TextMeshProUGUI camNr;
    [SerializeField] TextMeshProUGUI nightNr;
    [SerializeField] TextMeshProUGUI clockTime;
    int activeCam = -69;

    int freddyState = 0;
    [SerializeField] Transform freddy;
    [SerializeField] Transform freddyNormalPos; 
    [SerializeField] Transform freddyOtherPos;

    private void Awake()
    {
        transition.alpha = 1;
        LeanTween.value(gameObject, 1, 0, awaketransitionTime).setEase(LeanTweenType.easeInOutExpo).setOnUpdate(updateTransition);
        cams = new Transform[camFolder.childCount];
        for (int i = 0; i < cams.Length; i++)
        {
            cams[i] = camFolder.GetChild(i);
        }
        switchCam(0);
    }
    void updateTransition(float value)
    {
        transition.alpha = value;
        if (transition.alpha <= 0.001)
        {
            transition.alpha = 0;
        }
    }

    public void switchCam(int index) 
    {
        if (activeCam == index) return;

        if (freddyState == 0 && index == 2) freddyState = 1;
        else if (freddyState == 1 && index != 2) { freddyState = 2; freddy.SetPositionAndRotation(freddyOtherPos.position, freddyOtherPos.rotation); }
        else if (freddyState == 2 && index == 2) { freddyState = 3; }
        else if (freddyState == 3 && index != 2) { freddyState = 4; freddy.SetPositionAndRotation(freddyNormalPos.position, freddyNormalPos.rotation); }


        activeCam = index;
        securityCam.transform.SetPositionAndRotation(cams[index].position, cams[index].rotation);
        camNr.text = "cam" + index;
    }

    public void openL() { 
        if(lOpen) return;
        lOpen = true;   
        LeanTween.cancel(doorL.gameObject);
        doorL.LeanMove(doorLOpen.position, doorTime);
    }

    public void closeL()
    {
        if (!lOpen) return;
        lOpen = false;
        LeanTween.cancel(doorL.gameObject);
        doorL.LeanMove(doorLClosed.position, doorTime);
    }

    public void openR()
    {
        if (rOpen) return;
        rOpen = true;
        LeanTween.cancel(doorR.gameObject);
        doorR.LeanMove(doorROpen.position, doorTime);
    }

    public void closeR()
    {
        if (!rOpen) return;
        rOpen = false;
        LeanTween.cancel(doorR.gameObject);
        doorR.LeanMove(doorRClosed.position, doorTime);
    }


}
