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

    public TripService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public void InsertNewSegment(string name)
    {
        if (Session["trip"] == null)
        {
            throw new Exception();
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
