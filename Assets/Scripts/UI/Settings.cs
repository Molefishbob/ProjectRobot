﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour
{
    private const string PercentageFormat = " %";
    private const string Submit = "Submit";
    private const string Cancel = "Cancel";
    [SerializeField]
    private EventSystem _eventSystem = null;
    [SerializeField]
    private Button _backButton = null;
    [SerializeField]
    private TMP_Text _musicText = null, _sFXText = null, _masterText = null;
    [SerializeField]
    private Slider _musicSlider = null, _sFXSlider = null, _masterSlider = null;
    [SerializeField]
    private Toggle _musicMute = null, _sFXMute = null, _masterMute = null;
    [SerializeField]
    private Toggle _volumeSettings = null, _controlSettings = null;
    [SerializeField]
    private GameObject _volume = null, _controls = null;
    [SerializeField]
    private Toggle _invertedXAxis = null, _invertedYAxis = null;
    [SerializeField]
    private TMP_Text _xSensText = null, _ySensText = null, _zoomSpeedText = null, _fovText = null;
    [SerializeField]
    private Slider _xSensSlider = null, _ySensSlider = null, _zoomSpeedSlider = null, _fovSlider = null;
    [SerializeField]
    private GameObject _masterButton = null, _xSensButton = null, _fovButton = null;
    [SerializeField]
    private SingleUISound _sliderButtonSound = null;
    private const bool True = true;
    private const bool False = false;
    private GameObject _lastButton = null;
    private bool _stopIt = false;

    private void OnEnable()
    {

        // Sets the volume settings page to be the default page
        VolumeSettings();

        // Sets all the values for every UI element from save data
        _musicSlider.value = PrefsManager.Instance.AudioVolumeMusic;
        _sFXSlider.value = PrefsManager.Instance.AudioVolumeSFX;
        _masterSlider.value = PrefsManager.Instance.AudioVolumeMaster;

        _musicMute.isOn = PrefsManager.Instance.AudioMuteMusic;
        _masterMute.isOn = PrefsManager.Instance.AudioMuteMaster;
        _sFXMute.isOn = PrefsManager.Instance.AudioMuteSFX;
        _invertedXAxis.isOn = PrefsManager.Instance.InvertedCameraX;
        _invertedYAxis.isOn = PrefsManager.Instance.InvertedCameraY;

        _xSensSlider.value = PrefsManager.Instance.CameraXSensitivity;
        _ySensSlider.value = PrefsManager.Instance.CameraYSensitivity;
        _zoomSpeedSlider.value = PrefsManager.Instance.ZoomSpeed;
        _fovSlider.value = PrefsManager.Instance.FieldOfView;

        // Sets the number indicators to show the correct numbers
        _xSensText.SetText(FormatToDecimalNumber(_xSensSlider.value));
        _ySensText.SetText(FormatToDecimalNumber(_ySensSlider.value));
        _zoomSpeedText.SetText(FormatToDecimalNumber(_zoomSpeedSlider.value));
        _fovText.SetText(_fovSlider.value.ToString());
        _musicText.SetText(_musicSlider.value + PercentageFormat);
        _sFXText.SetText(_sFXSlider.value + PercentageFormat);
        _masterText.SetText(_masterSlider.value + PercentageFormat);

    }

    private void Update()
    {
        if (_lastButton != null)
        {
            if (_stopIt)
            {
                _stopIt = false;
            }
            else if (Input.GetButtonDown(Submit) || Input.GetButtonDown(Cancel))
            {
                PlaySliderButtonSound();
                _eventSystem.SetSelectedGameObject(_lastButton);
                _lastButton = null;
            }
        }
    }

    /// <summary>
    /// Plays the designated slider button sound
    /// </summary>
    public void PlaySliderButtonSound()
    {
        _sliderButtonSound.PlaySound();
    }

    /// <summary>
    /// Formats the given float value to a single desimal string
    /// </summary>
    /// <param name="value">Given float to be changed to x.x format</param>
    /// <returns>The formatted value as a string</returns>
    private string FormatToDecimalNumber(float value)
    {
        return (value / 10f).ToString("N1");
    }

    /// <summary>
    /// Changes the control to the slider assigned to the button
    /// Records the button that was used and starts to listen for inputs
    /// </summary>
    /// <param name="button">The button that was used</param>
    public void ToSlider(GameObject button)
    {
        _lastButton = button;
        _stopIt = true;
    }

    /// <summary>
    /// Called by the music slider. Changes the text value and saves the music volume to PrefsManager
    /// </summary>
    public void MusicPercentage()
    {
        _musicText.SetText(_musicSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeMusic = Mathf.RoundToInt(_musicSlider.value);
    }

    /// <summary>
    /// Called by the SFX slider. Changes the text value and saves the SFX volume to PrefsManager
    /// </summary>
    public void UISoundPercentage()
    {
        _sFXText.SetText(_sFXSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeSFX = Mathf.RoundToInt(_sFXSlider.value);
    }

    /// <summary>
    /// Called by the master volume slider. Changes the text value and saves the master volume to PrefsManager
    /// </summary>
    public void MasterSoundPercentage()
    {
        _masterText.SetText(_masterSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeMaster = Mathf.RoundToInt(_masterSlider.value);
    }

    /// <summary>
    /// Unmutes/Mutes the music
    /// </summary>
    public void MuteMusic()
    {
        PrefsManager.Instance.AudioMuteMusic = _musicMute.isOn;
    }

    /// <summary>
    /// Unmutes/Mutes the sound
    /// </summary>
    public void MuteSound()
    {
        PrefsManager.Instance.AudioMuteSFX = _sFXMute.isOn;
    }

    /// <summary>
    /// Unmutes/Mutes master sound
    /// </summary>
    public void MuteMaster()
    {
        PrefsManager.Instance.AudioMuteMaster = _masterMute.isOn;
    }

    /// <summary>
    /// Changes the page to VolumeSettings
    /// 
    /// Activates the volumesetting page and disables the controlsettings page
    /// Activates a button to go back to control settings
    /// Gives the back button new paths for controller movement
    /// Selects a slider for the console peasants
    /// </summary>
    public void VolumeSettings()
    {
        _volumeSettings.interactable = !True;
        _controlSettings.interactable = !False;

        _volume.SetActive(!False);
        _controls.SetActive(!True);

        _eventSystem.SetSelectedGameObject(_masterButton.gameObject);
        Navigation nav = _backButton.navigation;

        nav.selectOnUp = _sFXMute.GetComponent<Selectable>();
        nav.selectOnRight = _controlSettings.GetComponent<Selectable>();
        nav.selectOnLeft = null;

        _backButton.navigation = nav;

    }

    /// <summary>
    /// Changes the page to ControlSettings
    /// 
    /// Activates the ControlSettings page and disables the VolumeSettings page
    /// Activates a button to go back to Volume settings
    /// Gives the back button new paths for controller movement
    /// Selects a slider for the console peasants
    /// </summary>
    public void ControlSettings()
    {
        _controlSettings.interactable = !True;
        _volumeSettings.interactable = !False;

        _volume.SetActive(!True);
        _controls.SetActive(!False);

        _eventSystem.SetSelectedGameObject(_xSensButton.gameObject);

        Navigation nav = _backButton.navigation;

        nav.selectOnUp = _fovButton.GetComponent<Selectable>();
        nav.selectOnLeft = _volumeSettings.GetComponent<Selectable>();
        nav.selectOnRight = null;

        _backButton.navigation = nav;
    }

    /// <summary>
    /// Used by X-axis toggle. Saves the data to Prefsmanager
    /// </summary>
    public void InvertedXAxis()
    {
        PrefsManager.Instance.InvertedCameraX = _invertedXAxis.isOn;

    }

    /// <summary>
    /// Used by Y-axis toggle. Saves the data to Prefsmanager
    /// </summary>
    public void InvertedYAxis()
    {
        PrefsManager.Instance.InvertedCameraY = _invertedYAxis.isOn;
    }

    /// <summary>
    /// Used by Camera X-axis slider. 
    /// Saves the data to Prefsmanager and updates the value into the text
    /// </summary>
    public void CameraXSensitivity()
    {

        _xSensText.SetText(FormatToDecimalNumber(_xSensSlider.value));
        PrefsManager.Instance.CameraXSensitivity = Mathf.RoundToInt(_xSensSlider.value);
    }

    /// <summary>
    /// Used by Camera Y-axis slider. 
    /// Saves the data to Prefsmanager and updates the value into the text
    /// </summary>
    public void CameraYSensitivity()
    {
        _ySensText.SetText(FormatToDecimalNumber(_ySensSlider.value));
        PrefsManager.Instance.CameraYSensitivity = Mathf.RoundToInt(_ySensSlider.value);
    }

    /// <summary>
    /// Used by Camera zoomspeed slider. 
    /// Saves the data to Prefsmanager and updates the value into the text
    /// </summary>
    public void CameraZoomSpeed()
    {
        _zoomSpeedText.SetText(FormatToDecimalNumber(_zoomSpeedSlider.value));
        PrefsManager.Instance.ZoomSpeed = Mathf.RoundToInt(_zoomSpeedSlider.value);
    }

    /// <summary>
    /// Used by Camera FoV. 
    /// Saves the data to Prefsmanager and updates the value into the text
    /// </summary>
    public void FieldOfView()
    {
        _fovText.SetText(_fovSlider.value.ToString());
        PrefsManager.Instance.FieldOfView = Mathf.RoundToInt(_fovSlider.value);
    }
}
