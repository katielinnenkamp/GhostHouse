using UnityEngine;

public class libraryPuzzleManager : MonoBehaviour
{
    [SerializeField]
    private pedestal[] pedestals;
    private bool[] corrects;

    void Awake()
    {
        corrects = new bool[pedestals.Length];
    }
    
    public void Correct(pedestal submitter)
    {
        for(int i = 0; i < pedestals.Length; i++)
        {
            if(pedestals[i] == null) continue;
            if(pedestals[i] == submitter)
            {
                corrects[i] = true;

                for(int j = 0; j < corrects.Length; j++)
                {
                    if(corrects[j] == false)
                    {
                        return;
                    }
                }

                PuzzleComplete();

                return;
            }
        }
    }

    public void Incorrect(pedestal submitter)
    {
        for(int i = 0; i < pedestals.Length; i++)
        {
            if(pedestals[i] == null) continue;
            if(pedestals[i] == submitter)
            {
                corrects[i] = false;

                return;
            }
        }
    }

    private void PuzzleComplete()
    {
        Debug.Log("correct order of books");
    }
}
