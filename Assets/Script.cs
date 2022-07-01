using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script : MonoBehaviour
{
    string[] questions;
    string[] images;
    int[] links;
    int firstQuestion = LoadScenes.firstQuestion;
    public Text question; //Створення змінної для виведення питань на екран
    public Text answer;//Створення змінної для виведення відповідей на екран
    int numberOfQuestion;
    int currentQuestion;
    readonly string t = "Правильна відповідь";
    readonly string f = "Спробуйте ще раз";

    private void Start()
    {
        questions = LoadScenes.lines;
        links = LoadScenes.tags;
        images = LoadScenes.imagePaths;

        for (int i = 0; i < links.Length; i++)
        {
            GameObject x = GameObject.Find($"Plane{i + 1}");
            if (images[i].Substring(images[i].LastIndexOf(".")) == ".jpg" || images[i].Substring(images[i].LastIndexOf(".")) == ".png" || images[i].Substring(images[i].LastIndexOf(".")) == ".jpeg")
            {
                Texture2D y = LoadImage(images[i]);
                y.Apply();
                x.GetComponent<Renderer>().material.SetTexture("_MainTex", y);
            }
            else
            {
                var video = x.AddComponent<UnityEngine.Video.VideoPlayer>();
                string newUrl = images[i];
                Debug.Log(newUrl);
                video.url = newUrl;
                video.isLooping = true;
                video.Stop();
            }
        }
        currentQuestion = firstQuestion - 1;
        question.text = questions[firstQuestion - 1];
    }
    public void LoadQuestion(int x)
    {
        numberOfQuestion = x;
        var video = GameObject.Find($"Plane{numberOfQuestion + 1}").GetComponent<UnityEngine.Video.VideoPlayer>();
        if (video != null)
            video.Play();
    }

    public void DesableQuestion(GameObject x)
    {
        var video = x.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (video != null)
            video.Stop();
    }
    public void backClick()
    {
        GameObject.Find($"BackButton").transform.position = new Vector3(0, -Screen.height, 0);
        GameObject.Find($"Question").transform.position = new Vector3(-Screen.width, -Screen.height, 0);
        SceneManager.LoadScene(0);
    }
    public void onClick()
    {
        if (currentQuestion == numberOfQuestion)
        {
            if (links[currentQuestion] != firstQuestion)
            {
                answer.text = t;
                currentQuestion = links[currentQuestion] - 1;
                question.text = questions[currentQuestion];
            }
            else
            {
                question.text = "Вітаємо, ви пройшли квест!";
                GameObject.Find($"Background").transform.position = new Vector3(Screen.width / 2, Screen.height / 2, -10);
                GameObject.Find($"ChooseButton").transform.position = new Vector3(0, -Screen.height, 0);
                GameObject.Find($"Question").transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                StartCoroutine(Camera(0.5f));
            }
        }
        else { answer.text = f; }
        StartCoroutine(Clear());
    }
    public static Texture2D LoadImage(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }
    IEnumerator Camera(float x)
    {
        yield return new WaitForSeconds(x);
        GameObject.Find($"ARCamera").SetActive(false);
    }
    IEnumerator Clear() //функція для зтирання повідомлення
    {
        yield return new WaitForSeconds(2);
        while (answer.text != "")
        {
            answer.text = answer.text.Remove(answer.text.Length - 1);
            yield return new WaitForSeconds(0.01f);
        }
    }
}