<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:tokens="af:tokens">

    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-image-button">
        <a href="#" type ="image">
            <xsl:attribute name="class">
                submit form-button 
                <xsl:text> </xsl:text>
                <xsl:value-of select="CssClass"/>
            </xsl:attribute>
            <xsl:if test="CssStyles != ''">
                <xsl:attribute name="style">
                    <xsl:value-of select="CssStyles"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:if test="CausesValidation != 'False'">
                <xsl:attribute name="data-validation">on</xsl:attribute>
            </xsl:if>
            <xsl:attribute name="data-submiturl">
                <xsl:value-of select="/Form/Settings/AjaxSubmitUrl"/>&amp;event=click&amp;b=<xsl:value-of select="Id"/>
            </xsl:attribute>
            
            <img>
                <xsl:attribute name="src">
                    <xsl:value-of select="ImageURL"/>
                </xsl:attribute>
            </img>
            
        </a>
    </xsl:template>

</xsl:stylesheet>
