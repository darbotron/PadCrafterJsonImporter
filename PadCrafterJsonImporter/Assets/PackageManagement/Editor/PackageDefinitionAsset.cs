//////////////////////////////////////////////////////////////////////////////
// File content is © 2023 Darbotron Ltd
//
// This source file is part of the Darbotron Common UPM package.
//
// Distributed under the MIT license see file LICENSE in repository root, or
// this URL https://opensource.org/license/mit/
//////////////////////////////////////////////////////////////////////////////

using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using PixelWizards.PackageUtil;

namespace Darbotron.Common.Packages
{
	// members correspond to spec here:
	// https://docs.unity3d.com/Manual/cus-samples.html
	[System.Serializable]
	public class SampleDefinition
	{
		public string displayName;
		public string description;
		public string path;
	}

	// extends PixelWizards.PackageUtil.PackageUtilModel to handle samples
	//
	// https://docs.unity3d.com/Manual/cus-samples.html
	[System.Serializable]
	public class PackageUtilModelWithSamples : PackageUtilModel
	{
		public const string k_strFolder_Samples         = "Samples";
		public const string k_strFolder_Package_Samples = "Samples~";

		public SampleDefinition[] samples;

		public void ValidateSamplePaths()
		{
			foreach( var sample in samples )
			{
				// get path with win separators & no leading '/'
				var pathSafeSeparators = sample.path.Replace( Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar );

				if( pathSafeSeparators[ 0 ] == Path.DirectorySeparatorChar )
				{
					pathSafeSeparators.Remove( 0, 1 );
				}

				var arrayOfFoldersInPath = pathSafeSeparators.Split( Path.DirectorySeparatorChar );
				var firstDirectory       = arrayOfFoldersInPath[ 0 ];

				if( firstDirectory != k_strFolder_Samples )
				{
					sample.path = $"{k_strFolder_Samples}{Path.DirectorySeparatorChar}{sample.path}";
				}
			}
		}

		public void PreExport()
		{
			foreach( var sample in samples )
			{
				sample.path = sample.path.Replace( k_strFolder_Samples, k_strFolder_Package_Samples );
			}
		}

		public void PostExport()
		{
			foreach( var sample in samples )
			{
				sample.path = sample.path.Replace( k_strFolder_Package_Samples, k_strFolder_Samples );
			}
		}
	}

	[CreateAssetMenu( menuName = "Package Tools/Package Definition", fileName = "PackageDef.asset" )]
	public class PackageDefinitionAsset : ScriptableObject
	{
		[Header( "Package Paths (relative to project root)" )]
		public string PackageSourceFolder                  = "Assets/Package";
		public string DestinationPackageParentFolder       = "../Packages";
		public bool   OverwriteExistingDestinationOnExport = true;

		public PackageUtilModelWithSamples m_packageUtilsModel                               = new PackageUtilModelWithSamples();
		public List< string >              m_lstNamesOfTopLevelPackageFoldersToHideInPackage = new List< string >();

		public virtual string VOnPreExport( PackageDefinitionAsset  packageModelAsset, string packageSourceFolder, string packageOutputFolder, string packageOutputFolderParent ) => string.Empty;
		public virtual string VOnPostExport( PackageDefinitionAsset packageModelAsset, string packageSourceFolder, string packageOutputFolder, string packageOutputFolderParent ) => string.Empty;
	}

	[CustomEditor( typeof(PackageDefinitionAsset), editorForChildClasses: true )]
	class PackageModelAssetInspector : UnityEditor.Editor
	{
		const string k_ChooseSourcePathPrompt = "Choose Package Source Path";
		const string k_ChooseDestPathPrompt   = "Choose Package Export Path (Parent Folder)";

		public override void OnInspectorGUI()
		{
			var packageDefinitionAsset = serializedObject.targetObject as PackageDefinitionAsset;

			serializedObject.Update();

			bool exportPackagePressed = false;

			using( new GUILayout.VerticalScope( EditorStyles.helpBox ) )
			{
				EditorGUILayout.LabelField( "Package Definition Actions", EditorStyles.largeLabel );

				EditorGUILayout.Space();

				using( new EditorGUILayout.HorizontalScope() )
				{
					if( GUILayout.Button( k_ChooseSourcePathPrompt ) )
					{
						var absolutePath = PathUtils.GetAbsolutePathFromPathRelativeToProjectFolder( packageDefinitionAsset.PackageSourceFolder );
						var newPath      = EditorUtility.OpenFolderPanel( k_ChooseSourcePathPrompt, Path.GetDirectoryName( absolutePath ), Path.GetFileName( packageDefinitionAsset.m_packageUtilsModel.name ) );

						if( ! string.IsNullOrEmpty( newPath ) )
						{
							var property        = serializedObject.FindProperty( nameof(PackageDefinitionAsset.PackageSourceFolder) );
							var newPathRelative = PathUtils.GetPathRelativeToProjectFolder( newPath );
							property.stringValue = newPathRelative;
						}
					}

					if( GUILayout.Button( k_ChooseDestPathPrompt ) )
					{
						var absolutePath = PathUtils.GetAbsolutePathFromPathRelativeToProjectFolder( packageDefinitionAsset.DestinationPackageParentFolder );
						var newPath      = EditorUtility.OpenFolderPanel( k_ChooseDestPathPrompt, Path.GetDirectoryName( absolutePath ), Path.GetFileName( absolutePath ) );

						if( ! string.IsNullOrEmpty( newPath ) )
						{
							var property        = serializedObject.FindProperty( nameof(PackageDefinitionAsset.DestinationPackageParentFolder) );
							var newPathRelative = PathUtils.GetPathRelativeToProjectFolder( newPath );
							property.stringValue = newPathRelative;
						}
					}
				}

				EditorGUILayout.Space();

				exportPackagePressed = GUILayout.Button( "Export Package" );
				EditorGUILayout.Space();
			}

			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();

			DrawDefaultInspector();

			if( exportPackagePressed )
			{
				var errorString = PackageModelAssetExporter.ExportPackage( packageDefinitionAsset );
				if( ! string.IsNullOrEmpty( errorString ) )
				{
					Debug.LogError( $"Export of {packageDefinitionAsset.name} failed: {errorString}" );
				}
			}
		}
	}
}// namespace PlatformLayer.Editor.Packagesnamespace Darbotron.Common.Packages