<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TfsWorkItemHelp.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href="Styles.css" />
    <title>TFS Work Item Help</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Welcome to the TFS Work Item Help</h1>
        </div>
        <div>
            To use TFS Work Item Help please follow the following steps:
        </div>
        <div>
            <ol>
                <li class="ordered">
                    <p>
                        <strong>Configure application</strong><br />
                        Correct the values for the two configuration options in <em>web.config</em>
                    </p>
                    <table>
                        <tr>
                            <th>Option</th>
                            <th>Description</th>
                        </tr>
                        <tr>
                            <td><strong>ContentBaseUri</strong></td>
                            <td>Base URI where help content is stored. Usually this should be a relative URI (e.g. ~/Content/).</td>
                        </tr>
                        <tr>
                            <td><strong>ContentFileFormat</strong></td>
                            <td>Format string that is used to build the path and name of the help content file.<br />
                                You can use the following variables:
                                <ul style="margin: 10px 0 10px 20px;">
                                    <li><strong>$(Project)</strong> - Team project name</li>
                                    <li><strong>$(WIT)</strong> - Work item type name</li>
                                    <li><strong>$(OldState)</strong> - Original state of the work item as loaded from the database</li>
                                    <li><strong>$(NewState)</strong> - Current state of the work item as edited by the user</li>
                                </ul>
                                <strong>Example:</strong> $(Project)/$(WIT)/$(OldState)$(NewState).html
                            </td>
                        </tr>
                    </table>
                </li>
                <li class="ordered">
                    <p>
                        <strong>Configure Work Item Types</strong><br />
                        In each work item type add a <em>WebpageControl</em> and set its URL to the TFS Work Item Help URL. Add the necessary URL parameters (see <em>ContentFileFormat</em>)
                        and make sure that the control is set to reload its contents on parameter change. You can use the following <em>WebpageControloptionsType</em> element for reference.
                    </p>
                    <pre style="font: 12px Consolas; margin:0">
    &lt;WebpageControlOptions AllowScript="true" ReloadOnParamChange="true"&gt;
      &lt;Link UrlRoot="<%= Request.Url %>" UrlPath="?Project={0}&amp;amp;WIT={1}&amp;amp;OldState={2}&amp;amp;NewState={3}"&gt;
        &lt;Param Index="0" Value="System.TeamProject" /&gt;
        &lt;Param Index="1" Value="System.WorkItemType" /&gt;
        &lt;Param Index="2" Value="System.State" Type="Original" /&gt;
        &lt;Param Index="3" Value="System.State" Type="Current" /&gt;
      &lt;/Link&gt;
    &lt;/WebpageControlOptions&gt;</pre>
                </li>
                <li class="ordered">
                    <p>
                        <strong>Create help content</strong><br />
                        Create the help content files for each state and state transition of every work item type and store it according to the <em>ContentBaseUri</em>
                        and <em>ContentFileFormat</em> configuration options. If you use the example values from the above description, a help file describing the creation
                        of a <em>Task</em> work item in a team project called <em>MyProject</em> would be named <em>To Do.html</em> and stored in a folder named
                        <em>Content/MyProject/Task</em> beneath the TFS Work Item Help web application root folder.
                    </p>
                </li>
            </ol>
        </div>
    </form>
</body>
</html>
