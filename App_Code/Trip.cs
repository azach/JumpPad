using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;
using System.Data;
using MySql.Data.MySqlClient;

#region Enums
public enum Access
{
    Full = 0,
    ViewOnly = 1,
    None = 2
}
#endregion

#region Trip
/// <summary>
/// Summary description for Trip
/// </summary>
public class Trip
{
    public DataSet SegmentDataSet { get; private set; }

    public const string AuthSalt = "qeJ2Suy8";

    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Password { get; private set; }
    public Access Access { get; private set; }
    public Decimal Budget { get; private set; }

    public List<Segment> Segments { get; private set; }

    /// <summary>
    /// Populate trip object with database information
    /// </summary>
    /// <param name="tripId">ID of trip to retrieve from database</param>
    public Trip(string tripId) {

        this.ID = tripId;

        string connString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        MySqlConnection connection = new MySqlConnection(connString);
        MySqlDataReader reader = null;

        try
        {
            //Get Trip info
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Name, Access, Password, Budget FROM Trips WHERE Trip_ID = @id";
            command.Parameters.Add("id", MySqlDbType.Int32);
            command.Parameters["id"].Value = this.ID;

            connection.Open();
            reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                //Populate object information from DB
                reader.Read();
                this.Name = reader.GetValue(0).ToString();

                //Setup access information
                this.Access = (Access)reader.GetValue(1);
                if (this.Access != Access.Full)
                {
                    this.Password = reader.GetValue(2).ToString();
                }
                this.Budget = (Decimal)reader.GetValue(3);
            }
            /*else
            {
                throw new Exception("Trip does not exist in database.");
            }*/

            reader.Close();
            
            //Fill and store segment data
            MySqlDataAdapter dataset_Reader = new MySqlDataAdapter();
            dataset_Reader.SelectCommand = new MySqlCommand("SELECT * FROM Segments WHERE Segments.Trip_ID = @id", connection);
            dataset_Reader.SelectCommand.Parameters.Add("id", MySqlDbType.Int32).Value = tripId;
            SegmentDataSet = new DataSet();

            //Populate dataset for future manipulation -- from this point, make changes
            //to this object instead of using SQL connection
            dataset_Reader.Fill(SegmentDataSet, "Segments");

            dataset_Reader.Dispose();
        }
        catch (Exception e)
        {
            reader.Close();
            connection.Close();
            throw e;
        }
    }

    /// <summary>
    /// Commits any changes to segment data set to the database
    /// </summary>
    public void CommitChanges()
    {
        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        MySqlConnection connection = new MySqlConnection(connString);

        MySqlDataAdapter dsAdapter = new MySqlDataAdapter();        
        dsAdapter.SelectCommand = new MySqlCommand("SELECT * FROM Segments WHERE Segments.Trip_ID = " + this.ID, connection);

        MySqlCommandBuilder dsBuilder = new MySqlCommandBuilder(dsAdapter);

        dsBuilder.GetUpdateCommand();
        dsBuilder.GetInsertCommand();
        dsBuilder.GetDeleteCommand();

        try
        {
            dsAdapter.Update(this.SegmentDataSet, "Segments");
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.Write(e.ToString());
        }
        finally
        {
            dsBuilder.Dispose();
            dsAdapter.Dispose();
            connection.Dispose();
        }
    }

    /// <summary>
    /// Add a new segment to the DataSet for the trip. Appended to end of sequence.
    /// Call CommitChanges to finalize changes
    /// </summary>
    /// <param name="name">Name of segment</param>
    /// <param name="description">Description of segment (optional)</param>
    public void InsertSegment(string name, string description = "")
    {
        if (String.IsNullOrEmpty(name)) { return; }
        if (this.SegmentDataSet == null) { return; }

        DataRow segment_row = this.SegmentDataSet.Tables["Segments"].NewRow();

        segment_row["Trip_ID"] = this.ID;
        segment_row["Name"] = name;
        segment_row["Description"] = description;

        this.SegmentDataSet.Tables["Segments"].Rows.Add(segment_row);

        //TODO: Finalize changes less frequently (e.g. session end)
        this.CommitChanges();
    }

    /// <summary>
    /// Delete a segment from the DataSet for the trip.
    /// Call CommitChanges to finalize changes
    /// </summary>
    public void DeleteSegment(int segment_id)
    {
        if (this.SegmentDataSet == null) { return; }

        DataRow[] delRows = this.SegmentDataSet.Tables["Segments"].Select("Segment_ID = " + segment_id);

        if (delRows.Length == 0) { return; }

        foreach (DataRow row in delRows) {
            row.Delete();
        }

        //TODO: Finalize changes less frequently (e.g. session end)
        this.CommitChanges();
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
#endregion

#region Segment

public class Segment
{
    public string ID { get; private set; }
    public string Name { get; private set; }
    public DateTime Start_Date { get; private set; }
    public DateTime End_Date { get; private set; }
    public string Description { get; set; }    
}

#endregion