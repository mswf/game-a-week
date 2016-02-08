/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@freakshowstudio.com>
 * \author http://www.freakshowstudio.com
 * \date © 2011-2015
 */

using UnityEngine;
using UnityEditor;

namespace Extensions
{
	namespace SelectParent
	{
		/// <summary>
		/// Editor class for enabling the Select Parent function
		/// </summary>
		public class SelectParent : Editor
		{
			/// <summary>
			/// Boolean to hold the current status for if we should
			/// select the parent of all AlwaysSelectMyParent objects.
			/// </summary>
			private static bool alwaysSelectParent = false;

			/// <summary>
			/// Function for selecting the parent transform
			/// </summary>
			[MenuItem("Edit/Select Parent &p", false, 200001)]
			private static void SelectParentMenuitem()
			{
				if (Selection.activeTransform != null &&
					Selection.activeTransform.parent != null)
				{
					Selection.activeTransform = Selection.activeTransform.parent;
				}
			}

			/// <summary>
			/// Function for selecting the root transform
			/// </summary>
			[MenuItem("Edit/Select Root &#p", false, 200002)]
			private static void SelectRootMenuItem()
			{
				if (Selection.activeTransform != null &&
					Selection.activeTransform.root != null)
				{
					Selection.activeTransform = Selection.activeTransform.root;
				}
			}

			/// <summary>
			/// Function for turning Always Select Parent On
			/// </summary>
			[MenuItem("Edit/Always Select Parent/On", false, 200003)]
			private static void EnableSelectParent()
			{
				if (!alwaysSelectParent)
				{
					EditorApplication.update += SelectParents;
				}
				alwaysSelectParent = true;
			}

			/// <summary>
			/// Validation Function for turning Always Select Parent On
			/// </summary>
			[MenuItem("Edit/Always Select Parent/On", true, 200003)]
			private static bool EnableSelectParentValidate()
			{
				return (!alwaysSelectParent);
			}

			/// <summary>
			/// Function for turning Always Select Parent Off
			/// </summary>
			[MenuItem("Edit/Always Select Parent/Off", false, 200004)]
			private static void DisableSelectParent()
			{
				if (alwaysSelectParent)
				{
					EditorApplication.update -= SelectParents;
				}
				alwaysSelectParent = false;
			}

			/// <summary>
			/// Validation Function for turning Always Select Parent Off
			/// </summary>
			[MenuItem("Edit/Always Select Parent/Off", true, 200004)]
			private static bool DisableSelectParentValidate()
			{
				return alwaysSelectParent;
			}

			/// <summary>
			/// Function to toggle the state of Always Select Parent (On/Off)
			/// </summary>
			[MenuItem("Edit/Always Select Parent/Toggle %&#p", false, 200005)]
			private static void ToggleSelectParent()
			{
				if (!alwaysSelectParent)
				{
					EnableSelectParent();
				}
				else
				{
					DisableSelectParent();
				}
			}

			/// <summary>
			/// Function that is added to the editors update callback.
			/// This will select the parent of the current transform if
			/// it has the AlwaysSelectMyParent component added.
			/// </summary>
			private static void SelectParents()
			{
				Transform t = Selection.activeTransform;
				if (t != null)
				{
					if (t.GetComponent<AlwaysSelectMyParent>() != null)
					{
						if (t.parent != null)
						{
							Selection.activeTransform = t.parent;
						}
					}
				}
			}
		}


	}


}
