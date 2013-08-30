<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:utils="af:utils">

    <xsl:import href="textbox.xsl"/>
    <xsl:import href="textarea.xsl"/>
    <xsl:import href="dropdown.xsl"/>
    <xsl:import href="itemwithqty.xsl"/>
    <xsl:import href="checkbox.xsl"/>
    <xsl:import href="radio.xsl"/>
    <xsl:import href="checkbox-list.xsl"/>
    <xsl:import href="hidden.xsl"/>
    <xsl:import href="static.xsl"/>
    <xsl:import href="image.xsl"/>
    <xsl:import href="label-only.xsl"/>
    <xsl:import href="datepicker.xsl"/>
    <xsl:import href="captcha.xsl"/>

    <xsl:import href="country.xsl"/>
    <xsl:import href="region.xsl"/>

    <xsl:import href="upload.xsl"/>
    <xsl:import href="button.xsl"/>
    <xsl:import href="image-button.xsl"/>

    <xsl:output method="html" indent="no" omit-xml-declaration="yes" />

    <xsl:template match="/">
        <xsl:for-each select="/Form/Fields/Field">
            <xsl:call-template name="ctl-render" />
        </xsl:for-each>
    </xsl:template>

    <xsl:template name="ctl-render">
        <xsl:choose>
            <xsl:when test="InputType = 'open-text' or InputType = 'open-email' or InputType = 'open-number' or InputType = 'open-username'">
                <xsl:call-template name="ctl-textbox">
                    <xsl:with-param name="type">text</xsl:with-param>
                </xsl:call-template>
            </xsl:when>

            <xsl:when test="InputType = 'open-itemwithqty'">
                <xsl:call-template name="ctl-itemwithqty" />
            </xsl:when>
            <xsl:when test="InputType = 'open-password'">
                <xsl:call-template name="ctl-textbox">
                    <xsl:with-param name="type">password</xsl:with-param>
                </xsl:call-template>
            </xsl:when>
            <!--<xsl:when test="InputType = 'open-password-confirm'">
                <xsl:call-template name="ctl-textbox">
                    <xsl:with-param name="type">password</xsl:with-param>
                </xsl:call-template>
            </xsl:when>-->

            <xsl:when test="InputType = 'open-text-large'">
                <xsl:call-template name="ctl-textarea" />
            </xsl:when>
            <xsl:when test="InputType = 'open-text-rich'">
                <xsl:call-template name="ctl-textarea">
                    <xsl:with-param name="addclass">richedit</xsl:with-param>
                </xsl:call-template>
            </xsl:when>

            <xsl:when test="InputType = 'closed-truefalse'">
                <xsl:call-template name="ctl-checkbox" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-yesno'">
                <xsl:call-template name="ctl-radio" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-yesnomaybe'">
                <xsl:call-template name="ctl-radio" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-multiple-radio'">
                <xsl:call-template name="ctl-radio" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-multiple-radio-horiz'">
                <xsl:call-template name="ctl-radio" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-multiple-checkbox'">
                <xsl:call-template name="ctl-checkbox-list" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-multiple-checkbox-horiz'">
                <xsl:call-template name="ctl-checkbox-list" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-multiple-dropdown'">
                <xsl:call-template name="ctl-dropdown" />
            </xsl:when>

            <xsl:when test="InputType = 'closed-likert-agree'">
                <xsl:call-template name="ctl-radio" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-likert-frequency'">
                <xsl:call-template name="ctl-radio" />
            </xsl:when>
            <xsl:when test="InputType = 'closed-likert-importance'">
                <xsl:call-template name="ctl-radio" />
            </xsl:when>

            <xsl:when test="InputType = 'address-countryname'">
                <xsl:call-template name="ctl-country"/>
            </xsl:when>

            <xsl:when test="InputType = 'address-countrycode'">
                <xsl:call-template name="ctl-country" >
                    <xsl:with-param name="addclass">country</xsl:with-param>
                </xsl:call-template>
            </xsl:when>

            <xsl:when test="InputType = 'address-regionname'">
                <xsl:call-template name="ctl-region" />
            </xsl:when>
            <xsl:when test="InputType = 'address-regioncode'">
                <xsl:call-template name="ctl-region" />
            </xsl:when>

            <xsl:when test="InputType = 'address-usstate'">
                <xsl:call-template name="ctl-dropdown" />
            </xsl:when>
            <xsl:when test="InputType = 'address-usstate-name'">
                <xsl:call-template name="ctl-dropdown" />
            </xsl:when>

            <xsl:when test="InputType = 'hiden-constant'">
                <xsl:call-template name="ctl-hidden" />
            </xsl:when>
            <xsl:when test="InputType = 'hiden-userid'">
                <xsl:call-template name="ctl-hidden" />
            </xsl:when>
            <xsl:when test="InputType = 'static-labelonly'">
                <xsl:call-template name="ctl-label-only" />
            </xsl:when>
            <xsl:when test="InputType = 'static-text'">
                <xsl:call-template name="ctl-static" />
            </xsl:when>
            <xsl:when test="InputType = 'static-image'">
                <xsl:call-template name="ctl-image" />
            </xsl:when>

            <xsl:when test="InputType = 'dnnpotals-all'">
                <xsl:call-template name="ctl-dropdown" />
            </xsl:when>
            <xsl:when test="InputType = 'dnnpotals-except0'">
                <xsl:call-template name="ctl-dropdown" />
            </xsl:when>

            <xsl:when test="InputType = 'datetime'">
                <xsl:call-template name="ctl-datepicker" />
            </xsl:when>
            <xsl:when test="InputType = 'datetime-monthyear'">
                <xsl:call-template name="ctl-datepicker" />
            </xsl:when>

            <xsl:when test="InputType = 'captcha'">
                <xsl:call-template name="ctl-captcha" />
            </xsl:when>

            <xsl:when test="InputType = 'upload.single'">
                <xsl:call-template name="ctl-upload" />
            </xsl:when>

            <xsl:when test="InputType = 'upload.multi'">
                <xsl:call-template name="ctl-upload" />
            </xsl:when>

            <xsl:when test="InputType = 'button'">
                <xsl:call-template name="ctl-button" />
            </xsl:when>

            <xsl:when test="InputType = 'image-button'">
                <xsl:call-template name="ctl-image-button" />
            </xsl:when>
            
            <xsl:otherwise>
                <p style="color: red;">
                    UNKNOWN FROM FIELD <xsl:value-of select="InputType"/>
                </p>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>

