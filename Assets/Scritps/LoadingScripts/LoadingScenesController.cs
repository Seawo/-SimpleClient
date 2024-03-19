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
    Image           progressBar; // ��ũ�� ��

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
         for ���� ���鼭 ������ ������Ʈ �ϴµ�
        ���� 1�ʷ� �־���
         */
        for (int i =  0; i < image.Length; i++) 
        {
            yield return new WaitForSeconds(1f);
            switch(i)
            {
                case 0:
                    tipText.color = Color.white;
                    tipText.text = "��Tip��  �� �ȿ��� �� �������� �ִ�.";
                    break;
                case 1:
                    tipText.color = Color.red;
                    tipText.text = "��Tip��  �����ǿ��� �ʹ� ������.";
                    break;
                case 2:
                    tipText.color = Color.blue;
                    tipText.text = "��Tip��  �����Ͼƴ� ������� �����̴�.";
                    break;
                case 3:
                    tipText.color = Color.white;
                    tipText.text = "��Tip��  Ŀ�׽�Ƽ�� ������ ���� �Դϴ�.";
                    break;
                case 4:
                    tipText.color = Color.green;
                    tipText.text = "��Tip��  �丮���� ���� �����ϴ�.";
                    break;
            }
            background.sprite = image[i];
            yield return new WaitForSeconds(nextTime);
        }
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation       op =  SceneManager.LoadSceneAsync(nextScene); // �񵿱� �۾�
        op.allowSceneActivation = false; // �ڵ����� �ҷ��� ������ �̵��ҷ�?
                                         // false : 90% ���� �Ϸ� �ϰ� �Ǹ� ��ٸ��� �ȴ�
                                         // ture ��ȯ��, ������ 10%�� �ҷ����鼭 �� �̵�

        while (!op.isDone) // �� �ε尡 ������ ���� ���¶�� ��� �ݺ�
        {
            yield return null;

            if (op.progress < 0.9f) 
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                // fake �ε� 1�ʰ� �������� ä���
                timer += (Time.unscaledDeltaTime/10f); 
                progressBar.fillAmount = Mathf.Lerp(0.1f, 1f, timer); // 1�ʿ� ���ļ� �ٸ� ä���
                if(progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}