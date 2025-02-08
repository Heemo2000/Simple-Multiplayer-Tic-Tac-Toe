using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Game.UI;

namespace Game.Editor
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(Page))]
    public class PageEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            var animSpeedProp = serializedObject.FindProperty("animationSpeed");
            var exitOnNewPagePushProp = serializedObject.FindProperty("exitOnNewPagePush");

            root.Add(new PropertyField(animSpeedProp));
            root.Add(new PropertyField(exitOnNewPagePushProp));

            var entryModeProp = serializedObject.FindProperty("entryMode");
            var entryDirectionProp = serializedObject.FindProperty("entryDirection");
            var exitModeProp = serializedObject.FindProperty("exitMode");
            var exitDirectionProp = serializedObject.FindProperty("exitDirection");

            var entryModeEnumValues = System.Enum.GetValues(typeof(EntryMode));
            var directionEnumValues = System.Enum.GetValues(typeof(Direction));

            var entryModeEnumNames = System.Enum.GetNames(typeof(EntryMode));
            var directionEnumNames = System.Enum.GetNames(typeof(Direction));

            var entryModeDisplayOptions = new List<string>(entryModeEnumNames);
            var directionDisplayOptions = new List<string>(directionEnumNames);

            var entryModePopupField = new PopupField<string>("Entry Mode", entryModeDisplayOptions, entryModeProp.enumValueIndex);
            var entryDirectionPopupField = new PopupField<string>("Entry Direction", directionDisplayOptions, entryDirectionProp.enumValueIndex);
            var exitModePopupField = new PopupField<string>("Exit Mode", entryModeDisplayOptions, exitModeProp.enumValueIndex);
            var exitDirectionPopupField = new PopupField<string>("Exit Direction", directionDisplayOptions, exitDirectionProp.enumValueIndex);

            entryModePopupField.index = FindEntryModeIndex(entryModeProp.intValue);
            entryDirectionPopupField.index = FindDirectionIndex(entryDirectionProp.intValue);
            exitModePopupField.index = FindEntryModeIndex(exitModeProp.intValue);
            exitDirectionPopupField.index = FindDirectionIndex(exitDirectionProp.intValue);

            root.Add(entryModePopupField);
            root.Add(entryDirectionPopupField);
            root.Add(exitModePopupField);
            root.Add(exitDirectionPopupField);

            HandlePopupFieldEnabled((EntryMode)entryModeProp.intValue, entryDirectionPopupField);
            HandlePopupFieldEnabled((EntryMode)exitModeProp.intValue, exitDirectionPopupField);
            
            entryModePopupField.RegisterValueChangedCallback(evt => 
            {
                int selectedEnumValue = GetEntryModeValueFromIndex(evt.newValue, entryModeDisplayOptions);
                entryModeProp.intValue = selectedEnumValue;


                EntryMode entryMode = EntryMode.DoNothing;
                if(System.Enum.IsDefined(typeof(EntryMode), selectedEnumValue))
                {
                    entryMode = (EntryMode)selectedEnumValue;
                }
                else
                {
                    Debug.LogError("Selected entry mode not defined: " + selectedEnumValue);
                }
                
                HandlePopupFieldEnabled(entryMode, entryDirectionPopupField);
                entryModeProp.serializedObject.ApplyModifiedProperties();
            });

            exitModePopupField.RegisterValueChangedCallback(evt => 
            {
                int selectedEnumValue = GetEntryModeValueFromIndex(evt.newValue, entryModeDisplayOptions);
                exitModeProp.intValue = selectedEnumValue;


                EntryMode exitMode = EntryMode.DoNothing;
                if(System.Enum.IsDefined(typeof(EntryMode), selectedEnumValue))
                {
                    exitMode = (EntryMode)selectedEnumValue;
                }
                else
                {
                    Debug.LogError("Selected exit mode not defined: " + selectedEnumValue);
                }
                
                HandlePopupFieldEnabled(exitMode, exitDirectionPopupField);
                exitModeProp.serializedObject.ApplyModifiedProperties();
            });
            
            return root;
        }

        private int GetEntryModeValueFromIndex(string selectedValue, List<string> enumNames)
        {
            for(int i = 0; i < enumNames.Count; i++)
            {
                if(enumNames[i] == selectedValue)
                {
                    return (int)System.Enum.Parse(typeof(EntryMode), enumNames[i]);
                }
            }

            return 0;
        }

        private int GetDirectionFromIndex(string selectedValue, List<string> enumNames)
        {
            for(int i = 0; i < enumNames.Count; i++)
            {
                if(enumNames[i] == selectedValue)
                {
                    return (int)System.Enum.Parse(typeof(Direction), enumNames[i]);
                }
            }

            return 0;
        }

        private int FindEntryModeIndex(int enumValue)
        {
            EntryMode[] values = (EntryMode[])System.Enum.GetValues(typeof(EntryMode));

            for(int i = 0; i < values.Length; i++)
            {
                if((int)values[i] == enumValue)
                {
                    return i;
                }
            }

            return 0;
        }

        private int FindDirectionIndex(int enumValue)
        {
            Direction[] values = (Direction[])System.Enum.GetValues(typeof(Direction));

            for(int i = 0; i < values.Length; i++)
            {
                if((int)values[i] == enumValue)
                {
                    return i;
                }
            }

            return 0;
        }

        private void HandlePopupFieldEnabled(EntryMode mode, PopupField<string> directionField)
        {
            directionField.SetEnabled(false);
            switch(mode)
            {
                case EntryMode.Slide:
                                     directionField.SetEnabled(true);
                                     break;
                
                case EntryMode.Zoom:
                                     directionField.SetEnabled(false);
                                     break;

                case EntryMode.Fade:
                                     directionField.SetEnabled(false);
                                     break;
            }
        }
    }
    #endif
}
