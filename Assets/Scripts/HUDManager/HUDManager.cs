using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUDManager : MonoBehaviour
{
    public Text date;
    public Text terrain;
    public Text timer;
    public Text speed;
    public Text weather;
    public Text season;
    public Text cash;
    public Text health;
    public Text happy;
    public Text stamina;
    public Text hungry;

    public void ChangeDate(int d)
    {
        switch (d)
        {
            case 1: date.text = "星期一"; break;
            case 2: date.text = "星期二"; break;
            case 3: date.text = "星期三"; break;
            case 4: date.text = "星期四"; break;
            case 5: date.text = "星期五"; break;
            case 6: date.text = "星期六"; break;
            case 7: date.text = "星期天"; break;
            default: break;
        }
    }
    public void ChangeTerrain(int index)
    {
        switch (index)
        {
            case 1: terrain.text = "沙滩"; break;
            case 2: terrain.text = "石滩"; break;
            case 3: terrain.text = "峡湾"; break;
            case 4: terrain.text = "热带浅海"; break;
            case 5: terrain.text = "深海"; break;
            case 6: terrain.text = "冻洋"; break;
            case 7: terrain.text = "雪峰"; break;
            case 8: terrain.text = "石峰"; break;
            case 9: terrain.text = "冰峰"; break;
            case 10: terrain.text = "火山"; break;
            case 11: terrain.text = "草甸"; break;
            case 12: terrain.text = "雪林"; break;
            case 13: terrain.text = "丘陵"; break;
            case 14: terrain.text = "石林"; break;
            case 15: terrain.text = "竹林"; break;
            case 16: terrain.text = "热带雨林"; break;
            case 17: terrain.text = "常绿阔叶林"; break;
            case 18: terrain.text = "落叶阔叶林"; break;
            case 19: terrain.text = "针叶林"; break;
            case 20: terrain.text = "黑森林"; break;
            case 21: terrain.text = "樱花林"; break;
            case 22: terrain.text = "沼泽"; break;
            case 23: terrain.text = "红树林"; break;
            case 24: terrain.text = "江河"; break;
            case 25: terrain.text = "大湖"; break;
            case 26: terrain.text = "平原"; break;
            case 27: terrain.text = "花原"; break;
            case 28: terrain.text = "雪原"; break;
            case 29: terrain.text = "稀树草原"; break;
            case 30: terrain.text = "沙漠"; break;
            case 31: terrain.text = "平顶山"; break;
            case 32: terrain.text = "绿洲"; break;
            case 33: terrain.text = "煤滩"; break;
            case 34: terrain.text = "块野"; break;
            case 35: terrain.text = "TEMP#4"; break;
            default: terrain.text = "未知地形"; break;
        }
    }
    public void ChangeTime(int hour, int min)
    {
        timer.text = $"{hour}:{min}";
    }
    public void ChangeSpeed(int v)
    {
        speed.text = $"{v}km/h";
    }
    public void ChangeWeather(int index)
    {
        switch (index)
        {
            case 1: weather.text = "晴"; break;
            case 2: weather.text = "多云"; break;
            case 3: weather.text = "小雨"; break;
            case 4: weather.text = "大雨"; break;
            case 5: weather.text = "雷暴"; break;
            case 6: weather.text = "小雪"; break;
            case 7: weather.text = "大雪"; break;
            case 8: weather.text = "浓雾"; break;
            case 9: weather.text = "沙尘暴"; break;
            case 10: weather.text = "洪水"; break;
            case 11: weather.text = "冰雹"; break;
            case 12: weather.text = "龙卷风"; break;
            case 13: weather.text = "台风"; break;
            case 14: weather.text = "地震"; break;
            case 15: weather.text = "海啸"; break;
            case 16: weather.text = "火山喷发"; break;
            case 17: weather.text = "滑坡"; break;
            case 18: weather.text = "太阳风暴（极光）"; break;
            case 19: weather.text = "陨石撞击"; break;
            case 20: weather.text = "野火"; break;
            case 21: weather.text = "雪崩"; break;
            case 22: weather.text = "干旱"; break;
            case 23: weather.text = "辐射尘暴"; break;
            default: weather.text = "未知天气"; break;
        }
    }
    public void ChangeSeason(int index)
    {
        switch (index)
        {
            case 1: season.text = "春季"; break;
            case 2: season.text = "夏季"; break;
            case 3: season.text = "秋季"; break;
            case 4: season.text = "冬季"; break;
            default: season.text = "未知季节"; break;
        }
    }
    public void ChangeCash(int num)
    {
        cash.text = num.ToString();
    }
    public void ChangeCondition(int hea, int hap, int sta, int hun)
    {
        health.text = hea.ToString() + "%";
        happy.text = hap.ToString() + "%";
        stamina.text = sta.ToString() + "%";
        hungry.text = hun.ToString() + "%";
    }


}
