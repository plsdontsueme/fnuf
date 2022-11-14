using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    [SerializeField] string gameScene = "game";
    [SerializeField] float transitionTime = 1f;
    [SerializeField] float awaketransitionTime = 1.5f;

    [SerializeField] CanvasGroup transition;
    [SerializeField] CanvasGroup menu;
    bool inTransition = false;

    private void Awake()
    {
        transition.alpha = 1f;
        menu.alpha = 0f;
        LeanTween.value(gameObject, 0, 1, awaketransitionTime).setEase(LeanTweenType.easeInOutExpo).setOnUpdate(updateTransition);
    }

    void updateTransition(float value) 
    {
        transition.alpha = 1-value;
        menu.alpha = value;
        if (transition.alpha >= 0.999f)
        {
            transition.alpha = 0;
            menu.alpha = 1;
        }
    }

    void updateSceneTransition(float value)
    {
        transition.alpha = value;
        menu.alpha = 1 - value;
        if (transition.alpha >= 0.999f) 
        {
            SceneManager.LoadScene(gameScene);
        }
    }

    public void btStart() 
    {
        if (inTransition) return;
        inTransition = true;
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 0, 1, transitionTime).setEase(LeanTweenType.easeInOutExpo).setOnUpdate(updateSceneTransition);
    }


    /*
    VolumeProfile volumeProfile;
    LensDistortion lensDistortion;

    private void Awake()
    {
        volumeProfile = Camera.main.GetComponent<Volume>().profile;
        if(!volumeProfile.TryGet(out lensDistortion)) 
            throw new System.NullReferenceException(nameof(lensDistortion));
        lensDistortion.scale.Override(Constants.peakLensDistortionScale);
        lensDistortion.intensity.Override(Constants.peakLensDistortionIntens);
        Invoke("prepare", .3f);
    }

    void prepare()
    {
        LeanTween.value(gameObject, Constants.peakLensDistortionScale, Constants.menuLensDistortionScale, awaketransitionTime).setEase(LeanTweenType.easeInOutExpo).setOnUpdate(updateScaleTransition);
        LeanTween.value(gameObject, Constants.peakLensDistortionIntens, Constants.menuLensDistortionIntens, awaketransitionTime).setEase(LeanTweenType.easeInOutExpo).setOnUpdate(updateIntensityTransition);
    }

    void updateScaleTransition(float value) 
    {
        lensDistortion.scale.Override(value);
    }
    void updateIntensityTransition(float value)
    {
        lensDistortion.intensity.Override(value);
    }

    public IEnumerable loadGame() 
    {
        LeanTween.cancel(gameObject);

        yield return new WaitForSeconds(transitionTime);
    }
    */
}
