<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.1"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt">

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
		<xsl:apply-templates select="/document/comments/preliminary" />
		<xsl:apply-templates select="/document/comments/summary" />
		<!-- assembly information -->
		<xsl:if test="not($group='root' or $group='namespace')">
			<xsl:call-template name="requirementsInfo"/>
		</xsl:if>
		<xsl:apply-templates select="/document/comments/binblock_summary" />
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

	<xsl:template name="getElementDescription">
		<xsl:apply-templates select="summary[1]" />
	</xsl:template>

	<xsl:template name="getEnumMemberDescription">
		<xsl:apply-templates select="summary/node()" />
		<!-- enum members may have additional authored content in the remarks node -->
		<xsl:apply-templates select="remarks/node()" />
	</xsl:template>


	<!-- block sections -->

	<xsl:template match="attribute" mode="field">
		<xsl:param name="comments" select="parent::attributes/parent::*"/>

		<include item="typeLink">
			<parameter>
				<span class="keyword">
					<xsl:apply-templates select="." mode="formatlink"/>
				</span>
			</parameter>
			<parameter>
				<xsl:apply-templates select="." mode="formatinfo"/>
			</parameter>
			<parameter>
				<xsl:apply-templates select="." mode="conditioninfo">
					<xsl:with-param name="comments" select="$comments"/>
				</xsl:apply-templates>
			</parameter>
		</include>
		<br />
	</xsl:template>

	<xsl:template match="attribute" mode="length">
		<xsl:param name="comments" select="parent::attributes/parent::*"/>
		<xsl:variable name="lengthFormat">
			<xsl:call-template name="ResolveLengthFormat"/>
		</xsl:variable>

		<xsl:if test="normalize-space($lengthFormat) != ''">
			<include item="typeLink">
				<parameter>
					<span class="keyword">
						<xsl:value-of select="$lengthFormat"/>
					</span>
				</parameter>
				<parameter>
					<include item="{$lengthFormat}Format"/>
				</parameter>
				<parameter>
					<xsl:apply-templates select="." mode="conditioninfo">
						<xsl:with-param name="comments" select="$comments"/>
					</xsl:apply-templates>
				</parameter>
			</include>
			<br />
		</xsl:if>
	</xsl:template>

	<xsl:template match="attribute" mode="count">
		<xsl:variable name="countFormat">
			<xsl:call-template name="ResolveCountFormat"/>
		</xsl:variable>

		<xsl:if test="normalize-space($countFormat) != ''">
			<include item="typeLink">
				<parameter>
					<span class="keyword">
						<xsl:value-of select="$countFormat"/>
					</span>
				</parameter>
				<parameter>
					<include item="{$countFormat}Format"/>
				</parameter>
				<parameter/>
			</include>
			<br />
		</xsl:if>
	</xsl:template>

	<xsl:template match="attribute" mode="item">
		<xsl:variable name="itemType">
			<xsl:call-template name="ResolveItemType"/>
		</xsl:variable>

		<include item="typeLink">
			<parameter>
				<!--xsl:apply-templates select="value" mode="link">
					<xsl:with-param name="qualified" select="true()" />
				</xsl:apply-templates-->
				<referenceLink target="{$itemType}" show-templates="true" show-container="false"/>[]
			</parameter>
			<parameter>
				<include item="arrayFormat"/>
			</parameter>
			<parameter/>
		</include>
		<br />
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
						<referenceLink target="{value}" show-templates="true" show-container="true"/>
					</parameter>
					<parameter/>
					<parameter/>
				</include>
				<br />
				<xsl:call-template name="getElementDescription" />
			</dd>
		</dl>
	</xsl:template>

	<xsl:template match="summary|value|returns|bincondition_summary|binlength_summary|bincount_summary|bindata_summary|binblock_summary">
		<div class="summary">
			<xsl:apply-templates />
		</div>
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
				<referenceLink target="{@cref}" />
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

	<xsl:template match="paramref">
		<span class="parameter">
			<xsl:value-of select="@name" />
		</span>
	</xsl:template>

	<xsl:template match="typeparamref">
		<span class="typeparameter">
			<xsl:value-of select="@name" />
		</span>
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

	<xsl:template name="fieldsection">
		<xsl:param name="name"/>
		<xsl:param name="term"/>
		<xsl:param name="content"/>
		<xsl:param name="exists" select="$content and (normalize-space($content) != '' or msxsl:node-set($content)/*)"/>

		<xsl:if test="$exists">
			<dl fieldName="{$name}">
				<dt>
					<xsl:copy-of select="$term" />
				</dt>
				<dd>
					<xsl:copy-of select="$content" />
				</dd>
			</dl>
		</xsl:if>
	</xsl:template>

	<xsl:template name="section">
		<xsl:param name="toggleSwitch" />
		<xsl:param name="title" />
		<xsl:param name="content" />

		<xsl:variable name="toggleTitle" select="concat($toggleSwitch,'Toggle')" />
		<xsl:variable name="toggleSection" select="concat($toggleSwitch,'Section')" />

		<xsl:if test="$content and (normalize-space($content) != '' or msxsl:node-set($content)/*)">
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
		</xsl:if>

	</xsl:template>

	<xsl:template name="subSection">
		<xsl:param name="title" />
		<xsl:param name="content" />

		<xsl:if test="$content and (normalize-space($content) != '' or msxsl:node-set($content)/*)">
			<h4 class="subHeading">
				<xsl:copy-of select="$title" />
			</h4>
			<xsl:copy-of select="$content" />
		</xsl:if>

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
