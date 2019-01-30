using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {
    // (Optional) Prevent non-singleton constructor use.
    protected SoundManager() { }
    public HashSet<VolumeControl> SoundsSFX { get; private set; } = new HashSet<VolumeControl>();
    public HashSet<VolumeControl> SoundsMusic { get; private set; } = new HashSet<VolumeControl>();
    public HashSet<VolumeControl> SoundsUI { get; private set; } = new HashSet<VolumeControl>();
    //TODO: read values from player preferences!!!
    public bool MutedSFX { get; private set; } = false;
    public bool MutedMusic { get; private set; } = false;
    public bool MutedUI { get; private set; } = false;
    public bool MutedAll { get; private set; } = false;
    private float _volumeSFX = 1.0f;
    public float VolumeSFX {
        get {
            return _volumeSFX;
        }
        private set {
            _volumeSFX = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }
    private float _volumeMusic = 1.0f;
    public float VolumeMusic {
        get {
            return _volumeMusic;
        }
        private set {
            _volumeMusic = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }
    private float _volumeUI = 1.0f;
    public float VolumeUI {
        get {
            return _volumeUI;
        }
        private set {
            _volumeUI = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }
    private float _volumeAll = 1.0f;
    public float VolumeAll {
        get {
            return _volumeAll;
        }
        private set {
            _volumeAll = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }
    public void MuteSFX(bool muted) {
        MutedSFX = muted;
        foreach(VolumeControl control in SoundsSFX) {
            control.Mute(muted || MutedAll);
        }
    }
    public void MuteMusic(bool muted) {
        MutedMusic = muted;
        foreach(VolumeControl control in SoundsMusic) {
            control.Mute(muted || MutedAll);
        }
    }
    public void MuteUI(bool muted) {
        MutedUI = muted;
        foreach(VolumeControl control in SoundsUI) {
            control.Mute(muted || MutedAll);
        }
    }
    public void MuteAll(bool muted) {
        MutedAll = muted;
        foreach(VolumeControl control in SoundsSFX) {
            control.Mute(muted || MutedSFX);
        }
        foreach(VolumeControl control in SoundsMusic) {
            control.Mute(muted || MutedMusic);
        }
        foreach(VolumeControl control in SoundsUI) {
            control.Mute(muted || MutedUI);
        }
    }
    public void SetVolumeSFX(float volume) {
        VolumeSFX = volume;
        foreach(VolumeControl control in SoundsSFX) {
            control.SetVolume(VolumeSFX * VolumeAll);
        }
    }
    public void SetVolumeMusic(float volume) {
        VolumeMusic = volume;
        foreach(VolumeControl control in SoundsMusic) {
            control.SetVolume(VolumeMusic * VolumeAll);
        }
    }
    public void SetVolumeUI(float volume) {
        VolumeUI = volume;
        foreach(VolumeControl control in SoundsUI) {
            control.SetVolume(VolumeUI * VolumeAll);
        }
    }
    public void SetVolumeAll(float volume) {
        VolumeAll = volume;
        foreach(VolumeControl control in SoundsSFX) {
            control.SetVolume(VolumeSFX * VolumeAll);
        }
        foreach(VolumeControl control in SoundsMusic) {
            control.SetVolume(VolumeMusic * VolumeAll);
        }
        foreach(VolumeControl control in SoundsUI) {
            control.SetVolume(VolumeUI * VolumeAll);
        }
    }
}