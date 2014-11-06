<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.1">

	<!-- stuff specified to comments authored in DDUEXML -->
	<xsl:param name="omitXmlnsBoilerplate" select="'false'" />
	<xsl:variable name="languages" select="NONE" />

	<xsl:include href="htmlBody.xsl"/>
	<xsl:include href="utilities_bin.xsl" />

	<xsl:variable name="summary" select="normalize-space(/document/comments/summary)" />
	<xsl:variable name="abstractSummary" select="/document/comments/summary" />
	<xsl:variable name="hasSeeAlsoSection" select="boolean((count(/document/comments//seealso | /document/reference/elements/element/overloads//seealso) > 0)  or 
		($group='type' or $group='member' or $group='list'))"/>
	<xsl:variable name="examplesSection" select="false()"/>
	<xsl:variable name="omitVersionInformation">false</xsl:variable>

	<xsl:template name="body">
		<!-- auto-inserted info -->
		<!-- <xsl:apply-templates select="/document/reference/attributes" /> -->
		<!-- assembly information -->
		<xsl:if test="not($group='root' or $group='namespace')">
			<xsl:call-template name="requirementsInfo"/>
		</xsl:if>
		<xsl:apply-templates select="/document/comments/preliminary" />
		<xsl:apply-templates select="/document/comments/summary" />
		<!-- members -->
		<xsl:choose>
			<xsl:when test="$group='root'">
				<xsl:apply-templates select="/document/reference/elements" mode="root" />
			</xsl:when>
			<xsl:when test="$group='namespace'">
				<xsl:apply-templates select="/document/reference/elements" mode="namespace" />
			</xsl:when>
			<xsl:when test="$subgroup='enumeration'">
				<xsl:apply-templates select="/document/reference/elements" mode="enumeration" />
			</xsl:when>
			<xsl:when test="$group='type'">
				<xsl:apply-templates select="/document/reference/elements" mode="type" />
			</xsl:when>
			<xsl:when test="$group='member'">
				<xsl:apply-templates select="/document/reference" mode="field" />
			</xsl:when>
		</xsl:choose>
		<!-- remarks -->
		<xsl:apply-templates select="/document/comments/remarks" />
		<!-- other comment sections -->
		<!-- inheritance -->
		<xsl:apply-templates select="/document/reference/family" />
		<!--versions-->
		<xsl:if test="not($group='namespace' or $group='root' )">
			<xsl:apply-templates select="/document/reference/versions" />
		</xsl:if>
		<!-- see also -->
		<xsl:call-template name="seealso" />

	</xsl:template> 

	<xsl:template name="getConditionDescription">
		<xsl:apply-templates select="/document/comments/condition_summary" />
	</xsl:template>

	<xsl:template name="getLengthDescription">
		<xsl:apply-templates select="/document/comments/length_summary" />
	</xsl:template>

	<xsl:template name="getCountDescription">
		<xsl:apply-templates select="/document/comments/count_summary" />
	</xsl:template>

	<xsl:template name="getElementDescription">
		<xsl:apply-templates select="summary[1]" />
	</xsl:template>

	<xsl:template name="getEnumMemberDescription">
		<xsl:apply-templates select="summary/node()" />
		<!-- enum members may have additional authored content in the remarks node -->
		<xsl:apply-templates select="remarks/node()" />
	</xsl:template>


	<!-- block sections -->

	<xsl:template match="reference" mode="field">
		<xsl:variable name="binData" select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinDataAttribute']" />
		<xsl:variable name="formatArg" select="$binData/argument[type/@api = 'T:FRAFV.Binary.Serialization.BinFormat']|$binData/assignment[@name = 'Format']"/>
		<xsl:variable name="hasEncoding" select="$formatArg/enumValue/field[@name = 'CString' or @name = 'PString' or @name = 'Char'] or not($formatArg) and returns//type[@api = 'T:System.String' or @api = 'T:System.Char']"/>
		<xsl:variable name="hasLength" select="$formatArg/enumValue/field[@name = 'Binary' or @name = 'PString'] or not($formatArg) and returns/arrayOf/type[@api = 'T:System.Byte' or @api = 'T:System.Char']"/>

		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="'field'" />
			<xsl:with-param name="title">
				<include item="fieldDataTitle"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:if test="$hasEncoding">
					<xsl:apply-templates select="containers//type/attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinBlockAttribute' and assignment[@name = 'BinaryReaderType' or @name = 'BinaryWriterType']][1]" mode="serializer" />
				</xsl:if>
				<xsl:if test="$hasLength and $binData[not(assignment[@name = 'LengthCustomMethod']/value!='') or assignment[@name = 'LengthFormat']]">
					<xsl:apply-templates select="." mode="length"/>
				</xsl:if>
				<xsl:if test="$binData">
					<xsl:apply-templates select="." mode="data"/>
				</xsl:if>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="attribute" mode="serializer">
		<xsl:call-template name="subSection">
			<xsl:with-param name="title">
				<include item="serializerTitle"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:apply-templates select="assignment[@name = 'BinaryReaderType' or @name = 'BinaryWriterType']" mode="serializer"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

		<xsl:template match="assignment" mode="serializer">
		<dl serializerType="{@name}">
			<dt>
				<span class="parameter">
					<include item="{concat(@name,'Title')}"/>
				</span>
			</dt>
			<dd>
				<include item="typeLink">
					<parameter>
						<!--xsl:apply-templates select="value" mode="link">
							<xsl:with-param name="qualified" select="true()" />
						</xsl:apply-templates-->
						<referenceLink target="{value}" prefer-overload="false" show-templates="true" show-container="true"/>
					</parameter>
					<parameter/>
				</include>
				<br />
				<xsl:call-template name="getElementDescription" />
			</dd>
		</dl>
	</xsl:template>

	<xsl:template match="reference" mode="data">
		<xsl:call-template name="subSection">
			<xsl:with-param name="title">
				<include item="fieldValueTitle"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:apply-templates select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinDataAttribute']" mode="field">
					<xsl:with-param name="returns" select="returns"/>
				</xsl:apply-templates>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="reference" mode="length">
		<xsl:call-template name="subSection">
			<xsl:with-param name="title">
				<include item="fieldLengthTitle"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:apply-templates select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinDataAttribute']" mode="length"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="summary">
		<div class="summary">
			<xsl:apply-templates />
		</div>
	</xsl:template>

	<xsl:template match="value">
		<xsl:call-template name="subSection">
			<xsl:with-param name="title">
				<include item="fieldValueTitle" />
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:apply-templates />
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="returns">
		<xsl:call-template name="subSection">
			<xsl:with-param name="title">
				<include item="propertyValueTitle" />
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:apply-templates />
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="templates">
		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="'templates'" />
			<xsl:with-param name="title">
				<include item="templatesTitle" />
			</xsl:with-param>
			<xsl:with-param name="content">
				<dl>
					<xsl:for-each select="template">
						<xsl:variable name="templateName" select="@name" />
						<dt>
							<span class="parameter">
								<xsl:value-of select="$templateName"/>
							</span>
						</dt>
						<dd>
							<xsl:apply-templates select="/document/comments/typeparam[@name=$templateName]" />
						</dd>
					</xsl:for-each>
				</dl>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="remarks">
		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="'remarks'"/>
			<xsl:with-param name="title"><include item="remarksTitle" /></xsl:with-param>
			<xsl:with-param name="content"><xsl:apply-templates /></xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="para">
		<p><xsl:apply-templates /></p>
	</xsl:template>

