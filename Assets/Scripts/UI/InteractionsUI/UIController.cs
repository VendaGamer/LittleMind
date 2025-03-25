using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private Interactions interactions;

    private VisualElement menuContainer;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        // Find the ListView by name
        var listView = root.Q<ListView>("groups-list");

        // Set the data source
        listView.itemsSource = interactions.UIInteractionGroups;
        listView.bindItem += (element, i) =>
        {
            element.Q<ListView>("interactions-list").itemsSource = interactions.UIInteractionGroups[i].Interactions;
        } ;
    }

}