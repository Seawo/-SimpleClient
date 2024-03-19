using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting.FullSerializer;

public class LoadingScenesController : MonoBehaviour
{
    public Sprite[] image;
    public Image background;
    public Text tipText;

    static string   nextScene;

    private float nextTime = 1f; // image timer
    float timer = 0f; // Secen Load timer


    [SerializeField]
    Image           progressBar; // 스크롤 바

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScenes");
    }    

    // Start is called before the first frame update
    void Start()
    {
        //background = GetComponent<Image>();
        StartCoroutine(ChangeImage());
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator ChangeImage()
    {
        /*
         for 문을 돌면서 사진을 업데이트 하는데
        텀은 1초로 주었다
         */
        for (int i =  0; i < image.Length; i++) 
        {
            yield return new WaitForSeconds(1f);
            switch(i)
            {
                case 0:
                    tipText.color = Color.white;
                    tipText.text = "★Tip★  배 안에선 안 죽을수도 있다.";
                    break;
                case 1:
                    tipText.color = Color.red;
                    tipText.text = "★Tip★  슬리피우드는 너무 무섭다.";
                    break;
                case 2:
                    tipText.color = Color.blue;
                    tipText.text = "★Tip★  엘리니아는 현용님의 고향이다.";
                    break;
                case 3:
                    tipText.color = Color.white;
                    tipText.text = "★Tip★  커닝시티는 낭만의 도시 입니다.";
                    break;
                case 4:
                    tipText.color = Color.green;
                    tipText.text = "★Tip★  페리온은 뭔가 서늘하다.";
                    break;
            }
            background.sprite = image[i];
            yield return new WaitForSeconds(nextTime);
        }
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation       op =  SceneManager.LoadSceneAsync(nextScene); // 비동기 작업
        op.allowSceneActivation = false; // 자동으로 불러온 씬으로 이동할래?
                                         // false : 90% 까지 완료 하게 되며 기다리게 된다
                                         // ture 변환시, 나머지 10%를 불러오면서 씬 이동

        while (!op.isDone) // 씬 로드가 끝나지 않은 상태라면 계속 반복
        {
            yield return null;

            if (op.progress < 0.9f) 
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                // fake 로딩 1초간 나머지를 채운다
                timer += (Time.unscaledDeltaTime/10f); 
                progressBar.fillAmount = Mathf.Lerp(0.1f, 1f, timer); // 1초에 걸쳐서 바를 채우기
                if(progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}