<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="label.xsl"/>
    <xsl:import href="attr-common.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-checkbox">
        <div>
            <xsl:attribute name="class">control-group <xsl:if test="LabelCssClass != ''"> control-group-<xsl:value-of select="LabelCssClass"/></xsl:if></xsl:attribute>
            <div class="controls">
                <xsl:if test="/Form/Settings/LabelWidth > 0 and /Form/Settings/LabelAlign != 'top' and /Form/Settings/LabelAlign != 'inside'">
                    <xsl:attribute name="style">margin-left: <xsl:value-of select="/Form/Settings/LabelWidth + 20"/>px;</xsl:attribute>
                </xsl:if>
                <xsl:if test="/Form/Settings/LabelAlign = 'inside'">
                    <xsl:attribute name="style">
                        <xsl:text>margin-left: 0px;</xsl:text>
                    </xsl:attribute>
                </xsl:if>
                <label class="checkbox">
                    <input type="checkbox">
                        <xsl:call-template name="ctl-attr-common">
                            <xsl:with-param name="cssclass"><xsl:if test="/Form/Settings/ClientSideValidation ='True' and IsRequired='True'">required</xsl:if></xsl:with-param>
                            <xsl:with-param name="hasId">yes</xsl:with-param>
                            <xsl:with-param name="hasName">yes</xsl:with-param>
                        </xsl:call-template>
                        <xsl:if test="Value = 'True'">
                            <xsl:attribute name="checked">checked</xsl:attribute>
                        </xsl:if>
                        <xsl:if test="IsEnabled != 'True'">
                            <xsl:attribute name="disabled">disabled</xsl:attribute>
                        </xsl:if>
                    </input>
                    <xsl:value-of select="Title"/>
                </label>
                <div class="err-placeholder"></div>
            </div>
        </div>

    </xsl:template>
    
</xsl:stylesheet>
