namespace com.darbotron.padcrafterjsonimporter
{
	[System.Serializable]
	public class Buttons
	{
		public string leftTrigger;     // matches Unity "<Gamepad>/leftTrigger"
		public string rightBumper;     // matches Unity "<Gamepad>/rightShoulder"
		public string rightTrigger;    // matches Unity "<Gamepad>/rightTrigger"
		public string leftStickClick;  // matches Unity "<Gamepad>/leftStickButton"
		public string aButton;         // matches Unity "<Gamepad>/buttonSouth"
		public string xButton;         // matches Unity "<Gamepad>/buttonWest"
		public string bButton;         // matches Unity "<Gamepad>/buttonEast"
		public string yButton;         // matches Unity "<Gamepad>/buttonNorth"
		public string rightStickClick; // matches Unity "<Gamepad>/rightStickButton"
		public string backButton;      // matches Unity "<Gamepad>/selectButton"
		public string startButton;     // matches Unity "<Gamepad>/startButton"
		public string dpadUp;          // matches Unity "<Gamepad>/dpad/up"
		public string dpadDown;        // matches Unity "<Gamepad>/dpad/down"
		public string dpadRight;       // matches Unity "<Gamepad>/dpad/right"
		public string dpadLeft;        // matches Unity "<Gamepad>/dpad/left"
		public string leftBumper;      // matches Unity "<Gamepad>/leftShoulder"
		public string leftStick;       // matches Unity "<Gamepad>/leftStick"
		public string rightStick;      // matches Unity "<Gamepad>/rightStick
	}

	[System.Serializable]
	public class ControllerScheme
	{
		public string  templatename;
		public Buttons buttons;
	}

	[System.Serializable]
	public class PadCrafter
	{
		public ControllerScheme[] controllerSchemes;
	}

	[System.Serializable]
	public class PadCrafterContainer
	{
		public PadCrafter PadCrafter;

		public static PadCrafterContainer CreateDefault()
		{
			var defaultObject							= new PadCrafterContainer();
			defaultObject.PadCrafter					= new PadCrafter();
			defaultObject.PadCrafter.controllerSchemes	= new ControllerScheme[ 1 ] { new ControllerScheme() };
			return defaultObject;
		}
	}

}