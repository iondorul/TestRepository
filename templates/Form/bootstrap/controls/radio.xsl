<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">
   
    <xsl:import href="label.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-radio">
        <div>
            <xsl:attribute name="class">control-group <xsl:if test="LabelCssClass != ''"> control-group-<xsl:value-of select="LabelCssClass"/></xsl:if></xsl:attribute>
            <xsl:call-template name="ctl-label" />
            <div class="controls">
                <xsl:if test="/Form/Settings/LabelWidth > 0 and /Form/Settings/LabelAlign != 'top' and /Form/Settings/LabelAlign != 'inside'">
                    <xsl:attribute name="style">margin-left: <xsl:value-of select="/Form/Settings/LabelWidth + 20"/>px;</xsl:attribute>
                </xsl:if>
                <xsl:if test="/Form/Settings/LabelAlign = 'inside'">
                    <xsl:attribute name="style">
                        <xsl:text>margin-left: 0px;</xsl:text>
                    </xsl:attribute>
                </xsl:if>
                <xsl:for-each select="Option">
                    <label>
                        <xsl:attribute name="class">
                            <xsl:text> radio </xsl:text>
                            <xsl:if test="/Form/Settings/ClientSideValidation ='True' and IsRequired='True'">
                                <xsl:text> required </xsl:text>
                            </xsl:if>
                            <xsl:value-of select="../CssClass" />
                        </xsl:attribute>
                        <xsl:if test="../CssStyles != ''"><xsl:attribute name="style"><xsl:value-of select="../CssStyles"/></xsl:attribute></xsl:if>
                        

                        <input type="radio">
                            <xsl:attribute name="name"><xsl:value-of select="/Form/Settings/BaseId"/><xsl:value-of select="../Name" /></xsl:attribute>
                            <xsl:attribute name="value"><xsl:value-of select="@value"/></xsl:attribute>
                            <xsl:attribute name="class">
                                <xsl:if test="/Form/Settings/ClientSideValidation ='True' and ../IsRequired='True'">
                                    <xsl:text> required </xsl:text>
                                </xsl:if>
                            </xsl:attribute>
                            <xsl:if test="../Value = @value">
                                <xsl:attribute name="checked">checked</xsl:attribute>
                            </xsl:if>
                            <xsl:if test="IsEnabled != 'True'">
                                <xsl:attribute name="disabled">disabled</xsl:attribute>
                            </xsl:if>
                        </input>
                        <xsl:value-of select="."/>
                </label>
                </xsl:for-each>
                <div class="err-placeholder"></div>
            </div>
        </div>

    </xsl:template>
    
</xsl:stylesheet>
