<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:sd="http://xmldoc.transform" exclude-result-prefixes="sd">
  
  <xsl:output method="html" omit-xml-declaration="yes" indent="no" />

  <xsl:template match="text()[preceding-sibling::node() and following-sibling::node()]">
    <xsl:variable name="txt" select="normalize-space(concat('%', . ,'%'))" />
    <xsl:value-of select="substring($txt, 2, string-length($txt) - 2)" />
  </xsl:template>

  <xsl:template match="text()[preceding-sibling::node() and not(following-sibling::node())]">
    <xsl:variable name="txt" select="normalize-space(concat('%', .))" />
    <xsl:value-of select="substring($txt, 2, string-length($txt) - 1)" />
  </xsl:template>

  <xsl:template match="text()[not(preceding-sibling::node()) and following-sibling::node()]">
    <xsl:variable name="txt" select="normalize-space(concat(., '%'))" />
    <xsl:value-of select="substring($txt, 1, string-length($txt) - 1 )" />
  </xsl:template>

  <xsl:template match="text()[not(preceding-sibling::node()) and  not(following-sibling::node())]">
    <xsl:value-of select="normalize-space()" />
  </xsl:template>

  <xsl:template match="/">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="para">
    <p>
      <xsl:apply-templates />
    </p>
  </xsl:template>

  <xsl:template match="c">
    <code>
      <xsl:value-of select="normalize-space()" />
    </code>
  </xsl:template>

  <xsl:template match="code">
    <pre class="prettyprint">
      <xsl:text xml:space="preserve">&#10;</xsl:text>
      <xsl:value-of select="sd:TrimIndent(text())" />
      <xsl:text xml:space="preserve">&#10;</xsl:text>
    </pre>
  </xsl:template>

  <xsl:template match="paramref|typeparamref">
    <code>
      <xsl:value-of select="@name" />
    </code>
  </xsl:template>

  <xsl:template match="see[@langword]">
    <code>
      <xsl:value-of select="@langword" />
    </code>
  </xsl:template>

  <xsl:template match="see[@cref]">
    <a>
      <xsl:attribute name="href">
        <xsl:value-of select="sd:UrlOf(@cref)" />
      </xsl:attribute>
      <code>
        <xsl:value-of select="sd:NameOf(@cref)" />
      </code>
    </a>
  </xsl:template>

  <xsl:template match="note">
    <div class="note">
      <xsl:if test="@type">
        <h6 class="note-type">
          <xsl:value-of select="@type" />
        </h6>
      </xsl:if>
      <xsl:apply-templates />
    </div>
  </xsl:template>

  <xsl:template match="list">
    <xsl:choose>
      <xsl:when test="@type='table'">
        <table>
          <xsl:for-each select="listheader">
            <thead>
              <tr>
                <th class="term">
                  <xsl:apply-templates select="term" />
                </th>
                <th class="description">
                  <xsl:apply-templates select="description" />
                </th>
              </tr>
            </thead>
          </xsl:for-each>
          <tbody>
            <xsl:for-each select="item">
              <tr>
                <td class="term">
                  <xsl:apply-templates select="term" />
                </td>
                <td class="description">
                  <xsl:apply-templates select="description" />
                </td>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
      </xsl:when>
      <xsl:when test="@type='number'">
        <ol>
          <xsl:for-each select="listheader|item">
            <xsl:call-template name="list-item" />
          </xsl:for-each>
        </ol>
      </xsl:when>
      <xsl:otherwise>
        <ul>
          <xsl:for-each select="listheader|item">
            <xsl:call-template name="list-item" />
          </xsl:for-each>
        </ul>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="list-item">
    <li>
      <xsl:if test="term">
        <xsl:attribute name="class">
          <xsl:value-of select="'has-term'"/>
        </xsl:attribute>
        <strong class="term">
          <xsl:apply-templates select="term" />
        </strong>
        <br />
      </xsl:if>
      <xsl:apply-templates select="description" />
    </li>
  </xsl:template>

</xsl:stylesheet>
