<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:tokens="af:tokens">

    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-button">
        <button type ="button" data-loading-text ="Please Wait...">
            <xsl:attribute name="class">
                btn submit form-button loading btn-primary
                <xsl:text> </xsl:text>
                <xsl:value-of select="ButtonSize"></xsl:value-of>
                <xsl:text> </xsl:text>
                <xsl:value-of select="ButtonType"></xsl:value-of>
                <xsl:if test="/Settings/RenderContext = 'Form' and ShowIn/ButtonsPane = 'False'">
                    <xsl:text> btn-block</xsl:text>
                </xsl:if>
                <xsl:text> </xsl:text>
                <xsl:value-of select="CssClass"/>
            </xsl:attribute>
            <xsl:if test="CssStyles != ''">
                <xsl:attribute name="style">
                    <xsl:value-of select="CssStyles"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:if test="not(CausesValidation) or CausesValidation != 'False'">
                <xsl:attribute name="data-validation">on</xsl:attribute>
            </xsl:if>
            <xsl:attribute name="data-submiturl">
                <xsl:value-of select="/Form/Settings/AjaxSubmitUrl"/>&amp;event=click&amp;b=<xsl:value-of select="Id"/>
            </xsl:attribute>
            
            <xsl:value-of select="Title"/>
        </button>
        <xsl:text> </xsl:text>
    </xsl:template>

</xsl:stylesheet>
