<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
	
	<xsl:param name="second"/>
	
	<xsl:variable name="sec_doc" select="document($second)//member"/>

	<xsl:template match="member">
		<xsl:element name="{name()}">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="child::node()"/>
			<xsl:variable name="name" select="@name"/>
			<xsl:apply-templates select="$sec_doc[@name = $name]/child::node()"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="img">
		<div class="section">
			<p class="subHeading"><xsl:value-of select="@alt"/></p>
			<xsl:element name="{name()}">
				<xsl:copy-of select="@*"/>
			</xsl:element>
		</div>
	</xsl:template>

	<xsl:template match="*">
		<xsl:element name="{name()}">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="child::node()"/>
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>
