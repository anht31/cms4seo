using System;
using System.Collections.Generic;
using System.Text;

using CONSTANTS = HTMLTOCGenerator.Constants;
using DATA = HTMLTOCGenerator.Data;
using ELEMENT = HTMLTOCGenerator.Element;
using ELEMENT_TYPE = HTMLTOCGenerator.Constants.Element_Type;
using HTML_PARSER = HTMLTOCGenerator.HTMLParser;
using TOC_DIV = HTMLTOCGenerator.TOCDIV;
using TOC_NUMBERING = HTMLTOCGenerator.TOCNumbering;

/// <TODO>
/// the current algorithm uses an indexed buffer; the algorithm should 
/// be modified to use a StringReader; although StreamReader might be 
/// considered, using StreamReader binds the parsing to an IO method 
/// that is best left in the HTMLTOCGeneratorDialog
/// </TODO>
    

namespace HTMLTOCGenerator
    {

    // **************************************** class HTMLTOCGenerator

    public class HTMLTOCGenerator
        {

        // ****************************************** generate_TOC_div

        /// <summary>
        /// return a new <div> for the TOC_DIV. This TOC_DIV-div will 
        /// replace the existing TOC_DIV-div
        /// </summary>
        string generate_TOC_div ( )
            {
            int             bookmark_count = 0;
            bool            first_tag = true;
            StringBuilder   indent_sb = new StringBuilder ( );
            int             last_level = 0;
            int             missing_heading_count = 0;
            StringBuilder   TOC_div = new StringBuilder ( );
            Stack < int >   level_stack = new Stack < int > ( );

            TOC_div.Length = 0;
            TOC_NUMBERING.initialize_toc_numbering_levels ( );
                                        // create a outer <div> that 
                                        // reflects the current 
                                        // processing
            TOC_div.AppendFormat ( 
                "\n<div {0}=\"{1}\"\n" +
                "     {2}=\"{3}:",
                CONSTANTS.TOC_CLASS_ATTRIBUTE_NAME.ToLower ( ),
                CONSTANTS.TOC_CLASS_ATTRIBUTE_VALUE.ToLower ( ),
                CONSTANTS.TOC_STYLE_ATTRIBUTE_NAME.ToLower ( ),
                CONSTANTS.TOC_DIV_TOC_HEADERS.ToLower ( ) );
            foreach ( string desired_header in DATA.DesiredHeaders )
                {
                                        // ToLower because values will
                                        // appear as an HTML value
                TOC_div.AppendFormat (
                    "{0},",
                    desired_header.ToLower ( ).Trim ( ) );
                }
            TOC_div.Length--;            // remove last comma
            TOC_div.Append ( ";\n" );
            TOC_div.AppendFormat ( 
                "            {0}:{1};\n" +
                "            {2}:{3};\n" +
                "            {4}:{5};\n" +
                "            {6}:{7};\n",
                CONSTANTS.TOC_DIV_TOC_RETURN.ToLower ( ),
                ( TOC_DIV.WantsTocReturn ? "true" : "false" ),
                CONSTANTS.TOC_DIV_TOC_TITLE.ToLower ( ),
                TOC_DIV.TocTitle,
                CONSTANTS.TOC_DIV_TOC_IMAGE.ToLower ( ),
                TOC_DIV.TocImage,
                CONSTANTS.TOC_DIV_TOC_IMAGE_WIDTH.ToLower ( ),
                TOC_DIV.TocImageWidth );

            TOC_div.AppendFormat ( 
                "            {0}:{1};\n" +
                "            {2}:{3};",
                CONSTANTS.TOC_DIV_TOC_IMAGE_HEIGHT.ToLower ( ),
                TOC_DIV.TocImageHeight,
                CONSTANTS.TOC_DIV_TOC_HEADER_LEVEL.ToLower ( ),
                TOC_DIV.TocHeaderLevel );
                
            if ( TOC_NUMBERING.WantsTocNumbering )
                {
                TOC_div.AppendFormat ( 
                    "\n" +
                    "            {0}:{1};",
                    CONSTANTS.TOC_DIV_TOC_NUMBERING.ToLower ( ),
                    TOC_NUMBERING.TocNumberingPropertyValue );
                }

            TOC_div.Append ( "\" >\n" );

            indent_sb.Append ( CONSTANTS.INDENTATION_STRING );
                                        // add the TOC_DIV title 
                                        // header with class 
                                        // "toc-generated"
            if ( TOC_DIV.WantsTocTitle )
                {
                TOC_div.AppendFormat ( 
                    "{0}<{1} class=\"{2}\">{3}</{1}>\n",
                    indent_sb.ToString ( ),
                    TOC_DIV.TocHeaderLevel,
                    CONSTANTS.TOC_GENERATED_CLASS,
                    TOC_DIV.TocTitle );
                }
                                        // create an inner <div> that 
                                        // holds the actual TOC with 
                                        // class "toc-generated" 
            TOC_div.AppendFormat ( "{0}<div class=\"{1}\">\n",
                                   indent_sb.ToString ( ),
                                   CONSTANTS.TOC_GENERATED_CLASS );
            indent_sb.Append ( CONSTANTS.INDENTATION_STRING );
                                        // create the TOC_DIV bookmark
            if ( TOC_DIV.WantsTocReturn )
                {
                TOC_div.AppendFormat ( 
                    "{0}<a id=\"{1}\">&nbsp;</a>\n",
                    indent_sb.ToString ( ),
                    TOC_DIV.TocBookmark );
                }
                                        // process the headers
            foreach ( ELEMENT element in DATA.Elements )
                {
                string  heading = element.Content;
                bool    indent = false;
                bool    outdent = false;
                int     tag_level = 0;
                string  tag_name = element.TagName;
                                        // process h2 through h6 only
                if ( element.ElementType == ELEMENT_TYPE.TOCDIV )
                    {
                    continue;
                    }
                                        // if heading missing, create 
                                        // a dummy
                if ( String.IsNullOrEmpty ( heading ) )
                    {
                    heading = String.Format ( "missing_heading_{0}",
                                              missing_heading_count );
                    missing_heading_count++;
                    }
                                        // generate unique bookmark
                                        // for this heading
                element.Bookmark = String.Format ( "toc_bookmark_{0}",
                                                   bookmark_count++ );
                                        // compare the last level to 
                                        // the current level setting 
                                        // indent and outdent 
                                        // appropriately
                tag_level = Convert.ToInt32 ( 
                                        tag_name [ 1 ].ToString ( ) );
                                        // if first tag, set the 
                                        // last level to the tag level 
                                        // to avoid failing to 
                                        // increment second header
                if ( first_tag )
                    {
                    first_tag = false;
                    last_level = tag_level;
                    TOC_div.AppendFormat ( "{0}<ul>\n",
                                          indent_sb.ToString ( ) );
                    indent_sb.Append ( CONSTANTS.INDENTATION_STRING );
                    }
                    
                if ( tag_level < last_level )
                    {
                    indent = false;
                    outdent = true;
                    }
                else if ( tag_level == last_level )
                    {
                    indent = false;
                    outdent = false;
                    }
                else
                    {
                    indent = true;
                    outdent = false;
                    }
                                        // generate the numeric prefix
                if ( TOC_NUMBERING.WantsTocNumbering )
                    {
                    StringBuilder   sb = new StringBuilder ( );
                    
                    if ( indent )
                        {
                        TOC_NUMBERING.decrement_toc_numbering_level ( 
                                                    tag_level - 1 );
                        }

                    if ( outdent )
                        {
                        for ( int i = tag_level + 1; 
                                ( i < CONSTANTS.MAXIMUM_LEVELS ); 
                                  i++ )
                            {
                            TOC_NUMBERING.set_toc_numbering_level ( 
                                                                i, 
                                                                1 );
                            }
                        TOC_NUMBERING.increment_toc_numbering_level ( 
                                                        tag_level );
                        }

                    for ( int i = CONSTANTS.MINIMUM_LEVEL;
                            ( i <= tag_level );
                              i++ )
                        {
                        sb.AppendFormat ( 
                            "{0}.",
                            TOC_NUMBERING.get_toc_numbering_level ( 
                                                                i ) );
                        }
                    element.TocNumericPrefix = sb.ToString ( );
                    TOC_NUMBERING.increment_toc_numbering_level ( 
                                                        tag_level );
                    
                    }

                if ( indent )
                    {
                    level_stack.Push ( tag_level );
                    
                    if ( TOC_div.ToString ( ).ToLower ( ).EndsWith (
                                                        "</li>\n" ) )
                        {
                        TOC_div.Length -= "</li>\n".Length;
                        TOC_div.Append ( "\n" ); // add /n back
                        indent_sb.Append ( 
                            CONSTANTS.INDENTATION_STRING );
                        }
                    TOC_div.AppendFormat ( "{0}<ul>\n",
                                          indent_sb.ToString ( ) );
                    indent_sb.Append ( CONSTANTS.INDENTATION_STRING );
                    }

                if ( outdent )
                    {
                    while ( level_stack.Count > 0 )
                        {
                        if ( level_stack.Pop ( ) > tag_level )
                            {
                            indent_sb.Length -= CONSTANTS.INDENTATION;
                            TOC_div.AppendFormat ( 
                                        "{0}</ul>\n",
                                        indent_sb.ToString ( ) );
                            indent_sb.Length -= CONSTANTS.INDENTATION;
                            TOC_div.AppendFormat ( 
                                        "{0}</li>\n",
                                        indent_sb.ToString ( ) );
                            }
                        else
                            {
                            break;
                            }
                        }
                    level_stack.Push ( tag_level );
                    }

                if ( TOC_NUMBERING.WantsTocNumbering )
                    {
                    TOC_div.AppendFormat ( 
                        "{0}<li><a href=\"#{1}\">{2} {3}</a></li>\n",
                        indent_sb.ToString ( ),
                        element.Bookmark,
                        element.TocNumericPrefix,
                        heading );
                    }
                else
                    {
                    TOC_div.AppendFormat ( 
                        "{0}<li><a href=\"#{1}\">{2}</a></li>\n",
                        indent_sb.ToString ( ),
                        element.Bookmark,
                        heading );
                    }

                last_level = tag_level;
                }

            indent_sb.Length -= CONSTANTS.INDENTATION;
            TOC_div.AppendFormat ( "{0}</ul>\n",
                                  indent_sb.ToString ( ) );

            if ( TOC_DIV.WantsTocReturn )
                {
                TOC_div.AppendFormat (
                    "{0}<p>\n" +
                    "{0}The symbol \n" +
                    "{0}<a href=\"#{1}\">\n" +
                    "{0}  <img alt=\"{2}\"\n" +
                    "{0}       title=\"{2}\"\n" +
                    "{0}       src=\"{3}\"\n" +
                    "{0}       width=\"{4}\"\n" + 
                    "{0}       height=\"{5}\" />\n" +
                    "{0}</a> \n" +
                    "{0}returns the reader to the top of the " + 
                    "Table of Contents.\n" +
                    "{0}</p>\n",
                    indent_sb.ToString ( ),
                    TOC_DIV.TocBookmark,
                    TOC_DIV.TocTitle,
                    TOC_DIV.TocImage,
                    TOC_DIV.TocImageWidth,
                    TOC_DIV.TocImageHeight );
                }

            indent_sb.Length -= CONSTANTS.INDENTATION;
            TOC_div.AppendFormat ( "{0}</div>\n",
                                  indent_sb.ToString ( ) );
            indent_sb.Length -= CONSTANTS.INDENTATION;
            TOC_div.AppendFormat ( "{0}</div>\n",
                                  indent_sb.ToString ( ) );

            return ( TOC_div.ToString ( ) );
            }

        // ************************************************ new_header

        /// <summary>
        /// return a new <h?> for the specified element. This <h?> 
        /// will replace the existing <h?> for the element
        /// </summary>
        string new_header ( ELEMENT element )
            {
            StringBuilder  sb = new StringBuilder ( );

            sb.AppendFormat ( "\n<{0}>",
                              element.TagName );

            if ( TOC_NUMBERING.WantsTocNumbering )
                {
                sb.AppendFormat ( "<span class=\"{0}\" >" +
                                  "{1}&nbsp;" +
                                  "</span>",
                                  CONSTANTS.TOC_GENERATED_CLASS,
                                  element.TocNumericPrefix );
                
                }

            sb.AppendFormat ( "{0}\n",
                              element.Content );
                              
            sb.AppendFormat ( "  <a id=\"{0}\"\n" +
                              "     class=\"{1}\" >\n" +
                              "  </a>\n",
                              element.Bookmark,
                              CONSTANTS.TOC_GENERATED_CLASS );

            if ( TOC_DIV.WantsTocReturn )
                {
                sb.AppendFormat (
                    "  <a href=\"#{0}\"\n" +
                    "     class=\"{1}\" >\n" +
                    "    <img alt=\"{2}\"\n" +
                    "         title=\"{2}\"\n" +
                    "         src=\"{3}\"\n" +
                    "         width=\"{4}\"\n" + 
                    "         height=\"{5}\" />\n" +
                    "  </a> \n",
                    TOC_DIV.TocBookmark,
                    CONSTANTS.TOC_GENERATED_CLASS,
                    TOC_DIV.TocTitle,
                    TOC_DIV.TocImage,
                    TOC_DIV.TocImageWidth,
                    TOC_DIV.TocImageHeight );
                }

            sb.AppendFormat ( "</{0}>",
                              element.TagName );

            return ( sb.ToString ( ) );
            }

        // ********************************************** rewrite_html

        /// <summary>
        /// returns the HTML that was rewritten using the generated 
        /// TOC_DIV-div and the new headers
        /// </summary>
        string rewrite_html ( string HTML )
            {
            int             html_start = 0;
            int             html_to_copy = 0;
            StringBuilder   sb = new StringBuilder ( );
            string          TOC_div = string.Empty;
                                        // invoke generate_TOC_div 
                                        // before going through the 
                                        // elements; it generates the 
                                        // element bookmarks that are 
                                        // needed in new_header
            TOC_div = generate_TOC_div ( );
            foreach ( ELEMENT element in DATA.Elements )
                {
                                        // copy HTML up to the next 
                                        // header or TOC_DIV-div element
                html_to_copy = element.ElementStartsAt - 
                               html_start - 1;
                if ( html_to_copy > 0 )
                    {
                    sb.Append ( HTML, html_start, html_to_copy );
                    }
                                        // copy in the rewitten 
                                        // contents of the element
                if ( element.ElementType == ELEMENT_TYPE.TOCDIV )
                    {
                    sb.Append ( TOC_div );
                    }
                else
                    {
                    sb.Append ( new_header ( element ) );
                    }
                html_start = element.ElementEndsAt + 1;
                }
            html_to_copy = HTML.Length - html_start;
            sb.Append ( HTML, html_start, html_to_copy );

            return ( sb.ToString ( ) );
            }

        // ******************************************* add_TOC_to_html

        /// <summary>
        /// returns the HTML that was revised by applying the TOC_DIV-div 
        /// found in the supplied HTML
        /// </summary>
        /// </summary>
        /// <param name="HTML"></param>
        /// HTML that is to have a TOC_DIV generated
        /// <returns>
        /// if a TOC_DIV-div is found, the revised HTML; otherwise, the 
        /// source HTML
        /// </returns>
        public string add_TOC_to_html ( string html )
            {

                DATA.Elements.Clear();

            HTML_PARSER  HTML_parser = new HTML_PARSER ( );
            string      rewriten_HTML = html;

            HTML_parser.collect_all_desired_elements ( html );
            if ( TOC_DIV.HaveTOCDIV )
                {
                HTML_parser.revise_element_content ( );
                HTML_parser.eliminate_unwanted_elements ( );
                rewriten_HTML = rewrite_html ( html );
                }

            return ( rewriten_HTML );
            }

        // ************************************** remove_TOC_from_html

        /// <summary>
        /// removes HTML elements that are members of the class 
        /// "toc-generated"
        /// </summary>
        /// <param name="HTML">
        /// HTML that had a TOC_DIV generated
        /// </param>
        /// <returns>
        /// the HTML that has all HTML-TOC_DIV generated entries removed
        /// </returns>
        public string remove_TOC_from_html ( string html )
            {
            HTML_PARSER      html_parser = new HTML_PARSER ( );
            int             html_start = 0;
            int             html_to_copy = 0;
            StringBuilder   sb = new StringBuilder ( );

            html_parser.collect_all_desired_elements ( html );
            html_parser.revise_element_content ( );
            foreach ( ELEMENT element in DATA.Elements )
                {
                                        // copy HTML up to the next 
                                        // header or TOC_DIV-div element
                html_to_copy = element.ElementStartsAt - 
                               html_start - 1;
                if ( html_to_copy > 0 )
                    {
                    sb.Append ( html, html_start, html_to_copy );
                    }
                                        // copy in the rerwitten 
                                        // contents of the element
                if ( element.ElementType != ELEMENT_TYPE.TOCDIV )
                    {
                    sb.AppendFormat ( "\n<{0}>{1}</{0}>\n",
                                      element.TagName,
                                      element.Content );
                    }
                html_start = element.ElementEndsAt + 1;
                }
            html_to_copy = html.Length - html_start;
            sb.Append ( html, html_start, html_to_copy );

            return ( sb.ToString ( ) );
            }

        } // class HTMLTOCGenerator

    } // namespace HTMLTOCGenerator
