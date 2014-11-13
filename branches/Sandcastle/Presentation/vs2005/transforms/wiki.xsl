<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.1">

	<xsl:output method="text" indent="no" encoding="utf-8" />

	<xsl:param name="mediabase" />
	<xsl:param name="iconbase" />

	<xsl:template match="/">
		<xsl:text>#summary </xsl:text>
		<xsl:call-template name="FixText">
			<xsl:with-param name="text" select="html/head/title"/>
		</xsl:call-template>
		<xsl:text>

= </xsl:text>
		<xsl:call-template name="FixText">
			<xsl:with-param name="text" select="html/head/title"/>
		</xsl:call-template>
		<xsl:text> =
</xsl:text>

		<xsl:apply-templates select="html/body/div[@id='mainSection']/div[@id='mainBody']"/>
	</xsl:template>

	<xsl:template match="h1[@class='heading']">
		<xsl:text>
== </xsl:text>
		<xsl:call-template name="FixText">
			<xsl:with-param name="text" select="."/>
		</xsl:call-template>
		<xsl:text> ==
</xsl:text>
	</xsl:template>

	<xsl:template match="h4[@class='subHeading']">
		<xsl:text>
=== </xsl:text>
		<xsl:call-template name="FixText">
			<xsl:with-param name="text" select="."/>
		</xsl:call-template>
		<xsl:text> ===
</xsl:text>
	</xsl:template>

	<xsl:template match="p[@class='subHeading']">
		<xsl:text>
 ==== </xsl:text>
		<xsl:call-template name="FixText">
			<xsl:with-param name="text" select="."/>
		</xsl:call-template>
		<xsl:text> ====
</xsl:text>
	</xsl:template>

	<xsl:template match="span[@class='keyword']|span[@class='selflink']|span[@class='nolink']|b|strong">
		<xsl:text>*</xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>*</xsl:text>
	</xsl:template>

	<xsl:template match="span[@class='parameter']|i|em">
		<xsl:text>_</xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>_</xsl:text>
	</xsl:template>

	<xsl:template match="p[normalize-space(.)='']">
		<xsl:text>
</xsl:text>
	</xsl:template>

	<xsl:template match="br">
		<xsl:text disable-output-escaping="yes">&lt;br&gt;</xsl:text>
	</xsl:template>

	<xsl:template match="table">
		<xsl:text>
</xsl:text>
		<xsl:apply-templates select="tr"/>
	</xsl:template>

	<xsl:template match="tr">
		<xsl:text>|</xsl:text>
		<xsl:apply-templates select="td|th"/>
		<xsl:text>|
</xsl:text>
	</xsl:template>

	<xsl:template match="td">
		<xsl:text>| </xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text> |</xsl:text>
	</xsl:template>

	<xsl:template match="th">
		<xsl:text>| *</xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>* |</xsl:text>
	</xsl:template>

	<xsl:template match="a">
		<xsl:text>[</xsl:text>
		<xsl:choose>
			<xsl:when test="starts-with(@href,'http')">
				<xsl:value-of select="@href"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="translate(substring-before(@href,'.'),'`','_')"/>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:text> </xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>]</xsl:text>
	</xsl:template>

	<xsl:template match="span[@class='code']">
		<xsl:text>{{{</xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>}}}</xsl:text>
	</xsl:template>

	<xsl:template match="div[@class='summary' and not(p|ol|ul)]|div[@class='seeAlsoStyle']|p|dt|ol|ul">
		<xsl:text>
</xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>
</xsl:text>
	</xsl:template>

	<xsl:template match="dd|dd/p|dd/ol|dd/ul|dd/div[@class='summary' and (p|ol|ul)]/*">
		<xsl:text>
 </xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>
 </xsl:text>
	</xsl:template>

	<xsl:template match="dd/div[@class='summary' and not(p|ol|ul)]">
		<xsl:apply-templates select="node()"/>
		<xsl:text disable-output-escaping="yes">&lt;br&gt;</xsl:text>
	</xsl:template>

	<xsl:template match="ul/li">
		<xsl:text> * </xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>
</xsl:text>
	</xsl:template>

	<xsl:template match="ol/li">
		<xsl:text> # </xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>
