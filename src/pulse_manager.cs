using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;
using SineVita.Muguet;
using System.Net.Http.Headers;

public partial class pulse_manager : VBoxContainer
{

	[Export]
	public Button AddPulse;
	[Export]
	public Button DeletePulse;
	[Export]
	public PackedScene PulsePlayerScene;
	[Export] 		
	public PackedScene MidiPulseInputIndicatorScene;
	[Export]
	public menara_script Menara;
	[Export]
	public VBoxContainer MidiIndicatorContainer;
	[Export]
	public midi_event_handler MidiEventHandler;



	public string MidiPulseInputIndicatorPath = "res://prefabs/Control/midi_pulse_input_indicator.tscn";
	public string PulsePlayerPath = "res://prefabs/Control/pulse_player.tscn";
	public double TimeSinceMidiAction = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {	
		foreach (Node child in GetChildren()) {
			if (child.Name != "PulsePlayerEditor" && child.Name != "MidiIndicators" && child.Name != "MidiEvenHandler") {
				var pulse = ((pulse_player)child).Pulse;
				var id = pulse.PulseID;
				Menara.MutatePulse(id, pulse);
			}
		}
		foreach (int midi_index in MidiEventHandler.noteIds.Keys) {
			var pulse = new Pulse(new MidiPitch(midi_index), (byte)MidiEventHandler.noteVelocities[midi_index]);
			pulse.PulseID = MidiEventHandler.noteIds[midi_index];
			Menara.MutatePulse(pulse.PulseID, pulse);
		}
		foreach (Node child in MidiIndicatorContainer.GetChildren()) {
			foreach (int midi_index in MidiEventHandler.noteIds.Keys) {
				if (child.Name == MidiEventHandler.noteIds[midi_index].ToString()) {
					((Label)child.GetChild(1)).Text = MidiEventHandler.noteVelocities[midi_index].ToString();
				}
			}
		}
		TimeSinceMidiAction += delta;
		if (TimeSinceMidiAction > 2) {
			TimeSinceMidiAction = 0;
			foreach (Node child in MidiIndicatorContainer.GetChildren()) {
				Menara.DeletePulse(Int32.Parse((child.Name.ToString())));
				MidiIndicatorContainer.RemoveChild(child);
				child.QueueFree();
			}
		}


	}

	// * Midi Event handler

	public void MidiAddNote(int midi_index) {
		TimeSinceMidiAction = 0;
		var pulse = new Pulse(new MidiPitch(midi_index), (byte)MidiEventHandler.noteVelocities[midi_index]);
		pulse.PulseID = MidiEventHandler.noteIds[midi_index];
		Menara.AddPulse(pulse);

		var newIndicator = MidiPulseInputIndicatorScene.Instantiate();
		newIndicator.Name = $"{pulse.PulseID}";
		((Label)newIndicator.GetChild(1)).Text = HarmonyHelper.ConvertMidiToNoteName(midi_index);
		((Label)newIndicator.GetChild(1)).Text = pulse.Intensity.ToString();
		MidiIndicatorContainer.AddChild(newIndicator);
	}
	public void MidiRemoveNote(int id) {
		TimeSinceMidiAction = 0;
		foreach (Node child in MidiIndicatorContainer.GetChildren()) {
			if (child.Name == $"{id}") {
				MidiIndicatorContainer.RemoveChild(child);
				child.QueueFree();
				break;
			}
		}
		Menara.DeletePulse(id);
	}



	// * Pulse Player

	public void _on_add_pulse_button_up(){AddPulsePlayer();}
	public void _on_delete_pulse_button_up(){DeleteAllPulsePlayer();}

	public void AddPulsePlayer() {
		var newPlayer = (pulse_player)PulsePlayerScene.Instantiate();
		newPlayer.PulseManager = this;
		Menara.AddPulse(newPlayer.Pulse);
		AddChild(newPlayer);
	}

	public void RemovePulsePlayer(int id) {
		Menara.DeletePulse(id);
	}

	public void DeleteAllPulsePlayer() {
		foreach (Node child in GetChildren()) {
			if (child.Name != "PulsePlayerEditor" && child.Name != "MidiIndicators") {
				((pulse_player)child)._on_delete_button_button_up();
			}
		}
	}

	public void MutatePulsePlayerNote(Pulse pulse) {
		Menara.DeletePulse(pulse.PulseID);
		Menara.AddPulse(pulse);
	}

	public void RecieveMidiMessages() { // handle midi messgae

	}
}
