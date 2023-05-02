namespace com.darbotron.padcrafterjsonimporter
{
	using UnityEngine.InputSystem;

	[System.Serializable]
	public class Buttons
	{
		//
		// to see this on PadCrafter paste this URL into a browser ;D
		// https://www.padcrafter.com/index.php?templates=lookupTable&leftTrigger=leftTrigger&rightBumper=rightShoulder&rightTrigger=rightTrigger&leftStickClick=leftStickButton&aButton=buttonSouth&xButton=buttonWest&bButton=buttonEast&yButton=buttonNorth&rightStickClick=rightStickButton&backButton=selectButton&startButton=startButton&dpadUp=dpad%2Fup&dpadDown=dpad%2Fdown&dpadRight=dpad%2Fright&dpadLeft=dpad%2Fleft&plat=0&leftBumper=leftShoulder&col=%23242424%2C%23606A6E%2C%23FFFFFF&leftStick=leftStick&rightStick=rightStick#
		//
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


		public static System.Collections.Generic.Dictionary< string, string > GetLookUpPadCrafterButtonFieldNameToActionBindingPath()
		{
			return new System.Collections.Generic.Dictionary< string, string >
			{
				{ nameof( leftTrigger ),     "<Gamepad>/leftTrigger" },
				{ nameof( rightBumper ),     "<Gamepad>/rightShoulder" },
				{ nameof( rightTrigger ),    "<Gamepad>/rightTrigger" },
				{ nameof( leftStickClick ),  "<Gamepad>/leftStickButton" },
				{ nameof( aButton ),         "<Gamepad>/buttonSouth" },
				{ nameof( xButton ),         "<Gamepad>/buttonWest" },
				{ nameof( bButton ),         "<Gamepad>/buttonEast" },
				{ nameof( yButton ),         "<Gamepad>/buttonNorth" },
				{ nameof( rightStickClick ), "<Gamepad>/rightStickButton" },
				{ nameof( backButton ),      "<Gamepad>/selectButton" },
				{ nameof( startButton ),     "<Gamepad>/startButton" },
				{ nameof( dpadUp ),          "<Gamepad>/dpad/up" },
				{ nameof( dpadDown ),        "<Gamepad>/dpad/down" },
				{ nameof( dpadRight ),       "<Gamepad>/dpad/right" },
				{ nameof( dpadLeft ),        "<Gamepad>/dpad/left" },
				{ nameof( leftBumper ),      "<Gamepad>/leftShoulder" },
				{ nameof( leftStick ),       "<Gamepad>/leftStick" },
				{ nameof( rightStick ),      "<Gamepad>/rightStick" },
            };
		}

		public static System.Collections.Generic.Dictionary< string, InputActionType > GetLookUpPadCrafterButtonFieldNameToActionType()
		{
			return new System.Collections.Generic.Dictionary< string, InputActionType >
			{
				{ nameof( leftTrigger ),     InputActionType.Value },
				{ nameof( rightBumper ),     InputActionType.Button },
				{ nameof( rightTrigger ),    InputActionType.Value },
				{ nameof( leftStickClick ),  InputActionType.Button },
				{ nameof( aButton ),         InputActionType.Button },
				{ nameof( xButton ),         InputActionType.Button },
				{ nameof( bButton ),         InputActionType.Button },
				{ nameof( yButton ),         InputActionType.Button },
				{ nameof( rightStickClick ), InputActionType.Button },
				{ nameof( backButton ),      InputActionType.Button },
				{ nameof( startButton ),     InputActionType.Button },
				{ nameof( dpadUp ),          InputActionType.Button },
				{ nameof( dpadDown ),        InputActionType.Button },
				{ nameof( dpadRight ),       InputActionType.Button },
				{ nameof( dpadLeft ),        InputActionType.Button },
				{ nameof( leftBumper ),      InputActionType.Button },
				{ nameof( leftStick ),       InputActionType.Value },
				{ nameof( rightStick ),      InputActionType.Value },
			};
		}

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