<xsl:template name="seealso">
	<xsl:if test="$hasSeeAlsoSection">
		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="'seeAlso'" />
			<xsl:with-param name="title">
				<include item="relatedTitle" />
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:call-template name="autogenSeeAlsoLinks"/>
				<xsl:for-each select="/document/comments//seealso | /document/reference/elements/element/overloads//seealso">
					<div class="seeAlsoStyle">
						<xsl:apply-templates select=".">
							<xsl:with-param name="displaySeeAlso" select="true()" />
						</xsl:apply-templates>
					</div>
				</xsl:for-each>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

	<xsl:template match="list[@type='bullet']">
		<ul>
			<xsl:for-each select="item">
				<li><xsl:apply-templates /></li>
			</xsl:for-each>
		</ul>
	</xsl:template>

	<xsl:template match="list[@type='number']">
		<ol>
			<xsl:for-each select="item">
				<li><xsl:apply-templates /></li>
			</xsl:for-each>
		</ol>
	</xsl:template>

	<xsl:template match="list[@type='table']">
		<div class="tableSection">
		<table width="100%" cellspacing="2" cellpadding="5" frame="lhs" >
			<xsl:for-each select="listheader">
				<tr>
					<xsl:for-each select="*">
						<th><xsl:apply-templates /></th>
					</xsl:for-each>
				</tr>
			</xsl:for-each>
			<xsl:for-each select="item">
				<tr>
					<xsl:for-each select="*">
						<td>
							<xsl:apply-templates />
						</td>
					</xsl:for-each>
				</tr>
			</xsl:for-each>
		</table>
		</div>
	</xsl:template>

	<!-- inline tags -->

	<xsl:template match="see[@cref]">
		<xsl:choose>
			<xsl:when test="normalize-space(.)">
				<referenceLink target="{@cref}">
					<xsl:value-of select="." />
				</referenceLink>
			</xsl:when>
			<xsl:otherwise>
				<referenceLink target="{@cref}"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="see[@href]">
		<xsl:choose>
			<xsl:when test="normalize-space(.)">
				<a>
					<xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute>
					<xsl:value-of select="." />
				</a>
			</xsl:when>
			<xsl:otherwise>
				<a>
					<xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute>
					<xsl:value-of select="@href" />
				</a>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="seealso[@href]">
		<xsl:param name="displaySeeAlso" select="false()" />
		<xsl:if test="$displaySeeAlso">
		<xsl:choose>
			<xsl:when test="normalize-space(.)">
				<a>
					<xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute>
					<xsl:value-of select="." />
				</a>
			</xsl:when>
			<xsl:otherwise>
				<a>
					<xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute>
					<xsl:value-of select="@href" />
				</a>
			</xsl:otherwise>
		</xsl:choose>
		</xsl:if>
	</xsl:template>

	<xsl:template match="seealso">
		<xsl:param name="displaySeeAlso" select="false()" />
		<xsl:if test="$displaySeeAlso">
		<xsl:choose>
			<xsl:when test="normalize-space(.)">
				<referenceLink target="{@cref}" qualified="true">
					<xsl:value-of select="." />
				</referenceLink>
			</xsl:when>
			<xsl:otherwise>
				<referenceLink target="{@cref}" qualified="true" />
			</xsl:otherwise>
		</xsl:choose>
		</xsl:if>
	</xsl:template>

	<xsl:template match="c">
		<span class="code"><xsl:apply-templates/></span>
	</xsl:template>

	<xsl:template name="runningHeader">
		<include item="runningHeaderText" />
	</xsl:template>

	<!-- pass through html tags -->

	<xsl:template match="p|ol|ul|li|dl|dt|dd|table|tr|th|td|a|img|b|i|strong|em|del|sub|sup|br|hr|h1|h2|h3|h4|h5|h6|pre|div|span|blockquote|abbr|acronym|u|font|map|area">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>

	<!-- extra tag support -->

	<xsl:template match="note">
		<div class="alert">
		<img>
			<includeAttribute item="iconPath" name="src">
				<parameter>alert_note.gif</parameter>
			</includeAttribute>
			<includeAttribute name="alt" item="noteAltText" />
			<includeAttribute name="title" item="noteAltText" />
		</img>
		<xsl:text> </xsl:text>
		<include item="noteTitle" />
		<xsl:apply-templates />
		</div>
	</xsl:template>

	<xsl:template match="preliminary">
		<div class="preliminary">
			<include item="preliminaryText" />
		</div>
	</xsl:template>

	<!-- move these off into a shared file -->

	<xsl:template name="createReferenceLink">
		<xsl:param name="id" />
		<xsl:param name="qualified" select="false()" />

		<referenceLink target="{$id}" qualified="{$qualified}" />

	</xsl:template>
 
	<xsl:template name="section">
		<xsl:param name="toggleSwitch" />
		<xsl:param name="title" />
		<xsl:param name="content" />

		<xsl:variable name="toggleTitle" select="concat($toggleSwitch,'Toggle')" />
		<xsl:variable name="toggleSection" select="concat($toggleSwitch,'Section')" />

		<h1 class="heading">
			<span onclick="ExpandCollapse({$toggleTitle})" style="cursor:default;" onkeypress="ExpandCollapse_CheckKey({$toggleTitle}, event)" tabindex="0">
			<img id="{$toggleTitle}" class="toggle" name="toggleSwitch">
				<includeAttribute name="src" item="iconPath">
					<parameter>collapse_all.gif</parameter>
				</includeAttribute>
			</img>
			<xsl:copy-of select="$title" />
			</span>
		</h1>

		<div id="{$toggleSection}" class="section" name="collapseableSection" style="">
			<xsl:copy-of select="$content" />
		</div>

	</xsl:template>

	<xsl:template name="subSection">
		<xsl:param name="title" />
		<xsl:param name="content" />

		<h4 class="subHeading">
			<xsl:copy-of select="$title" />
		</h4>
		<xsl:copy-of select="$content" />

	</xsl:template>

	<xsl:template name="codelangAttributes">
	</xsl:template>

	<!-- Footer stuff -->

	<xsl:template name="foot">
		<div id="footer">

		<div class="footerLine">
		<img width="100%" height="3px">
			<includeAttribute name="src" item="iconPath">
				<parameter>footer.gif</parameter>
			</includeAttribute>
			<includeAttribute name="alt" item="footerImage" />
			<includeAttribute name="title" item="footerImage" />
		</img>
		</div>

		<include item="footer">
			<parameter>
				<xsl:value-of select="$key"/>
			</parameter>
			<parameter>
				<xsl:call-template name="topicTitlePlain"/>
			</parameter>
			<parameter>
				<xsl:value-of select="/document/metadata/item[@id='PBM_FileVersion']" />
			</parameter>
			<parameter>
				<xsl:value-of select="/document/metadata/attribute[@name='TopicVersion']" />
			</parameter>
		</include>
		</div>
	</xsl:template>
</xsl:stylesheet>
