using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;
using System.Collections;

public class LoadScenes : MonoBehaviour
{
    public Text test;
    private string FileType;
    public static string[] lines;
    public static int[] tags;
    public static string questionPath;
    public static string[] imagePaths;
    public static int firstQuestion;

    void Start()
    {
        FileType = NativeFilePicker.ConvertExtensionToFileType("txt");
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            Screen.orientation = ScreenOrientation.Landscape;
            StartCoroutine(Wait());
        }

    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject.Find($"Question").transform.position = new Vector3(Screen.width / 2, Screen.height - Screen.height / 10, 0);
        GameObject.Find($"BackButton").transform.position = new Vector3(Screen.width / 13, Screen.height / 10.5f, 0);
        GameObject.Find($"Background").transform.position = new Vector3(Screen.width / 2, Screen.height / 2, Screen.height * 5);
    }
    public void OpenQuest()
    {
        GameObject t = GameObject.Find($"TestText");
        string s = check(imagePaths, lines);
        Debug.Log(s);
        if (s != null)
        {
            t.GetComponent<Text>().text = s;
            return;
        }
        SceneManager.LoadScene(1);
    }
    string check(string[] x, string[] z)
    {
        if (x == null && z == null) return "Питання та медiа не завантажено!";
        if (x == null) return "Медiа не завантажено";
        if (z == null) return "Питання не завантажено";
        if (x.Length < z.Length) return "Медiа недостатьно, спробуйте ще раз";
        return null;
    }
    public void LoadQuestions()
    {
        if (NativeFilePicker.IsFilePickerBusy())
            return;
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null) { test.text = "Питання не завантажено"; return; }
            try
            {
                Maping(File.ReadAllLines(path));
                questionPath = path.Remove(path.LastIndexOf("/") + 1);
                int x = checkQuestions(tags, firstQuestion, 1);
                if (x == 0)
                    throw new InvalidOperationException();
                if (x > 10)
                    throw new DivideByZeroException();
                test.text = "Питань завантажено : " + x;
            }
            catch (FormatException)
            {
                lines = null;
                test.text = "Помилка:\nНеправильний макет";
                return;
            }
            catch (DivideByZeroException)
            {
                lines = null;
                test.text = "Помилка:\nПитань бiльше за 10";
                return;
            }
            catch (InvalidOperationException)
            {
                lines = null;
                test.text = "Помилка:\nЦиклiчнi посилання у питаннях";
                return;
            }
            catch (Exception)
            {
                lines = null;
                test.text = "Помилка:\nПосилання на неiснуюче питання";
            }
        }, new string[] { FileType });
    }
    public void LoadImages()
    {
        string[] fileTypes = new string[] { "image/*", "video/*" };
        NativeFilePicker.Permission permission = NativeFilePicker.PickMultipleFiles((paths) =>
        {
            if (paths == null) { test.text = "Фото/вiдео не завантажено"; return; }
            imagePaths = paths;
            test.text = "Фото/вiдео завантажено : " + paths.Length;
        }, fileTypes);
    }
    void Maping(string[] s)
    {
        int i = 0;
        int j = 0;
        List<string> tmpstring = new List<string>();
        List<int> tmpint = new List<int>();
        firstQuestion = Int32.Parse(s[j++]);
        while (j < s.Length)
        {
            tmpstring.Add(s[j++]);
            tmpint.Add(Int32.Parse(s[j++]));
            i++;
        }
        lines = tmpstring.ToArray();
        tags = tmpint.ToArray();
    }
    int checkQuestions(int[] x, int z, int c)
    {
        Debug.Log("\tx[" + (z - 1) + "] = " + (x[z - 1]) + "\tCount : " + c);
        if (x[z - 1] == firstQuestion)
            return c;
        if (c > x.Length)
            return 0;
        return checkQuestions(x, x[z - 1], c + 1);
    }
    public void Exit()
    {
        Application.Quit();
    }
}