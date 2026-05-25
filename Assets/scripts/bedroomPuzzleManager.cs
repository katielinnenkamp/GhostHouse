using UnityEngine;

public class bedroomPuzzleManager : MonoBehaviour
{
    [SerializeField]
    private paintings[] paintingSpots;

    private bool[] corrects;

    [SerializeField]
    private GameObject reward;

    void Awake()
    {
        corrects = new bool[paintingSpots.Length];

        reward.SetActive(false);
    }

    public void Correct(paintings submitter)
    {
        for (int i = 0; i < paintingSpots.Length; i++)
        {
            if (paintingSpots[i] == null)
                continue;

            if (paintingSpots[i] == submitter)
            {
                corrects[i] = true;

                // Check if every slot is correct
                for (int j = 0; j < corrects.Length; j++)
                {
                    if (corrects[j] == false)
                    {
                        return;
                    }
                }

                PuzzleComplete();

                return;
            }
        }
    }

    public void Incorrect(paintings submitter)
    {
        for (int i = 0; i < paintingSpots.Length; i++)
        {
            if (paintingSpots[i] == null)
                continue;

            if (paintingSpots[i] == submitter)
            {
                corrects[i] = false;

                return;
            }
        }
    }

    private void PuzzleComplete()
    {
        reward.SetActive(true);

        Debug.Log("Painting puzzle complete!");
    }
}