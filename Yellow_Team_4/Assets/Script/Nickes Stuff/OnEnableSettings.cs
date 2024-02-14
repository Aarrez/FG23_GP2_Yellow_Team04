using UnityEngine;
using UnityEngine.UI;

public class OnEnableSettings : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider masterSlider;
    private void OnEnable()
    {
        // var sv = UserDataManager.GetSavedVolume.Invoke();
        // sfxSlider.value = sv.Sfx;
        // masterSlider.value = sv.Master;
        // musicSlider.value = sv.Music;
    }
}
