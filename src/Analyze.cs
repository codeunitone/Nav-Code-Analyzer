﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NavCodeReferenceGenerator
{
	static public class Analyze
	{
		public static void analyzeFile(string fileName)
		{
			string[] fileContent = File.ReadAllLines(fileName);

			Regex tableRegEx = new Regex("^OBJECT Table\\s\\d[0-9]{0,}");

			// the first line is always Object Type, ID and Name
			if (tableRegEx.IsMatch(fileContent[0]) == true)
			{
				analyzeTable(fileContent);
			}
		}

		static void analyzeTable(string[] fileContent)
		{
			string id = Regex.Replace(fileContent[0].Substring(13),@"\D","");
			string name = Regex.Replace(fileContent[0].Substring(13),@"\d","");
			
			Table table = new Table(id, name);

			List<Procedure> procedureList = new List<Procedure>();

			for (int i = 0; i < fileContent.Length; i++)
			{
				Regex procedureRegEx = new Regex("PROCEDURE\\s[A-Za-z0-9]{0,}\\@\\d[0-9]{0,}\\(");
				if (procedureRegEx.IsMatch(fileContent[i]) == true)
				{
					List<string> procedureContent = new List<string>();
					while (fileContent[i].Contains("BEGIN") != true)
					{
						procedureContent.Add(fileContent[i].TrimStart());
						i++;
					}
					procedureList.Add(new Procedure(procedureContent.ToArray()));
					
				}
				table.procedures = procedureList;
			}

			string fileName = "TAB" + table.id + ".json";
			JsonGenerator.Generate(table,fileName);
		}
	}
}
