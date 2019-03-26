﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour
{
    private const string PercentageFormat = " %";
    [SerializeField]
    private EventSystem  _eventSystem = null;
    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private TMP_Text  _musicText = null, _sFXText = null, _masterText = null;
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

    private const bool True = true;
    private const bool False = false;

    private void OnEnable() {
        VolumeSettings();
        _musicSlider.value = PrefsManager.Instance.AudioVolumeMusic;
        _sFXSlider.value = PrefsManager.Instance.AudioVolumeSFX;
        _masterSlider.value = PrefsManager.Instance.AudioVolumeMaster;

        _musicMute.isOn = PrefsManager.Instance.AudioMuteMusic;
        _masterMute.isOn = PrefsManager.Instance.AudioMuteMaster;
        _sFXMute.isOn = PrefsManager.Instance.AudioMuteSFX;

        _musicText.SetText(_musicSlider.value + PercentageFormat);
        _sFXText.SetText(_sFXSlider.value + PercentageFormat);
        _masterText.SetText(_masterSlider.value + PercentageFormat);
    }

    public void MusicPercentage() {
        _musicText.SetText(_musicSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeMusic =  Mathf.RoundToInt(_musicSlider.value);
    }
    public void UISoundPercentage() {
        _sFXText.SetText(_sFXSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeSFX = Mathf.RoundToInt(_sFXSlider.value);
    }
    public void MasterSoundPercentage() {
        _masterText.SetText(_masterSlider.value + PercentageFormat);
        PrefsManager.Instance.AudioVolumeMaster = Mathf.RoundToInt(_masterSlider.value);
    }
    public void MuteMusic()
    {
        PrefsManager.Instance.AudioMuteMusic = !PrefsManager.Instance.AudioMuteMusic;
    }
    public void MuteSound()
    {
        PrefsManager.Instance.AudioMuteSFX = !PrefsManager.Instance.AudioMuteSFX;
    }
    public void MuteMaster()
    {
        PrefsManager.Instance.AudioMuteMaster = !PrefsManager.Instance.AudioMuteMaster;
    }

    public void VolumeSettings() {
        _volumeSettings.interactable = !True;
        _controlSettings.interactable = !False;

        _volume.SetActive(!False);
        _controls.SetActive(!True);

        _eventSystem.SetSelectedGameObject(_masterSlider.gameObject);
        Navigation nav = _backButton.navigation;

        nav.selectOnUp = _controlSettings.GetComponent<Selectable>();
        nav.selectOnRight = _controlSettings.GetComponent<Selectable>();

        _backButton.navigation = nav;

    }
    public void ControlSettings() {
        _controlSettings.interactable = !True;
        _volumeSettings.interactable = !False;

        _volume.SetActive(!True);
        _controls.SetActive(!False);

        _eventSystem.SetSelectedGameObject(_xSensSlider.gameObject);

         Navigation nav = _backButton.navigation;

        nav.selectOnUp = _volumeSettings.GetComponent<Selectable>();
        nav.selectOnLeft = _volumeSettings.GetComponent<Selectable>();

        _backButton.navigation = nav;
    }
    public void InvertedXAxis()
    {
        PrefsManager.Instance.InvertedCameraX = _invertedXAxis.isOn;
        
    }
    public void InvertedYAxis()
    {
        PrefsManager.Instance.InvertedCameraY = _invertedYAxis.isOn;
    }
    public void CameraXSensitivity()
    {
        
        _xSensText.SetText((Mathf.Round(_xSensSlider.value) / 10f).ToString());
    }
    public void CameraYSensitivity()
    {
        _ySensText.SetText((Mathf.Round(_ySensSlider.value) / 10f).ToString());
    }
    public void CameraZoomSpeed()
    {
        _zoomSpeedText.SetText((Mathf.Round(_zoomSpeedSlider.value) / 10f).ToString());
    }public void FieldOfView()
    {
        _fovText.SetText(_fovSlider.value.ToString());
    }
}