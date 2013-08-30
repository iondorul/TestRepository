<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="controls/main.xsl"/>

    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />
    
    <xsl:template match="/">

        <div class="bstrap">
            <xsl:attribute name="id"><xsl:value-of select="/Form/Settings/BaseId"/>root</xsl:attribute>
            <div class="">
                <xsl:attribute name="class">
                    <xsl:text>c-form </xsl:text>
                    <xsl:if test="/Form/Settings/LabelAlign != 'top'"> form-horizontal </xsl:if>
                    <xsl:value-of select="/Form/Settings/FieldSpacing" />
                    <xsl:text> row-fluid</xsl:text>
                </xsl:attribute>

                <xsl:choose>
                    <xsl:when test="/Form/Settings/HasCustomLayout = 'True'">
                        <xsl:value-of select="/Form/Settings/LayoutHtml" disable-output-escaping="yes" />
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:for-each select="/Form/Fields/Field[InputType != 'button' or ShowIn/Form = 'True']">
                            <xsl:if test="position() = 1 or RowIndex != preceding-sibling::node()[1]/RowIndex">
                                <xsl:if test="position() > 1"><xsl:text disable-output-escaping="yes">&lt;/div&gt;</xsl:text></xsl:if>
                                <xsl:text disable-output-escaping="yes">&lt;div class="row-fluid"&gt;</xsl:text>
                            </xsl:if>
                            <div>
                                <xsl:attribute name="class">offset<xsl:value-of select="ColOffset"/> span<xsl:value-of select="ColSpan"/></xsl:attribute>
                                <xsl:call-template name="ctl-render" />
                            </div>
                        </xsl:for-each>
                        
                        <!--close last row-->
                        <xsl:text disable-output-escaping="yes">&lt;/div&gt;</xsl:text>
                        
                        <div class="clearfix"></div>

                        <xsl:if test="/Form/Fields/Field[ShowIn/ButtonsPane='True']">
                            <div class="form-actions">
                                <xsl:if test="/Form/Settings/LabelWidth > 0 and /Form/Settings/LabelAlign != 'top' and /Form/Settings/LabelAlign != 'inside'">
                                    <xsl:attribute name="style">
                                        padding-left: <xsl:value-of select="/Form/Settings/LabelWidth + 30"/>px;
                                    </xsl:attribute>
                                </xsl:if>
                                <xsl:if test="/Form/Settings/LabelAlign = 'inside'">
                                    <xsl:attribute name="style">
                                        <xsl:text>padding-left: 21px;</xsl:text>
                                    </xsl:attribute>
                                </xsl:if>

                                <xsl:for-each select="/Form/Fields/Field[ShowIn/ButtonsPane='True']">
                                    <xsl:choose>
                                        <xsl:when test="InputType = 'image-button'">
                                            <xsl:call-template name="ctl-image-button" />
                                        </xsl:when>
                                        <xsl:otherwise>
                                            <xsl:call-template name="ctl-button" />
                                        </xsl:otherwise>
                                    </xsl:choose>
                                </xsl:for-each>

                                <img style="display: none; margin-top: 7px;" class="pull-right submit-progress">
                                    <xsl:attribute name="src">
                                        <xsl:value-of select="/Form/Settings/FormTemplateFolder" />/img/loading.gif
                                    </xsl:attribute>
                                </img>

                            </div>
                        </xsl:if>
                    </xsl:otherwise>
                </xsl:choose>

                <div class="alert alert-error server-error" style="display: none;">
                </div>

                
            </div>

            <div class="alert alert-info submit-confirm" style="display: none; text-align: center;">
                
            </div>
            
        </div>

        <script>
            <xsl:attribute name="src">
                <xsl:value-of select="/Form/Settings/FormTemplateFolder"/>/fileupload/js/vendor/jquery.ui.widget.js
            </xsl:attribute>
        </script>

        <script>
            <xsl:attribute name="src">
                <xsl:value-of select="/Form/Settings/FormTemplateFolder"/>/fileupload/js/jquery.iframe-transport.js
            </xsl:attribute>
        </script>

        <script>
            <xsl:attribute name="src">
                <xsl:value-of select="/Form/Settings/FormTemplateFolder"/>/fileupload/js/jquery.fileupload.js
            </xsl:attribute>
        </script>

    </xsl:template>
</xsl:stylesheet>
