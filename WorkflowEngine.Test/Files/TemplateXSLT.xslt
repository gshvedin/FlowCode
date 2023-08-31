<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xs="http://www.w3.org/2001/XMLSchema"
  exclude-result-prefixes="xs"
  version="3.0">
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>
  <xsl:template match="/">
    <elements>
      <xsl:for-each select="//*">
        <element name="{name(.)}">
          <xsl:value-of select="." />
        </element>
      </xsl:for-each>
    </elements>
  </xsl:template>
</xsl:stylesheet>