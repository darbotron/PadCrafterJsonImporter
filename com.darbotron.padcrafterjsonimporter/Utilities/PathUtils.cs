//////////////////////////////////////////////////////////////////////////////
// File content is © 2023 Darbotron Ltd
//
// This source file is part of the Darbotron Common UPM package.
//
// Distributed under the MIT license see file LICENSE in repository root, or
// this URL https://opensource.org/license/mit/
//////////////////////////////////////////////////////////////////////////////


using System.IO;
using UnityEngine;

namespace Darbotron.Common
{
	public static class PathUtils
	{
		//------------------------------------------------------------------------
		public static string ConvertAbsolutePathToStartFromAssetFolder( string strAbsolutePath )
		{
			System.Uri uriInkLOCsFolderAbsolutePath    = new System.Uri( strAbsolutePath );
			System.Uri uriUnityAssetPath               = new System.Uri( Application.dataPath );
			System.Uri uriInkLOCsFoldeRelativeToAssets = uriUnityAssetPath.MakeRelativeUri( uriInkLOCsFolderAbsolutePath );

			return uriInkLOCsFoldeRelativeToAssets.ToString();
		}

		//------------------------------------------------------------------------
		// As Platform Layer is relocatable between projects we need some way to
		// locate the PL_3rdParty folder so that we can ensure any temporary or
		// dynamic files written by 3rd Party code end up in an appropriate place
		//------------------------------------------------------------------------
		public static string GetPathOfFolderInAssetsRoot( string strFolderName )
		{
			// in the editor this returns <project folder>/assets
			string   strAssetFolder          = Application.dataPath;
			string[] astrMatchingFolderPaths = Directory.GetDirectories( strAssetFolder, strFolderName, SearchOption.AllDirectories );
			Debug.Assert( ( 1 == astrMatchingFolderPaths.Length ), "Found more than 1 PL_3rdParty folder under <projectroot>/Assets!" );

			return astrMatchingFolderPaths[ 0 ];
		}

		//------------------------------------------------------------------------
		public static string GetProjectFolderPath() => Path.GetDirectoryName( Application.dataPath ) + "/";

		//------------------------------------------------------------------------
		public static string GetAbsolutePathFromPathRelativeToProjectFolder( string pathRelativeToProjectFolder )
		{
			var projectFolder                = Path.GetDirectoryName( GetProjectFolderPath() );
			var combinedPathWrtProjectFolder = Path.Combine( projectFolder, pathRelativeToProjectFolder );
			var absolutePath                 = Path.GetFullPath( combinedPathWrtProjectFolder );

			return absolutePath;
		}

		//------------------------------------------------------------------------
		public static string GetPathRelativeToProjectFolder( string absolutePath )
		{
			var uriProjectFolder = new System.Uri( GetProjectFolderPath() );
			var uriAbsolutePath  = new System.Uri( absolutePath );
			var relativeUri      = uriProjectFolder.MakeRelativeUri( uriAbsolutePath );
			var relativePath     = System.Uri.UnescapeDataString( relativeUri.ToString() );

			return relativePath;
		}

		//------------------------------------------------------------------------
		public static string GetAssetFolderPath() => Application.dataPath;

		//------------------------------------------------------------------------
		public static string GetAbsolutePathFromPathRelativeToAssetFolder( string pathRelativeToAssetFolder )
		{
			var assetFolder                = Path.GetDirectoryName( GetAssetFolderPath() );
			var combinedPathWrtAssetFolder = Path.Combine( assetFolder, pathRelativeToAssetFolder );
			var absolutePath               = Path.GetFullPath( combinedPathWrtAssetFolder );

			return absolutePath;
		}

		//------------------------------------------------------------------------
		public static string GetPathRelativeToAssetFolder( string absolutePath )
		{
			var uriAssetFolder  = new System.Uri( GetAssetFolderPath() );
			var uriAbsolutePath = new System.Uri( absolutePath );
			var relativeUri     = uriAssetFolder.MakeRelativeUri( uriAbsolutePath );
			var relativePath    = System.Uri.UnescapeDataString( relativeUri.ToString() );

			return relativePath;
		}
	}

} //namespace Darbotron.Common