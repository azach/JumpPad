using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Runtime.Serialization.Json;
using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for PatientService
/// </summary>
[WebService(Namespace = "JumpPad")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService] //Allow javascript to access web service
public class TripService : System.Web.Services.WebService {

    /// <summary>
    /// Sets the latitude and longitude for a segment
    /// </summary>
    /// <param name="segment">Segment ID</param>
    /// <param name="lat">Latitude</param>
    /// <param name="lng">Longitude</param>
    [WebMethod]
    public void SetLocation(string segment, string lat, string lng)
    {
        //TODO: Validation against trip

        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
        MySqlConnection connection = new MySqlConnection(connString);
        try
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Segments SET Latitude=@latitude, Longitude=@longitude WHERE Segment_ID=@id";
            command.Parameters.Add("latitude", MySqlDbType.String).Value = lat;
            command.Parameters.Add("longitude", MySqlDbType.String).Value = lng;
            command.Parameters.Add("id", MySqlDbType.Int32).Value = segment;

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        catch
        {
            connection.Close();
        }
    }

    [WebMethod]
    public int Create(string name) {
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

            return (int) command.LastInsertedId;
        }
        catch
        {
            connection.Close();
            Session["trip"] = "";
            return 0;
        }
    }
}
