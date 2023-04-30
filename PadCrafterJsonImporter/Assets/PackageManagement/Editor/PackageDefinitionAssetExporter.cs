//////////////////////////////////////////////////////////////////////////////
// File content is © 2023 Darbotron Ltd
//
// This source file is part of the Darbotron Common UPM package.
//
// Distributed under the MIT license see file LICENSE in repository root, or
// this URL https://opensource.org/license/mit/
//////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

using UnityEngine;

using PixelWizards.PackageUtil;

namespace Darbotron.Common.Packages
{
	public static class PackageModelAssetExporter
	{
		/// <summary>
		/// returns empty string on success, or error message on failure
		/// </summary>
		public static string ExportPackage( PackageDefinitionAsset packageModelAsset )
		{
			var packageSourceFolder       = PathUtils.GetAbsolutePathFromPathRelativeToProjectFolder( packageModelAsset.PackageSourceFolder );
			var packageOutputFolder       = Path.Combine( PathUtils.GetAbsolutePathFromPathRelativeToProjectFolder( packageModelAsset.DestinationPackageParentFolder ), packageModelAsset.m_packageUtilsModel.name ).Replace( Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar );
			var packageOutputFolderParent = PathUtils.GetAbsolutePathFromPathRelativeToProjectFolder( packageModelAsset.DestinationPackageParentFolder ).Replace( Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar );

			if( packageModelAsset.OverwriteExistingDestinationOnExport && Directory.Exists( packageOutputFolder ) )
			{
				Directory.Delete( packageOutputFolder, true );
			}

			packageModelAsset.m_packageUtilsModel.ValidateSamplePaths();

			packageModelAsset.m_packageUtilsModel.PreExport();

			{
				var preExportErrorMessage = packageModelAsset.VOnPreExport( packageModelAsset, packageSourceFolder, packageOutputFolder, packageOutputFolderParent );
				if( ! string.IsNullOrEmpty( preExportErrorMessage ) )
				{
					return$"failed on pre export step: {preExportErrorMessage}";
				}
			}

			PackageUtilController.Init();
			PackageUtilController.Model                  = packageModelAsset.m_packageUtilsModel;
			PackageUtilController.packageSourcePath      = packageSourceFolder;
			PackageUtilController.packageDestinationPath = packageOutputFolderParent;

			PackageUtilController.ExportPackage();

			Debug.Log( PackageUtilController.outputLog.ToString() );

			{
				var postExportErrorMessage = packageModelAsset.VOnPostExport( packageModelAsset, packageSourceFolder, packageOutputFolder, packageOutputFolderParent );
				if( ! string.IsNullOrEmpty( postExportErrorMessage ) )
				{
					return$"failed on post export step: {postExportErrorMessage}";
				}
			}

			packageModelAsset.m_packageUtilsModel.PostExport();

			//
			// post-steps
			//

			// 1. hide the "Samples" folder (adds a '~' to the filename)
			HideTopLevelPackageFolder( packageOutputFolder, PackageUtilModelWithSamples.k_strFolder_Samples );

			// 2. hide an other folders we were asked to
			foreach( var topLevelFolderNameToHide in packageModelAsset.m_lstNamesOfTopLevelPackageFoldersToHideInPackage )
			{
				HideTopLevelPackageFolder( packageOutputFolder, topLevelFolderNameToHide );
			}

			return String.Empty;
		}

		private static void HideTopLevelPackageFolder( string packageOutputFolder, string topLevelFolderNameToHide )
		{
			var folderToHideFullPath = Path.Combine( packageOutputFolder, topLevelFolderNameToHide );
			if( Directory.Exists( folderToHideFullPath ) )
			{
				var hiddenFolderName = $"{folderToHideFullPath}~";
				Directory.Move( folderToHideFullPath, hiddenFolderName );
				File.Delete( $"{folderToHideFullPath}.meta" );
			}
		}
	}

}// namespace Darbotron.Common.Packages