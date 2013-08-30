<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="label.xsl"/>
    <xsl:import href="attr-common.xsl"/>
    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template name="ctl-upload">
        <xsl:param name="addclass" />

        <div>
            <xsl:attribute name="class">upload-root control-group <xsl:if test="LabelCssClass != ''"> control-group-<xsl:value-of select="LabelCssClass"/></xsl:if>
            </xsl:attribute>
            <xsl:call-template name="ctl-label">
                <xsl:with-param name="for">
                    <xsl:value-of select="/Form/Settings/BaseId"/>
                    <xsl:value-of select="Name"/>
                </xsl:with-param>
            </xsl:call-template>
            <div class="controls">
                <xsl:if test="/Form/Settings/LabelWidth > 0 and /Form/Settings/LabelAlign != 'top' and /Form/Settings/LabelAlign != 'inside'">
                    <xsl:attribute name="style">
                        margin-left: <xsl:value-of select="/Form/Settings/LabelWidth + 20"/>px;
                    </xsl:attribute>
                </xsl:if>
                <xsl:if test="/Form/Settings/LabelAlign = 'inside'">
                    <xsl:attribute name="style">
                        <xsl:text>margin-left: 0px;</xsl:text>
                    </xsl:attribute>
                </xsl:if>
                <!-- The fileinput-button span is used to style the file input field as button -->
                <span class="btn btn-default fileinput-button pull-left">
                    <span>Select file...</span>
                    <!-- The file input field used as target for the file upload widget -->
                    <!--<input id="fileupload" type="file" name="files[]" multiple="" />-->
                    <input type="file" name="files">
                        <xsl:call-template name="ctl-attr-common">
                            <xsl:with-param name="cssclass">
                                file-upload <xsl:if test="IsRequired='True'">required</xsl:if> <xsl:value-of select="$addclass"/>
                            </xsl:with-param>
                            <xsl:with-param name="hasId">yes</xsl:with-param>
                            <xsl:with-param name="hasName">yes</xsl:with-param>
                        </xsl:call-template>
                        <xsl:attribute name="data-uploadurl">
                            <xsl:value-of select="/Form/Settings/UploadUrl"/>?mid=<xsl:value-of select="/Form/Settings/ModuleId"/>&amp;fieldid=<xsl:value-of select="Id"/>
                        </xsl:attribute>
                    </input>
                        
                </span>
                
                <!-- The global progress bar -->
                <div class="progress progress-success progress-striped pull-left span6" style="margin: 9px 10px 0 10px; height: 12px; min-height: 12px; display: none;">
                    <div class="bar"></div>
                </div>
                <!-- The container for the uploaded files -->
                <div class="files pull-left" style="margin: 6px 10px 0 10px; display: none;"></div>
                <div style="display: none;" class="relative-url"></div>

                <div class="clearfix"></div>

                <!--<input type="file">
                    <xsl:call-template name="ctl-attr-common">
                        <xsl:with-param name="cssclass">
                            span12 <xsl:if test="IsRequired='True'">required</xsl:if> <xsl:value-of select="$addclass"/>
                        </xsl:with-param>
                        <xsl:with-param name="hasId">yes</xsl:with-param>
                        <xsl:with-param name="hasName">yes</xsl:with-param>
                    </xsl:call-template>
                    <xsl:choose>
                        <xsl:when test="/Form/Settings/LabelAlign = 'inside'">
                            <xsl:attribute name="placeholder">
                                <xsl:value-of select="Title"/>
                            </xsl:attribute>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:attribute name="placeholder">
                                <xsl:value-of select="ShortDesc"/>
                            </xsl:attribute>
                        </xsl:otherwise>
                    </xsl:choose>
                    <xsl:attribute name="value">
                        <xsl:value-of select="Value"/>
                    </xsl:attribute>
                </input>-->
            </div>
        </div>
    </xsl:template>

</xsl:stylesheet>
