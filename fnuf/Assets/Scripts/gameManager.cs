using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    //scene management
    [SerializeField] string menuScene = "menu";

    //door
    [SerializeField] float doorTime = .2f;
    [SerializeField] Transform doorL;
    [SerializeField] Transform doorLOpen;
    [SerializeField] Transform doorLClosed;
    [SerializeField] Transform doorR;
    [SerializeField] Transform doorROpen;
    [SerializeField] Transform doorRClosed;
    bool lOpen = false;
    bool rOpen = false;

    //transition
    [SerializeField] CanvasGroup transition;
    [SerializeField] float awaketransitionTime = 1.5f;
    //seccam power lost transition
    [SerializeField] CanvasGroup seccamPowerTransition;
    [SerializeField] float seccamPowerTransitionTime = 1.5f;

    //cams
    [SerializeField] Transform camFolder;
    Transform[] cams;
    [SerializeField] Transform securityCam;
    int activeCam = -69;

    //freddy
    int freddyState = 0;
    [SerializeField] Transform freddy;
    [SerializeField] Transform freddyNormalPos; 
    [SerializeField] Transform freddyOtherPos;
    //amogus
    [SerializeField] GameObject amogusjumpscare;
    [SerializeField] GameObject jumpscareAudio;

    //power
    float power = 100f;
    float powerUsagePerSec = 0f;
    [SerializeField] float doorUsagePerSec = .3f;
    [SerializeField] float camUsagePerSec = .2f;
    float maxPowerUsagePerSec;
    bool powerout = false;

    //ui
    [SerializeField] TextMeshProUGUI powerUI;
    [SerializeField] Slider slPowerUsage;

    [SerializeField] TextMeshProUGUI camNr;
    [SerializeField] TextMeshProUGUI nightNr;
    [SerializeField] TextMeshProUGUI clockTime;

    private void Awake()
    {
        amogusjumpscare.SetActive(false);

        if (instance == null) instance = this;
        else Destroy(this);

        maxPowerUsagePerSec = doorUsagePerSec * 2 + camUsagePerSec;
        slPowerUsage.maxValue = maxPowerUsagePerSec;

        seccamPowerTransition.alpha = 0f;
        openL();
        openR();
        transition.alpha = 1;
        LeanTween.value(gameObject, 1, 0, awaketransitionTime).setEase(LeanTweenType.easeInOutExpo).setOnUpdate(updateTransition);
        cams = new Transform[camFolder.childCount];
        for (int i = 0; i < cams.Length; i++)
        {
            cams[i] = camFolder.GetChild(i);
        }
        switchCam(0);
    }

    private void FixedUpdate()
    {
        updatePower();
    }

    void updatePower()
    {
        power -= powerUsagePerSec * Time.fixedDeltaTime;
        if(power <= 0)
        {
            power = 0;
            if (powerout == false)
            {
                powerout = true;
                //shut off all systems that use power
                openL();
                openR();
                LeanTween.value(gameObject, 0, 1, seccamPowerTransitionTime).setEase(LeanTweenType.easeInOutExpo).setOnUpdate(updateSeccamPowerTransition);
                powerUsagePerSec -= camUsagePerSec;
                if (powerUsagePerSec < 0f) powerUsagePerSec = 0f;
                Invoke("gameOver", 5f);
            }

        }
        powerUI.text = "power: " + Mathf.RoundToInt(power) + "%";
        slPowerUsage.value = powerUsagePerSec;
    }

    void gameOver() {
        amogusjumpscare.SetActive(true);
        Instantiate(jumpscareAudio, transform.position, transform.rotation);
        Invoke("backToMenu", 22f);
    }

    void backToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    void updateSeccamPowerTransition(float value) 
    {
        seccamPowerTransition.alpha = value;
        if (seccamPowerTransition.alpha >= 0.999)
        {
            seccamPowerTransition.alpha = 1;
        }
    }

    void updateTransition(float value)
    {
        transition.alpha = value;
        if (transition.alpha <= 0.001)
        {
            transition.alpha = 0;
        }
    }

    public void onCamOpen()
    {
        powerUsagePerSec += camUsagePerSec;
    }

    public void onCamClose()
    {
        powerUsagePerSec -= camUsagePerSec;
        if (powerUsagePerSec < 0f) powerUsagePerSec = 0f;
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

    public void openL() 
    { 
        if (lOpen) return;

        powerUsagePerSec -= doorUsagePerSec;
        if (powerUsagePerSec < 0f) powerUsagePerSec = 0f;

        lOpen = true;   
        LeanTween.cancel(doorL.gameObject);
        doorL.LeanMove(doorLOpen.position, doorTime);
    }

    public void closeL()
    {
        if (!lOpen || power <= 0f) return;

        powerUsagePerSec += doorUsagePerSec;

        lOpen = false;
        LeanTween.cancel(doorL.gameObject);
        doorL.LeanMove(doorLClosed.position, doorTime);
    }

    public void openR()
    {
        if (rOpen) return;

        powerUsagePerSec -= doorUsagePerSec;
        if (powerUsagePerSec < 0f) powerUsagePerSec = 0f;

        rOpen = true;
        LeanTween.cancel(doorR.gameObject);
        doorR.LeanMove(doorROpen.position, doorTime);
    }

    public void closeR()
    {
        if (!rOpen || power <= 0f) return;

        powerUsagePerSec += doorUsagePerSec;

        rOpen = false;
        LeanTween.cancel(doorR.gameObject);
        doorR.LeanMove(doorRClosed.position, doorTime);
    }


}
