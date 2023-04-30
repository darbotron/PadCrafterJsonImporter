using UnityEngine;
using UnityEditor;

namespace com.darbotron.padcrafterjsonimporter
{
	public static class GuiUtils
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
	}
}