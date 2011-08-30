using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;

public partial class ViewTrip : System.Web.UI.Page
{
    protected void btn_auth_Validate(object sender, ServerValidateEventArgs e)
    {
        e.IsValid = false;
    }
    /// <summary>
    /// Checks whether the user is authenticated for the trip
    /// </summary>
    /// <param name="tripId">ID of trip to check</param>
    /// <returns>True if the user is validated, false otherwise</returns>
    private bool Authenticated(string tripId)
    {
        Trip trip = GetTrip(tripId);
        if (trip == null) { return true; }

        try
        {
            string token = Trip.GetMD5Hash(Session.SessionID);
            if (Request.Cookies["authtoken"].Value.Equals(Session[token].ToString()))
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Click event for authorize button. Validates the user's credentials against the locked trip
    /// and sets cookies for session authentication.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_auth_Click(object sender, EventArgs e)
    {
        Trip trip = GetTrip(Request["id"]);
        if (trip == null) { return; }

        string password = TripAuthPassword.Text;

        //Clear input box
        TripAuthPassword.Text = "";

        if (String.IsNullOrWhiteSpace(password)) { return; }

        string hash = Trip.GetSHA1Hash(password);
        
        if (hash == trip.Password)
        {
            string token = Trip.GetMD5Hash(Session.SessionID);
            HttpCookie cookie = new HttpCookie("authtoken", token);
            Response.Cookies.Add(cookie);
            Session[token] = trip.ID;
            //Response.Cookies["authsession"][hash] = trip.ID;
            //Response.Cookies["authsession"].Expires = DateTime.Now.AddHours(1);
            //Redirect to prevent accidental resubmission
            Response.Redirect(Request.Url.ToString(), false);
        }
    }

    protected void btn_delete_Segment(object sender, EventArgs e)
    {
        Trip trip = GetTrip(Request["id"]);
        if (trip == null) { return; }

        if (String.IsNullOrEmpty(Delete_Segment_ID.Value)) { return; }        

        trip.DeleteSegment(Convert.ToInt32(Delete_Segment_ID.Value));

        Segment_Menu.DataSource = trip.SegmentDataSet;
        Segment_Menu.DataBind();
        Segment_Content.DataSource = trip.SegmentDataSet;
        Segment_Content.DataBind();
    }

    /// <summary>
    /// Click event for Add New Segment button. Inserts a new segment into the trip based on the provided name/description.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_new_segment(object sender, EventArgs e)
    {
        //TODO: Better error handling
        if (String.IsNullOrWhiteSpace(Segment_Name.Text)) { return; }       
        Trip trip = GetTrip(Request["id"]);
        if (trip == null) { return; }

        trip.InsertSegment(Segment_Name.Text, Segment_Description.Text);
       
        Segment_Menu.DataSource = trip.SegmentDataSet;
        Segment_Menu.DataBind();

        Segment_Content.DataSource = trip.SegmentDataSet;
        Segment_Content.DataBind();
    }

    /// <summary>
    /// Updates the trip's access status when the Lock/View Only button is clicked
    /// </summary>
    protected void btn_set_access(object sender, EventArgs e)
    {
        #region LoadTrip
        Control senderControl = (Control)sender;

        Trip trip = GetTrip(Request["id"]);

        if (trip == null) { return; }
        
        #endregion LoadTrip


        string password;
        
        switch(senderControl.ID) 
        {
            case "TripUnlockButton":
                password = TripUnlockPassword.Text;
                TripUnlockPassword.Text = "";
                break;
            default:
                password = TripLockPassword.Text;
                TripLockPassword.Text = "";
                break;
        }

        if (String.IsNullOrWhiteSpace(password)) { return; }
        
        //Authenticate the user based on provided password
        //TODO: Display error to user
        string hash = Trip.GetSHA1Hash(password);
        if ((trip.Password != null) && (trip.Password != hash)) { return; }

        //Set access depending on which action was taken
        switch (senderControl.ID)
        {
            case "TripUnlockButton":
                trip.SetAccess(Access.Full);
                TripLockIcon.CssClass = "unlocked";
                break;
            case "TripLockButton":
                trip.SetAccess(Access.None, hash);
                TripLockIcon.CssClass = "locked";
                break;
            case "TripViewOnlyButton":
                trip.SetAccess(Access.ViewOnly, hash);
                TripLockIcon.CssClass = "locked";
                break;
        }        
        
        //Redirect to avoid accidentally resubmission form
        Response.Redirect("/ViewTrip.aspx?id=" + trip.ID);
    }

    /// <summary>
    /// Page load event for ViewTrip.aspx
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        //Initialize trip
        Trip trip = GetTrip(Request["id"]);

        //Trip does not exist or could not be created; redirect to default page
        //TODO: Display error page instead
        if (trip == null) { Response.Redirect("/Default.aspx"); }

        bool authenticated = Authenticated(trip.ID);

        //Build page -----------------------------------

        // Define the name, type and url of the client script on the page.
        Type cstype = this.GetType();

        // Get a ClientScriptManager reference from the Page class.
        ClientScriptManager cs = Page.ClientScript;

        // Check to see if the include script exists already.
        if (!cs.IsClientScriptIncludeRegistered(cstype, "jquery.tools.min.js"))
        {
            cs.RegisterClientScriptInclude(cstype, "jquery.tools.min.js", ResolveClientUrl("http://cdn.jquerytools.org/1.2.5/jquery.tools.min.js"));
        }

        //Trip header
        this.Title = trip.Name;

        //Lock icon
        TripLockIcon.CssClass = (trip.Access != Access.Full) ? "locked" : "unlocked";
        TripLockIcon.Attributes.Add("rel", "#TripLockFormContainer");        

        //Set up trip lock form depending on trip's current access
        switch (trip.Access)
        {
            case Access.Full:
                TripLockIcon.Attributes.Add("rel", "#TripLockFormContainer");
                break;
            case Access.ViewOnly:
                TripLockIcon.Attributes.Add("rel", "#TripUnlockFormContainer");
                //Pencil icon
                TripEditIcon.Visible = true;
                TripEditIcon.Attributes.Add("rel", "#TripAuthFormContainer");
                TripNewSegment.Visible = authenticated;
                break;
            case Access.None:
                TripLockIcon.Attributes.Add("rel", "#TripUnlockFormContainer");
                TripNewSegment.Visible = authenticated;
                if (!authenticated)
                {
                    cs.RegisterClientScriptBlock(cstype, "TripAuthLoad", "<script type='text/javascript'>$(document).ready(function() { $('#TripAuthFormContainer').overlay({ load: true, closeOnClick: false, top: '27%' }); });</script>");
                }
                break;
        }

        //Stop here if user doesn't have access to view rest of trip

        if ((trip.Access == Access.None) && (!authenticated))
        {
            TripName.Text = "Private Trip";
            return;
        }

        //Trip header

        TripName.Text = trip.Name;

        //Load segment information
        if ((trip.Access != Access.None) | (trip.Access == Access.None && authenticated))
        {
            Segment_Menu.DataSource = trip.SegmentDataSet;
            Segment_Menu.DataBind();

            Segment_Content.DataSource = trip.SegmentDataSet;
            Segment_Content.DataBind();
        }

    }

    /// <summary>
    /// Return a Trip object from session based on the provided trip ID.
    /// Does not do validation, but will create Trip if validation has been done.
    /// </summary>
    /// <param name="tripId">ID of trip to retrieve</param>
    /// <returns>Initialized trip pulled from session</returns>
    private Trip GetTrip(string tripId)
    {
        if (String.IsNullOrWhiteSpace(tripId)) { return null; }

        Trip trip;

        if (Session["trip"] != null)
        {
            trip = (Trip)Session["trip"];
            if (trip.ID == tripId) { return trip; }
        }
        
        //TODO: Validate before creating trip
        try { trip = new Trip(tripId); }
        catch { return null; }

        return trip;
    }
}