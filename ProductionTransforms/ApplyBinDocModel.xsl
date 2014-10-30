<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:ms="urn:schemas-microsoft-com:xslt"
	exclude-result-prefixes="ms"
	version="1.1">

	<xsl:output indent="yes" encoding="UTF-8" />
	<xsl:param name="BinReflection" />

	<xsl:variable name="doc" select="document($BinReflection)/reflection/apis" />
	<xsl:variable name="bin" select="$doc/api[apidata/@group='member' and attributes/attribute/type[starts-with(@api,'T:FRAFV.Binary.Serialization.')]]" />
	<xsl:variable name="binser" select="/reflection/apis/api[apidata/@group='member' and implements//type[@api='T:FRAFV.Binary.Serialization.IBinSerializable']]" />

	<xsl:template match="/">
		<reflection>
			<xsl:copy-of select="/reflection/assemblies" />
			<apis>
				<xsl:copy-of select="/reflection/apis/api" />
				<xsl:apply-templates select="$doc/api" />
			</apis>
		</reflection>
	</xsl:template>

	<!-- Process a generic API (for namespaces and members; types and overloads are handled explicitly below) -->

	<xsl:template match="api">
	</xsl:template>

	<xsl:template match="api[apidata/@group='namespace']">
		<xsl:variable name="ns" select="@id" />
		<xsl:if test="$binser[containers/namespace/@api=$ns]">
			<api id="Bin.{@id}">
				<topicdata group="api" typeId="{@id}" />
				<apidata name="{apidata/@name}" group="namespace" serialization="binary" />
				<xsl:apply-templates select="elements" />
			</api>
		</xsl:if>
	</xsl:template>

	<xsl:template match="elements">
		<elements>
			<xsl:apply-templates select="element" />
		</elements>
	</xsl:template>

	<xsl:template match="api[apidata/@group='namespace']/elements/element">
		<xsl:variable name="type" select="@api" />
		<xsl:if test="$binser[containers/type/@api=$type] or $bin[returns/type/@api=$type] and $doc/api[@id=$type and apidata/@subgroup='enumeration']">
			<xsl:apply-templates select="." mode="binary" />
		</xsl:if>
	</xsl:template>

	<xsl:template match="api[apidata/@group='type']">
		<xsl:variable name="type" select="@id" />
		<xsl:if test="$binser[containers/type/@api=$type]">
			<api id="Bin.{@id}">
				<topicdata group="api" subgroup="full" typeId="{@id}" />
				<apidata name="{apidata/@name}" group="type" subgroup="{apidata/@subgroup}" serialization="binary" />
				<xsl:copy-of select="typedata" />
				<xsl:apply-templates select="family|elements|containers" />
			</api>
		</xsl:if>
	</xsl:template>

	<xsl:template match="api[apidata/@group='type' and apidata/@subgroup='enumeration']">
		<xsl:variable name="type" select="@id" />
		<xsl:if test="$bin[returns/type/@api=$type]">
			<api id="Bin.{@id}">
				<topicdata group="api" typeId="{@id}" />
				<apidata name="{apidata/@name}" group="type" subgroup="enumeration" serialization="binary" />
				<xsl:copy-of select="typedata" />
				<xsl:apply-templates select="elements|containers" />
			</api>
		</xsl:if>
	</xsl:template>

	<xsl:template match="family">
		<family>
			<xsl:apply-templates select="ancestors|descendents" />
		</family>
	</xsl:template>

	<xsl:template match="ancestors|descendents">
		<xsl:element name="{name()}">
			<xsl:apply-templates select="type" />
		</xsl:element>
	</xsl:template>

	<xsl:template match="type">
		<xsl:variable name="type" select="@api"/>
		<xsl:if test="$binser[containers/type/@api=$type]">
			<xsl:apply-templates select="." mode="binary" />
		</xsl:if>
	</xsl:template>

	<xsl:template match="api[apidata/@group='type' and not(apidata/@subgroup='enumeration')]/elements">
		<elements>
			<xsl:call-template name="Ordered">
				<xsl:with-param name="list" select="element" />
			</xsl:call-template>
		</elements>
	</xsl:template>

	<xsl:template name="Ordered">
		<xsl:param name="list" />
		<xsl:if test="$list">
			<xsl:for-each select="$doc/api[@id=$list[1]/@api]/containers/type[1]">
				<xsl:variable name="prefix" select="concat(substring-after(@api,':'),'.')" />
				<xsl:if test="$list[not(starts-with(substring-after(@api,':'),$prefix))]">
					<xsl:call-template name="Ordered">
						<xsl:with-param name="list" select="$list[not(starts-with(substring-after(@api,':'),$prefix))]" />
					</xsl:call-template>
				</xsl:if>
				<xsl:apply-templates select="$list[starts-with(substring-after(@api,':'),$prefix)]" />
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="api[apidata/@group='type']/elements/element">
		<xsl:variable name="name" select="@api" />
		<xsl:for-each select="$bin[@id=$name]">
			<element api="Bin.F:{substring-after(@id,':')}">
				<apidata name="{apidata/@name}" group="member" subgroup="{apidata/@subgroup}" />
				<xsl:copy-of select="returns|attributes" />
			</element>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="api[apidata/@group='type' and apidata/@subgroup='enumeration']/elements/element">
		<xsl:variable name="name" select="@api" />
		<xsl:for-each select="$doc/api[@id=$name]">
			<element api="Bin.{@id}">
				<apidata name="{apidata/@name}" group="member" subgroup="field" />
				<xsl:copy-of select="returns|attributes|value" />
			</element>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="api[apidata/@group='type']/containers">
		<containers>
			<xsl:copy-of select="library|type"/>
			<xsl:apply-templates select="namespace" mode="binary"/>
		</containers>
	</xsl:template>

	<xsl:template match="api[apidata/@group='member']">
		<xsl:variable name="name" select="@id"/>
		<xsl:if test="$bin[@id=$name]">
			<api id="Bin.F:{substring-after(@id,':')}">
				<topicdata group="api" />
				<apidata name="{apidata/@name}" group="member" subgroup="{apidata/@subgroup}" serialization="binary" />
				<xsl:copy-of select="memberdata|fielddata|proceduredata|propertydata|getter|setter|returns|attributes" />
				<xsl:apply-templates select="containers" />
			</api>
		</xsl:if>
	</xsl:template>

	<xsl:template match="api[apidata/@group='member']/containers">
		<containers>
			<xsl:copy-of select="library"/>
			<xsl:apply-templates select="namespace|type" mode="binary"/>
		</containers>
	</xsl:template>

	<xsl:template match="element" mode="binary">
		<element api="Bin.{@api}">
			<xsl:copy-of select="@source" />
			<xsl:copy-of select="*" />
		</element>
	</xsl:template>

	<xsl:template match="type" mode="binary">
		<type api="Bin.{@api}">
			<xsl:copy-of select="@ref"/>
			<xsl:copy-of select="*"/>
		</type>
	</xsl:template>

	<xsl:template match="namespace" mode="binary">
		<namespace api="Bin.{@api}">
			<xsl:copy-of select="*"/>
		</namespace>
	</xsl:template>

</xsl:stylesheet>
