using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance {set;get;}

    public bool IsDead {set;get;}
    private PlayerMotor motor;
    private bool isGameStarted = false;


    // UI and UI fields
    public Animator gameCanvas, menuAnim, diamondAnim;
    public TextMeshProUGUI scoreText, coinText, modifierText, hiScoreText; 
    private float score,coinScore,modifierScore; 
    private int lastScore;

    //Death menu
    public Animator deathMenuAnim;
    public TextMeshProUGUI deadScoreText, deadCoinText;
    
    private void Awake(){
        Instance = this;
        modifierScore = 1;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();


        modifierText.text = "x" + modifierScore.ToString("0.0");
        scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0");
        // if (PlayerPrefs.GetInt("Hiscore").ToString() != null)
        // {
        //     hiScoreText.text = PlayerPrefs.GetInt("Hiscore").ToString();

        // }
    }

    private void Update(){
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            motor.StartRunning();
            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;
            gameCanvas.SetTrigger("Show");
            menuAnim.SetTrigger("Hide");
        }
        if (isGameStarted && !IsDead)
        {
            // bump up score
            score += (Time.deltaTime * modifierScore);
            if (lastScore != (int)score)
            {
                lastScore = (int)score;
                Debug.Log(lastScore);
                scoreText.text = score.ToString("0");
            }
           
        }

    }
    public void GetCoin(){
        coinScore++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT;
        diamondAnim.SetTrigger("Collect");
        scoreText.text = score.ToString("0");
    }
    public void UpdateModifier(float modifierAmount){
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }
    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    public void OnDeath(){
        IsDead = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;

        deadScoreText.text = score.ToString("0");
        deadCoinText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        gameCanvas.SetTrigger("Hide");

        // Check if this is a high score
        // if (score > PlayerPrefs.GetInt("Hiscore"))
        // {
        //     float s = score;
        //     if(s % 1 == 0)
        //         s += 1;
        //     PlayerPrefs.SetInt("Hiscore", (int)s);
        // }
    }
}
 