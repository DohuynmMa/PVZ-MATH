using System;

public enum Musics
{
    闯关模式 = 0,
    REVIVE = 1,
    开心地解题 = 2,
    LoseX = 3,
    与四则共舞 = 4,
    逃离几何 = 5,
    ZCubeAC1 = 6,
    ZCubeAC2 = 7,
    None = 8
}

public enum Voices
{
}

public enum Sounds
{
    none = 0,
    放置想法 = 1,//
    使用卡牌 = 2,//
    改变蓝条 = 3,
    SC发射 = 4,
    P攻击 = 5,
    滴 = 6,
    SC子弹击中 = 7,
    P倒地 = 8,
    按钮点击 = 9,
    prize = 10,//
    导出 = 11,
    导入 = 12,
    导数 = 13,
    胜利 = 14,
    失败 = 15,
    selected = 16,//
    错误 = 17,//
    sc_die = 18,
    计算器爆炸 = 19,
    逆时针启动 = 20,
    ZCUBE故障 = 21,
    ZCUBE激光命中AC1 = 22,
    ZCube扫描AC2 = 23,
    ZCube激光蓄力AC2 = 24,
    ZCube激光发射AC2 = 25,
    ZCube激光击中AC2 = 26,
    ZCube激光命中后爆炸 = 27,
    Zcube死亡爆炸 = 28,
}

public static class AudioHelper
{
    public static T random<T>() where T : Enum
    {
        var values = typeof(T).GetEnumValues();
        return (T) values.random();
    }

    public static object random(this Array array)
    {
        var index = UnityEngine.Random.Range(0, array.Length);
        return array.GetValue(index);
    }

    public static T random<T>(this T[] array)
    {
        var index = UnityEngine.Random.Range(0, array.Length);
        return array[index];
    }
    public static void play(this Musics sound, bool loop)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playMusic(id, loop);
    }
    public static void play(this Voices sound)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playVoice(id);
    }
    public static void play(this Sounds sound)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSounds(id);
    }
    public static void play(this Sounds sound,float time)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSounds(id,time);
    }
    public static void play(this Sounds sound, float time,float speed)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSounds(id, time,speed);
    }
    public static void playWithPitch(this Sounds sound, float pitch,float volume)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSoundsWithPitch(id, pitch,volume);
    }
    public static void playWithPitch(this Sounds sound, float pitch)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSoundsWithPitch(id, pitch);
    }
    public static void playWithPitch(this Sounds sound)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSoundsWithPitch(id);
    }
}
