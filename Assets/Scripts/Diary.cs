using UnityEngine;

public class Diary : MonoBehaviour
{
    [SerializeField] private Transform leftPageContainer; // Left pages
    [SerializeField] private Transform rightPageContainer; // Right pages

    private GameObject[] leftPages;
    private GameObject[] rightPages;

    private int currentPageIndex = 0;

    private void Start()
    {
        // Load all pages into arrays
        leftPages = new GameObject[leftPageContainer.childCount];
        rightPages = new GameObject[rightPageContainer.childCount];

        for (int i = 0; i < leftPageContainer.childCount; i++)
        {
            leftPages[i] = leftPageContainer.GetChild(i).gameObject;
            leftPages[i].SetActive(false); // Disable all pages initially
        }

        for (int i = 0; i < rightPageContainer.childCount; i++)
        {
            rightPages[i] = rightPageContainer.GetChild(i).gameObject;
            rightPages[i].SetActive(false); // Disable all pages initially
        }

        if (leftPages.Length > 0)
        {
            ShowCurrentPages(); // Show the first pair of pages
        }
    }

    public void NegateActiveState()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void FlipToNextPage()
    {
        if (currentPageIndex < leftPages.Length - 1)
        {
            HideCurrentPages();
            currentPageIndex++;
            ShowCurrentPages();
        }
    }

    public void FlipToPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            HideCurrentPages();
            currentPageIndex--;
            ShowCurrentPages();
        }
    }

    public void UnlockPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < leftPages.Length)
        {
            leftPages[pageIndex].SetActive(true);
            rightPages[pageIndex].SetActive(true);
        }
    }

    private void ShowCurrentPages()
    {
        leftPages[currentPageIndex].SetActive(true);
        rightPages[currentPageIndex].SetActive(true);
    }

    private void HideCurrentPages()
    {
        leftPages[currentPageIndex].SetActive(false);
        rightPages[currentPageIndex].SetActive(false);
    }
}
