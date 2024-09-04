using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public Transform Hand;
    public LevelMechanics Mechanics;
    public GameObject BlockingPanel;

    public List<Step> Steps;

    public bool IsTutorial(int levelIndex, string difficulty)
    {
        return levelIndex == 0 && difficulty.Equals("easy");
    }

    public void Clear()
    {
        StopAllCoroutines();
        InputManager.IsFaking = false;
        InputManager.MouseButtons[0] = false;
        if(BlockingPanel != null)
        BlockingPanel.gameObject.SetActive(false);
    }

    public void MakeTutorial()
    {
        Debug.Log("Starting Tutorial");
        StartCoroutine(Solve());
    }

    private void OnDisable()
    {
        Clear();
    }

    IEnumerator Solve()
    {
        BlockingPanel.gameObject.SetActive(true);
        InputManager.IsFaking = true;

        yield return new WaitForSeconds(0.3f);
        Mechanics.StartSolver();

        foreach (Step s in Steps)
        {

            InputManager.MouseButtons[0] = true;
            float currentTime = 0;
            while (currentTime <= s.Time)
            {
                InputManager.mousePosition = Camera.main.WorldToScreenPoint(Vector3.Lerp(s.From, s.To, currentTime / s.Time));
                currentTime += Time.deltaTime;
                yield return null;
            }
            Debug.Log("Step ended");
            InputManager.MouseButtons[0] = false;
            yield return null;
            yield return null;
        }
        InputManager.MouseButtons[0] = false;
        InputManager.IsFaking = false;
        BlockingPanel.SetActive(false);
    }

    [System.Serializable]
    public class Step
    {
        public Vector3 From;
        public Vector3 To;
        public float Time;
        public bool FakeMove = true;
    }

}
