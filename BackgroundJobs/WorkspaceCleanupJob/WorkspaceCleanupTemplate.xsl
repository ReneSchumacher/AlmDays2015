<?xml version="1.0" encoding="utf-8"?>
<!--
//////////////////////////////////////////////////////////////////////////////////
// This file is part of a Microsoft sample.
//
// (c) 2013 Microsoft Corporation. All rights reserved. 
// 
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//////////////////////////////////////////////////////////////////////////////////
-->

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:import href="TeamFoundation.xsl"/>

  <xsl:template match="WorkspaceCleanup">
    <head>
      <!-- Pull in the command style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <body>
      <h1>Workspace Cleanup Job</h1>
      <p>
        The following lists show workspaces for Team Project Collection <xsl:value-of select="@collection"/> on Team Foundation Server <xsl:value-of select="@server"/> assigned to you that have not been accessed
        for at least <xsl:value-of select="@warnAge"/> days. To prevent them from being deleted, you must access each workspace before it reaches the age of <xsl:value-of select="@delAge"/> days.
      </p>
      <table>
        <tr>
          <td class="ColHeading">Computer</td>
          <td class="ColHeading">Workspace</td>
          <td class="ColHeading">Comment</td>
          <td class="ColHeading">Type</td>
          <td class="ColHeading">Last Accessed (Age)</td>
        </tr>
        <xsl:for-each select="Workspace">
          <tr>
            <td class="ColData">
              <xsl:value-of select="@computer"/>
            </td>
            <td class="ColData">
              <xsl:value-of select="@name"/>
            </td>
            <td class="ColData">
              <xsl:value-of select="@comment"/>
            </td>
            <td class="ColData">
              <xsl:value-of select="@type"/>
            </td>
            <td class="ColData">
              <xsl:value-of select="@lastAccessed"/> (<xsl:value-of select="@age"/>)
            </td>
          </tr>
        </xsl:for-each>
      </table>
      <p>
        <b>Note:</b> Deleting the workspace does <b>not</b> delete the local data on the machine the workspace resides on. Nevertheless, you will not be able to check in or undo any of the existing pending changes
        once the workspace has been deleted.
      </p>
      <p class="footer">
        <br/>
        This email has been automatically generated on <xsl:value-of select="@dateGenerated"/> for user <xsl:value-of select="@ownerDisplayName"/> (<xsl:value-of select="@owner"/>).
      </p>
    </body>
  </xsl:template>
  
</xsl:stylesheet>
