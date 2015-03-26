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

  <xsl:template match="ShelvesetCleanup">
    <head>
      <!-- Pull in the command style settings -->
      <xsl:call-template name="style">
      </xsl:call-template>
    </head>
    <body>
      <h1>Shelveset Cleanup Job</h1>
      <p>
        The following lists show shelvesets for Team Project Collection <xsl:value-of select="@collection"/> on Team Foundation Server <xsl:value-of select="@server"/> assigned to you that have are older than
        <xsl:value-of select="@warnAge"/> days. They will be automatically deleted, if they reach the age of <xsl:value-of select="@delAge"/> days.
      </p>
      <table>
        <tr>
          <td class="ColHeading">Shelveset</td>
          <td class="ColHeading">Comment</td>
          <td class="ColHeading">Created (Age)</td>
        </tr>
        <xsl:for-each select="Shelveset">
          <tr>
            <td class="ColData">
              <xsl:value-of select="@name"/>
            </td>
            <td class="ColData">
              <xsl:value-of select="@comment"/>
            </td>
            <td class="ColData">
              <xsl:value-of select="@created"/> (<xsl:value-of select="@age"/>)
            </td>
          </tr>
        </xsl:for-each>
      </table>
      <p class="footer">
        <br/>
        This email has been automatically generated on <xsl:value-of select="@dateGenerated"/> for user <xsl:value-of select="@ownerDisplayName"/> (<xsl:value-of select="@owner"/>).
      </p>
    </body>
  </xsl:template>
</xsl:stylesheet>
