using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill,Gold , Time, Health, Totalgold ,CountTime,Score}
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch(type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                if(curExp == 0)
                    mySlider.value = 0;
                else
                    mySlider.value = curExp / maxExp;

                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Gold:
                myText.text = string.Format("{0:F0}", GameManager.instance.gold);
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.maxgameTime - GameManager.instance.gameTime;
                if (remainTime <= 0)
                {
                    myText.text = "00:00";
                }
                else
                {
                    int min = Mathf.FloorToInt(remainTime / 60);
                    int sec = Mathf.FloorToInt(remainTime % 60);
                    myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                }
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.Health;
                float maxHealth = GameManager.instance.MaxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
            case InfoType.Totalgold:
                myText.text = string.Format("{0:F0}", GameManager.instance.totalGold);
                break;
            case InfoType.CountTime:
                float countime = GameManager.instance.gameTime;

                if (countime <= 0)
                {
                    myText.text = "00:00";
                }
                else
                {
                    int min = Mathf.FloorToInt(countime / 60);
                    int sec = Mathf.FloorToInt(countime % 60);
                    myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                }
                break ;
            case InfoType.Score:
                float gametime = GameManager.instance.gameTime;
                int min1 = Mathf.FloorToInt(gametime / 60);
                int sec1 = Mathf.FloorToInt(gametime % 60);
                float score = GameManager.instance.kill * 100 + min1 * 1000 + sec1 * 100;
                myText.text = string.Format("{0:F0}", score);
                break;

        }
    }
}
