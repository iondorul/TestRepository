<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="label.xsl"/>
    <xsl:import href="attr-common.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-datepicker">
        
        <div>
            <xsl:attribute name="class">control-group <xsl:if test="LabelCssClass != ''"> control-group-<xsl:value-of select="LabelCssClass"/></xsl:if></xsl:attribute>
            <xsl:call-template name="ctl-label">
                <xsl:with-param name="for"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/></xsl:with-param>
            </xsl:call-template>
            <div class="controls">
                <xsl:if test="/Form/Settings/LabelWidth > 0 and /Form/Settings/LabelAlign != 'top' and /Form/Settings/LabelAlign != 'inside'">
                    <xsl:attribute name="style">margin-left: <xsl:value-of select="/Form/Settings/LabelWidth + 20"/>px;</xsl:attribute>
                </xsl:if>
                <xsl:if test="/Form/Settings/LabelAlign = 'inside'">
                    <xsl:attribute name="style">
                        <xsl:text>margin-left: 0px;</xsl:text>
                    </xsl:attribute>
                </xsl:if>
                <input type="text">
                    <xsl:call-template name="ctl-attr-common">
                        <xsl:with-param name="cssclass">span12 datepicker <xsl:if test="/Form/Settings/ClientSideValidation ='True' and IsRequired='True'">required</xsl:if></xsl:with-param>
                        <xsl:with-param name="hasId">yes</xsl:with-param>
                        <xsl:with-param name="hasName">yes</xsl:with-param>
                    </xsl:call-template>
                    <xsl:choose>
                        <xsl:when test="/Form/Settings/LabelAlign = 'inside'">
                            <xsl:attribute name="placeholder"><xsl:value-of select="Title"/></xsl:attribute>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:attribute name="placeholder"><xsl:value-of select="ShortDesc"/></xsl:attribute>
                        </xsl:otherwise>
                    </xsl:choose>

                    <xsl:attribute name="data-dateformat">
                    <xsl:choose>
                        <xsl:when test="DateFormat!=''">
                            <xsl:value-of select = "DateFormat"></xsl:value-of>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of select="/Form/Settings/DateFormat"/>
                        </xsl:otherwise>
                    </xsl:choose>
                    </xsl:attribute>
                    <xsl:attribute name="data-theme"><xsl:value-of select="/Form/Settings/jQueryTheme"/></xsl:attribute>
                    <xsl:attribute name="value"><xsl:value-of select="Value"/></xsl:attribute>
                    <xsl:if test="Flag[text()='yearpick']">
                        <xsl:attribute name="data-changeyear">true</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="Flag[text()='monthpick']">
                        <xsl:attribute name="data-changemonth">true</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="YearRange != ''">
                        <xsl:attribute name="data-yearrange">
                            <xsl:value-of select = "YearRange"></xsl:value-of>
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:if test="IsEnabled != 'True'">
                        <xsl:attribute name="disabled">disabled</xsl:attribute>
                    </xsl:if>
                </input>
            </div>
        </div>
    </xsl:template>
    
</xsl:stylesheet>
