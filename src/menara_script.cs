using Godot;
using System;
using System.IO;
using SineVita.Muguet.Asteraceae;
using SineVita.Muguet.Asteraceae.Cosmosia;
using System.Collections.Generic;
using System.Reflection;
using SineVita.Muguet;
using Godot.NativeInterop;


public partial class menara_script : Node 
// ok this can only inherity node, but not Menera, gotta figure this out, well it can just aggrate it so :shrug:
{
	// basic file paths information
	public string ResonatorParameterPath = Path.Combine("assets","json","resonator-parameters");

	public List<int> ResonatorIDs = new List<int>() {};
	public ResonatorCosmosia Resonator { get { return _resonator; } }
	private ResonatorCosmosia _resonator;
	
	[Export] // necessary node control
	public Control FrontendManager;
	[Export]
	public Sprite2D MandalaVisualizer;

	// * Ready
	public override void _Ready() {
		ResonanceHelper.ParametersFolderPath = ResonatorParameterPath; // replace static
		_resonator = new ResonatorCosmosia(244215);
	}

	// * Processes
	public override void _Process(double delta) {
		_Process_Input(delta);
		_Process_UpdateMuguet(delta);
		var param = new Godot.Collections.Array(){0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		foreach (CosmosiaPulse pulse in _resonator.Lonicera.Nodes) {
			if (pulse.PulseID != _resonator.Lonicera.Nodes[0].PulseID) {
				int index = (int)HarmonyHelper.CalculateHtzToMidi(pulse.Pitch.Frequency) % 12;
				if (index % 2 == 0) {index = (index + 6) % 12;}
				if (index == 0) {index = 12;}
				param[index] = index;
			}
			
		}
		Variant paramV = Variant.CreateFrom(param);
		((ShaderMaterial)MandalaVisualizer.Material).SetShaderParameter("activeMasks", paramV);
	}
	private void _Process_Input(double delta) {}
	private void _Process_Display(double delta) {
		GD.Print($" - - {_resonator.Lonicera.NodeCount} nodes total - - ");
		foreach (CosmosiaPulse pulse in _resonator.Lonicera.Nodes) {
			GD.Print($"{pulse.PulseID} | {HarmonyHelper.ConvertHtzToNoteName(pulse.Pitch.Frequency)} | {pulse.Intensity}");
		}
		GD.Print(_resonator.Resonance);

	}
	private void _Process_UpdateMuguet(double delta) {_resonator.Process(delta);}

	public void _on_timer_timeout() {
		// _Process_Display(1.0d);
	}

	
	// * ResonatorCosmosia Stuff
	public void AddPulse(Pulse pulse) {var success = _resonator.AddPulse(pulse); GD.Print($"add success {success} - {pulse.PulseID}");}
	public void DeletePulse(int id) {var success = _resonator.DeletePulse(id); GD.Print($"delete success {success} - {id}");}
	public void MutatePulse(int id, Pulse newPulse) {var success = _resonator.MutatePulse(id, newPulse);}// GD.Print($"mutate success {success}");}
}
