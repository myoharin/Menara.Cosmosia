using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Menara.Cosmosia {
    public struct MidiNoteStatus {
        public bool IsOn;
        public float InputVelocity;
        public float AdsrVelocity;
        public float HeldDuration;
        public int PulseId;
    }

    public partial class midi_event_handler : Node {
        [Export]
        public pulse_manager PulseManager;

        // Dictionary to keep track of note statuses
        public Dictionary<int, MidiNoteStatus> NoteStatus = new Dictionary<int, MidiNoteStatus>();

        public AdsrModule AdsrModule;

        public override void _Ready() {
            OS.OpenMidiInputs();
            AdsrModule = new AdsrModule();
        }

        public override void _Input(InputEvent inputEvent) {
            if (inputEvent is not InputEventMidi) { return; }
            InputEventMidi midiEvent = (InputEventMidi)inputEvent;

            // Handle note on/off events
            if (midiEvent.Message == (MidiMessage)9) {
                int newId = new Random().Next();
                NoteStatus[midiEvent.Pitch] = new MidiNoteStatus {
                    IsOn = true,
                    InputVelocity = midiEvent.Velocity,
                    AdsrVelocity = 0,
                    HeldDuration = 0,
                    PulseId = newId
                };
                PulseManager.MidiAddNote(midiEvent.Pitch);
            } else if (midiEvent.Message == (MidiMessage)8 && NoteStatus.ContainsKey(midiEvent.Pitch)) {
                var status = NoteStatus[midiEvent.Pitch];
                status.IsOn = false;
                NoteStatus[midiEvent.Pitch] = status;
            } else if (midiEvent.Message == (MidiMessage)12 && NoteStatus.ContainsKey(midiEvent.Pitch)) { // channel pressure change
                var status = NoteStatus[midiEvent.Pitch];
                status.InputVelocity = midiEvent.Pressure;
                NoteStatus[midiEvent.Pitch] = status;
            }

            // Print current state
            // PrintCurrentNoteStates();
        }

        public void PrintCurrentNoteStates() {
            foreach (var note in NoteStatus) {
                GD.Print($"Pitch: {note.Key}, Is On: {note.Value.IsOn}, Input Velocity: {note.Value.InputVelocity}, Adsr Velocity: {note.Value.AdsrVelocity}, Held Duration: {note.Value.HeldDuration}");
            }
        }

        public override void _Process(double delta) {
            var keys = new List<int>(NoteStatus.Keys);
            foreach (var key in keys) {
                var status = NoteStatus[key];
                status.HeldDuration += (float)delta;
                status.AdsrVelocity = AdsrModule.AdsrInterpolate(status.HeldDuration, true); // replace with adsr calculation
                if (status.AdsrVelocity == 0) {
                    PulseManager.MidiRemoveNote(status.PulseId);
                    NoteStatus.Remove(key);
                } else {
                    NoteStatus[key] = status;
                }
            }
        }
    }
}
