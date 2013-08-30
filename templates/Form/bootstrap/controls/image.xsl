<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:tokens="af:tokens">
    
    <xsl:import href="attr-common.xsl"/>
    <xsl:import href="label.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-image">
        <div>
            <xsl:attribute name="class">control-group <xsl:if test="LabelCssClass != ''"> control-group-<xsl:value-of select="LabelCssClass"/></xsl:if></xsl:attribute>

            <xsl:if test="ShowLabel = 'True'">
                <xsl:call-template name="ctl-label">
                    <xsl:with-param name="for"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/></xsl:with-param>
                </xsl:call-template>
            </xsl:if>

            <div class="controls">
                <xsl:if test="/Form/Settings/LabelWidth > 0 and /Form/Settings/LabelAlign != 'top' and /Form/Settings/LabelAlign != 'inside'">
                    <xsl:attribute name="style">
                        margin-left: <xsl:value-of select="/Form/Settings/LabelWidth + 26"/>px;
                    </xsl:attribute>
                </xsl:if>
                <img>
                    <xsl:attribute name="src">
                        <xsl:choose>
                            <xsl:when test="ImageURL != ''">
                                    <xsl:value-of select="ImageURL" disable-output-escaping="yes"/>
                            </xsl:when>
                            <xsl:otherwise>
                                <xsl:value-of select="/Form/Settings/PortalHomeUrl "/>
                                <xsl:text>/</xsl:text>
                                <xsl:value-of select="PortalImage" disable-output-escaping="yes"/>
                            </xsl:otherwise>
                        </xsl:choose>
                    </xsl:attribute>
                    <xsl:if test="CssStyles != ''">
                        <xsl:attribute name="style">
                            <xsl:value-of select="CssStyles"/>
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:if test="CssClass != ''">
                        <xsl:attribute name="class">
                            <xsl:value-of select="CssClass"/>
                        </xsl:attribute>
                    </xsl:if>
                </img>
                <!--<p>
                    <xsl:call-template name="ctl-attr-common">
                        <xsl:with-param name="cssclass">static</xsl:with-param>
                    </xsl:call-template>
                    <xsl:value-of select="Value" disable-output-escaping="yes"/>
                </p>-->
            </div>
        </div>
    </xsl:template>

</xsl:stylesheet>
