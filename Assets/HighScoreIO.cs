using System.IO;
using UnityEngine;

public class HighScoreIO
{
    /// <summary>
    /// Writes the high score to a file in the persistent data path
    /// </summary>
    /// <param name="score">Score to be saved</param>
    public static void SaveHighScore(uint score)
    {
        string path = Application.persistentDataPath + "/highscore.sv";
        File.WriteAllText(path, score.ToString());
    }

    /// <summary>
    /// Reads the highscore file if it exists, otherwise returns 0
    /// </summary>
    /// <returns></returns>
    public static string ReadHighScore()
    {
        string path = Application.persistentDataPath + "/highscore.sv";
        if (File.Exists(path))
            return File.ReadAllText(path);
        else
            return "0";
    }
}
