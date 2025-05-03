using UnityEngine;
public class Diary : MonoBehaviour
{
    [SerializeField] private Transform leftPageContainer; // Left pages
    [SerializeField] private Transform rightPageContainer; // Right pages

    private DiaryPage[] leftPages;
    private DiaryPage[] rightPages;

    private int currentPageIndex = 0;

    private void Start()
    {
        // Load all pages into arrays
        leftPages = new DiaryPage[leftPageContainer.childCount];
        rightPages = new DiaryPage[rightPageContainer.childCount];

        for (int i = 0; i < leftPageContainer.childCount; i++)
        {
            leftPages[i] = leftPageContainer.GetChild(i).GetComponent<DiaryPage>();
            leftPages[i].gameObject.SetActive(false); // Disable all pages initially
        }

        for (int i = 0; i < rightPageContainer.childCount; i++)
        {
            rightPages[i] = rightPageContainer.GetChild(i).GetComponent<DiaryPage>();
            rightPages[i].gameObject.SetActive(false); // Disable all pages initially
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
            leftPages[pageIndex].gameObject.SetActive(true);
            rightPages[pageIndex].gameObject.SetActive(true);
        }
    }

    private void ShowCurrentPages()
    {
        leftPages[currentPageIndex].gameObject.SetActive(true);
        rightPages[currentPageIndex].gameObject.SetActive(true);
    }

    private void HideCurrentPages()
    {
        leftPages[currentPageIndex].gameObject.SetActive(true);
        rightPages[currentPageIndex].gameObject.SetActive(true);
    }
}
