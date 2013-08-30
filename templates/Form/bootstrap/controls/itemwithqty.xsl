<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="label.xsl"/>
    <xsl:import href="attr-common.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-itemwithqty">
        
        <div>
            <xsl:attribute name="class">control-group itemwithqty <xsl:if test="LabelCssClass != ''"> control-group-<xsl:value-of select="LabelCssClass"/></xsl:if></xsl:attribute>
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
				<select class="input-small">
					<xsl:attribute name="id"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="Name"/>Qty</xsl:attribute>
					<xsl:call-template name="ctl-itemwithqty-option">
						<xsl:with-param name="count">20</xsl:with-param>
						<xsl:with-param name="i">1</xsl:with-param>
						<xsl:with-param name="val"><xsl:value-of select="substring-before(Value,' ')"/></xsl:with-param>
					</xsl:call-template>
                    <xsl:if test="IsEnabled != 'True'">
                        <xsl:attribute name="disabled">disabled</xsl:attribute>
                    </xsl:if>
				</select>
				
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
                    <xsl:attribute name="value"><xsl:value-of select="substring-after(Value,' ')"/></xsl:attribute>
                    <xsl:if test="IsEnabled != 'True'">
                        <xsl:attribute name="disabled">disabled</xsl:attribute>
                    </xsl:if>
                </input>
            </div>
        </div>
    </xsl:template>
	
	<xsl:template name="ctl-itemwithqty-option">
		<xsl:param name="i">1</xsl:param>
		<xsl:param name="count"></xsl:param>
		<xsl:param name="val"></xsl:param>
		<option>
			<xsl:if test="$i = $val">
				<xsl:attribute name="selected">selected</xsl:attribute>
			</xsl:if>
			<xsl:value-of select="$i" />
		</option>
		
		<xsl:if test="$i &lt; $count">
			<xsl:call-template name="ctl-itemwithqty-option">
				<xsl:with-param name="count"><xsl:value-of select="$count" /></xsl:with-param>
				<xsl:with-param name="i"><xsl:value-of select="$i + 1" /></xsl:with-param>
				<xsl:with-param name="val"><xsl:value-of select="$val" /></xsl:with-param>
			</xsl:call-template>
		</xsl:if>
		
    </xsl:template>
	
</xsl:stylesheet>
