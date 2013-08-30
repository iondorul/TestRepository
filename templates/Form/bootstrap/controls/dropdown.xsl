<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="label.xsl"/>
    <xsl:import href="attr-common.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-dropdown">
        <xsl:param name="addclass" />
        
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
                <select>
                    <xsl:call-template name="ctl-attr-common">
                        <xsl:with-param name="cssclass">span12 <xsl:value-of select="$addclass"/></xsl:with-param>
                        <xsl:with-param name="hasId">yes</xsl:with-param>
                        <xsl:with-param name="hasName">yes</xsl:with-param>
                    </xsl:call-template>
                    <xsl:if test="IsEnabled != 'True'">
                        <xsl:attribute name="disabled">disabled</xsl:attribute>
                    </xsl:if>
                    <xsl:for-each select="Option">
                        <option>
                            <xsl:attribute name="value">
                                <xsl:value-of select="@value"/>
                            </xsl:attribute>
                            <xsl:if test="../Value = @value">
                                <xsl:attribute name="selected">selected</xsl:attribute>
                            </xsl:if>
                            <xsl:value-of select="."/>
                        </option>
                    </xsl:for-each>
                </select>
            </div>
        </div>
    </xsl:template>
    
</xsl:stylesheet>
