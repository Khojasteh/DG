<?xml version="1.0" encoding="utf-8"?>
<!--
Copyright (c) 2019 Kambiz Khojasteh
Released under the MIT software license, see the accompanying
file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.
-->
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:sd="http://xmldoc.transform" exclude-result-prefixes="sd">

  <xsl:output method="text" omit-xml-declaration="yes" indent="no" />

  <xsl:template match="text()[preceding-sibling::node() and following-sibling::node()]">
    <xsl:variable name="txt" select="normalize-space(concat('%', . ,'%'))" />
    <xsl:value-of disable-output-escaping="yes" select="substring($txt, 2, string-length($txt) - 2)" />
  </xsl:template>

  <xsl:template match="text()[preceding-sibling::node() and not(following-sibling::node())]">
    <xsl:variable name="txt" select="normalize-space(concat('%', .))" />
    <xsl:value-of disable-output-escaping="yes" select="substring($txt, 2, string-length($txt) - 1)" />
  </xsl:template>

  <xsl:template match="text()[not(preceding-sibling::node()) and following-sibling::node()]">
    <xsl:variable name="txt" select="normalize-space(concat(., '%'))" />
    <xsl:value-of disable-output-escaping="yes" select="substring($txt, 1, string-length($txt) - 1 )" />
  </xsl:template>

  <xsl:template match="text()[not(preceding-sibling::node()) and  not(following-sibling::node())]">
    <xsl:value-of disable-output-escaping="yes" select="normalize-space()" />
  </xsl:template>

  <xsl:template match="/">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="para">
    <xsl:text xml:space="preserve">&#10;&#10;</xsl:text>
    <xsl:apply-templates />
    <xsl:text xml:space="preserve">&#10;&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="c">
    <xsl:text>`</xsl:text>
    <xsl:value-of disable-output-escaping="yes" select="normalize-space()" />
    <xsl:text>`</xsl:text>
  </xsl:template>

  <xsl:template match="code">
    <xsl:text>&#10;```</xsl:text>
    <xsl:choose>
      <xsl:when test="@lang">
        <xsl:value-of disable-output-escaping="yes" select="@lang" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of disable-output-escaping="yes" select="'csharp'" />
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text xml:space="preserve">&#10;</xsl:text>
    <xsl:value-of disable-output-escaping="yes" select="sd:TrimIndent(text())" />
    <xsl:text xml:space="preserve">&#10;```&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="paramref|typeparamref">
    <xsl:text>`</xsl:text>
    <xsl:value-of disable-output-escaping="yes" select="@name" />
    <xsl:text>`</xsl:text>
  </xsl:template>

  <xsl:template match="see[@langword]">
    <xsl:text>`</xsl:text>
    <xsl:value-of disable-output-escaping="yes" select="@langword" />
    <xsl:text>`</xsl:text>
  </xsl:template>

  <xsl:template match="see[@cref]">
    <xsl:variable name="url">
      <xsl:value-of disable-output-escaping="yes" select="sd:UrlOf(@cref)" />
    </xsl:variable>
    <xsl:variable name="txt">
      <xsl:value-of disable-output-escaping="yes" select="sd:NameOf(@cref)" />
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="$url">
        <xsl:text>[`</xsl:text>
        <xsl:value-of disable-output-escaping="yes" select="$txt" />
        <xsl:text>`]</xsl:text>
        <xsl:text>(</xsl:text>
        <xsl:value-of disable-output-escaping="yes" select="$url" />
        <xsl:text>)</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of disable-output-escaping="yes" select="$txt" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="note">
    <xsl:text xml:space="preserve">&#10;&#10;&gt; </xsl:text>
    <xsl:if test="@type">
      <xsl:text xml:space="preserve">**</xsl:text>
      <xsl:value-of disable-output-escaping="yes" select="@type" />
      <xsl:text xml:space="preserve">**\&#10;&gt; </xsl:text>
    </xsl:if>
    <xsl:apply-templates />
    <xsl:text xml:space="preserve">&#10;&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="list">
    <xsl:text xml:space="preserve">&#10;&#10;</xsl:text>
    <xsl:choose>
      <xsl:when test="@type='table'">
        <xsl:for-each select="listheader">
          <xsl:apply-templates select="term" />
          <xsl:text xml:space="preserve"> | </xsl:text>
          <xsl:apply-templates select="description" />
          <xsl:text xml:space="preserve">&#10;</xsl:text>
        </xsl:for-each>
        <xsl:text xml:space="preserve">--- | ---&#10;</xsl:text>
        <xsl:for-each select="item">
          <xsl:apply-templates select="term" />
          <xsl:text xml:space="preserve"> | </xsl:text>
          <xsl:apply-templates select="description" />
          <xsl:text xml:space="preserve">&#10;</xsl:text>
        </xsl:for-each>
      </xsl:when>
      <xsl:when test="@type='number'">
        <xsl:for-each select="listheader|item">
          <xsl:number value="position()" format="1" />
          <xsl:text>. </xsl:text>
          <xsl:call-template name="list-item" />
        </xsl:for-each>
      </xsl:when>
      <xsl:otherwise>
        <xsl:for-each select="listheader|item">
          <xsl:text>- </xsl:text>
          <xsl:call-template name="list-item" />
        </xsl:for-each>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="list-item">
    <xsl:if test="term">
      <xsl:text>**</xsl:text>
      <xsl:apply-templates select="term" />
      <xsl:text xml:space="preserve">**\&#10;</xsl:text>
    </xsl:if>
    <xsl:apply-templates select="description" />
    <xsl:text xml:space="preserve">&#10;&#10;</xsl:text>
  </xsl:template>

</xsl:stylesheet>
