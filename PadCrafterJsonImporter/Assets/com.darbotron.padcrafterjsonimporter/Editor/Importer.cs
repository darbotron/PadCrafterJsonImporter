using System;

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace com.darbotron.padcrafterjsonimporter
{
	public class PadCrafterJsonImporter : EditorWindow
	{
		private static readonly string k_PrefsKey_FileLocation_OutputAsset = $"{typeof( PadCrafterJsonImporter ).FullName}.{nameof(k_PrefsKey_FileLocation_OutputAsset)}";
		private static readonly string k_PrefsKey_FileLocation_InputJson   = $"{typeof( PadCrafterJsonImporter ).FullName}.{nameof(k_PrefsKey_FileLocation_InputJson)}";

		//------------------------------------------------------------------------
		[MenuItem( "Darbotron/PadCrafter Json Importer" )]
		public static void OpenEditorWindow()
		{
			EditorWindow.GetWindow< PadCrafterJsonImporter >( utility: true, "PadCrafter Json Importer", focus: true );
		}

		[SerializeField] private PadCrafterContainer m_padCrafterContainer = null;


		//------------------------------------------------------------------------
		public void CreateGUI()
		{
			var visualTree = AssetDatabase.LoadAssetAtPath< VisualTreeAsset >( "Assets/com.darbotron.padcrafterjsonimporter/Editor/UIBuilder/PadCrafterJsonImporterUI.uxml" );
			visualTree.CloneTree( rootVisualElement );

			{
				var button = rootVisualElement.Q< Button >( "ButtonImportJsonFromClipboard" );
				button.clickable.clicked += OnButton_ImportDataFromClipboard;
			}
			{
				var button = rootVisualElement.Q< Button >( "ButtonExportJsonToClipboard" );
				button.clickable.clicked += OnButton_ExportImportedDataJsonToClipboard;
			}
			{
				var button = rootVisualElement.Q< Button >( "ButtonResetImportedData" );
				button.clickable.clicked += OnButton_ResetImportedDataToDefault;
			}
			{
				var scrollView = rootVisualElement.Q< ScrollView >( "ImportedJsonScrollView" );
				m_jsonScrollView = scrollView;
			}
			{
				var textField = rootVisualElement.Q< TextField >( "ImportedPadCrafterJson" );
				m_uiTextField = textField;
			}

			m_errorBox = new HelpBox( "not set", HelpBoxMessageType.Error );

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
				}
				else
				{
					OnButton_ResetImportedDataToDefault();
					ErrorBoxShow( $"<color=red>string in clipboard appears to be invalid PadCrafter json</color>\n" );
					// show the unvalidated json for the user to see
					m_uiTextField.value = GUIUtility.systemCopyBuffer;
				}
			}
			catch( Exception ex )
			{
				OnButton_ResetImportedDataToDefault();
				ErrorBoxShow( $"<color=red>deserialise PadCrafter from clipboard failed:</color> {ex.Message}" );
				m_uiTextField.value = GUIUtility.systemCopyBuffer;
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
															//"(\"PadCrafter\":\\s*\\{[.\\n]*\\s*\")\\w*(\":\\s*\\[)";
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
		private void ErrorBoxShow( string errorMessage )
		{
			m_errorBox.text = errorMessage;
			m_jsonScrollView.Add( m_errorBox );
			m_errorBox.SendToBack();
		}

		//------------------------------------------------------------------------
		private void ErrorBoxHide()
		{
			m_errorBox.text = String.Empty;
			m_errorBox.RemoveFromHierarchy();
		}


		private Vector2    m_lastScrollPosition = Vector2.zero;
		private string     m_lastImportError    = string.Empty;
		private ScrollView m_jsonScrollView     = null;
		private TextField  m_uiTextField        = null;
		private HelpBox    m_errorBox           = null;
	}
}