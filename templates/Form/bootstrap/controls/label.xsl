<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-label">
        <xsl:param name="for" />
        <xsl:if test="/Form/Settings/LabelAlign != 'inside'">
            <label class="">
                <xsl:if test="$for"><xsl:attribute name="for"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/></xsl:attribute></xsl:if>
                <xsl:attribute name="class">
                    control-label
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="LabelCssClass"/>
                    <xsl:text> </xsl:text>
                    <xsl:if test="IsRequired='True'">required</xsl:if>
                </xsl:attribute>
                <xsl:attribute name="style">
                    <xsl:if test="/Form/Settings/LabelAlign != 'default'">
                        <xsl:text>text-align: </xsl:text>
                        <xsl:value-of select="/Form/Settings/LabelAlign" />
                        <xsl:text>; </xsl:text>
                    </xsl:if>
                    <xsl:if test="/Form/Settings/LabelWidth > 0">
                        <xsl:text>width: </xsl:text>
                        <xsl:value-of select="/Form/Settings/LabelWidth"/>
                        <xsl:text>px; </xsl:text>
                    </xsl:if>
                    <xsl:value-of select="LabelCssStyles"/></xsl:attribute>
                <xsl:value-of select="Title"/>
            </label>
        </xsl:if>
    </xsl:template>
    
</xsl:stylesheet>
