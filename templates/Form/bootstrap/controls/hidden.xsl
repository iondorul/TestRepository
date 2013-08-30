<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-hidden">
        
        <input type="hidden">
            <xsl:attribute name="name"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name" /></xsl:attribute>
            <xsl:attribute name="value"><xsl:value-of select="Value"/></xsl:attribute>
        </input>

    </xsl:template>
    
</xsl:stylesheet>
