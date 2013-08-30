<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-attr-common">
        <xsl:param name="cssclass" />
        <xsl:param name="hasId" />
        <xsl:param name="hasName" />

        <xsl:attribute name="class">
            <xsl:value-of select="CssClass"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="$cssclass"/>
            <xsl:if test="ValidationGroup != ''">
                <xsl:text> </xsl:text>
                <xsl:value-of select="ValidationGroup"/>
                <xsl:if test="GroupValidator != ''">
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="ValidationGroup"/>-<xsl:value-of select="GroupValidatorJsName"/>
                </xsl:if>
            </xsl:if>
            <xsl:text> </xsl:text>
            <xsl:value-of select="CustomValidator1JsName"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="CustomValidator2JsName" />
        </xsl:attribute>
        <xsl:if test="CssStyles != ''">
            <xsl:attribute name="style"><xsl:value-of select="CssStyles"/></xsl:attribute>
        </xsl:if>
        <xsl:if test="$hasId='yes'">
            <xsl:attribute name="id"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/></xsl:attribute>
        </xsl:if>
        <xsl:if test="$hasName='yes'">
            <xsl:attribute name="name"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/></xsl:attribute>
        </xsl:if>
        <xsl:attribute name="data-fieldid"><xsl:value-of select="Id"/></xsl:attribute>
    </xsl:template>

</xsl:stylesheet>
