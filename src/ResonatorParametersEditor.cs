using Godot;
using System;
using SineVita.Muguet;

public partial class ResonatorParametersEditor : Control
{
	[Export]
	public int CurrentParameter;
	[Export]
	public menara_script Menara;
	// exported text box SO MANY
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	//Signals
	public void _on_apply_changes_button_down() {
		// validate data on all em
		// write to file
		// tell Menera to reinstantiate the resonator properties
	}
	public void _on_revert_changes_button_down() {
		// read file
		// write resonator parameter
	}
}
