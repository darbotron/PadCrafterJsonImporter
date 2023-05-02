using System;
using System.IO;
using System.Reflection;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.darbotron.padcrafterjsonimporter
{
	public class PadCrafterJsonImporter : EditorWindow
	{
		private const string k_PathInPackageFolderOfUiXMLAsset       = "com.darbotron.padcrafterjsonimporter/Editor/UIBuilder/PadCrafterJsonImporterUI.uxml";
		private const string k_JsonForEmptyInputActionAsset          = @"{""name"": ""EmptyInputActionAsset"",""maps"": [],""controlSchemes"": []}";
		private const string k_Path_DefaultGeneratedInputActionAsset = "Assets/InputActionAssetFromPadCrafterJson.inputactions";

		private static readonly string k_PrefsKey_FileLocation_OutputAsset = $"{nameof(k_PrefsKey_FileLocation_OutputAsset)}";


		//------------------------------------------------------------------------
		[MenuItem( "Darbotron/PadCrafter Json Importer" )]
		public static void OpenEditorWindow()
		{
			EditorWindow.GetWindow< PadCrafterJsonImporter >( utility: true, "PadCrafter Json Importer", focus: true );
		}

		[SerializeField] private PadCrafterContainer m_padCrafterContainer = null;


		//------------------------------------------------------------------------
		private static string GetPathForUnityUiXml()
		{
			var assetGuidArray = AssetDatabase.FindAssets( "PadCrafterJsonImporterUI" );
			var assetPathArray = new string[ assetGuidArray.Length ];

			string packagePathUiXml = String.Empty;
			string lastFoundUiXml = String.Empty;

			for( int i = 0; i < assetGuidArray.Length; ++i )
			{
				assetPathArray[ i ] = AssetDatabase.GUIDToAssetPath( assetGuidArray[ i ] );

				if( assetPathArray[ i ].Contains( "Packages/" ) )
				{
					packagePathUiXml = assetPathArray[ i ];
					break;
				}

				lastFoundUiXml = assetPathArray[ i ];
			}

			return string.IsNullOrEmpty( packagePathUiXml ) ? lastFoundUiXml : packagePathUiXml;
		}


		//------------------------------------------------------------------------
		public void CreateGUI()
		{
			var visualTree = AssetDatabase.LoadAssetAtPath< VisualTreeAsset >( GetPathForUnityUiXml() );
			visualTree.CloneTree( rootVisualElement );

			{
				if( rootVisualElement.TryGetChildVisualElementWithAssert( "ButtonImportJsonFromClipboard", out Button button ) )
				{
					button.clickable.clicked += OnButton_ImportDataFromClipboard;
				}
			}
			{
				if( rootVisualElement.TryGetChildVisualElementWithAssert( "ButtonExportJsonToClipboard", out Button button ) )
				{
					button.clickable.clicked += OnButton_ExportImportedDataJsonToClipboard;
				}
			}
			{
				if( rootVisualElement.TryGetChildVisualElementWithAssert( "ButtonResetImportedData", out Button button ) )
				{
					button.clickable.clicked += OnButton_ResetImportedDataToDefault;
				}
			}
			{
				if( rootVisualElement.TryGetChildVisualElementWithAssert( "ButtonGenerateInputActionAsset", out m_buttonGenerateAsset ) )
				{
					m_buttonGenerateAsset.clickable.clicked += OnButton_GenerateAsset;
				}
			}

			rootVisualElement.TryGetChildVisualElementWithAssert( "ImportedPadCrafterJson", out m_uiTextField );
			rootVisualElement.TryGetChildVisualElementWithAssert( "InfoBoxParent",          out m_groupBoxInfoBoxParent );

			m_errorBox = new HelpBox( "not set", HelpBoxMessageType.Error );
			m_infoBox  = new HelpBox( "not set", HelpBoxMessageType.Info );

			EnsureWindowIsInitialised();
		}

		//------------------------------------------------------------------------
		private void OnButton_ImportDataFromClipboard()
		{
			try
			{
				if( TryValidateJsonFromClipboard( out var validatedJsonFromClipBoard ) )
				{
					EditorJsonUtility.FromJsonOverwrite( validatedJsonFromClipBoard, m_padCrafterContainer );
					m_uiTextField.value = EditorJsonUtility.ToJson( m_padCrafterContainer, prettyPrint: true );
					ErrorBoxHide();
					InfoBoxShow( "Json Imported Ok" );
				}
				else
				{
					OnButton_ResetImportedDataToDefault();
					ErrorBoxShow(	$"string in clipboard appears to be invalid PadCrafter json\nText from Clipboard:\n\n" +
									$"{GUIUtility.systemCopyBuffer}" );
					InfoBoxShow( "Json was reset to default" );
				}
			}
			catch( Exception ex )
			{
				OnButton_ResetImportedDataToDefault();
				ErrorBoxShow( $"deserialise PadCrafter from clipboard failed:\nException: {ex.Message}\nText from Clipboard:\n\n{GUIUtility.systemCopyBuffer}" );
				InfoBoxShow( "Json was reset to default" );
			}

		}

		//------------------------------------------------------------------------
		private void OnButton_ExportImportedDataJsonToClipboard()
		{
			var defaultPadCrafterJson = EditorJsonUtility.ToJson( m_padCrafterContainer, prettyPrint: true );
			GUIUtility.systemCopyBuffer = defaultPadCrafterJson;
		}

		//------------------------------------------------------------------------
		private void OnButton_ResetImportedDataToDefault()
		{
			m_padCrafterContainer = PadCrafterContainer.CreateDefault();
			m_uiTextField.value   = EditorJsonUtility.ToJson( m_padCrafterContainer, prettyPrint: true );
		}

		//------------------------------------------------------------------------
		private void EnsureWindowIsInitialised()
		{
			if( null == m_padCrafterContainer )
			{
				OnButton_ResetImportedDataToDefault();
			}
		}

		//------------------------------------------------------------------------
		private bool TryValidateJsonFromClipboard( out string outPossiblyValidjsonStringClipboard )
		{
			// 3 'match groups, (each group is a sub-part of a regex bounded by brackets).
			//
			// note: regex match group index 0 is the entire string.. the "(...)" group matches in a regex are indexed from 1
			//
			// $1 - validates json has "PadCrafter" in the expected place
			// $2 - captures the (unquoted) name of the ControllerScheme array
			// $3 - captures the trailing quote of the ControllerScheme array name and the start of the json array
			const string k_PadCrafterJsonValidationRegEx  = "(\"PadCrafter\":\\s*\\{[.\\n]*\\s*\")(\\w*)(\":\\s*\\[)";
			const string k_JsonControllerSchemesArrayName = "controllerSchemes";

			var clipboardString = GUIUtility.systemCopyBuffer;

			var validationRegEx = new System.Text.RegularExpressions.Regex( pattern: k_PadCrafterJsonValidationRegEx );
			var validationMatches = validationRegEx.Match( clipboardString );

			// fails validation: pass through string but fail validation
			if( ! validationMatches.Success )
			{
				outPossiblyValidjsonStringClipboard = clipboardString;
				return false;
			}

			// check for name of ControllerScheme array
			// * original website json had an empty string here
			// * Unity json deserialiswe needs it to be a named member we used "controllerSchemes"

			// $2 - captures the (unquoted) name of the ControllerScheme array
			//
			// if this matches we can pass through the unmodified json string
			if( validationMatches.Groups[ 2 ].Value == k_JsonControllerSchemesArrayName )
			{
				outPossiblyValidjsonStringClipboard = clipboardString;
				return true;
			}

			// we know the regex matched so we replace the empty / incorrect array name with the correct array name
			outPossiblyValidjsonStringClipboard = validationRegEx.Replace( clipboardString, $"$1{k_JsonControllerSchemesArrayName}$3" );
			return true;
		}

		//------------------------------------------------------------------------
		private void InfoBoxShow( string infoMessage )
		{
			m_infoBox.text = infoMessage;
			m_groupBoxInfoBoxParent.Add( m_infoBox );
		}

		//------------------------------------------------------------------------
		private void InfoBoxHide()
		{
			m_infoBox.text = String.Empty;
			m_infoBox.RemoveFromHierarchy();
		}

		//------------------------------------------------------------------------
		private void ErrorBoxShow( string errorMessage )
		{
			m_errorBox.text = errorMessage;
			m_groupBoxInfoBoxParent.Add( m_errorBox );
		}

		//------------------------------------------------------------------------
		private void ErrorBoxHide()
		{
			m_errorBox.text = String.Empty;
			m_errorBox.RemoveFromHierarchy();
		}

		//------------------------------------------------------------------------
		private void OnButton_GenerateAsset()
		{
			//----------------------------------------------------------------------------------------------------------
			#region reflection magic

			const string k_TypeName_InputActionSerializationHelpers = "InputActionSerializationHelpers";
			const string k_TypeName_InputActionAssetManager         = "InputActionAssetManager";
			const string k_TypeName_InputActionImporter				= "InputActionImporter";

			var tidInputActionAssetManager         = default( Type );
			var tidInputActionSerializationHelpers = default( Type );
			var tidInputActionImporter             = default( Type );

			foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
			{
				if( assembly.GetName().Name == "Unity.InputSystem" )
				{
					foreach( var type in assembly.GetTypes() )
					{
						if( type.Name == k_TypeName_InputActionAssetManager )
						{
							Debug.Log( $"found:{type.FullName}" );
							tidInputActionAssetManager = type;
						}

						if( type.Name == k_TypeName_InputActionSerializationHelpers )
						{
							Debug.Log( $"found:{type.FullName}" );
							tidInputActionSerializationHelpers = type;
						}

						if( type.Name == k_TypeName_InputActionImporter )
						{
							Debug.Log( $"found:{type.FullName}" );
							tidInputActionImporter = type;
						}
					}
				}
			}

			if(		CheckForErrorConditionAndShowPopUpWithMessage( ( default( Type ) == tidInputActionAssetManager ),         $"failed to find class {k_TypeName_InputActionAssetManager} in loaded assemblies" )
				||	CheckForErrorConditionAndShowPopUpWithMessage( ( default( Type ) == tidInputActionSerializationHelpers ), $"failed to find class {k_TypeName_InputActionSerializationHelpers} in loaded assemblies" )
				||	CheckForErrorConditionAndShowPopUpWithMessage( ( default( Type ) == tidInputActionImporter ),             $"failed to find class {k_TypeName_InputActionImporter} in loaded assemblies" ) )
			{
				return;
			}

			// get values / functions etc by reflection
			var inputActionAssetIconPath = tidInputActionImporter.GetField( "kAssetIcon", BindingFlags.Static | BindingFlags.NonPublic ).GetValue( null ) as string;

			var minfoSerializationHelper_AddActionMap = tidInputActionSerializationHelpers.GetMethod( "AddActionMap", BindingFlags.Static | BindingFlags.Public );
			var minfoSerializationHelper_AddAction    = tidInputActionSerializationHelpers.GetMethod( "AddAction",    BindingFlags.Static | BindingFlags.Public );
			var minfoSerializationHelper_AddBinding   = tidInputActionSerializationHelpers.GetMethod( "AddBinding",   BindingFlags.Static | BindingFlags.Public );

			#endregion reflection magic
			//----------------------------------------------------------------------------------------------------------


			// local function using reflection to add an action map to a serialized InputActionAsset
			SerializedProperty AddActionMap( SerializedObject serializedInputActionAsset, string nameOfActionMap )
			{
				// note: need to pass Type.Missing for optional params in reflection
				var newActionMapAsProperty = minfoSerializationHelper_AddActionMap.Invoke( null, new object[]{ serializedInputActionAsset, Type.Missing } ) as SerializedProperty;

				newActionMapAsProperty.FindPropertyRelative( "m_Name" ).stringValue = nameOfActionMap;

				return newActionMapAsProperty;
			}

			// local function using reflection to add an action to a serialized ActionMap of a serialized InputActionAsset
			SerializedProperty AddAction( SerializedProperty serializedActionMap, string nameOfAction, InputActionType inputActionType )
			{
				// note: need to pass Type.Missing for optional params in reflection
				var newActionAsProperty = minfoSerializationHelper_AddAction.Invoke( null, new object[]{ serializedActionMap, Type.Missing } ) as SerializedProperty;

				newActionAsProperty.FindPropertyRelative( "m_Name" ).stringValue = nameOfAction;
				newActionAsProperty.FindPropertyRelative("m_Type").intValue      = (int)inputActionType;
				return newActionAsProperty;
			}

			// local function using reflection to add a binding to an action on an action map of a serialized InputActionAsset
			SerializedProperty AddBinding( SerializedProperty serializedAction, SerializedProperty serialisedActionMap, string bindingPath )
			{
				var arrayOfAddBindingParams = new object[]
				{
					serializedAction,    // SerializedProperty actionProperty,
					serialisedActionMap, // SerializedProperty actionMapProperty = null,
					Type.Missing,        // SerializedProperty afterBinding = null,
					Type.Missing,        // string groups = "",
					bindingPath,         // string path = "",
					Type.Missing,        // string name = "",
					Type.Missing,        // string interactions = "",
					Type.Missing,        // string processors = "",
					Type.Missing         // InputBinding.Flags flags = InputBinding.Flags.None
				};

				var newActionAsProperty = minfoSerializationHelper_AddBinding.Invoke( null, arrayOfAddBindingParams ) as SerializedProperty;

				return newActionAsProperty;
			}


			// query user for save path - early out on error
			var previouslyStoredAssetPath = GetLastSavePathFromPlayerPrefs();
			var previousAssetDirectory    = Path.GetDirectoryName( previouslyStoredAssetPath );
			var previousAssetFilename     = Path.GetFileNameWithoutExtension( previouslyStoredAssetPath );
			var previousAssetExtension    = Path.GetExtension( previouslyStoredAssetPath ).Replace( ".", "" );

			var assetOutputPathFullPath = EditorUtility.SaveFilePanel
			(
				title: "Save Generated InputActionAsset",
				directory: previousAssetDirectory,
				defaultName: previousAssetFilename,
				extension: previousAssetExtension
			);

			if( string.IsNullOrEmpty( assetOutputPathFullPath ) )
			{
				return;
			}

			// ensure the folder exists
			try
			{
				var outputDirectory = Path.GetDirectoryName( assetOutputPathFullPath );
				if( ! Directory.Exists( outputDirectory ) )
				{
					Directory.CreateDirectory( outputDirectory );
				}
			}
			catch( Exception ex )
			{
				CheckForErrorConditionAndShowPopUpWithMessage( errorCondition: true, $"failed to create output directory: {assetOutputPathFullPath} - exception: {ex.Message}" );
				return;
			}

			// convert the path into one relative to the project folder and save the path in playerprefs
			var projectFolderPath            = Utility.GetProjectFolderPath();
			var assetPathForInputActionAsset = Utility.GetPathRelativeTo( assetOutputPathFullPath, projectFolderPath );

			assetPathForInputActionAsset = Path.ChangeExtension( assetPathForInputActionAsset, UnityEngine.InputSystem.InputActionAsset.Extension );

			CacheLastSavePathToPlayerPrefs( assetPathForInputActionAsset );


			// create an empty asset to modify
			var emptyInputActionAsset  = UnityEngine.InputSystem.InputActionAsset.FromJson( k_JsonForEmptyInputActionAsset );
			var inputActionsSerialised = new SerializedObject( emptyInputActionAsset );


			// iterate the imported json and use it to modify the (initially) empty InputActionAsset
			{
				var dictPadCrafterButtonNameToInputSystemBindingPath	= Buttons.GetLookUpPadCrafterButtonFieldNameToActionBindingPath();
				var dictPadCrafterButtonNameToInputActionType			= Buttons.GetLookUpPadCrafterButtonFieldNameToActionType();

				// local function to get unique string for any missing strings
				int uniqueNumberForMissingStrings = 0;
				string GetUniqueMissingString() => $"MissingName_{uniqueNumberForMissingStrings:00}";

				foreach( var controllerScheme in m_padCrafterContainer.PadCrafter.controllerSchemes )
				{
					// action maps MUST have names
					var actionMapName = string.IsNullOrEmpty( controllerScheme.templatename ) ? GetUniqueMissingString() : controllerScheme.templatename;

					var newMapAsSerializedProperty = AddActionMap( inputActionsSerialised, actionMapName );

					foreach( var fieldInfo in typeof(Buttons).GetFields( BindingFlags.Instance | BindingFlags.Public ) )
					{
						if(		dictPadCrafterButtonNameToInputSystemBindingPath.	TryGetValue( fieldInfo.Name, out var bindingPathForButton )
							&&  dictPadCrafterButtonNameToInputActionType.			TryGetValue( fieldInfo.Name, out var inputActionType ) )
						{
							// only add actions for PadCrafter buttons with strings
							var padCrafterButtonDescriptorString = fieldInfo.GetValue( controllerScheme.buttons ) as string;

							if( string.IsNullOrEmpty( padCrafterButtonDescriptorString ) )
							{
								continue;
							}

							var newActionAsSerializedProperty = AddAction( newMapAsSerializedProperty, padCrafterButtonDescriptorString, inputActionType );

							AddBinding( newActionAsSerializedProperty, newMapAsSerializedProperty, bindingPathForButton );
						}
					}
				}
			}

			inputActionsSerialised.ApplyModifiedProperties();


			// Looked at the InputSystem Package code to see how to do this
			try
			{
				AssetDatabase.Refresh();

				ProjectWindowUtil.CreateAssetWithContent
				(
					assetPathForInputActionAsset,
					emptyInputActionAsset.ToJson(),
					(Texture2D) EditorGUIUtility.Load( inputActionAssetIconPath )
				);

			}
			catch( Exception exception )
			{
				CheckForErrorConditionAndShowPopUpWithMessage( errorCondition: true, $"writing the asset to file failed: {exception.Message}" );
			}

			AssetDatabase.Refresh();
		}


		//------------------------------------------------------------------------
		private static string GetPlayerPrefsKey()                           => $"{typeof( PadCrafterJsonImporter ).FullName}.{Application.productName}.{k_PrefsKey_FileLocation_OutputAsset}";
		private static string GetLastSavePathFromPlayerPrefs()              => PlayerPrefs.GetString( GetPlayerPrefsKey(), k_Path_DefaultGeneratedInputActionAsset );
		private static void   CacheLastSavePathToPlayerPrefs( string path ) => PlayerPrefs.GetString( GetPlayerPrefsKey(), path );


		//------------------------------------------------------------------------
		bool CheckForErrorConditionAndShowPopUpWithMessage( bool errorCondition, string messageIfErrorConditionIsMet )
		{
			if( errorCondition )
			{
				EditorUtility.DisplayDialog( "Error", messageIfErrorConditionIsMet, "Ok" );
				return true;
			}
			return false;
		}


		private TextField m_uiTextField           = null;
		private GroupBox  m_groupBoxInfoBoxParent = null;
		private HelpBox   m_errorBox              = null;
		private HelpBox   m_infoBox               = null;
		private Button    m_buttonGenerateAsset   = null;
	}
}