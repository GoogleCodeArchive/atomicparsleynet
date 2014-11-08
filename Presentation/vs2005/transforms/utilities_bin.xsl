<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.1"
	xmlns:MSHelp="http://msdn.microsoft.com/mshelp"
	xmlns:mshelp="http://msdn.microsoft.com/mshelp"
	xmlns:ddue="http://ddue.schemas.microsoft.com/authoring/2003/5"
	xmlns:xlink="http://www.w3.org/1999/xlink"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	>

	<xsl:import href="../../shared/transforms/utilities_reference.xsl"/>

	<xsl:output method="xml" omit-xml-declaration="yes" encoding="utf-8" />
	<!-- <xsl:output method="xml" omit-xml-declaration="yes" encoding="utf-8" doctype-public="-//W3C//DTD HTML 4.0 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd" /> -->

	<!-- key parameter is the api identifier string -->
	<xsl:param name="key" />
	<xsl:param name="metadata" select="'false'" />
	<xsl:param name="componentizeBy">namespace</xsl:param>

	<xsl:include href="metadataHelp30.xsl" />
	<xsl:include href="metadataHelp20.xsl"/>

	<xsl:template match="/">
		<html>
			<head>
				<META HTTP-EQUIV="Content-Type" CONTENT="text/html; charset=UTF-8"/>
				<META NAME="save" CONTENT="history"/>
				<xsl:call-template name="insertNoIndexNoFollow" />
				<title>
					<xsl:call-template name="topicTitlePlain">
						<xsl:with-param name="titleSuffix">PageTitle</xsl:with-param>
					</xsl:call-template>
				</title>
				<xsl:call-template name="insert30Metadata" />
				<xsl:call-template name="insertStylesheets" />
				<xsl:call-template name="insertScripts" />
				<xsl:call-template name="insertFilename" />
				<xsl:call-template name="insertMetadata" />
			</head>
			<body>

				<xsl:call-template name="upperBodyStuff"/>
				<xsl:call-template name="main"/>
			</body>
		</html>
	</xsl:template>

	<!-- useful global variables -->

	<xsl:variable name="xdata" select="/document/reference/apidata[/document/reference/topicdata/@group = 'api']|/document/reference/topicdata[not(/document/reference/topicdata/@group = 'api')]" />

	<xsl:variable name="group" select="$xdata/@group" />
	<xsl:variable name="subgroup" select="$xdata/@subgroup" />
	<xsl:variable name="subsubgroup" select="$xdata/@subsubgroup" />
	<xsl:variable name="namespaceName" select="/document/reference/containers/namespace/apidata/@name" />

	<!-- document head -->

	<xsl:template name="insertNoIndexNoFollow">
		<xsl:if test="/document/metadata/attribute[@name='NoSearch']">
			<META NAME="ROBOTS" CONTENT="NOINDEX, NOFOLLOW" />
		</xsl:if>
	</xsl:template>

	<xsl:template name="insertStylesheets">
		<link rel="stylesheet" type="text/css">
			<includeAttribute name="href" item="stylePath">
				<parameter>presentation.css</parameter>
			</includeAttribute>
		</link>
		<!-- make mshelp links work -->
		<link rel="stylesheet" type="text/css" href="ms-help://Hx/HxRuntime/HxLink.css" />
	</xsl:template>

	<xsl:template name="insertScripts">
		<script type="text/javascript">
			<includeAttribute name="src" item="scriptPath">
				<parameter>EventUtilities.js</parameter>
			</includeAttribute>
			<xsl:text> </xsl:text>
		</script>
		<script type="text/javascript">
			<includeAttribute name="src" item="scriptPath">
				<parameter>SplitScreen.js</parameter>
			</includeAttribute>
			<xsl:text> </xsl:text>
		</script>
		<script type="text/javascript">
			<includeAttribute name="src" item="scriptPath">
				<parameter>Dropdown.js</parameter>
			</includeAttribute>
			<xsl:text> </xsl:text>
		</script>
		<script type="text/javascript">
			<includeAttribute name="src" item="scriptPath">
				<parameter>script_manifold.js</parameter>
			</includeAttribute>
			<xsl:text> </xsl:text>
		</script>
		<script type="text/javascript">
			<includeAttribute name="src" item="scriptPath">
				<parameter>script_feedBack.js</parameter>
			</includeAttribute>
			<xsl:text> </xsl:text>
		</script>
		<script type="text/javascript">
			<includeAttribute name="src" item="scriptPath">
				<parameter>CheckboxMenu.js</parameter>
			</includeAttribute>
			<xsl:text> </xsl:text>
		</script>
		<script type="text/javascript">
			<includeAttribute name="src" item="scriptPath">
				<parameter>CommonUtilities.js</parameter>
			</includeAttribute>
			<xsl:text> </xsl:text>
		</script>
	</xsl:template>

	<xsl:template match="element" mode="root">
		<tr>
			<td>
				<xsl:choose>
					<xsl:when test="apidata/@name = ''">
						<referenceLink target="{@api}" qualified="false">
							<include item="defaultNamespace" />
						</referenceLink>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="createReferenceLink">
							<xsl:with-param name="id" select="@api" />
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:call-template name="getElementDescription" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="element" mode="namespace">
		<tr>
			<xsl:attribute name="data">
				<xsl:value-of select="apidata/@subgroup" />
			</xsl:attribute>
			<td>
				<xsl:call-template name="createReferenceLink">
					<xsl:with-param name="id" select="@api" />
				</xsl:call-template>
			</td>
			<td>
				<xsl:if test="attributes/attribute/type[@api='T:System.ObsoleteAttribute']">
					<xsl:text> </xsl:text>
					<include item="obsoleteRed" />
				</xsl:if>
				<xsl:call-template name="getElementDescription" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="element" mode="enumeration">
		<tr>
			<xsl:variable name="id" select="@api" />
			<td target="{$id}">
				<span class="selflink">
					<xsl:value-of select="apidata/@name"/>
				</span>
			</td>
			<td>
				<xsl:value-of select="value"/>
			</td>
			<td>
				<xsl:if test="attributes/attribute/type[@api='T:System.ObsoleteAttribute']">
					<xsl:text> </xsl:text>
					<include item="obsoleteRed" />
				</xsl:if>
				<xsl:call-template name="getEnumMemberDescription" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="element" mode="derivedType">
		<tr>
			<td>
				<xsl:choose>
					<xsl:when test="@display-api">
						<referenceLink target="{@api}" display-target="{@display-api}" />
					</xsl:when>
					<xsl:otherwise>
						<referenceLink target="{@api}" />
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>

				<xsl:if test="attributes/attribute/type[@api='T:System.ObsoleteAttribute']">
					<xsl:text> </xsl:text>
					<include item="obsoleteRed" />
				</xsl:if>
				<xsl:call-template name="getElementDescription" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template name="insertFilename">
		<meta name="container">
			<xsl:attribute name="content">
				<xsl:choose>
					<xsl:when test="$componentizeBy='assembly'">
						<xsl:choose>
							<xsl:when test="normalize-space(/document/reference/containers/library/@assembly)">
								<xsl:value-of select="normalize-space(/document/reference/containers/library/@assembly)"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>Namespaces</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:when>
					<!-- the default is to componentize by namespace. For non-componentized builds, the <meta name="container"> value is ignored. -->
					<xsl:otherwise>
						<xsl:choose>
							<!-- get the namespace name from containers/namespace/@api for most members -->
							<xsl:when test="normalize-space(substring-after(/document/reference/containers/namespace/@api,':'))">
								<xsl:value-of select="normalize-space(substring-after(/document/reference/containers/namespace/@api,':'))"/>
							</xsl:when>
							<!-- use 'default_namespace' for members in the default namespace (where namespace/@api == 'N:') -->
							<xsl:when test="normalize-space(/document/reference/containers/namespace/@api)">
								<xsl:text>default_namespace</xsl:text>
							</xsl:when>
							<!-- for the default namespace topic, use 'default_namespace' -->
							<xsl:when test="/document/reference/apidata[@group='namespace' and @name='']">
								<xsl:text>default_namespace</xsl:text>
							</xsl:when>
							<!-- for other namespace topics, get the name from apidata/@name -->
							<xsl:when test="/document/reference/apidata/@group='namespace'">
								<xsl:value-of select="normalize-space(/document/reference/apidata/@name)"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>unknown</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
		</meta>
		<meta name="file" content="{/document/reference/file/@name}" />
		<meta name="guid">
			<xsl:attribute name="content">
				<xsl:value-of select="/document/reference/file/@name" />
			</xsl:attribute>
		</meta>
	</xsl:template>

	<!-- document body -->

	<!-- control window -->

	<!-- the plain-text title -->
	<!-- used in TOC and on topic window bar -->

	<xsl:template name="topicTitlePlain">
		<xsl:param name="qualifyMembers" select="false()" />
		<xsl:param name="titleSuffix">TopicTitle</xsl:param>
		<include>
			<xsl:attribute name="item">
				<xsl:choose>
					<!-- api topic titles -->
					<xsl:when test="$topic-group='api'">
						<!-- the subsubgroup, subgroup, or group determines the title -->
						<xsl:choose>
							<xsl:when test="string($api-subsubgroup)">
								<xsl:value-of select="$api-subsubgroup" />
							</xsl:when>
							<xsl:when test="string($api-subgroup)">
								<xsl:value-of select="$api-subgroup"/>
							</xsl:when>
							<xsl:when test="string($api-group)">
								<xsl:value-of select="$api-group"/>
							</xsl:when>
						</xsl:choose>
					</xsl:when>
					<!-- overload root titles  -->
					<xsl:when test="$topic-group='root'">
						<xsl:value-of select="$topic-group" />
					</xsl:when>
				</xsl:choose>
				<xsl:value-of select="$titleSuffix"/>
			</xsl:attribute>
			<parameter>
				<xsl:call-template name="shortNamePlain">
					<xsl:with-param name="qualifyMembers" select="$qualifyMembers" />
				</xsl:call-template>
			</parameter>
		</include>
	</xsl:template>

  <!-- the language-variant, marked-up topic title -->
  <!-- used as the big title in the non-scrolling region -->

	<xsl:template name="topicTitleDecorated">
		<xsl:param name="titleSuffix">PageTitle</xsl:param>
		<include>
			<xsl:attribute name="item">
				<xsl:choose>
					<!-- api topic titles -->
					<xsl:when test="$topic-group='api'">
						<xsl:choose>
							<xsl:when test="string($api-subsubgroup)">
								<xsl:value-of select="$api-subsubgroup" />
							</xsl:when>
							<xsl:when test="string($api-subgroup)">
								<xsl:value-of select="$api-subgroup" />
							</xsl:when>
							<xsl:when test="string($api-group)">
								<xsl:value-of select="$api-group" />
							</xsl:when>
						</xsl:choose>
					</xsl:when>
					<!-- overload root titles  -->
					<xsl:when test="$topic-group='root'">
						<xsl:value-of select="$topic-group" />
					</xsl:when>
				</xsl:choose>
				<xsl:value-of select="$titleSuffix"/>
			</xsl:attribute>
			<parameter>
				<xsl:call-template name="shortNameDecorated" />
			</parameter>
		</include>
	</xsl:template>

	<!-- Title in TOC -->

	<!-- Index entry -->
	
	<!-- main window -->

	<xsl:template name="main">
		<div id="mainSection">

			<div id="mainBody">
				<div id="allHistory" class="saveHistory" onsave="saveAll()" onload="loadAll()"/>

				<!-- 'header' shared content item is used to show optional boilerplate at the top of the topic's scrolling region, e.g. pre-release boilerplate -->
				<include item="header" />

				<xsl:call-template name="body" />
			</div>
			<xsl:call-template name="foot" />
		</div>

	</xsl:template>

	<xsl:template match="elements" mode="root">
		<xsl:if test="count(element) > 0">

			<xsl:call-template name="section">
				<xsl:with-param name="toggleSwitch" select="'namespaces'"/>
				<xsl:with-param name="title"><include item="namespacesTitle" /></xsl:with-param>
				<xsl:with-param name="content">
				<table class="members" id="memberList" frame="lhs" cellpadding="2">
					<tr>
						<th class="nameColumn"><include item="namespaceNameHeader"/></th>
						<th class="descriptionColumn"><include item="namespaceDescriptionHeader" /></th>
					</tr>
					<xsl:apply-templates select="element" mode="root">
						<xsl:sort select="apidata/@name" />
					</xsl:apply-templates>
				</table>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<xsl:template name="namespaceSection">
		<xsl:param name="listSubgroup" />
		<xsl:variable name="header" select="concat($listSubgroup, 'TypesFilterLabel')"/>
		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="$listSubgroup"/>
			<xsl:with-param name="title">
				<include item="{$header}" />
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:call-template name="namespaceList">
					<xsl:with-param name="listSubgroup" select="$listSubgroup" />
				</xsl:call-template>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="elements" mode="namespace">

		<xsl:if test="element/apidata/@subgroup = 'class'">
			<xsl:call-template name="namespaceSection">
				<xsl:with-param name="listSubgroup" select="'class'" />
			</xsl:call-template>
		</xsl:if>

		<xsl:if test="element/apidata/@subgroup = 'structure'">
			<xsl:call-template name="namespaceSection">
				<xsl:with-param name="listSubgroup" select="'structure'" />
			</xsl:call-template>
		</xsl:if>

		<xsl:if test="element/apidata/@subgroup = 'enumeration'">
			<xsl:call-template name="namespaceSection">
				<xsl:with-param name="listSubgroup" select="'enumeration'" />
			</xsl:call-template>
		</xsl:if>

	</xsl:template>

	<xsl:template name="namespaceList">
		<xsl:param name="listSubgroup" />

		<table id="typeList" class="members" frame="lhs" cellpadding="2">
			<col width="10%"/>
			<tr>
				<th class="nameColumn">
					<include item="{$listSubgroup}NameHeader"/>
				</th>
				<th class="descriptionColumn">
					<include item="typeDescriptionHeader" />
				</th>
			</tr>
			<xsl:apply-templates select="element[apidata/@subgroup=$listSubgroup]" mode="namespace">
				<xsl:sort select="@api" />
			</xsl:apply-templates>
		</table>

	</xsl:template>

	<xsl:template match="elements" mode="enumeration">
		<div id="enumerationSection">
			<xsl:if test="count(element) > 0">
				<xsl:call-template name="section">
					<xsl:with-param name="toggleSwitch" select="'members'"/>
					<xsl:with-param name="title">
						<include item="enumMembersTitle" />
					</xsl:with-param>
					<xsl:with-param name="content">
						<table class="members" id="memberList" frame="lhs" cellpadding="2">
							<tr>
								<th class="nameColumn">
									<include item="memberNameHeader"/>
								</th>
								<th class="descriptionColumn">
									<include item="memberValueHeader" />
								</th>
								<th class="descriptionColumn">
									<include item="memberDescriptionHeader" />
								</th>
							</tr>
							<xsl:apply-templates select="element" mode="enumeration"/>
						</table>
					</xsl:with-param>
				</xsl:call-template>
			</xsl:if>
		</div>
	</xsl:template>



	<xsl:template match="elements[element]" mode="type">
		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="'fields'" />
			<xsl:with-param name="title">
				<include item="fieldsTitle"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:for-each select="element">

					<xsl:variable name="fieldName" select="normalize-space(apidata/@name)" />
					<xsl:variable name="binData" select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinDataAttribute']" />
					<xsl:variable name="binArray" select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinArrayAttribute']" />
					<xsl:variable name="lengthItem">
						<xsl:choose>
							<xsl:when test="$binArray or bincount_summary">lengthItemFieldName</xsl:when>
							<xsl:otherwise>lengthFieldName</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>

					<xsl:call-template name="fieldsection">
						<xsl:with-param name="name" select="concat($fieldName,' Count')"/>
						<xsl:with-param name="term">
							<span class="parameter">
								<include item="countFieldName">
									<parameter>
										<xsl:value-of select="$fieldName"/>
									</parameter>
								</include>
							</span>
						</xsl:with-param>
						<xsl:with-param name="content">
							<xsl:apply-templates select="$binArray" mode="count"/>
							<xsl:apply-templates select="bincount_summary"/>
						</xsl:with-param>
					</xsl:call-template>

					<xsl:call-template name="fieldsection">
						<xsl:with-param name="name" select="concat($fieldName,' Length')"/>
						<xsl:with-param name="term">
							<span class="parameter">
								<include item="{$lengthItem}">
									<parameter>
										<xsl:value-of select="$fieldName"/>
									</parameter>
								</include>
							</span>
						</xsl:with-param>
						<xsl:with-param name="content">
							<xsl:apply-templates select="$binData" mode="length"/>
							<xsl:apply-templates select="binlength_summary"/>
						</xsl:with-param>
					</xsl:call-template>

					<xsl:call-template name="fieldsection">
						<xsl:with-param name="name" select="$fieldName"/>
						<xsl:with-param name="term">
							<xsl:choose>
								<xsl:when test="$binData and not($binData[not(assignment[@name = 'Condition' and value != ''])])">
									<include item="fieldNameOptional">
										<parameter>
											<referenceLink target="{@api}" />
										</parameter>
									</include>
								</xsl:when>
								<xsl:otherwise>
									<referenceLink target="{@api}" />
								</xsl:otherwise>
							</xsl:choose>
						</xsl:with-param>
						<xsl:with-param name="content">
							<xsl:apply-templates select="$binData" mode="field"/>
							<xsl:apply-templates select="bindata_summary"/>
							<xsl:call-template name="getElementDescription" />
						</xsl:with-param>
						<xsl:with-param name="exists" select="true()"/>
					</xsl:call-template>
				</xsl:for-each>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="reference" mode="field">
		<xsl:variable name="binBlock" select="containers//type/attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinBlockAttribute']" />
		<xsl:variable name="hasEncoding">
			<xsl:call-template name="HasEncoding"/>
		</xsl:variable>

		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="'field'" />
			<xsl:with-param name="title">
				<include item="fieldDataTitle"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:if test="normalize-space($hasEncoding)!=''">
					<xsl:apply-templates select="$binBlock[assignment[@name = 'BinaryReaderType' or @name = 'BinaryWriterType']][1]" mode="serializer" />
				</xsl:if>
				<xsl:apply-templates select="." mode="count"/>
				<xsl:apply-templates select="." mode="length"/>
				<xsl:apply-templates select="." mode="data"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="reference" mode="data">
		<xsl:variable name="binData" select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinDataAttribute']" />

		<xsl:call-template name="subSection">
			<xsl:with-param name="title">
				<include item="fieldValueTitle"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:apply-templates select="$binData" mode="field">
					<xsl:with-param name="comments" select="/document/comments"/>
				</xsl:apply-templates>
				<xsl:apply-templates select="/document/comments/bindata_summary"/>
				<xsl:apply-templates select="/document/comments/value"/>
				<xsl:apply-templates select="/document/comments/returns"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="reference" mode="length">
		<xsl:variable name="binData" select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinDataAttribute']" />
		<xsl:variable name="binArray" select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinArrayAttribute']" />
		<xsl:variable name="lengthItem">
			<xsl:choose>
				<xsl:when test="$binArray or bincount_summary">fieldItemLengthTitle</xsl:when>
				<xsl:otherwise>fieldLengthTitle</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:call-template name="subSection">
			<xsl:with-param name="title">
				<include item="{$lengthItem}"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:apply-templates select="$binData" mode="length">
					<xsl:with-param name="comments" select="/document/comments"/>
				</xsl:apply-templates>
				<xsl:apply-templates select="/document/comments/binlength_summary"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="reference" mode="count">
		<xsl:variable name="binArray" select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinArrayAttribute']" />

		<xsl:call-template name="subSection">
			<xsl:with-param name="title">
				<include item="fieldCountTitle"/>
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:apply-templates select="$binArray" mode="count">
					<xsl:with-param name="comments" select="/document/comments"/>
				</xsl:apply-templates>
				<xsl:apply-templates select="/document/comments/bincount_summary"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="attribute" mode="formatlink">
		<xsl:variable name="context" select="parent::attributes/parent::*"/>
		<xsl:variable name="format">
			<xsl:call-template name="ResolveFormat"/>
		</xsl:variable>
		<xsl:variable name="binArray" select="$context/attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinArrayAttribute']" />

		<xsl:choose>
			<xsl:when test="$format = 'Binary' or $format = 'UInt8'">Byte</xsl:when>
			<xsl:when test="$format = 'PString'">Char</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$format"/>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:if test="$format='Binary' or $format='PString' or binlength_summary">[<xsl:call-template name="ResolveLengthNumber"/>]</xsl:if>
		<xsl:for-each select="$binArray">[<xsl:call-template name="ResolveCountNumber"/>]</xsl:for-each>
		<xsl:if test="not($binArray) and $context/bincount_summary">[]</xsl:if>
	</xsl:template>

	<xsl:template match="attribute" mode="formatinfo">
		<xsl:variable name="context" select="parent::attributes/parent::*"/>
		<xsl:variable name="format">
			<xsl:call-template name="ResolveFormat"/>
		</xsl:variable>
		<xsl:variable name="lengthFormat">
			<xsl:call-template name="ResolveLengthFormat"/>
		</xsl:variable>
		<xsl:variable name="lengthNum">
			<xsl:call-template name="ResolveLengthNumber"/>
		</xsl:variable>
		<xsl:variable name="binArray" select="$context/attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinArrayAttribute']|$context/bincount_summary" />
		<xsl:variable name="formatItem">
			<xsl:choose>
				<xsl:when test="$binArray or $context/bincount_summary">ArrayFormat</xsl:when>
				<xsl:otherwise>Format</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="countFormat">
			<xsl:for-each select="$binArray">
				<xsl:call-template name="ResolveCountFormat"/>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="countNum">
			<xsl:for-each select="$binArray">
				<xsl:call-template name="ResolveCountNumber"/>
			</xsl:for-each>
		</xsl:variable>

		<include item="{$format}{$formatItem}">
			<xsl:choose>
				<xsl:when test="normalize-space($lengthNum) != ''">
					<parameter>
						<include item="NumLengthFormat">
							<parameter>
								<xsl:value-of select="$lengthNum"/>
							</parameter>
						</include>
					</parameter>
				</xsl:when>
				<xsl:when test="normalize-space($lengthFormat) != ''">
					<parameter>
						<include item="LengthFormat"/>
					</parameter>
				</xsl:when>
				<xsl:when test="$format='Binary' or $format='PString' or binlength_summary">
					<parameter>
						<include item="SomeLengthFormat"/>
					</parameter>
				</xsl:when>
			</xsl:choose>
			<xsl:choose>
				<xsl:when test="normalize-space($countNum) != ''">
					<parameter>
						<include item="NumCountFormat">
							<parameter>
								<xsl:value-of select="$countNum"/>
							</parameter>
						</include>
					</parameter>
				</xsl:when>
				<xsl:when test="normalize-space(countFormat) != ''">
					<parameter>
						<include item="CountFormat"/>
					</parameter>
				</xsl:when>
				<xsl:when test="$binArray or $context/bincount_summary">
					<parameter>
						<include item="SomeCountFormat"/>
					</parameter>
				</xsl:when>
			</xsl:choose>
		</include>
	</xsl:template>

	<xsl:template match="attribute" mode="conditioninfo">
		<xsl:param name="comments" select="parent::attributes/parent::*"/>
		<xsl:variable name="conditionArg" select="assignment[@name = 'Condition' and value != '']"/>

		<xsl:if test="$conditionArg">
			<xsl:choose>
				<xsl:when test="$comments/bincondition_summary">
					<include item="conditionSummary">
						<parameter>
							<xsl:apply-templates select="$comments/bincondition_summary"/>
						</parameter>
					</include>
				</xsl:when>
				<xsl:otherwise>
					<include item="condition"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>

	<xsl:template name="ResolveFormat">
		<xsl:variable name="context" select="parent::attributes/parent::*"/>
		<xsl:variable name="formatArg" select="argument[type/@api = 'T:FRAFV.Binary.Serialization.BinFormat']|assignment[@name = 'Format']"/>
		<xsl:variable name="type" select="$context/returns/type[1]/@api"/>
		<xsl:variable name="array" select="$context/returns/arrayOf/type[1]/@api"/>
		<xsl:choose>
			<xsl:when test="$formatArg[enumValue/field/@name != 'Auto']">
				<xsl:value-of select="$formatArg/enumValue/field/@name"/>
			</xsl:when>
			<xsl:when test="$type = 'T:System.SByte'">Int8</xsl:when>
			<xsl:when test="$type = 'T:System.Int16'">Int16</xsl:when>
			<xsl:when test="$type = 'T:System.Int32'">Int32</xsl:when>
			<xsl:when test="$type = 'T:System.Int64'">Int64</xsl:when>
			<xsl:when test="$type = 'T:System.Byte'">UInt8</xsl:when>
			<xsl:when test="$type = 'T:System.UInt16'">UInt16</xsl:when>
			<xsl:when test="$type = 'T:System.UInt32'">UInt32</xsl:when>
			<xsl:when test="$type = 'T:System.UInt64'">UInt64</xsl:when>
			<xsl:when test="$type = 'T:System.Boolean'">Bit</xsl:when>
			<xsl:when test="$type = 'T:System.Single'">Float</xsl:when>
			<xsl:when test="$type = 'T:System.Double'">Double</xsl:when>
			<xsl:when test="$type = 'T:System.String'">CString</xsl:when>
			<xsl:when test="$type = 'T:System.Char'">Char</xsl:when>
			<xsl:when test="$array = 'T:System.Byte'">Binary</xsl:when>
			<xsl:when test="$array = 'T:System.Char'">PString</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="ResolveLengthFormat">
		<xsl:variable name="format">
			<xsl:call-template name="ResolveFormat"/>
		</xsl:variable>
		<xsl:variable name="lengthFormatArg" select="assignment[@name = 'LengthFormat']"/>
		<xsl:variable name="lengthCustomMethodArg" select="assignment[@name = 'LengthCustomMethod' and value != '']"/>
		<xsl:choose>
			<xsl:when test="$lengthFormatArg[enumValue/field/@name != 'Auto']">
				<xsl:value-of select="$lengthFormatArg/enumValue/field/@name"/>
			</xsl:when>
			<xsl:when test="not($lengthCustomMethodArg) and ($format='Binary' or $format='PString')">
				<xsl:value-of select="containers//type/attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinBlockAttribute']/assignment[@name = 'LengthFormat' and enumValue/field/@name != 'Auto'][1]/enumValue/field/@name"/>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="ResolveLengthNumber">
		<xsl:variable name="lengthCustomMethodArg" select="assignment[@name = 'LengthCustomMethod' and value != '']"/>
		<xsl:if test="$lengthCustomMethodArg and translate($lengthCustomMethodArg/value, '0123456789', '')=''">
			<xsl:value-of select="$lengthCustomMethodArg/value"/>
		</xsl:if>
	</xsl:template>

	<xsl:template name="ResolveCountFormat">
		<xsl:variable name="countFormatArg" select="assignment[@name = 'CountFormat']"/>
		<xsl:variable name="countCustomMethodArg" select="assignment[@name = 'CountCustomMethod' and value != '']"/>
		<xsl:choose>
			<xsl:when test="$countFormatArg[enumValue/field/@name != 'Auto']">
				<xsl:value-of select="$countFormatArg/enumValue/field/@name"/>
			</xsl:when>
			<xsl:when test="not($countCustomMethodArg)">
				<xsl:value-of select="containers//type/attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinArrayAttribute']/assignment[@name = 'CountFormat' and enumValue/field/@name != 'Auto'][1]/enumValue/field/@name"/>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="ResolveCountNumber">
		<xsl:variable name="countCustomMethodArg" select="assignment[@name = 'CountCustomMethod' and value != '']"/>
		<xsl:if test="$countCustomMethodArg and translate($countCustomMethodArg/value, '0123456789', '')=''">
			<xsl:value-of select="$countCustomMethodArg/value"/>
		</xsl:if>
	</xsl:template>

	<xsl:template name="HasEncoding">
		<xsl:variable name="binData" select="attributes/attribute[type/@api = 'T:FRAFV.Binary.Serialization.BinDataAttribute']" />

		<xsl:for-each select="$binData">
			<xsl:variable name="format">
				<xsl:call-template name="ResolveFormat"/>
			</xsl:variable>
			<xsl:if test="$format = 'CString' or $format = 'PString' or $format = 'Char'">
				<xsl:value-of select="$format"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<xsl:template name="IsMemberSupportedOnXna">
	</xsl:template>

	<xsl:template name="IsMemberSupportedOnCf">
	</xsl:template>

		<xsl:template match="elements" mode="derivedType">
		<xsl:if test="count(element) > 0">
			<xsl:call-template name="section">
				<xsl:with-param name="toggleSwitch" select="'DerivedClasses'"/>
				<xsl:with-param name="title">
					<include item="derivedClasses" />
				</xsl:with-param>
				<xsl:with-param name="content">
					<table class="members" id="memberList" frame="lhs" cellpadding="2">
						<tr>
							<th class="nameColumn">
								<include item="memberNameHeader"/>
							</th>
							<th class="descriptionColumn">
								<include item="memberDescriptionHeader" />
							</th>
						</tr>
						<xsl:apply-templates select="element" mode="derivedType">
							<xsl:sort select="apidata/@name" />
						</xsl:apply-templates>
					</table>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<!-- Assembly information -->

	<xsl:template name="requirementsInfo">
		<p/>
		<include item="requirementsNamespaceLayout" />
		<xsl:text>&#xa0;</xsl:text>
		<referenceLink target="{/document/reference/containers/namespace/@api}" />
		<br/>
		<xsl:call-template name="assembliesInfo"/>
	</xsl:template>

	<xsl:template name="assemblyNameAndModule">
		<xsl:param name="library" select="/document/reference/containers/library"/>
		<include item="assemblyNameAndModule">
			<parameter>
				<span sdata="assembly">
					<xsl:value-of select="$library/@assembly"/>
				</span>
			</parameter>
			<parameter>
				<xsl:value-of select="$library/@module"/>
			</parameter>
			<parameter>
				<xsl:choose>
					<xsl:when test="$library/@kind = 'DynamicallyLinkedLibrary'">
						<xsl:text>dll</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>exe</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</parameter>
		</include>
	</xsl:template>

	<xsl:template name="assembliesInfo">
		<xsl:choose>
			<xsl:when test="count(/document/reference/containers/library)&gt;1">
				<include item="requirementsAssembliesLabel"/>
				<xsl:for-each select="/document/reference/containers/library">
					<xsl:text>&#xa0;&#xa0;</xsl:text>
					<xsl:call-template name="assemblyNameAndModule">
						<xsl:with-param name="library" select="."/>
					</xsl:call-template>
					<br/>
				</xsl:for-each>
			</xsl:when>
			<xsl:otherwise>
				<include item="requirementsAssemblyLabel"/>
				<xsl:text>&#xa0;</xsl:text>
				<xsl:call-template name="assemblyNameAndModule"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- Platform information -->

	<xsl:template match="platforms[platform]">
		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="'platformsTitle'"/>
			<xsl:with-param name="title">
				<include item="platformsTitle" />
			</xsl:with-param>
			<xsl:with-param name="content">
				<p>
					<xsl:for-each select="platform">
						<include item="{.}" />
						<xsl:if test="position()!=last()">
							<xsl:text>, </xsl:text>
						</xsl:if>
					</xsl:for-each>
				</p>
				<xsl:if test="/document/reference/versions/versions[@name='netfw' or @name='netcfw']//version">
					<p>
						<include item="SystemRequirementsLinkBoilerplate"/>
					</p>
				</xsl:if>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<!-- Version information -->

	<xsl:template match="versions">
		<xsl:if test="$omitVersionInformation != 'true'">
			<xsl:call-template name="section">
				<xsl:with-param name="toggleSwitch" select="'versionsTitle'"/>
				<xsl:with-param name="title">
					<include item="versionsTitle" />
				</xsl:with-param>
				<xsl:with-param name="content">
					<xsl:call-template name="processVersions" />
				</xsl:with-param>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>

	<xsl:template name="processVersions">
    <xsl:param name="frameworkGroup" select="true()"/>
    <xsl:choose>
      <xsl:when test="versions and $frameworkGroup">
        <xsl:for-each select="versions">
          <!-- $platformFilterExcluded is based on platform filtering information -->
          <xsl:variable name="platformFilterExcluded" select="boolean(/document/reference/platforms and ( (@name='netcfw' and not(/document/reference/platforms/platform[.='PocketPC']) and not(/document/reference/platforms/platform[.='SmartPhone']) and not(/document/reference/platforms/platform[.='WindowsCE']) ) or (@name='xnafw' and not(/document/reference/platforms/platform[.='Xbox360']) ) ) )" />
          <xsl:if test="not($platformFilterExcluded) and count(.//version) &gt; 0">
            <h4 class ="subHeading">
              <include item="{@name}" />
            </h4>
            <xsl:call-template name="processVersions">
              <xsl:with-param name="frameworkGroup" select="false()"/>
            </xsl:call-template>
          </xsl:if>
        </xsl:for-each>
      </xsl:when>
      <xsl:otherwise>
        <!-- show the versions in which the api is supported, if any -->
        <xsl:variable name="supportedCount" select="count(version[not(@obsolete)] | versions[version[not(@obsolete)]])"/>
        <xsl:if test="$supportedCount &gt; 0">
          <include item="supportedIn_{$supportedCount}">
            <xsl:for-each select="version[not(@obsolete)] | versions[version[not(@obsolete)]]">
              <xsl:variable name="versionName">
                <xsl:choose>
                  <!-- A versions[version] node at this level is for releases that had subsequent service packs. 
                       For example, versions for .NET 3.0 has version nodes for 3.0 and 3.0 SP1. 
                       We show only the first node, which is the one in which the api was first released, 
                       that is, we show 3.0 SP1 only if the api was introduced in SP1. -->
                  <xsl:when test="local-name()='versions'">
                    <xsl:value-of select="version[not(@obsolete)][not(preceding-sibling::version[not(@obsolete)])]/@name"/>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="@name"/>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:variable>
              <parameter>
                <include item="{$versionName}" />
              </parameter>
            </xsl:for-each>
          </include>
          <br/>
        </xsl:if>
        <!-- show the versions in which the api is obsolete with a compiler warning, if any -->
        <xsl:for-each select=".//version[@obsolete='warning']">
          <include item="obsoleteWarning">
            <parameter>
              <include item="{@name}" />
            </parameter>
          </include>
          <br/>
        </xsl:for-each>
        <!-- show the versions in which the api is obsolete and does not compile, if any -->
        <xsl:for-each select=".//version[@obsolete='error']">
          <xsl:if test="position()=last()">
            <include item="obsoleteError">
              <parameter>
                <include item="{@name}" />
              </parameter>
            </include>
            <br/>
          </xsl:if>
        </xsl:for-each>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

	<!-- Inheritance hierarchy -->

	<xsl:template match="family">

		<xsl:call-template name="section">
			<xsl:with-param name="toggleSwitch" select="'family'"/>
			<xsl:with-param name="title">
				<include item="familyTitle" />
			</xsl:with-param>
			<xsl:with-param name="content">
				<xsl:variable name="ancestorCount" select="count(ancestors/*)" />
				<xsl:variable name="childCount" select="count(descendents/*)" />

				<xsl:for-each select="ancestors/type">
					<xsl:sort select="position()" data-type="number" order="descending" />
					<!-- <xsl:sort select="@api"/> -->

					<xsl:call-template name="indent">
						<xsl:with-param name="count" select="position()" />
					</xsl:call-template>

					<xsl:apply-templates select="self::type" mode="link">
						<xsl:with-param name="qualified" select="true()" />
					</xsl:apply-templates>

					<br/>
				</xsl:for-each>

				<xsl:call-template name="indent">
					<xsl:with-param name="count" select="$ancestorCount + 1" />
				</xsl:call-template>

				<referenceLink target="{$key}" qualified="true"/>
				<br/>

				<xsl:choose>

					<xsl:when test="descendents/@derivedTypes">
						<xsl:call-template name="indent">
							<xsl:with-param name="count" select="$ancestorCount + 2" />
						</xsl:call-template>
						<referenceLink target="{descendents/@derivedTypes}" qualified="true">
							<include item="derivedClasses"/>
						</referenceLink>
					</xsl:when>
					<xsl:otherwise>

						<xsl:for-each select="descendents/type">
							<xsl:sort select="@api" />

							<xsl:if test="not(self::type/@api=preceding-sibling::*/self::type/@api)">
								<xsl:call-template name="indent">
									<xsl:with-param name="count" select="$ancestorCount + 2" />
								</xsl:call-template>

								<xsl:apply-templates select="self::type" mode="link">
									<xsl:with-param name="qualified" select="true()" />
								</xsl:apply-templates>

								<br/>
							</xsl:if>
						</xsl:for-each>
					</xsl:otherwise>
				</xsl:choose>

			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="createTableEntries">
		<xsl:param name="count" />
		<xsl:if test="number($count) > 0">
			<td>&#x20;</td>
			<xsl:call-template name="createTableEntries">
				<xsl:with-param name="count" select="number($count)-1" />
			</xsl:call-template>
		</xsl:if>
	</xsl:template>


	<!-- decorated names -->

	<xsl:template name="shortNameDecorated">
		<xsl:choose>
			<!-- type overview pages and member list pages get the type name -->
			<xsl:when test="($topic-group='api' and $api-group='type') or ($topic-group='list')">
				<xsl:for-each select="/document/reference[1]">
					<xsl:call-template name="typeNameDecorated" />
				</xsl:for-each>
			</xsl:when>
			<!-- eii members -->
			<xsl:when test="document/reference[memberdata[@visibility='private'] and proceduredata[@virtual = 'true']]">
				<xsl:for-each select="/document/reference/containers/type[1]">
					<xsl:call-template name="typeNameDecorated" />
				</xsl:for-each>
				<span class="languageSpecificText">
					<span class="cs">.</span>
					<span class="vb">.</span>
					<span class="cpp">::</span>
					<span class="nu">.</span>
					<span class="fs">.</span>
				</span>
				<xsl:for-each select="/document/reference/implements/member">
					<xsl:for-each select="type">
						<xsl:call-template name="typeNameDecorated" />
					</xsl:for-each>
					<span class="languageSpecificText">
						<span class="cs">.</span>
						<span class="vb">.</span>
						<span class="cpp">::</span>
						<span class="nu">.</span>
						<span class="fs">.</span>
					</span>
					<xsl:value-of select="apidata/@name" />
					<xsl:apply-templates select="templates" mode="decorated" />
				</xsl:for-each>
			</xsl:when>
			<!-- normal member pages use the qualified member name -->
			<xsl:when test="$topic-group='api' and $api-group='member'">
				<xsl:for-each select="/document/reference/containers/type[1]">
					<xsl:call-template name="typeNameDecorated" />
				</xsl:for-each>
				<xsl:if test="not($api-subsubgroup='operator'and (document/reference/apidata/@name='Explicit' or document/reference/apidata/@name='Implicit'))">
					<span class="languageSpecificText">
						<span class="cs">.</span>
						<span class="vb">.</span>
						<span class="cpp">::</span>
						<span class="nu">.</span>
						<span class="fs">.</span>
					</span>
				</xsl:if>
				<xsl:for-each select="/document/reference[1]">
					<xsl:choose>
						<xsl:when test="$api-subsubgroup='operator' and (apidata/@name='Explicit' or apidata/@name='Implicit')">
							<xsl:text>&#xa0;</xsl:text>
							<span class="languageSpecificText">
								<span class="cs">
									<xsl:value-of select="apidata/@name" />
								</span>
								<span class="vb">
									<xsl:choose>
										<xsl:when test="apidata/@name='Explicit'">
											<xsl:text>Narrowing</xsl:text>
										</xsl:when>
										<xsl:when test="apidata/@name='Implicit'">
											<xsl:text>Widening</xsl:text>
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="apidata/@name" />
										</xsl:otherwise>
									</xsl:choose>
								</span>
								<span class="cpp">
									<xsl:value-of select="apidata/@name" />
								</span>
								<span class="nu">
									<xsl:value-of select="apidata/@name" />
								</span>
								<span class="fs">
									<xsl:value-of select="apidata/@name" />
								</span>
							</span>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="apidata/@name" />
						</xsl:otherwise>
					</xsl:choose>
					<xsl:apply-templates select="templates" mode="decorated" />
				</xsl:for-each>
			</xsl:when>
			<!-- namespace (and any other) topics just use the name -->
			<xsl:when test="/document/reference/apidata/@name = ''">
				<include item="defaultNamespace" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="/document/reference/apidata/@name" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<!-- plain names -->

	<xsl:template name="shortNamePlain">
		<xsl:param name="qualifyMembers" select="false()" />
		<xsl:choose>
			<!-- type overview pages and member list pages get the type name -->
			<xsl:when test="($topic-group='api' and $api-group='type') or ($topic-group='list')">
				<xsl:for-each select="/document/reference[1]">
					<xsl:call-template name="typeNamePlain" />
				</xsl:for-each>
			</xsl:when>
			<!-- member pages use the member name, qualified if the qualified flag is set -->
			<xsl:when test="$topic-group='api' and $api-group='member'">
				<!-- check for qualify flag and qualify if it is set -->
				<xsl:if test="$qualifyMembers">
					<xsl:for-each select="/document/reference/containers/type[1]">
						<xsl:call-template name="typeNamePlain" />
					</xsl:for-each>
					<xsl:choose>
						<xsl:when test="$api-subsubgroup='operator' and (document/reference/apidata/@name='Explicit' or document/reference/apidata/@name='Implicit')">
							<xsl:text>&#xa0;</xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<xsl:text>.</xsl:text>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
				<xsl:choose>
					<!-- EII names are interfaceName.interfaceMemberName, not memberName -->
					<xsl:when test="document/reference[memberdata[@visibility='private'] and proceduredata[@virtual = 'true']]">
						<xsl:for-each select="/document/reference/implements/member">
							<xsl:for-each select="type">
								<xsl:call-template name="typeNamePlain" />
							</xsl:for-each>
							<xsl:text>.</xsl:text>
							<xsl:value-of select="apidata/@name" />
							<xsl:apply-templates select="templates" mode="plain" />
						</xsl:for-each>
					</xsl:when>
					<xsl:otherwise>
						<!-- but other members just use the name -->
						<xsl:for-each select="/document/reference[1]">
							<xsl:value-of select="apidata/@name" />
							<xsl:apply-templates select="templates" mode="plain" />
						</xsl:for-each>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<!-- namespace, member (and any other) topics just use the name -->
			<xsl:when test="/document/reference/apidata/@name = ''">
				<include item="defaultNamespace" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="/document/reference/apidata/@name" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>
