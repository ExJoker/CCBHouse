using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 需要引用的DLL文件：
/// Mysql.Data.dll
/// 在unity中需要：
/// l18n.cjk.dll
/// l18n.dll
/// system.data.dll
/// system.drawing.dll
/// </summary>

public class DataDBContorller: MonoBehaviour
{
	//远程连接
	//  string connectionString= "User ID = ; Password =.; Host =; Port =3306;Database = ;Charset = ";
	//本地连接
	public static     string connectionString= "User ID =root ; Password =888888 ; Host =localhost ; Port = 3306;Database =ccb ;Charset =utf8";

	public static MySqlConnection dbConnection;

	static void Main(string[] args)
	{
	}

	 
        
	//打开数据库链接
	public static void OpenSqlConnection(string connectionString)
	{
		dbConnection = new MySqlConnection(connectionString);
		dbConnection.Open();
	}

	//关闭数据库连接
	public static void CloseConnection()
	{
		if (dbConnection != null)
		{
			dbConnection.Close();
			dbConnection.Dispose();
			dbConnection = null;
		}
	}

	//保存数据
	public static  DataSet GetDataSet(string sqlString)
	{
		DataSet ds = new DataSet();
		try
		{
			//用于检索和保存数据
			//Fill(填充)能改变DataSet中的数据以便于数据源中数据匹配
			//Update(更新)能改变数据源中的数据以便于DataSet中的数据匹配
              
			MySqlDataAdapter da = new MySqlDataAdapter(sqlString, dbConnection);
			da.Fill(ds);



		}
		catch (Exception ee)
		{
			throw new Exception("SQL:" + sqlString + "\n" + ee.Message.ToString());
		}
		return ds;



	}

	//增 insert
	static void Add()
	{
		OpenSqlConnection(connectionString);
		string sqlstring= "insert into userinformation(name,password,tel) values();";//保证sql语句的正确性
		GetDataSet(sqlstring);
		CloseConnection();
	}

	//删 delete
	static void Delete()
	{
		OpenSqlConnection(connectionString);
		string sqlstring = "delete from 表名;";
		GetDataSet(sqlstring);
		CloseConnection();
	}

	//改 update
	public static void Update(string sqlstring)
	{
		OpenSqlConnection(connectionString);
		GetDataSet(sqlstring);
		CloseConnection();

	}

	//查 select
	public static List<string[]> Select(String sql)
	{
		
		List<string[]> list =new List<string[]>();
		String[] temp;
		OpenSqlConnection(connectionString);
		MySqlCommand mysqlcommand = new MySqlCommand(sql, dbConnection);
		MySqlDataReader reader = mysqlcommand.ExecuteReader();
		temp=new string[reader.FieldCount];
		try
		{
			while (reader.Read())
			{
				if (reader.HasRows)
				{
					for (int i = 0; i < reader.FieldCount; i++)
					{
							try{
								temp[i]= reader.GetString(i);
							}
							catch(Exception)
							{
								temp[i]= "";
							}
							
					}
					list.Add(temp);
					temp=new string[reader.FieldCount];
				}
			}
			return list;
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
			return null;
		}
		finally
		{
			reader.Close();
			CloseConnection();
		}

	}
	static void ResultToString(List<string[]> list)
	{
		foreach(string[] s in list)
		{
			for(int i=0 ;i<s.Length ; i++)
			{
				Debug.Log(list.IndexOf(s)+":"+s[i]);
			}
		}
	}

	}


	
