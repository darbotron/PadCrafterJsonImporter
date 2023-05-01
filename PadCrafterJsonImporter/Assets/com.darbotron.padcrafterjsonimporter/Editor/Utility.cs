using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.darbotron.padcrafterjsonimporter
{
	public static class Utility
	{
		private static readonly Color k_DefaultLineColour      = new Color( ( 25f / 255f ), ( 25f / 255f ), ( 25f / 255f ), 1f );
		private const           float k_DefaultLineWidth       = 1f;
		private static readonly float k_DefaultVerticalPadding = EditorGUIUtility.singleLineHeight;
		private static readonly float k_DefaultSpaceBefore     = EditorGUIUtility.singleLineHeight * 0.5f;
		private static readonly float k_DefaultSpaceAfter      = EditorGUIUtility.singleLineHeight;

		//------------------------------------------------------------------------
		public static void GUIDrawSectionSeparator( string label ) => GUIDrawSectionSeparator( label, k_DefaultSpaceBefore, k_DefaultSpaceAfter );

		//------------------------------------------------------------------------
		public static void GUIDrawSectionSeparator( string label, float spaceBefore, float spaceAfter )
		{
			if( spaceBefore != 0f ) { EditorGUILayout.Space( spaceBefore ); }

			HorizontalLine();

			if( ! string.IsNullOrEmpty( label ) ) { EditorGUILayout.LabelField( label ); }

			if( spaceAfter != 0f ) { EditorGUILayout.Space( spaceAfter ); }
		}

		//------------------------------------------------------------------------
		public static void HorizontalLine() => HorizontalLine( k_DefaultLineColour, k_DefaultLineWidth, k_DefaultVerticalPadding );

		//------------------------------------------------------------------------
		public static void HorizontalLine( Color colour, float lineThickness, float verticalPadding )
		{
			// GetControlRect reserves a rect in the GUILayout
			var lineRect = EditorGUILayout.GetControlRect( GUILayout.Height( lineThickness + verticalPadding ) );
			lineRect.y      += ( verticalPadding / 2f );
			lineRect.height =  lineThickness;
			EditorGUI.DrawRect( lineRect, colour );
		}

		//------------------------------------------------------------------------
		public static bool TryGetChildVisualElement< T >( this VisualElement rootElement, string nameOfChildElement, out T outChildElement ) where T : VisualElement
		{
			outChildElement = rootElement.Q< T >( nameOfChildElement );
			return ( null != outChildElement );
		}

		//------------------------------------------------------------------------
		public static bool TryGetChildVisualElementWithAssert< T >( this VisualElement rootElement, string nameOfChildElement, out T outChildElement, string assertMessageOnFailure ) where T : VisualElement
		{
			if( ! rootElement.TryGetChildVisualElement( nameOfChildElement, out outChildElement ) )
			{
				Debug.Assert( false, $"failed to find child of {rootElement.name} of type {typeof( T ).Name} called '{nameOfChildElement}'{(( ! string.IsNullOrEmpty( assertMessageOnFailure ) ) ? "" : $": {assertMessageOnFailure}" )}" );
				return false;
			}

			return true;
		}

		public static bool TryGetChildVisualElementWithAssert< T >( this VisualElement rootElement, string nameOfChildElement, out T outChildElement ) where T : VisualElement
		 => TryGetChildVisualElementWithAssert( rootElement, nameOfChildElement, out outChildElement, assertMessageOnFailure: string.Empty );

		//------------------------------------------------------------------------
		public static string GetAssetFolderPath() => Application.dataPath;

		//------------------------------------------------------------------------
		public static string GetProjectFolderPath()
		{
			var assetFolderPath = GetAssetFolderPath();
			var projectPath = System.IO.Path.GetDirectoryName( assetFolderPath ).Replace( "\\", "/" );
			return projectPath;
		}

			//------------------------------------------------------------------------
		public static string GetPathRelativeTo( string absolutePathToMakeRelative, string absolutePathOfRelativePathSource )
		{
			// validate params - absolutePathOfRelativePathSource needs a trailing "/"
			if(		( absolutePathOfRelativePathSource.Length > 0 )
				&&	( absolutePathOfRelativePathSource[ absolutePathOfRelativePathSource.Length - 1 ] != '/' ) )
			{
				absolutePathOfRelativePathSource += "/";
			}

			var uriAssetFolder  = new Uri( absolutePathOfRelativePathSource );
			var uriAbsolutePath = new Uri( absolutePathToMakeRelative );
			var relativeUri     = uriAssetFolder.MakeRelativeUri( uriAbsolutePath );
			var relativePath    = Uri.UnescapeDataString( relativeUri.ToString() );

			return relativePath;
		}

	}
}