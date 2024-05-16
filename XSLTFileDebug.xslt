<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>
	<xsl:template match="/root">
	
		<html>
			<head>
				<title>Payment details</title>
			</head>
			<body>
				<h1>Payment details</h1>
				<p>
					Service ID: <xsl:value-of select="serviceId"/>
				</p>
				<p>
					Phone: <xsl:value-of select="phone"/>
				</p>
				<p>
					Amount: <xsl:value-of select="amount"/>
				</p>
				<p>
					Account: <xsl:value-of select="account"/>
				</p>
				<table border="0">
					<tr>
						<th>Param</th>
						<th>Value</th>
					</tr> 
					<xsl:for-each select="AddParamConnector_Result">
						<tr>
							<td>
								<xsl:value-of select="param"/>
							</td>
							<td>
								<xsl:value-of select="value"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>