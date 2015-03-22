using System;
using System.Configuration;
using System.Web;

namespace TfsWorkItemHelp
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var contentUri = BuildContentUri();

            if (contentUri != null)
            {
                Response.Redirect(contentUri, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        string BuildContentUri()
        {
            var project = Request.QueryString["Project"];
            var workItemType = Request.QueryString["WIT"];
            var oldState = Request.QueryString["OldState"];
            var newState = Request.QueryString["NewState"];

            if (project == null || workItemType == null || newState == null)
                return null;

            try
            {
                var baseUri = ConfigurationManager.AppSettings["ContentBaseUri"];
                var contentFileFormat = ConfigurationManager.AppSettings["ContentFileFormat"];
                var contentFile = contentFileFormat.Replace("$(Project)", project).Replace("$(WIT)", workItemType).Replace("$(OldState)", oldState).Replace("$(NewState)", newState);
                return baseUri + contentFile;
            }
            catch (ConfigurationErrorsException)
            {
                return null;
            }


        }
    }
}