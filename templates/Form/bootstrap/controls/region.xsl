<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="label.xsl"/>
    <xsl:import href="attr-common.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-region">
        
        <div>
            <xsl:attribute name="class">control-group region-root <xsl:if test="LabelCssClass != ''"> control-group-<xsl:value-of select="LabelCssClass"/></xsl:if></xsl:attribute>
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
                <input type="hidden" class="region-value">
                    <xsl:attribute name="name"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/></xsl:attribute>
                </input>
                <div class="region-loading" style="font-style: italic; padding: 8px; color: #626262; display: none;">
                    <xsl:value-of select="utils:localize('message.pleaseWait', 'Please wait...')"></xsl:value-of>
                </div>
                
                <input type="text" style="">
                    <xsl:call-template name="ctl-attr-common">
                        <xsl:with-param name="cssclass">span12 region-textbox <xsl:if test="/Form/Settings/ClientSideValidation ='True' and IsRequired='True'">required</xsl:if></xsl:with-param>
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
                    <xsl:attribute name="id"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/>tb</xsl:attribute>
                    <xsl:if test="IsEnabled != 'True'">
                        <xsl:attribute name="disabled">disabled</xsl:attribute>
                    </xsl:if>
                </input>

                <select style="display: none;">
                    <xsl:call-template name="ctl-attr-common">
                        <xsl:with-param name="cssclass">span12 region-dropdown</xsl:with-param>
                    </xsl:call-template>
                    <xsl:attribute name="id"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/>dd</xsl:attribute>
                    <xsl:attribute name="data-mode">
                        <xsl:choose>
                            <xsl:when test="Flag[text()='code']">code</xsl:when>
                            <xsl:otherwise>text</xsl:otherwise>
                        </xsl:choose>
                    </xsl:attribute>
                    <xsl:if test="IsEnabled != 'True'">
                        <xsl:attribute name="disabled">disabled</xsl:attribute>
                    </xsl:if>
                </select>
            </div>
        </div>
    </xsl:template>
    
</xsl:stylesheet>
