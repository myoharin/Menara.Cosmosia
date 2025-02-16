using Godot;
using System;
using System.Text.Json;
using System.Collections.Generic;
using SineVita.Muguet;
using System.Net.Http.Headers;
namespace Menara.Cosmosia {
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

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta) {	
			foreach (Node child in GetChildren()) {
				if (child is pulse_player) {
					var pulse = ((pulse_player)child).Pulse;
					var id = pulse.PulseID;
					Menara.MutatePulse(id, pulse);
				}
			}
			foreach (int midi_index in MidiEventHandler.NoteStatus.Keys) {
				var pulse = new Pulse(new MidiPitch(midi_index), (byte)MidiEventHandler.NoteStatus[midi_index].AdsrVelocity);
				pulse.PulseID = MidiEventHandler.NoteStatus[midi_index].PulseId;
				Menara.MutatePulse(pulse.PulseID, pulse);
			}
			foreach (Node child in MidiIndicatorContainer.GetChildren()) {
				foreach (int midi_index in MidiEventHandler.NoteStatus.Keys) {
					if (child.Name == MidiEventHandler.NoteStatus[midi_index].PulseId.ToString()) {
						((Label)child.GetChild(1)).Text = MidiEventHandler.NoteStatus[midi_index].AdsrVelocity.ToString();
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
			var pulse = new Pulse(new MidiPitch(midi_index), (byte)MidiEventHandler.NoteStatus[midi_index].AdsrVelocity);
			pulse.PulseID = MidiEventHandler.NoteStatus[midi_index].PulseId;
			Menara.AddPulse(pulse);

			var newIndicator = MidiPulseInputIndicatorScene.Instantiate();
			newIndicator.Name = $"{pulse.PulseID}";
			((Label)newIndicator.GetChild(1)).Text = HarmonyHelper.MidiToNoteName(midi_index);
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
				if (child is pulse_player) {
					((pulse_player)child)._on_delete_button_button_up();
				}
			}
		}

		public void MutatePulsePlayerNote(Pulse pulse) {
			Menara.DeletePulse(pulse.PulseID);
			Menara.AddPulse(pulse);
		}
	}
}
