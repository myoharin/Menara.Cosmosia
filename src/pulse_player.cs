using Godot;
using System;
using SineVita.Muguet;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;

public partial class pulse_player : BoxContainer
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public Button ActivateButton;
	[Export]
	public Button ManualADSR;
	[Export]
	public OptionButton NoteNameCombo;
	[Export]
	public SpinBox MidiValueSpinBox;
	[Export]
	public SpinBox OctaveSpinBox;
	[Export]
	public SpinBox IntensitySpinBox;
	[Export]
	public Timer NoteChangedTimer;
	[Export]
	public pulse_manager PulseManager;

	private Pulse _pulse = new Pulse(new MidiPitch(69), 0); 
	public Pulse Pulse { get {	
		Pulse pulse = _pulse;
		pulse.Intensity = Math.Max((byte)IntensitySpinBox.Value,adsrByteIntensity);
		pulse.Pitch.UpdateFrequency();
		return pulse;
	} }

	// * Internal Vars
	public bool CanChangeNote = true;

	// * Manual ADSR Variables

	private static float _attackTime = 1; // seconds
	private static float _decayTime = 1; // seconds
	private static float _sustainTime = 1; // seconds
	private static float _releaseTime = 1; // seconds

	private static byte _attackIntensity = 60; // dB
	private static byte _sustainIntensity = 60; // dB

	private static byte attackLerp(byte b, double t, byte? a=null) {
		t = Math.Clamp(t, 0, 1);
		a = a??0;
		return (byte)(a + (b - a) * t);
	}
	private static byte decayLerp(byte b, double t, byte? a=null) {
		t = Math.Clamp(t, 0, 1);
		a = a??_attackIntensity;
		return (byte)(a + (b - a) * t);
	}
	private static byte sustainLerp(byte b, double t, byte? a=null) {
		t = Math.Clamp(t, 0, 1);
		a = a??_sustainIntensity;
		return (byte)(a + (b - a) * t);
	}
	private static byte releaseLerp(byte b, double t, byte? a=null) {
		t = Math.Clamp(t, 0, 1);
		a = a??_sustainIntensity;
		return (byte)(a + (b - a) * t);
	}

	private float adsrIntensity = 0;
	private byte adsrByteIntensity { get { 
		if (ActivateButton.ButtonPressed) {
			return (byte)Math.Clamp(Math.Round(adsrIntensity),0,127);
		} 
		else {return 0;}
	} }

	// * * Signals
	public void _on_activate_button_toggled(bool toggled_on){

	}
	public void _on_note_changed_timer_timeout() {CanChangeNote = true;}
	public void _on_delete_button_button_up() {
		PulseManager.RemovePulsePlayer(_pulse.PulseID);
		QueueFree();
	}

	// * Change Note
	public void _on_note_name_combo_item_selected(int index) {
		if (CanChangeNote) {
			NoteChangedTimer.Start();
			MidiValueSpinBox.Value = (OctaveSpinBox.Value+1) * 12 + NoteNameCombo.Selected;
			_noteChanged();
		}
	}
	public void _on_octave_spin_box_value_changed(float value) {
		if (CanChangeNote) {
			NoteChangedTimer.Start();
			MidiValueSpinBox.Value = (OctaveSpinBox.Value+1) * 12 + NoteNameCombo.Selected;	
			_noteChanged();
		}
	}
	public void _on_midi_value_spin_box_value_changed(float value) {
		if (CanChangeNote) {
			NoteChangedTimer.Start();
			OctaveSpinBox.Value = Math.Floor(MidiValueSpinBox.Value / 12) - 1;
			NoteNameCombo.Selected = (int)Math.Floor(MidiValueSpinBox.Value % 12);
			_noteChanged();
		}
	}
	private void _noteChanged() {
		((MidiPitch)_pulse.Pitch).PitchIndex = (int)MidiValueSpinBox.Value;
		PulseManager.MutatePulsePlayerNote(Pulse);
	}

	// * Intensity
	public void _on_manual_adsr_button_down() {
		
	}
	public void _on_manual_adsr_button_up() {

	}

}
