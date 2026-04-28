using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class DataManager : MonoBehaviour
{
    private static readonly Encoding encoding = new UTF8Encoding(false);
    private static string dataPath = Application.isMobilePlatform ? Application.persistentDataPath + "data" : (Application.isEditor ?
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + ".pvzmathEditor" : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + ".pvzmath") + Path.DirectorySeparatorChar + "data.json";
    public static DataManager Instance;
    public MyData data;
    public MyData newPlayerData;
    public MyData testData;//测试存档
    private string savedNewPlayerData;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        dataPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + ".pvzmath" + Path.DirectorySeparatorChar + "data.json";
        savedNewPlayerData = JsonUtility.ToJson(newPlayerData);
        loadPlayerData();
        if (data == null) return;
        ShortKeyManager.Instance.loadShortKey();

        SoundsManager.setMusicVolume(data.musicVolume);
        SoundsManager.setSoundVolume(data.soundVolume);
        SoundsManager.setVoiceVolume(data.voiceVolume);

        CardManager.Instance.refreshCard(data.cardTypes);
        GlobalUIManager.Instance.refreshBagItem();
    }
    public void addCard(CardType card)
    {
        data.cardTypes.Add(card);
        savePlayerData();
    }
    /// <summary>
    /// 存档
    /// </summary>
    public void savePlayerData()
    {
        data.loadCardShortKey();
        string json = JsonUtility.ToJson(data);
        var file = new FileInfo(dataPath);
        if (!file.Directory.Exists) file.Directory.Create();
        File.WriteAllTextAsync(dataPath, json, encoding);
    }
    /// <summary>
    /// 加载存档
    /// </summary>
    public void loadPlayerData()
    {
        var mmum = MainMenuUIManager.Instance;
        SoundsManager.Instance.sync();
        string json = File.Exists(dataPath) ? File.ReadAllText(dataPath, encoding) : null;
        data = json == null ? null : JsonUtility.FromJson<MyData>(json);
        if (data == null || !data.completedNewPlayerTutorial)
        {
            MainMenuUIManager.Instance.mainUi.SetActive(false);
            print("检测到新玩家,正在新建存档并启动新手教程...");
            resetPlayerData();
            Camera.main.GetComponent<BlurLayer>().blur();
            Sounds.滴.play();
            GameManager.Instance.loadLevel("*REVIVE*", "NEW", 0, 100, true);
            return;
        }
        else
        {
            mmum.mainUi.SetActive(true);
            mmum.mainUi.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
            mmum.systemUI.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
            mmum.updateDaveSystemNameText();
            HandManager.Instance.trueMouse.SetActive(true);
        }

        //加载并修复卡牌快捷键
        data.loadCardShortKey();
        Musics.开心地解题.play(true);
        autoFixData();
    }
    /// <summary>
    /// 重置玩家存档
    /// </summary>
    public void resetPlayerData()
    {
        print("正在创建新存档...");
        data = JsonUtility.FromJson<MyData>(savedNewPlayerData);
        savePlayerData();
    }
    /// <summary>
    /// 自动修复异常数据
    /// </summary>
    private void autoFixData()
    {
        var type = typeof(MyData);
        var saveFlag = false;
        foreach (var field in type.GetFields())
        {
            void setField(object v)
            {
                field.SetValue(data, v);
                saveFlag = true;
            }
            var value = field.GetValue(data);
            var intRange = field.GetCustomAttribute<IntRangeAttribute>();
            if (intRange != null && value is int)
            {
                if ((int)value < intRange.min) setField(intRange.min);
                if ((int)value > intRange.max) setField(intRange.max);
            }
            var floatRange = field.GetCustomAttribute<FloatRangeAttribute>();
            if (floatRange != null && value is float)
            {
                if ((float)value < floatRange.min) setField(floatRange.min);
                if ((float)value > floatRange.max) setField(floatRange.max);
            }
            var doubleRange = field.GetCustomAttribute<DoubleRangeAttribute>();
            if (doubleRange != null && value is double)
            {
                if ((double)value < doubleRange.min) setField(doubleRange.min);
                if ((double)value > doubleRange.max) setField(doubleRange.max);
            }
            var mappingData = field.GetCustomAttribute<MappingSimpleListDataAttribute>();
            if (mappingData != null)
            {
                var valueNew = field.GetValue(newPlayerData);
                //后面会增加其他类型
                if (value is List<int>)
                {
                    setField(mappingList<int>(value, valueNew));
                }
                else if (value is List<bool>)
                {
                    setField(mappingList<bool>(value, valueNew));
                }
                else if (value is List<double>)
                {
                    setField(mappingList<double>(value, valueNew));
                }
                else if (value is List<float>)
                {
                    setField(mappingList<float>(value, valueNew));
                }
                else if (value is List<KeyCode>)
                {
                    setField(mappingList<KeyCode>(value, valueNew));
                }
            }
        }
        if (saveFlag) savePlayerData();
    }
    private List<T> mappingList<T>(object source, object target)
    {
        var sl = source as IList;
        ConvertToTypedList<T>(sl, out var sl2);
        var sourceList = new List<T>(sl2);

        var tl = target as IList;
        ConvertToTypedList<T>(tl, out var tl2);
        var targetList = new List<T>(tl2);
        for (int i = 0; i < sourceList.Count; i++)
        {
            targetList[i] = sourceList[i];
        }
        return targetList;
    }
    /// <summary>
    /// 将object（实际为List<T>或数组）转为IList操作后，转回List<T>
    /// </summary>
    /// <typeparam name="T">目标List的元素类型</typeparam>
    /// <param name="obj">原始object</param>
    /// <param name="resultList">转换后的List<T></param>
    /// <returns>是否转换成功</returns>
    public bool ConvertToTypedList<T>(object obj, out List<T> resultList)
    {
        resultList = new List<T>();
        if (obj is not IList originalList)
            return false;

        try
        {
            resultList = originalList.Cast<T>().ToList();
            return true;
        }
        catch (InvalidCastException)
        {
            Console.WriteLine($"元素类型无法转换为{typeof(T).Name}");
            return false;
        }
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class IntRangeAttribute : Attribute
{
    public int min;
    public int max;
    public IntRangeAttribute(int min = int.MinValue, int max = int.MaxValue)
    {
        this.min = min;
        this.max = max;
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class FloatRangeAttribute : Attribute
{
    public float min;
    public float max;
    public FloatRangeAttribute(float min = float.MinValue, float max = float.MaxValue)
    {
        this.min = min;
        this.max = max;
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DoubleRangeAttribute : Attribute
{
    public double min;
    public double max;
    public DoubleRangeAttribute(double min = double.MinValue, double max = double.MaxValue)
    {
        this.min = min;
        this.max = max;
    }
}
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class MappingSimpleListDataAttribute : Attribute
{
}
