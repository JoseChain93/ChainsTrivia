using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;  // Usamos TextMesh Pro
using System.Collections;
using System.Collections.Generic;

public class TriviaManager : MonoBehaviour
{
    [System.Serializable]
    public class ApiResponse
    {
        public int response_code;
        public Question[] results;
    }

    [System.Serializable]
    public class Question
    {
        public string category;
        public string type;
        public string difficulty;
        public string question;
        public string correct_answer;
        public string[] incorrect_answers;
    }

    [Header("Referencias de UI")]
    public TextMeshProUGUI questionText;  // Texto de la pregunta
    public Button[] answerButtons;  // Botones A B C D
    public TextMeshProUGUI resultText;  // Texto del resultado
    public Button nextButton;  // Botón de siguiente pregunta

    private ApiResponse triviaData; // Datos recibidos de la API
    private int currentQuestionIndex = 0; // Contador de preguntas
    private string correctAnswer;

    void Start()
    {
        nextButton.gameObject.SetActive(false);  // Inicialmente, el botón "Siguiente" estará oculto
        StartCoroutine(FetchQuestions());
    }

    // Corrutina para obtener preguntas desde la API
    IEnumerator FetchQuestions()
    {
        string url = "https://opentdb.com/api.php?amount=10";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener las preguntas: " + request.error);
            // Error al contactar la API
        }
        else
        {
            // Analizar la respuesta JSON
            string json = request.downloadHandler.text;
            triviaData = JsonUtility.FromJson<ApiResponse>(json);

            if (triviaData != null && triviaData.results.Length > 0)
            {
                ShowQuestion(triviaData.results[currentQuestionIndex]);
            }
        }
    }

    void ShowQuestion(Question q)
    {
        questionText.text = System.Net.WebUtility.HtmlDecode(q.question);
        correctAnswer = q.correct_answer;

        // Combinar la respuesta correcta con las incorrectas
        List<string> answers = new List<string>(q.incorrect_answers);
        answers.Add(q.correct_answer);

        // Mezclar las respuestas
        for (int i = 0; i < answers.Count; i++)
        {
            string temp = answers[i];
            int randomIndex = Random.Range(i, answers.Count);
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }

        // Asignar respuestas a los botones
        for (int i = 0; i < answerButtons.Length; i++)
        {
            string answer = answers[i];
            string buttonLabel = (char)('A' + i) + ": ";  // Etiqueta A, B, C, D

            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = buttonLabel + System.Net.WebUtility.HtmlDecode(answer); // Mostrar la respuesta real
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => CheckAnswer(answer));
        }
    }

    // Verificar si la respuesta seleccionada es correcta
    void CheckAnswer(string selectedAnswer)
    {
        Color customColor;
        ColorUtility.TryParseHtmlString("#36405d", out customColor);

        if (selectedAnswer == correctAnswer)
        {
            resultText.text = "¡Respuesta Correcta!";
            resultText.color = customColor;
        }
        else
        {
            resultText.text = "Incorrecto. La respuesta correcta era: " + correctAnswer;
            resultText.color = customColor;
        }

        nextButton.gameObject.SetActive(true);  // Mostrar botón "Siguiente"
    }

     // Cargar la siguiente pregunta
    public void LoadNextQuestion()
    {
        // Reiniciar el texto de resultado
        resultText.text = "";
        resultText.color = Color.white;

        currentQuestionIndex++;

        // Verificar si hay más preguntas
        if (currentQuestionIndex < triviaData.results.Length)
        {
            ShowQuestion(triviaData.results[currentQuestionIndex]);
            nextButton.gameObject.SetActive(false);  // Ocultar botón "Siguiente"
        }
        else
        {
            resultText.text = "¡Has completado el trivia!";
            resultText.color = Color.white;
            nextButton.gameObject.SetActive(false);  // Ocultar botón al finalizar
        }
    }
}