<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">
    
    <xsl:import href="attr-common.xsl"/>
    <xsl:import href="label.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-static">
        <div>
            <xsl:attribute name="class">control-group <xsl:if test="LabelCssClass != ''"> control-group-<xsl:value-of select="LabelCssClass"/></xsl:if></xsl:attribute>

            <xsl:if test="ShowLabel = 'True'">
                <xsl:call-template name="ctl-label">
                    <xsl:with-param name="for"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/></xsl:with-param>
                </xsl:call-template>
            </xsl:if>

            <xsl:if test="ShowText = 'True'">
                <div class="controls">
                    <xsl:if test="/Form/Settings/LabelWidth > 0 and /Form/Settings/LabelAlign != 'top' and /Form/Settings/LabelAlign != 'inside'">
                        <xsl:attribute name="style">
                            margin-left: <xsl:value-of select="/Form/Settings/LabelWidth + 26"/>px;
                        </xsl:attribute>
                    </xsl:if>
                    <p>
                        <xsl:call-template name="ctl-attr-common">
                            <xsl:with-param name="cssclass">static</xsl:with-param>
                        </xsl:call-template>
                        <xsl:value-of select="Value" disable-output-escaping="yes"/>
                    </p>
                </div>
            </xsl:if>
        </div>
    </xsl:template>

</xsl:stylesheet>
