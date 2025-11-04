using UnityEngine;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{
    public Text scoreText;

    private int score;

    //This is a C# property.
    //Score is a variable.
    public int Score
    {
        //When we access Score - we're actually returning the value of the private "score" variable
        get 
        { 
            return score; 
        }

        //The contents inside the section of a property are run every time we make changes to the value of the property
        //When we set the value of Score - we also set the text on the scoreText variable as well.
        set
        {
            score = value;
            scoreText.text = $"Score: {score}";
        }
    }
}
