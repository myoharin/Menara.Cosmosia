using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class midi_event_handler : Node
{
    [Export]
    public pulse_manager PulseManager;

    // Dictionary to keep track of note states
    public Dictionary<int, bool> noteStates = new Dictionary<int, bool>(); // Key: Pitch, Value: Is Note On
    public Dictionary<int, float> noteVelocities = new Dictionary<int, float>(); // Key: Pitch, Value: Velocity
    public Dictionary<int, int> noteIds = new Dictionary<int, int>(); // used as note ids

    public override void _Ready() {OS.OpenMidiInputs();}
    public override void _Input(InputEvent inputEvent) {
        if (inputEvent is not InputEventMidi) {return;}
        InputEventMidi midiEvent = (InputEventMidi)inputEvent;

        // Handle note on/off events
        if (midiEvent.Message == (MidiMessage)9) {
            int newId = new Random().Next();
            noteIds[midiEvent.Pitch] = newId; // Assign a new random ID to the note
            noteStates[midiEvent.Pitch] = true; // Turn note on
            noteVelocities[midiEvent.Pitch] = midiEvent.Velocity; // Record velocity
            PulseManager.MidiAddNote(midiEvent.Pitch);
            
        }
        else if (midiEvent.Message == (MidiMessage)8 && noteIds.ContainsKey(midiEvent.Pitch)) {
            PulseManager.MidiRemoveNote(noteIds[midiEvent.Pitch]);
            noteStates.Remove(midiEvent.Pitch); // Turn note off
            noteVelocities.Remove(midiEvent.Pitch); // Optionally remove velocity tracking
            noteIds.Remove(midiEvent.Pitch);
        }
        else if (midiEvent.Message == (MidiMessage)12) { // channel pressure change
            noteVelocities[midiEvent.Pitch] = midiEvent.Pressure;
        }

        // Print current state
        // PrintCurrentNoteStates();
    }

    private void PrintCurrentNoteStates() {
        foreach (var note in noteStates) {
            GD.Print($"Pitch: {note.Key}, Is On: {note.Value}, Velocity: {(noteVelocities.ContainsKey(note.Key) ? noteVelocities[note.Key].ToString() : "N/A")}");
        }
    }
    public override void _Process(double delta) {
        foreach(var key in noteVelocities.Keys) {
            noteVelocities[key] = (int)Math.Max(0, noteVelocities[key] - 5 * delta);
            if (noteVelocities[key] == 0) {
                PulseManager.MidiRemoveNote(noteIds[key]);
                noteStates.Remove(key); // Turn note off
                noteVelocities.Remove(key); // Optionally remove velocity tracking
                noteIds.Remove(key);
            }
        }
    }
}