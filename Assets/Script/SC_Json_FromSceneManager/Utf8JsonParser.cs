/*
使い方
	Utf8JsonSample.cs を参照。

変更履歴

   2019/12/05
		初版。LaSolv(Windows)のMain.csからUtf8Json処理部分を抜き出し。
		YosegakiAR(iPad)でWebサーバ上にある画像一覧をリクエストし、
		json形式でパースして画像をダウンロードの為のurlを得る為に使用。
*/

using UnityEngine;
using Utf8Json;
using System.IO;
using System.Text;

public static class Utf8JsonExtension
{
	/// <summary>
	/// JSONファイルを読んで dynamic オブジェクトを返す。尚、配列は全て List<dynamic> で返る
	/// </summary>
	public static T ReadJsonFile<T>(string fileName)
	{
		string jsonText = ReadFile(fileName);
		return ParseJsonText<T>(jsonText);
	}

	public static T ParseJsonText<T>(string jsonText)
	{
		T tmpJson = JsonSerializer.Deserialize<T>(jsonText);

		if (tmpJson == null /*|| tmpJson.Count == 0*/)
		{
			//Debug.LogErrorFormat("json形式として問題があるか、rootノードが１以上の配列のjsonになっていません");
			Debug.LogErrorFormat("Utf8JsonExtension.ParseJsonText：json形式として問題があります");
		}
		else
		{
			Debug.LogFormat("Utf8JsonExtension.ParseJsonText：jsonとしてパースしました");
		}
		return tmpJson;
	}

	/// <summary>
	/// 指定したファイルが存在したら、全て読み込んで文字列で返す
	/// </summary>
	/// <param name="fileName"></param>
	/// <param name="Enc">指定しないと Encoding.UTF8 として読み込む</param>
	/// <returns></returns>
	public static string ReadFile(string fileName, Encoding Enc = null)
	{
		string resultText = "";
		FileInfo fi = new FileInfo(fileName);
		if (fi.Exists)
		{
			if (Enc == null) Enc = Encoding.UTF8;
			resultText = File.ReadAllText(fileName, Enc);
		}
		else
		{
			Debug.LogWarningFormat("{0}( {1} ) file not found !", GetClassMethodString(), fileName);
		}
		return resultText;
	}

	/// <summary>
	/// 呼び出した側のクラス名とメソッド名を日時ミリ秒と共に文字列として返すデバッグ用メソッド（スタティッククラスを含む全クラスで使用可能。但し、IL2CPPでないホロレンズ(.NET Standard 1.4相当)の場合クラス名が表示されません。）
	/// 使用例：Debug.Log(DB.GetClassMethodString());
	/// </summary>
	/// <param name="callerName"></param>
	/// <returns></returns>
	public static string GetClassMethodString([System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
	{
#if UNITY_EDITOR || !UNITY_METRO
		var className = new System.Diagnostics.StackFrame(1).GetMethod().ReflectedType.FullName;
#else
		var className = "Unknown";
#endif
#if UNITY_EDITOR
		return string.Format("{0}：{1}", className, callerName);
#else
		return string.Format("[{0}] {1}：{2}", TimeStr(), className, callerName);
#endif
	}


}
