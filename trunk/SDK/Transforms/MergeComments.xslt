<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
	
	<xsl:param name="second"/>
	
	<xsl:variable name="sec_doc" select="document($second)//member"/>

	<xsl:template match="member">
		<xsl:element name="{name()}">
			<xsl:copy-of select="@*"/>
			<xsl:variable name="name" select="@name"/>
			<xsl:if test="summary|$sec_doc[@name = $name]/summary">
				<xsl:element name="summary">
					<xsl:apply-templates select="summary/child::node()"/>
					<xsl:apply-templates select="$sec_doc[@name = $name]/summary/child::node()"/>
				</xsl:element>
			</xsl:if>
			<xsl:apply-templates select="*[not(self::summary) and not(self::remarks)]"/>
			<xsl:apply-templates select="$sec_doc[@name = $name]/*[not(self::summary) and not(self::remarks)]"/>
			<xsl:if test="remarks|$sec_doc[@name = $name]/remarks">
				<xsl:element name="remarks">
					<xsl:apply-templates select="remarks/child::node()"/>
					<xsl:apply-templates select="$sec_doc[@name = $name]/remarks/child::node()"/>
				</xsl:element>
			</xsl:if>
		</xsl:element>
	</xsl:template>

	<xsl:template match="div/img">
		<xsl:element name="{name()}">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates select="child::node()"/>
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