</xsl:text>
	</xsl:template>

	<xsl:template match="dd/ul/li|dd/div[@class='summary']/ul/li">
		<xsl:text> * </xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>
 </xsl:text>
	</xsl:template>

	<xsl:template match="dd/ol/li|dd/div[@class='summary']/ol/li">
		<xsl:text> # </xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>
 </xsl:text>
	</xsl:template>

	<xsl:template match="li/p|li/div[@class='summary' and not(p)]|li/div[@class='summary']/p">
		<xsl:apply-templates select="node()"/>
		<xsl:text disable-output-escaping="yes">&lt;br&gt;</xsl:text>
	</xsl:template>

	<xsl:template match="div[@class='summary' and (p|ol|ul)]">
		<xsl:apply-templates select="*"/>
	</xsl:template>

	<xsl:template match="td/div[@class='summary']|td/p">
		<xsl:apply-templates select="node()"/>
	</xsl:template>

	<xsl:template match="span[@class='languageSpecificText']">
		<xsl:value-of select="span[@class='cs']"/>
	</xsl:template>

	<xsl:template match="div[@class='section']/img">
		<xsl:value-of select="$mediabase"/>
		<xsl:value-of select="@src"/>
		<xsl:text>
</xsl:text>
	</xsl:template>

	<xsl:template match="div[@class='alert']">
		<xsl:text>
</xsl:text>
		<xsl:value-of select="$iconbase"/>
		<xsl:text>alert_note.gif </xsl:text>
		<xsl:apply-templates select="node()"/>
		<xsl:text>
</xsl:text>
	</xsl:template>

	<xsl:template match="div[@id='allHistory']">
	</xsl:template>

	<xsl:template match="font|input|img[@id]">
	</xsl:template>
	
	<xsl:template match="text()">
		<xsl:if test="normalize-space(.)!=' ' and normalize-space(.)!=''">
			<xsl:choose>
				<xsl:when test="preceding-sibling::node() and following-sibling::node()">
					<xsl:variable name="norm" select="normalize-space(concat('[',.,']'))"/>
					<xsl:call-template name="FixText">
						<xsl:with-param name="text" select="substring($norm,2,string-length($norm)-2)"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:when test="following-sibling::node()">
					<xsl:variable name="norm" select="normalize-space(concat(.,']'))"/>
					<xsl:call-template name="FixText">
						<xsl:with-param name="text" select="substring($norm,1,string-length($norm)-1)"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:when test="preceding-sibling::node()">
					<xsl:variable name="norm" select="normalize-space(concat('[',.))"/>
					<xsl:call-template name="FixText">
						<xsl:with-param name="text" select="substring($norm,2)"/>
					</xsl:call-template>
				</xsl:when>
				<xsl:otherwise>
					<xsl:call-template name="FixText">
						<xsl:with-param name="text" select="normalize-space(.)"/>
					</xsl:call-template>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>

	<xsl:template match="a/text()">
		<xsl:value-of select="normalize-space(.)"/>
	</xsl:template>

	<xsl:template name="FixText">
		<xsl:param name="text"/>
		<xsl:choose>
			<xsl:when test="contains($text,' ')">
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-before($text,' ')"/>
				</xsl:call-template>
				<xsl:text> </xsl:text>
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-after($text,' ')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="contains($text,'.')">
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-before($text,'.')"/>
				</xsl:call-template>
				<xsl:text>.</xsl:text>
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-after($text,'.')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="contains($text,'[')">
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-before($text,'[')"/>
				</xsl:call-template>
				<xsl:text>[**</xsl:text>
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-after($text,'[')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="contains($text,']')">
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-before($text,']')"/>
				</xsl:call-template>
				<xsl:text>]</xsl:text>
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-after($text,']')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="contains($text,'(')">
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-before($text,'(')"/>
				</xsl:call-template>
				<xsl:text>(</xsl:text>
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-after($text,'(')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="contains($text,')')">
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-before($text,')')"/>
				</xsl:call-template>
				<xsl:text>)</xsl:text>
				<xsl:call-template name="FixText">
					<xsl:with-param name="text" select="substring-after($text,')')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="counter" select="translate($text,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','..........................')"/>
				<xsl:if test="string-length($text)>1 and starts-with($counter,'.') and contains(substring($counter,2),'.') and not(contains($counter,'..')) and substring($counter,string-length($counter))!='.'">!</xsl:if>
				<xsl:value-of select="$text"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
