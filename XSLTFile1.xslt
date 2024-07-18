<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fn="urn:custom-functions" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl fn">
	<xsl:output method="xml" indent="no" omit-xml-declaration="yes"/>

	<!-- Шаблон для обработки корневого элемента -->
	<xsl:template match="/">
		<root>
			<xsl:for-each select="root/inputParam">
		 
					<inputParam>
						<orderid>
							<xsl:value-of select="normalize-space(orderid)"/>
						</orderid>
						<name>
							<xsl:value-of select="normalize-space(name)"/>
						</name>
						<kindId>
							<xsl:value-of select="normalize-space(kindId)"/>
						</kindId>
						<identifierType>
							<xsl:value-of select="normalize-space(identifierType)"/>
						</identifierType>
						<value>
							<xsl:value-of select="normalize-space(value)"/>
						</value>
						<xsl:choose>
							<xsl:when test="string-length(normalize-space(value)) > 0">
								<stage>newStageValue</stage>
								<!-- Замените newStageValue на нужное значение -->
							</xsl:when>
							<xsl:otherwise>
								<stage>
									<xsl:value-of select="normalize-space(stage)"/>
								</stage>
							</xsl:otherwise>
						</xsl:choose>
						<caption>
							<xsl:value-of select="normalize-space(caption)"/>
						</caption>
					</inputParam>
	 
			</xsl:for-each>
		</root>
	</xsl:template>
</xsl:stylesheet>
