<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="label.xsl"/>
    <xsl:import href="attr-common.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-captcha">
        
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
                <label>
                    <xsl:attribute name="for"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/></xsl:attribute>
                    <xsl:attribute name="class">control-label <xsl:value-of select="LabelCssClass"/></xsl:attribute>
                    <xsl:attribute name="style">text-align: left; float: none; <xsl:if test="/Form/Settings/LabelWidth > 0">width: <xsl:value-of select="/Form/Settings/LabelWidth"/>px;</xsl:if> <xsl:value-of select="LabelCssStyles"/></xsl:attribute>
                    <xsl:value-of select="Title"/>
                </label>
                
                <div style="margin: 4px 0;">
                <img>
                    <xsl:attribute name="src">
                        <xsl:value-of select="Data/ImageUrl"/>
                    </xsl:attribute>
                    <xsl:call-template name="ctl-attr-common" />
                </img>
                </div>

                <input type="hidden">
                    <xsl:attribute name="name"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/>captchaenc</xsl:attribute>
                    <xsl:attribute name="value"><xsl:value-of select="Data/CaptchaEncrypted" /></xsl:attribute>
                </input>
                
                <input type="text">
                    <xsl:call-template name="ctl-attr-common">
                        <xsl:with-param name="cssclass">span12 <xsl:if test="/Form/Settings/ClientSideValidation ='True' and IsRequired='True'">required</xsl:if></xsl:with-param>
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
                    <xsl:attribute name="value"><xsl:value-of select="Value"/></xsl:attribute>
                </input>
            </div>
        </div>
    </xsl:template>
    
</xsl:stylesheet>
