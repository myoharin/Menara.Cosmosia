using Godot;
using System;
using System.IO;
using SineVita.Muguet.Asteraceae;
using SineVita.Muguet.Asteraceae.Cosmosia;
using System.Collections.Generic;
using System.Reflection;
using SineVita.Muguet;
using Godot.NativeInterop;
using SineVita.Basil.Muguet;

namespace Menara.Cosmosia {
	public partial class menara_script : Node 
	// ok this can only inherity node, but not Menera, gotta figure this out, well it can just aggrate it so :shrug:
	{
		// basic file paths information
		public string ResonatorParameterPath = Path.Combine("assets","json","resonator-parameters");

		public List<int> ResonatorIDs = new List<int>() {};
		public ResonatorCosmosia Resonator { get { return _resonator; } }
		private ResonatorCosmosia _resonator;
		
		[Export] // necessary node control
		public Sprite2D ManaMetre;
		[Export]
		public Sprite2D MandalaVisualizer;
		[Export]
		public float TimeScale = 1.0f;

		// * Ready
		public override void _Ready() {
			ResonanceHelper.ParametersFolderPath = ResonatorParameterPath; // replace static 
			_resonator = new ResonatorCosmosia(244215);
			BasilMuguetCosmosia.DebugFolder = null; // set to null to disable debug
		}

		// * Processes
		public override void _Process(double delta) {
			_process_UpdateMuguet(delta * TimeScale);
			_process_UpdateMandala(delta);	
			_process_UpdateManaMetre(delta);
			BasilMuguetCosmosia.LogResonatorIdyll(_resonator);
		}
		private void _process_UpdateMuguet(double delta) {
			if (delta > 0) {_resonator.Process(delta);}
		}
		private void _process_UpdateMandala(double delta) {
			var param = new Godot.Collections.Array(){0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
			foreach (CosmosiaPulse pulse in _resonator.Lonicera.Nodes) {
				if (pulse.PulseID != _resonator.Lonicera.Nodes[0].PulseID) {
					int index = (int)HarmonyHelper.HtzToMidi(pulse.Pitch.Frequency) % 12;
					if (index % 2 == 0) {index = (index + 6) % 12;}
					if (index == 0) {index = 12;}
					param[index] = index;
				}
			}
			Variant paramV = Variant.CreateFrom(param);
			((ShaderMaterial)MandalaVisualizer.Material).SetShaderParameter("activeMasks", paramV);
		}
		private void _process_UpdateManaMetre(double delta) {
			var percentage = Resonator.Resonance / Resonator.Parameter.MaxIdyllAmount;
			((ShaderMaterial)ManaMetre.Material).SetShaderParameter("fillPercentage", percentage);
		}
		
		public void _on_timer_timeout() {
			_time_display();
		}
		private void _time_display() {
			var message = "";
			message += $"\nResonance: {Math.Round(Resonator.Resonance,2)} | Inflow: {Math.Round(Resonator.NetInflow,2)} | Outflow: {Math.Round(Resonator.NetOutflow,2)} | Overflow: {Math.Round(Resonator.NetOverflow,2)}";
			foreach (CosmosiaPulse pulse in Resonator.Pulses) {
				message += $"\n- {pulse.Pitch.NoteName} | Intensity: {pulse.Intensity}";
				BasilMuguetCosmosia.LogPulse(pulse);
			}
			message += "\n";
			foreach (CosmosiaChannel channel in Resonator.Channels) {
				message += $"\n- {channel.Interval.IntervalName} | Intensity: {channel.Intensity} | Inflow: {Math.Round(channel.RawInflowRate,2)} | Outflow: {Math.Round(channel.OutflowRate,2)} | Oveflow: {Math.Round(channel.OverflowRate,2)}";
			}
			message += $"\n{float.IsNaN(Resonator.Resonance)} | {float.IsNaN(Resonator.NetInflow)} | {float.IsNaN(Resonator.NetOutflow)} | {float.IsNaN(Resonator.NetOverflow)}";
			GD.Print(message);
			BasilMuguet.Log(message);
		}

		// * ResonatorCosmosia Stuff
		public void AddPulse(Pulse pulse) {
			var success = _resonator.AddPulse(pulse); 
			var message = $"add success {success} - {pulse.PulseID}";
			GD.Print(message);
			BasilMuguet.Log(message);
			BasilMuguetCosmosia.LogPulse(pulse);
		}
		public void DeletePulse(int id) {
			var success = _resonator.DeletePulse(id); 
			var message = $"delete success {success} - {id}";
			GD.Print(message);
			BasilMuguet.Log(message);
		}
		public void MutatePulse(int id, Pulse newPulse) {var success = _resonator.MutatePulse(id, newPulse);}// GD.Print($"mutate success {success}");}
	}
}

