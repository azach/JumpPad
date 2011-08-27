using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Init(object sender, EventArgs e)
    {        
        // Define the name, type and url of the client script on the page.
        Type cstype = this.GetType();

        // Get a ClientScriptManager reference from the Page class.
        ClientScriptManager cs = Page.ClientScript;

        // Register scripts used site-wide
        if (!cs.IsClientScriptIncludeRegistered(cstype, "jquery.min.js"))
        {
            cs.RegisterClientScriptInclude(cstype, "jquery.min.js", ResolveClientUrl("/scripts/jquery.min.js"));
        }
        if (!cs.IsClientScriptIncludeRegistered(cstype, "jquery.watermarkinput.js"))
        {
            cs.RegisterClientScriptInclude(cstype, "jquery.watermarkinput.js", ResolveClientUrl("/scripts/jquery.watermarkinput.js"));
        }
    }
}
