//////////////////////////////////////////////////////////////////////////////
// File content is © 2023 Darbotron Ltd
//
// This source file is part of the Darbotron Common UPM package.
//
// Distributed under the MIT license see file LICENSE in repository root, or
// this URL https://opensource.org/license/mit/
//////////////////////////////////////////////////////////////////////////////

using UnityEditor;

namespace Darbotron.Common.Packages
{
	public static class MenuExportAllPackages
	{
		[MenuItem( "Darbotron/Packages/Export All" )]
		public static void Menu_ExportAllPackages() => ExportAllPackages();

		private static void ExportAllPackages()
		{
			var allPackageAssetGuids = AssetDatabase.FindAssets( $"t:{nameof(PackageDefinitionAsset)}" );

			foreach( var guid in allPackageAssetGuids )
			{
				var assetPath = AssetDatabase.GUIDToAssetPath( guid );
				var loadedAsset = AssetDatabase.LoadAssetAtPath< PackageDefinitionAsset >( assetPath );
				PackageModelAssetExporter.ExportPackage( loadedAsset );
			}
		}
	}

}// namespace Darbotron.Common.Packages