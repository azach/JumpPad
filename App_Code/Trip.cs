using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

public enum Access
{
    Full = 0,
    ViewOnly = 1,
    None = 2
}

public class Segment
{
    public string ID { get; private set; }
    public string Name { get; private set; }
    public DateTime Start_Date { get; private set; }
    public DateTime End_Date { get; private set; }
    public string Description { get; set; }
}

/// <summary>
/// Summary description for Trip
/// </summary>
public class Trip
{
    public const string AuthSalt = "qeJ2Suy8";

    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Password { get; private set; }
    public Access Access { get; private set; }
    public Decimal Budget { get; private set; }

    public List<Segment> Segments { get; private set; }

	public Trip()
	{
	}

    /// <summary>
    /// Populate trip object with database information
    /// </summary>
    /// <param name="tripId">ID of trip to retrieve from database</param>
    public Trip(string tripId) {        
        this.ID = tripId;

        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        MySqlConnection connection = new MySqlConnection(connString);
        try
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Name, Access, Password, Budget FROM Trips WHERE Trip_ID = @id";
            command.Parameters.Add("id", MySqlDbType.Int32).Value = tripId;

            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();            

            if (reader.HasRows)
            {
                reader.Read();
                this.Name = reader.GetValue(0).ToString();

                //Setup access information
                this.Access = (Access) reader.GetValue(1);
                if (this.Access != Access.Full)
                {
                    this.Password = reader.GetValue(2).ToString();
                }
                this.Budget = (Decimal) reader.GetValue(3);
            }
            else
            {
                throw new Exception("Trip does not exist in database.");
            }

            connection.Close();
        }
        catch(Exception e)
        {
            connection.Close();
            throw e;
        }
    }

    
    /// <summary>
    /// Sets a trip to a given access status
    /// </summary>
    public void SetAccess(Access type)
    {
        this.Access = type;
        int typeInt = (int)type;
        this.SetValue("Access", typeInt.ToString(), MySqlDbType.Int32);

        //Shouldn't have a password
        if (type == Access.Full)
        {
            this.SetValue("Password", "", MySqlDbType.String);
        }
    }

    public void SetAccess(Access type, string hash)
    {
        this.Access = type;
        int typeInt = (int)type;
        this.SetValue("Access", typeInt.ToString(), MySqlDbType.Int32);
        this.SetValue("Password", hash, MySqlDbType.String);
    }

    /// <summary>
    /// Generate salted MD5 hash of input string. Used as an authentication token for cookies.
    /// </summary>
    /// <param name="input">Session ID</param>
    /// <returns>Hash string</returns>
    public static string GetMD5Hash(string input)
    {
        var hmacMD5 = new System.Security.Cryptography.HMACMD5(System.Text.Encoding.UTF8.GetBytes(AuthSalt));
        var saltedHash = hmacMD5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(saltedHash);
    }

    /// <summary>
    /// Generate salted SHA1 hash of input string. Used to store trip password.
    /// </summary>
    /// <param name="input">String to encrypt</param>
    /// <returns>Hashed string</returns>
    public static string GetSHA1Hash(string input)
    {
        var hmacSHA1 = new System.Security.Cryptography.HMACSHA1(System.Text.Encoding.UTF8.GetBytes(AuthSalt));
        var saltedHash = hmacSHA1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(saltedHash);
    }

    public static Trip Create(string name)
    {
        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        MySqlConnection connection = new MySqlConnection(connString);
        try
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Trips (Name) VALUES (@name)";
            command.Parameters.Add("name", MySqlDbType.String).Value = name;

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return new Trip(command.LastInsertedId.ToString());
        }
        catch
        {
            connection.Close();
            return null;
        }
    }
    
    private bool SetValue(string column, string value, MySqlDbType type)
    {
        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        MySqlConnection connection = new MySqlConnection(connString);
        try
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Trips SET " + column + "=@value WHERE Trip_ID=@id";
            command.Parameters.Add("value", type).Value = value;
            command.Parameters.Add("id", MySqlDbType.Int32).Value = this.ID;

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            return true;
        }
        catch(Exception e)
        {
            connection.Close();
            return false;
        }     
    }
}