using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

// This attribute tells Unity to use this class to draw the InteractionState property.
[CustomPropertyDrawer(typeof(StatefulInteractionProvider.InteractionState))]
public class InteractionStateDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Get the parent StatefulInteractionProvider component. This is key to accessing the 'allInteractions' list.
        var provider = property.serializedObject.targetObject as StatefulInteractionProvider;

        // Create a root VisualElement to hold our custom UI.
        var container = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Column,
                marginBottom = 5
            }
        };

        // Find the properties of the InteractionState class.
        var stateNameProp = property.FindPropertyRelative("StateName");
        var interactionsProp = property.FindPropertyRelative("InteractionsIndexes");

        // Create a standard field for the state name.
        var stateNameField = new PropertyField(stateNameProp);

        // --- Create the custom ListView for the interaction indexes ---

        // Get the names from the main 'allInteractions' array for our dropdown choices.
        // We'll add a "(None)" option for clarity.
        var interactionNames = GetAllInteractionNames(provider);

        // The ListView will display and manage our array of interaction indexes.
        var interactionListView = new ListView
        {
            headerTitle = "Interactions In This State",
            showFoldoutHeader = true,
            showAddRemoveFooter = true,
            reorderable = true,
            reorderMode = ListViewReorderMode.Animated,
            // Bind the list view to our array property.
            bindingPath = interactionsProp.propertyPath,
            // Define how to create a single item in the list (a dropdown).
            makeItem = () =>
            {
                var popup = new PopupField<string>(interactionNames, 0);
                return popup;
            },
            // Define how to bind data to a single item in the list.
            bindItem = (element, i) =>
            {
                var popupField = element as PopupField<string>;
                var indexProp = interactionsProp.GetArrayElementAtIndex(i);

                // Set the dropdown's current value based on the integer in the array.
                // If the index is invalid, default to 0.
                if (popupField != null)
                {
                    popupField.index = (indexProp.intValue < interactionNames.Count) ? indexProp.intValue : 0;

                    // When the dropdown value changes, update the integer in the serialized property.
                    popupField.RegisterValueChangedCallback(evt =>
                    {
                        indexProp.intValue = interactionNames.IndexOf(evt.newValue);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
        };

        // Add the created fields to our container.
        container.Add(stateNameField);
        container.Add(interactionListView);

        return container;
    }

    /// <summary>
    /// Helper method to get all interaction names from the provider for dropdowns.
    /// </summary>
    private List<string> GetAllInteractionNames(StatefulInteractionProvider provider)
    {
        // This is a bit of reflection magic to get the array from the serialized object.
        var interactionsObj = provider.GetType().GetField("allInteractions",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (interactionsObj == null) return new List<string> { "(No Interactions Defined)" };

        if (interactionsObj.GetValue(provider) is Interaction[] { Length: > 0 } interactionsArray)
        {
            // Use the ActionName from your Interaction class.
            return interactionsArray.Select(i => i?.ActionName ?? "NULL").ToList();
        }
        return new List<string> { "(No Interactions Defined)" };
    }
}


[CustomEditor(typeof(StatefulInteractionProvider))]
public class StatefulInteractionProviderEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        // Find all the properties we need to work with.
        var allStatesProp = serializedObject.FindProperty("allStates");
        var allInteractionsProp = serializedObject.FindProperty("allInteractions");
        var initialStateIndexProp = serializedObject.FindProperty("initialStateIndex");

        // --- Create UI Elements ---

        // A help box to explain the workflow.
        var helpBox = new HelpBox(
            "1. Define all possible Interactions below.\n" +
            "2. Define all States and assign Interactions to them.\n" +
            "3. Select the Initial State.", HelpBoxMessageType.Info);
        
        // Default property fields for the interaction and state lists.
        // The 'allStates' field will automatically use the PropertyDrawer we just made!
        var interactionsField = new PropertyField(allInteractionsProp, "1. All Possible Interactions");
        var statesField = new PropertyField(allStatesProp, "2. All Possible States");
        
        // --- Custom Dropdown for Initial State ---

        var stateNames = GetStateNames(allStatesProp);
        var initialStatePopup = new PopupField<string>("3. Initial State", stateNames, initialStateIndexProp.intValue);
        
        initialStatePopup.RegisterValueChangedCallback(evt =>
        {
            // When the user picks a state from the dropdown, update the underlying integer property.
            initialStateIndexProp.intValue = stateNames.IndexOf(evt.newValue);
            serializedObject.ApplyModifiedProperties();
        });

        // Add everything to the root visual element.
        root.Add(helpBox);
        root.Add(interactionsField);
        root.Add(statesField);
        root.Add(initialStatePopup);

        return root;
    }

    /// <summary>
    /// Helper method to get a list of state names from the 'allStates' property.
    /// </summary>
    private List<string> GetStateNames(SerializedProperty allStatesProp)
    {
        if (allStatesProp == null || !allStatesProp.isArray) return new List<string>();

        var names = new List<string>(allStatesProp.arraySize);
        for (int i = 0; i < allStatesProp.arraySize; i++)
        {
            var stateProp = allStatesProp.GetArrayElementAtIndex(i);
            var nameProp = stateProp.FindPropertyRelative("StateName");
            names.Add(nameProp?.stringValue ?? "Unnamed State");
        }
        return names;
    }
}