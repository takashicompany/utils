namespace takashicompany.Unity
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.RegularExpressions;
	using UnityEngine;

	public class CSV
	{
		protected List<List<string>> _raw;

		public CSV(TextAsset csvFile)
		{
			_raw = LoadCSV(csvFile);
		}

		public string GetValue(int row, int col)
		{
			return GetValue(_raw, row, col);
		}

		/// <summary>
		/// CSVファイルを読み込んでパースする（ダブルクォーテーション対応）
		/// </summary>
		/// <param name="filePath">Resourcesフォルダ内のCSVファイルのパス（拡張子不要）</param>
		/// <returns>2次元リストとしてのCSVデータ</returns>
		public static List<List<string>> LoadCSV(TextAsset csvFile)
		{
			if (csvFile == null)
			{
				Debug.LogError("CSVファイルが見つかりません");
				return null;
			}

			List<List<string>> data = new List<List<string>>();
			StringReader reader = new StringReader(csvFile.text);

			while (reader.Peek() > -1)
			{
				string line = reader.ReadLine();
				List<string> parsedLine = ParseCSVLine(line);
				
				// 空の行を無視する
				if (parsedLine.Count > 0 && parsedLine.Exists(value => !string.IsNullOrWhiteSpace(value)))
				{
					data.Add(parsedLine);
				}
			}

			return data;
		}

		/// <summary>
		/// CSVの1行をパースし、リストとして返す（ダブルクォーテーション対応）
		/// </summary>
		/// <param name="line">CSVの1行の文字列</param>
		/// <returns>パースされた文字列リスト</returns>
		private static List<string> ParseCSVLine(string line)
		{
			List<string> result = new List<string>();
			string pattern = @"(?:^|,)(?:(?:""((?:[^""]|"""")*)"")|([^,]*))";
			Regex regex = new Regex(pattern);

			foreach (Match match in regex.Matches(line))
			{
				string value = match.Groups[1].Success ? match.Groups[1].Value.Replace("\"\"", "\"") : match.Groups[2].Value;
				result.Add(value);
			}

			return result;
		}

		/// <summary>
		/// CSVデータから指定した行・列の値を取得
		/// </summary>
		/// <param name="data">パース済みのCSVデータ</param>
		/// <param name="row">取得する行（0始まり）</param>
		/// <param name="col">取得する列（0始まり）</param>
		/// <returns>指定されたセルの値（範囲外なら空文字）</returns>
		public static string GetValue(List<List<string>> data, int row, int col)
		{
			if (row < 0 || row >= data.Count || col < 0 || col >= data[row].Count)
			{
				Debug.LogWarning("指定された行・列が範囲外です");
				return string.Empty;
			}
			return data[row][col];
		}
	}
}
