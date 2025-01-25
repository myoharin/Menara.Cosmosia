using Godot;
using SineVita.Muguet;
using System;

public partial class midi_pulse_input_indicator : Control
{
    [Export]
    public Label NoteNameLabel;
    [Export]
    public Label IntensityLabel;

    private int _midiValue = 69;
    private byte _intensity = 0;

    public const string IntensitySuffix = "dB";

    public void Set(int midiValue, byte intensity)
    {
        _midiValue = midiValue;
        _intensity = intensity;
        NoteNameLabel.Text = HarmonyHelper.MidiToNoteName(_midiValue);
        IntensityLabel.Text = $"{_intensity} {IntensitySuffix}";
    }
}