using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

public partial class _Default : System.Web.UI.Page
{
    public void DisplayError(string title, string message)
    {
        // Define the name, type and url of the client script on the page.
        Type cstype = this.GetType();

        // Get a ClientScriptManager reference from the Page class.
        ClientScriptManager cs = Page.ClientScript;

        // Check to see if the include script exists already.
        if (!cs.IsClientScriptIncludeRegistered(cstype, "error"))
        {
            StringBuilder csError = new StringBuilder();
            csError.AppendLine("<script type=\"text/javascript\">");
            csError.AppendLine("Sys.Application.add_load(errorLoadHandler);");
            csError.AppendLine("function errorLoadHandler(sender, args) {");
            csError.AppendLine(" alert('" + title + "');");
            csError.AppendLine("} </script>");
            cs.RegisterClientScriptBlock(cstype, "error", csError.ToString());
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        // Define the name, type and url of the client script on the page.
        Type cstype = this.GetType();

        // Get a ClientScriptManager reference from the Page class.
        ClientScriptManager cs = Page.ClientScript;

        // Check to see if the include script exists already.
        if (!cs.IsClientScriptIncludeRegistered(cstype, "jquery.easing.1.3.js"))
        {
            cs.RegisterClientScriptInclude(cstype, "jquery.easing.1.3.js", ResolveClientUrl("http://gsgd.co.uk/sandbox/jquery/easing/jquery.easing.1.3.js"));
        }
        this.DisplayError("Help", "Error");
    }
    
    /// <summary>
    /// Click event from create new trip button. Creates trip and redirects to ViewTrip
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_create_Click(object sender, EventArgs e)
    {
        string tripName = Trip_Name.Text;
        
        if (String.IsNullOrEmpty(tripName)) { return; }
        
        Trip trip = Trip.Create(tripName);

        //Trip creation was successfully, redirect to ViewTrip
        if (trip != null)
        {
            Session["trip"] = trip;
            Response.Redirect("/ViewTrip.aspx?id=" + trip.ID);
        }
        else
        {
            //TODO: Error handling
        }
    }
}