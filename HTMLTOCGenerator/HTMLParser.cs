using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using ATTRIBUTE= HTMLTOCGenerator.Attribute;
using ATTRIBUTES = HTMLTOCGenerator.Attributes;
using CONSTANTS = HTMLTOCGenerator.Constants;
using DATA = HTMLTOCGenerator.Data;
using ELEMENT = HTMLTOCGenerator.Element;
using ELEMENT_TYPE = HTMLTOCGenerator.Constants.Element_Type;
using TOC_DIV = HTMLTOCGenerator.TOCDIV;
using TOC_NUMBERING = HTMLTOCGenerator.TOCNumbering;

namespace HTMLTOCGenerator
    {

    // ********************************************** class HTMLParser

    /// <summary>
    /// original written by Jeff Heaton (http://www.jeffheaton.com)
    /// 
    /// see http://www.developer.com/net/csharp/article.php/
    ///     10918_2230091_2/Parsing-HTML-in-Microsoft-C.htm
    ///     
    /// revised by gggustafson for the HTML-TOC Generator
    /// 
    /// </summary>
    /// <remarks>
    /// There are three public methods:
    /// 
    ///     void collect_all_desired_elements ( string  html )
    ///     
    ///         parses the supplied HTML, locates and records all 
    ///         headers (h2 though h6) and div's with the class 
    ///         "toc"; returns its results in a List < Element >; 
    ///         desired elements are defined by TAG_END_TAG in the 
    ///         Constants.cs
    ///     
    ///     void revise_element_content ( )
    ///     
    ///         using the List < Element > produced by 
    ///         collect_all_desired_elements, removes existing 
    ///         HTML-TOC generated content; such content is signalled 
    ///         by the class "toc-generated"; returns its results in a 
    ///         revised List < Element >
    ///         
    ///     void eliminate_unwanted_elements ( )
    ///     
    ///         during previous collection and revision, all headers 
    ///         were collected into the List < Element >; at this 
    ///         point in processing, the TOC-div properties have been 
    ///         obtained so those headers not of interest can be 
    ///         removed from the List < Element >; ; returns its 
    ///         results in a revised List < Element >
    ///         
    /// </remarks>
    public class HTMLParser
        {

        // ************************************************** position
        
        /// <summary>
        /// returns the position where current_ch is located within 
        /// the HTML
        /// </summary>
        int position 
            {
            
            get
                {
                return ( DATA.Index );
                }
                
            set
                {
                DATA.Index = value;
                }
            }
                
        // ********************************************* is_whitespace

        /// <summary>
        /// Determine if the specified character is whitespace or not
        /// </summary>
        /// <param name="ch">
        /// character for which to determine if whitespace
        /// </param>
        /// <returns>
        /// true if the character is whitespace; false, otherwise
        /// </returns>
        /// <remarks>
        /// white space is defined by WHITESPACE as " \t\r\n\f" in 
        /// Constants.cs
        /// </remarks>
        bool is_whitespace ( char ch )
            {

            return ( CONSTANTS.WHITESPACE.IndexOf ( ch ) != -1 );
            }


        // ******************************************** eat_whitespace

        /// <summary>
        /// moves the current character over white space, stopping 
        /// when a non-whitespace character is encountered
        /// </summary>
        void eat_whitespace ( )
            {
            char  ch;
            
            while ( !eof ( ) )
                {
                ch = current_ch ( );
                if ( !is_whitespace ( ch ) )
                    {
                    break;
                    }
                advance ( );
                }
            }

        // ******************************************************* eof

        /// <summary>
        /// returns whether or not the current character position is 
        /// at or past the end of input
        /// </summary>
        bool eof ( )
            {

            return ( position >= DATA.Source.Length );
            }

        // ************************************************ current_ch

        /// <summary>
        /// retrieve the current character from source
        /// </summary>
        char current_ch ( )
            {

            return ( current_ch ( 0 ) );
            }


        // ************************************************ current_ch

        /// <summary>
        /// retrieve character a specified number of characters ahead 
        /// of the current character
        /// </summary>
        /// <param name="ahead">
        /// number of characters ahead of the current character
        /// </param>
        /// <returns>
        /// if not past end of source, the character retrieved; 
        /// otherwise, the NUL character
        /// </returns>
        char current_ch ( int ahead )
            {

            if ( ( position + ahead ) < DATA.Source.Length )
                {
                return ( DATA.Source [ position + ahead ] );
                }
            else
                {
                return ( CONSTANTS.NUL );
                }
            }

        // ********************************** current_char_and_advance

        /// <summary>
        /// obtain the current character and advance the position 
        /// by one
        /// </summary>
        /// <returns>
        /// current character
        /// </returns>
        char current_char_and_advance ( )
            {
            char  ch = current_ch ( );

            advance ( );

            return ( ch );
            }

        // *************************************************** advance

        /// <summary>
        /// Move the index forward by a specified amount
        /// </summary>
        /// <param name="amount">
        /// amount by which to move index ahead
        /// </param>
        void advance ( int  amount )
            {

            for ( int i = 0; ( i <= amount ); i++ )
                {
                if ( eof ( ) )
                    {
                    break;
                    }
                else
                    {
                    position++;
                    }
                }
            }

        // *************************************************** advance

        /// <summary>
        /// move the position forward by one
        /// </summary>
        void advance ( )
            {

            advance ( 0 );
            }

        // ********************************* skip_to_start_of_next_tag

        /// <summary>
        /// moves the position to the beginning of the next tag; stops 
        /// at the < character
        /// </summary>
        void skip_to_start_of_next_tag ( )
            {

            while ( !eof ( ) )
                {
                if ( current_ch ( ).Equals ( '<' ) )
                    {
                    break;
                    }
                else
                    {
                    advance ( );
                    }
                }
            }

        // ***************************************** skip_past_tag_end

        /// <summary>
        /// moves the position to just after the closing > of the tag; 
        /// stops just past the > character
        /// </summary>
        void skip_past_tag_end ( )
            {

            while ( !eof ( ) )
                {
                if ( current_ch ( ).Equals ( '>' ) )
                    {
                    break;
                    }

                advance ( );
                }

            if ( !eof ( ) )
                {
                advance ( );
                }
            }

        // ***************************************** retrieve_tag_name

        /// <summary>
        /// initially positioned at the opening < of a tag, scans the 
        /// input until either whitespace or the tag's > is 
        /// encountered
        /// </summary>
        /// <returns>
        /// the tag name unless the tag is a comment when the empty 
        /// string is returned
        /// </returns>
        string retrieve_tag_name ( )
            {
            StringBuilder   tag_name = new StringBuilder ( );
                                        // positioned at <
            advance ( );
                                        // is it a comment?
            if ( current_ch ( ).Equals ( '!' ) &&
                 current_ch ( 1 ).Equals ( '-' ) &&
                 current_ch ( 2 ).Equals ( '-' ) )
                {
                skip_past_tag_end ( );  // past >
                tag_name.Length = 0;
                return ( String.Empty );
                }
                                        // extract tag_name name
            while ( !eof ( ) )
                {
                if ( is_whitespace ( current_ch ( ) ) ||
                     current_ch ( ).Equals ( '>' ) )
                    {
                    break;
                    }
                tag_name.Append ( current_ch ( ) );
                advance ( );
                }

            return ( tag_name.ToString ( ) );
            }

        // **************************************** get_attribute_name

        /// <summary>
        /// retrieves the name of an attribute
        /// </summary>
        string get_attribute_name ( )
            {
            StringBuilder   name_sb = new StringBuilder ( );

            eat_whitespace ( );
            while ( !eof ( ) )
                {
                if ( is_whitespace ( current_ch ( ) ) ||
                     current_ch ( ).Equals ( '=' ) ||
                     current_ch ( ).Equals ( '>' ) )
                    {
                    break;
                    }
                name_sb.Append ( current_char_and_advance ( ) );
                }

            eat_whitespace ( );

            return ( name_sb.ToString ( ) );
            }


        // *************************************** get_attribute_value

        /// <summary>
        /// retrieves the value of an attribute
        /// </summary>
        string get_attribute_value ( )
            {
            char            delimiter = CONSTANTS.NUL;
            StringBuilder   value_sb = new StringBuilder ( );

            if ( current_ch ( ).Equals ( '=' ) )
                {
                advance ( );
                eat_whitespace ( );
                if ( current_ch ( ).Equals ( '\'' ) ||
                     current_ch ( ).Equals ( '\"' ) )
                    {
                    delimiter = current_ch ( );
                    advance ( );
                    while ( !current_ch ( ).Equals ( delimiter ) )
                        {
                        value_sb.Append ( current_ch ( ) );
                        advance ( );
                        }
                    advance ( );
                    }
                else
                    {
                    while ( !eof ( ) &&
                            !is_whitespace ( current_ch ( ) ) &&
                            !current_ch ( ).Equals ( '>' ) )
                        {
                        value_sb.Append ( 
                                    current_char_and_advance ( ) );
                        }
                    }

                eat_whitespace ( );
                }

            return ( value_sb.ToString ( ) );
            }

        // *************************************** retrieve_attributes

        /// <summary>
        /// retrieves the attributes found within an element
        /// </summary>
        ATTRIBUTES retrieve_attributes ( )
            {
            ATTRIBUTES  attributes = new ATTRIBUTES ( );

            eat_whitespace ( );

            while ( !eof ( ) )
                {
                ATTRIBUTE   attribute = new ATTRIBUTE ( );
                char        ch;
                
                ch = current_ch ( );
                if ( ch == '>' )
                    {
                    break;
                    }

                attribute.Name = get_attribute_name ( );

                if ( current_ch ( ).Equals ( '>' ) )
                    {
                    if ( !String.IsNullOrEmpty ( attribute.Name ) )
                        {
                        attribute.Value = String.Empty;
                        attributes.Add ( attribute );
                        }
                    break;
                    }
                                        // get the value (if any)
                attribute.Value = get_attribute_value ( );
                if ( !String.IsNullOrEmpty ( attribute.Name ) )
                    {
                    attributes.Add ( attribute );
                    }
                }

            advance ( );

            return ( attributes );
            }

        // ************************************ skip_to_end_of_element

        /// <summary>
        /// moves the position to the element's >
        /// </summary>
        int skip_to_end_of_element ( string  start_tag,
                                     string  end_tag )
            {
            int                 element_ends_at = 0;
            Stack < string >    stack = new Stack < string > ( );
            string              tag = String.Empty;
                                        // positioned at space or >
                                        // following start tag as in
                                        // '<tag>' or '<tag '
                                        //      ^          ^
            stack.Push ( end_tag );
            while ( !eof ( ) )
                {
                skip_to_start_of_next_tag ( );
                tag = retrieve_tag_name ( ).ToUpper ( ).Trim ( );
                if ( tag.Equals ( stack.Peek ( ) ) )
                    {
                    stack.Pop ( );
                    if ( stack.Count == 0 )
                        {
                        element_ends_at = position;
                        break;
                        }
                    }
                else if ( tag.Equals ( start_tag ) )
                    {
                    stack.Push ( end_tag );
                    }
                }

            return ( element_ends_at );
            }

        // ************************************ skip_to_end_of_element

        /// <summary>
        /// moves the position to the element's >
        /// </summary>
        int skip_to_end_of_element ( string  start_tag )
            {
            int                 element_ends_at = 0;
            string              end_tag = "/" + start_tag;
            Stack < string >    stack = new Stack < string > ( );
            string              tag = String.Empty;
                                        // positioned at space or >
                                        // following start tag as in
                                        // '<tag>' or '<tag '
                                        //      ^          ^
            stack.Push ( end_tag );
            while ( !eof ( ) )
                {
                skip_to_start_of_next_tag ( );
                tag = retrieve_tag_name ( ).ToUpper ( ).Trim ( );
                if ( tag.Equals ( stack.Peek ( ) ) )
                    {
                    stack.Pop ( );
                    if ( stack.Count == 0 )
                        {
                        element_ends_at = position;
                        break;
                        }
                    }
                else if ( tag.Equals ( start_tag ) )
                    {
                    stack.Push ( end_tag );
                    }
                }

            return ( element_ends_at );
            }

        // *************************************** default_the_headers

        /// <summary>
        /// resets the TAG_END_TAG and DesiredHeaders to their initial 
        /// values
        /// </summary>
        void default_the_headers ( )
            {

            CONSTANTS.TAG_END_TAG.Clear ( );
            CONSTANTS.TAG_END_TAG.Add ( "DIV", "/DIV" );
            CONSTANTS.TAG_END_TAG.Add ( "H2",  "/H2" );
            CONSTANTS.TAG_END_TAG.Add ( "H3",  "/H3" );
            CONSTANTS.TAG_END_TAG.Add ( "H4",  "/H4" );
            CONSTANTS.TAG_END_TAG.Add ( "H5",  "/H5" );
            CONSTANTS.TAG_END_TAG.Add ( "H6",  "/H6" );

            DATA.DesiredHeaders.Clear ( );
            DATA.DesiredHeaders.Add ( "H2" );
            DATA.DesiredHeaders.Add ( "H3" );
            DATA.DesiredHeaders.Add ( "H4" );
            DATA.DesiredHeaders.Add ( "H5" );
            DATA.DesiredHeaders.Add ( "H6" );
            }

        // **************************************** set_default_values

        /// <summary>
        /// resets application variables to their initial values
        /// </summary>
        void set_default_values ( )
            {

            default_the_headers ( );

            TOC_DIV.TocHeaderLevel = "h2";
            TOC_DIV.TocTitle = "Table of Contents";
            TOC_DIV.TocImage = 
                "/app_themes/codeproject/img/gototop16.png";
            TOC_DIV.TocImageHeight = "16";
            TOC_DIV.TocImageWidth = "16";
            TOC_NUMBERING.TocNumberingPropertyValue = 
                                            "h21,h31,h41,h51,h61";
            TOC_NUMBERING.WantsTocNumbering = false;
            TOC_DIV.WantsTocReturn = true;
            TOC_DIV.WantsTocTitle = true;
            }

        // ******************************************* TOC_div_headers

        /// <specification>
        /// The headers property may be omitted. If omitted, entries 
        /// for all HTML headings tags, appearing in the HTML 
        /// document, will be placed in the TOC. Likewise, if the 
        /// headers property is present, but the heading-tags-list is 
        /// omitted, entries for all HTML headings tags, appearing in 
        /// the HTML document, will be placed in the TOC.
        /// 
        /// The heading-tags-list is composed of one or more of "h2", 
        /// "h3", "h4", "h5", or "h6", in any order, in any case, 
        /// separated by commas. White-space within the 
        /// heading-tags-list is ignored. An empty heading-tags-list 
        /// is treated as if the heading-tags-list was omitted and 
        /// entries for all HTML headings tags, appearing in the HTML 
        /// document, will be placed in the TOC. Unrecognized and 
        /// duplicate values within the heading-tags-list are ignored.
        /// </specification>
        bool TOC_div_headers ( string headers_property_value )
            {
            string [ ]  property_values;

            if ( String.IsNullOrEmpty ( headers_property_value ) )
                {
                default_the_headers ( );
                return ( true );
                }
                                        // desired_headers contains 
                                        // the header levels that will 
                                        // be included in the TOC
            property_values = headers_property_value.Split ( 
                                            new char [ ] { ',' },
                                            StringSplitOptions.
                                                RemoveEmptyEntries );
            if ( property_values.Length == 0 )
                {
                default_the_headers ( );
                return ( true );
                }

            DATA.DesiredHeaders.Clear ( );
            foreach ( string property_value in property_values )
                {
                string  uppercase = property_value.ToUpper ( ).
                                                   Trim ( );

                uppercase = Regex.Replace ( uppercase, @"\s+", "" );
                uppercase = Regex.Replace ( uppercase, "'", "" );
                uppercase = Regex.Replace ( uppercase, "\"", "" );

                if ( CONSTANTS.ALLOWED_HEADERS.Contains ( uppercase ) )
                    {
                    if ( !DATA.DesiredHeaders.Contains ( uppercase ) )
                        {
                        DATA.DesiredHeaders.Add ( uppercase );
                        }
                    }
                }

            if ( DATA.DesiredHeaders.Count > 0 )
                {
                DATA.DesiredHeaders.Sort ( );
                }

            return ( true );
            }

        // **************************************** TOC_div_toc_return

        /// <specification>
        /// if the toc-return attribute is omitted or if the 
        /// toc-return attribute is present but an attribute value is 
        /// not or if the toc-return attribute is present but an 
        /// unrecognized attribute value is supplied, return links to 
        /// the TOC will be placed in the innerHtml of the HTML 
        /// headings tags and a bookmark will be placed in the TOC
        /// </specification>

        void TOC_div_toc_return ( string toc_return_value )
            {
            bool  wants_toc_return = true;

            if ( String.IsNullOrEmpty ( toc_return_value ) )
                {
                wants_toc_return = true;
                }
            else if ( !Boolean.TryParse (     toc_return_value,
                                          out wants_toc_return ) )
                {
                wants_toc_return = true;
                }
            else
                {
                wants_toc_return = true;
                }

            TOC_DIV.WantsTocReturn = wants_toc_return;
            }

        // ********************************** TOC_div_toc_header_level

        /// <specification>
        /// The toc-header-level property specifies the HTML header 
        /// level that will be used to display the TOC title. If the 
        /// toc-header-level property is missing, the HTML header 
        /// level "h2" will be used for the TOC title element. 
        /// 
        /// The value of the property may be any of the HTML header 
        /// levels "h2", "h3", "h4", "h5", or "h6". Any other value 
        /// will be ignored and the toc-header-level property value 
        /// will become "h2". 
        /// </specification>

        void TOC_div_toc_header_level ( string  header_level )
            {
            string  uppercase = header_level.ToUpper ( ).Trim ( );

            if ( String.IsNullOrEmpty ( uppercase ) )
                {
                TOC_DIV.TocHeaderLevel = "h2";
                }
            else if ( !CONSTANTS.ALLOWED_HEADERS.Contains ( uppercase ) )
                {
                TOC_DIV.TocHeaderLevel = "h2";
                }
            else
                {
                TOC_DIV.TocHeaderLevel = uppercase.ToLower ( );
                }
            }

        // ***************************************** TOC_div_toc_title

        /// <specification>
        /// The toc-title attribute specifies a title for the TOC. If 
        /// the toc-title attribute is missing, the title "Table of 
        /// Contents" will be inserted into the TOC. 
        /// 
        /// The value of the attribute may contain any alphanumeric 
        /// character plus any of the following characters:
        /// 
        ///     Tilde (~)
        ///     Exclamation mark (!)
        ///     Number sign (#)
        ///     Dollar sign ($)
        ///     Percent sign (%)
        ///     Circumflex accent (^)
        ///     Ampersand (&)
        ///     Asterisk (*)
        ///     Left parenthesis (()
        ///     Right parenthesis ())
        ///     Underscore (_)
        ///     Plus sign (+)
        ///     Grave accent (`)
        ///     Hyphen (-)
        ///     Equals sign (=)
        ///     Left bracket ([)
        ///     Right bracket (])
        ///     Vertical line (|)
        ///     Semicolon (;)
        ///     Colon (:)
        ///     Greater-than symbol (>)
        ///     Question mark(?)
        ///     Comma (,) 
        ///     Period (.)
        ///     Space ( )
        /// 
        /// Any other character will be removed from the toc-title. 
        /// 
        /// If after processing, an empty string results, no TOC title 
        /// will be generated.
        /// </specification>

        void TOC_div_toc_title ( string  toc_title )
            {
            string          allowed = CONSTANTS.LOWERCASE_ALPHABETIC +
                                      CONSTANTS.UPPERCASE_ALPHABETIC +
                                      CONSTANTS.NUMERIC +
                                      CONSTANTS.SPECIAL + " " +
                                      CONSTANTS.SIGN_ALPHABETIC;

            StringBuilder   rewritten_sb = new StringBuilder ( );
            string          rewritten = String.Empty;

            foreach ( char ch in toc_title )
                {
                if ( allowed.IndexOf ( ch ) != -1 )
                    {
                    rewritten_sb.Append ( ch );
                    }
                }
            rewritten = rewritten_sb.ToString ( );

            if ( String.IsNullOrEmpty ( rewritten ) )
                {
                TOC_DIV.WantsTocTitle = false;
                TOC_DIV.TocTitle = String.Empty;
                }
            else
                {
                TOC_DIV.WantsTocTitle = true;
                TOC_DIV.TocTitle = rewritten;
                }
            }

        // ***************************************** TOC_div_toc_image

        /// <specification>
        /// The toc-return-image property specifies the path to an 
        /// image that will be placed in the return link to the TOC in 
        /// the text of the HTML headings tag. If the toc-return-image 
        /// property is missing, the path 
        /// 
        ///     "/app_themes/codeproject/img/gototop16.png" 
        ///     
        /// will be inserted into the TOC. 
        /// 
        /// The value of the property may contain any valid path 
        /// character. No test is made to insure that a valid path is 
        /// provided.
        /// 
        /// The default path is defined specifically for Code Project 
        /// articles. Documents for which a TOC is generated but will 
        /// not be published at Code Project should have a toc-return-
        /// image specified. The image path must be "visible" to the 
        /// HTML document.
        /// </specification>
        void TOC_div_toc_image ( string  image_path )
            {

            if ( !String.IsNullOrEmpty ( image_path ) )
                {
                TOC_DIV.TocImage = image_path;
                }
            }

        // *********************************** TOC_div_toc_image_width
        
        /// <summary>
        /// 
        /// </summary>
        void TOC_div_toc_image_width ( string  width )
            {
            bool    found_non_digit = false;
            string  suffix = String.Empty;
            string  width_str = String.Empty;
            
            if ( String.IsNullOrEmpty ( width ) )
                {
                return;
                }
            
            for ( int i = 0; ( i < width.Length ); i++ )
                {
                char  ch = width [ i ];
                
                if ( found_non_digit )
                    {
                    suffix += ch;
                    }
                else if ( Char.IsDigit ( ch ) )
                    {
                    width_str += ch;
                    }
                else
                    {
                    found_non_digit = true;
                    suffix += ch;
                    }
                }
                 
            if ( String.IsNullOrEmpty ( width_str ) )
                {
                return;
                }

            TOC_DIV.TocImageWidth = width_str;
            if ( String.IsNullOrEmpty ( suffix ) )
                {

                }
            else
                {
                TOC_DIV.TocImageWidth += suffix;
                }
            }

        // ********************************** TOC_div_toc_image_height
        
        /// <summary>
        /// 
        /// </summary>
        void TOC_div_toc_image_height ( string  height )
            {
            bool    found_non_digit = false;
            string  height_str = String.Empty;
            string  suffix = String.Empty;
            
            if ( String.IsNullOrEmpty ( height ) )
                {
                return;
                }
            
            for ( int i = 0; ( i < height.Length ); i++ )
                {
                char  ch = height [ i ];
                
                if ( found_non_digit )
                    {
                    suffix += ch;
                    }
                else if ( Char.IsDigit ( ch ) )
                    {
                    height_str += ch;
                    }
                else
                    {
                    found_non_digit = true;
                    suffix += ch;
                    }
                }
                 
            if ( String.IsNullOrEmpty ( height_str ) )
                {
                return;
                }

            TOC_DIV.TocImageHeight = height_str;
            if ( String.IsNullOrEmpty ( suffix ) )
                {

                }
            else
                {
                TOC_DIV.TocImageHeight += suffix;
                }
            }

        // ************************************* TOC_div_toc_numbering

        /// <specification>
        /// The toc-numbering property provides for the insertion of 
        /// heading numbering within the HTML document. If the 
        /// toc-numbering property is missing, heading numbering will 
        /// not be inserted into the HTML document.
        /// 
        /// If the toc-numbering property is present but the 
        /// toc-numbering property value is missing, heading numbering 
        /// of all headers will be inserted into the HTML document 
        /// using a level-list of "h21,h31,h41,h51,h61". This 
        /// level-list will produce the following heading numbering:
        /// 
        /// 1. H2 heading
        /// 1.1. H3 heading
        /// 1.1.1. H4 heading
        /// 1.1.1.1. H5 heading
        /// 1.1.1.1.1. H6 heading
        /// 
        /// If a large HTML document is broken into separate HTML 
        /// documents, by using a level-list that differs from one 
        /// HTML document to the next, heading numbering can be made 
        /// continuous across the separate HTML documents.
        /// 
        /// For example, a portion of a large HTML document is:
        /// 
        /// <div class="toc" style="toc-numbering;"></div>
        /// <h2>Heading 1</h2>
        /// :
        /// large amount of HTML
        /// :
        /// <h2>Heading 2</h2>
        /// :
        /// large amount of HTML
        /// :
        /// <h2>Heading 2</h2>
        /// :
        /// large amount of HTML
        /// :
        /// 
        /// Because the HTML generated text between the individual h2 
        /// elements is too large to fit into the desired page size, 
        /// the HTML document will be broken into smaller HTML 
        /// documents at the h2 header levels. However, heading 
        /// numbering is desired to be continuous across all pieces of 
        /// the document. By modifying the level-list property for 
        /// each of the smaller HTML documents, a continuous header 
        /// numbering can be achieved.
        /// 
        /// <div class="toc" style="toc-numbering:h21;"></div>
        /// <h2>Heading 1</h2>
        /// :
        /// large amount of HTML
        /// :
        /// 
        /// <div class="toc" style="toc-numbering:h22;"></div>
        /// <h2>Heading 2</h2>
        /// :
        /// large amount of HTML
        /// :
        /// 
        /// <div class="toc" style="toc-numbering:h23;"></div>
        /// <h2>Heading 3</h2>
        /// :
        /// large amount of HTML
        /// :
        /// 
        /// Any level (h2 through h6) can have its starting level 
        /// number specified.
        /// </specification>
        /// 
        // we are here because there was a toc-numbering property so 
        // there will be TOC numbering; this method determines just 
        // how
        //
        // TOC-div           ::= <div class="toc" 
        //                            style="toc-numbering[:<level-list>];]"
        //                       </div> .
        // 
        // level-list........::= level-value
        //                   ::= level-value, level-list .
        // 
        // level-value       ::= [heading-tag] digit .
        // 
        // digit             ::= "0"
        //                   ::= "1"
        //                   ::= "2"
        //                   ::= "3"
        //                   ::= "4"
        //                   ::= "5"
        //                   ::= "6"
        //                   ::= "7"
        //                   ::= "8"
        //                   ::= "9" .

        void TOC_div_toc_numbering ( string level_list )
            {
            int             base_index = 1;
            bool            have_value = false;
            string [ ]      level_values;
            StringBuilder   sb = new StringBuilder ( );

            TOC_NUMBERING.WantsTocNumbering = true;
            TOC_NUMBERING.initialize_toc_numbering_levels ( );

            if ( String.IsNullOrEmpty ( level_list ) )
                {
                return;
                }
                                        // level_values contain the 
                                        // header tags and start value 
                                        // for TOC numbering that will 
                                        // be included in the TOC
            level_values = level_list.Split ( new char [ ] { ',' } );
            if ( level_values.Length == 0 )
                {
                return;
                }

            foreach ( string level_value in level_values )
                {
                string  heading_tag = String.Empty;
                int     index = 0;
                int     level = 0;
                string  uppercase = level_value.ToUpper ( ).Trim ( );

                base_index++;
                
                uppercase = Regex.Replace ( uppercase, @"\s+", "" );
                uppercase = Regex.Replace ( uppercase, "'", "" );
                uppercase = Regex.Replace ( uppercase, "\"", "" );

                if ( ( uppercase [ 0 ].Equals ( 'H' ) ) &&
                     ( Char.IsDigit ( uppercase [ 1 ] ) ) ) 
                    {
                    heading_tag = uppercase.Substring ( 0, 2 );

                    if ( !CONSTANTS.ALLOWED_HEADERS.Contains ( 
                                                    heading_tag ) )
                        {
                        TOC_NUMBERING.
                            initialize_toc_numbering_levels ( );
                        break;
                        }
                    index = Convert.ToInt32 ( 
                                heading_tag [ 1 ].ToString ( ) );
                    if ( !Int32.TryParse (     
                                     uppercase.Substring ( 2 ),
                                 out level ) )
                        {
                        TOC_NUMBERING.
                            initialize_toc_numbering_levels ( );
                        break;
                        }
                    TOC_NUMBERING.set_toc_numbering_level ( index, 
                                                            level );
                    have_value = true;
                    }
                else if ( int.TryParse (     uppercase, 
                                         out level ) )
                    {
                    TOC_NUMBERING.set_toc_numbering_level ( index, 
                                                            level );
                    have_value = true;
                    }
                else
                    {
                    TOC_NUMBERING.initialize_toc_numbering_levels ( );
                    break;
                    }
                }

            if ( have_value )
                {
                for ( int i = CONSTANTS.MINIMUM_LEVEL; 
                        ( i < CONSTANTS.MAXIMUM_LEVELS ); 
                          i++ )
                    {
                    sb.AppendFormat ( 
                        "h{0} {1},", 
                        i,
                        TOC_NUMBERING.get_toc_numbering_level ( i ) );
                    }
                sb.Length--;
                
                TOC_NUMBERING.TocNumberingPropertyValue = 
                    sb.ToString ( );
                }
            }

        // ****************************************** get_TOC_div_info

        /// <summary>
        /// retrieves the various property values supplied in the 
        /// TOC-div; supplies defaults for missing properties
        /// 
        /// <div class="toc" 
        ///      [style="[toc-headers[:<heading-tags-list>];]
        ///              [toc-return[:(true|false)];]
        ///              [toc-title[:<title-of-toc>];]
        ///              [toc-image[:<path-to-image>];]
        ///              [toc-image-width[:<width-in-pixels>];]
        ///              [toc-image-height[:<height-in-pixels>];]
        ///              [toc-header-level[:<heading-tag>];]
        ///              [toc-numbering[:<level-list>];]"
        /// </div> 
        /// 
        /// </summary>
        bool get_TOC_div_info ( string  style )
        {
            string allowed = CONSTANTS.LOWERCASE_ALPHABETIC +
                             CONSTANTS.UPPERCASE_ALPHABETIC +
                             CONSTANTS.NUMERIC +
                             CONSTANTS.SPECIAL + " " +
                             "/" +
                             CONSTANTS.SIGN_ALPHABETIC;

            string [ ]      properties;
            string          rewritten = String.Empty;
            StringBuilder   rewritten_sb = new StringBuilder ( );

            set_default_values ( );
            if ( String.IsNullOrEmpty ( style ) )
                {
                return ( true );
                }
                
            foreach ( char ch in style )
                {
                if ( allowed.IndexOf ( ch ) != -1 )
                    {
                    rewritten_sb.Append ( ch );
                    }
                }
            rewritten = rewritten_sb.ToString ( );
            if ( String.IsNullOrEmpty ( rewritten ) )
                {
                return ( true );
                }

            properties = rewritten.Split ( new char [ ] { ';' },
                                           StringSplitOptions.
                                               RemoveEmptyEntries );
            if ( properties.Length == 0 )
                {
                return ( true );
                }

            foreach ( string property in properties )
                {
                string [ ]  name_value;
                string      property_name = String.Empty;;
                string      property_value = String.Empty;;

                name_value = property.Split ( new char [ ] { ':' },
                                              StringSplitOptions.
                                                 RemoveEmptyEntries );
                if ( name_value.Length == 2 )
                    {
                    property_name = name_value [ 0 ] ;
                    property_value = name_value [ 1 ] ;
                    }
                else if ( name_value.Length == 1 )
                    {
                    property_name = name_value [ 0 ] ;
                    property_value = String.Empty;
                    }
                else if ( name_value.Length == 0 )
                    {
                    property_name = String.Empty;
                    property_value = String.Empty;
                    continue;
                    }

                switch ( property_name.ToUpper().Trim() )
                    {
                    case CONSTANTS.TOC_DIV_TOC_HEADERS:
                        TOC_div_headers ( property_value );
                        break;

                    case CONSTANTS.TOC_DIV_TOC_RETURN:
                        TOC_div_toc_return ( property_value );
                        break;

                    case CONSTANTS.TOC_DIV_TOC_TITLE:
                        TOC_div_toc_title ( property_value );
                        break;

                    case CONSTANTS.TOC_DIV_TOC_IMAGE:
                        TOC_div_toc_image ( property_value );
                        break;

                    case CONSTANTS.TOC_DIV_TOC_IMAGE_WIDTH:
                        TOC_div_toc_image_width ( property_value );
                        break;
                        
                    case CONSTANTS.TOC_DIV_TOC_IMAGE_HEIGHT:
                        TOC_div_toc_image_height ( property_value );
                        break;
                        
                    case CONSTANTS.TOC_DIV_TOC_HEADER_LEVEL:
                        TOC_div_toc_header_level ( property_value );
                        break;

                    case CONSTANTS.TOC_DIV_TOC_NUMBERING:
                        TOC_div_toc_numbering ( property_value );
                        break;

                    default:
                                        // ignore unrecognized
                        break;
                    }
                }
            return ( true );
            }

        // ****************************** collect_all_desired_elements

        /// <summary>
        /// all desired elements are headers (h2 though h6) and the 
        /// div with the class "toc"
        /// </summary>
        public void collect_all_desired_elements ( string  html )
            {

            DATA.Source = html;
            position = 0;

            TOC_DIV.HaveTOCDIV = false;

            while ( !eof ( ) )
                {
                ATTRIBUTES  attributes = null;
                string      element_class = String.Empty;
                string      element_style = String.Empty;
                int         end_of_content = position;
                int         end_of_element = position;
                string      end_tag_name = String.Empty;;
                int         start_of_content = position;
                int         start_of_element = position;
                string      tag_name = String.Empty;
                string      uppercase = String.Empty;

                skip_to_start_of_next_tag ( );          // <

                if ( eof ( ) )
                    {
                    break;
                    }

                start_of_element = position;
                tag_name = retrieve_tag_name ( );       // ' ' or >
                tag_name = tag_name.ToLower ( ).Trim ( );
                uppercase = tag_name.ToUpper ( ).Trim ( );
                                            // ignore empty tags
                if ( String.IsNullOrEmpty ( tag_name ) )
                    {
                    continue;
                    }
                                            // ignore tags that are 
                                            // not of interest
                else if ( !CONSTANTS.TAG_END_TAG.ContainsKey ( 
                                                        uppercase ) )
                    {
                    continue;
                    }
                else if ( uppercase.Equals ( "DIV" ) )
                    {
                    attributes = retrieve_attributes ( );
                                            // ignore div tags without 
                                            // attributes
                    if ( attributes.Count == 0 )
                        {
                        continue;
                        }
                    element_class = attributes.
                                    attribute_value_by_name ( 
                                    CONSTANTS.TOC_CLASS_ATTRIBUTE_NAME );
                                            // ignore div tags without 
                                            // class
                    if ( String.IsNullOrEmpty ( element_class ) )
                        {
                        continue;
                        }
                                            // ignore div tags without 
                                            // the class "toc"
                    else if ( !element_class.Equals ( 
                                CONSTANTS.TOC_CLASS_ATTRIBUTE_VALUE ) )
                        {
                        continue;
                        }
                    element_style = attributes.
                                    attribute_value_by_name ( 
                                    CONSTANTS.TOC_STYLE_ATTRIBUTE_NAME );
                    }
                else
                    {
                    skip_past_tag_end ( );              // past >
                    }

                start_of_content = position;
                end_of_element = position;
                end_of_content = position;

                end_tag_name = CONSTANTS.TAG_END_TAG [ uppercase ];
                end_of_element = skip_to_end_of_element ( 
                                                    uppercase,
                                                    end_tag_name );
                end_of_content = end_of_element - 
                                 end_tag_name.Length - 2;

                if ( end_of_element > 0 )
                    {
                    int     length = 0;

                    ELEMENT  element = new ELEMENT ( tag_name,
                                                     start_of_element,
                                                     end_of_element );

                    if ( !String.IsNullOrEmpty ( element_class ) )
                        {
                        element.Class = element_class;
                        }

                    if ( !String.IsNullOrEmpty ( element_style ) )
                        {
                        element.Style = element_style;
                        }

                    if ( start_of_content > 0 )
                        {
                        element.ContentStartsAt = start_of_content;
                        }

                    if ( end_of_content > 0 )
                        {
                        element.ContentEndsAt = end_of_content;
                        }

                    length = element.ContentEndsAt - 
                             element.ContentStartsAt;

                    if ( length > 0 )
                        {
                        length++;
                        element.Content = DATA.Source.Substring (
                                            element.ContentStartsAt,
                                            length );
                        }
                    else
                        {
                        element.Content = String.Empty;
                        }

                    if ( element.Class.Equals ( 
                                CONSTANTS.TOC_CLASS_ATTRIBUTE_VALUE ) )
                        {
                        element.ElementType = CONSTANTS.Element_Type.
                                                    TOCDIV;
                        TOC_DIV.HaveTOCDIV = true;
                        get_TOC_div_info ( element.Style );
                        }

                    DATA.Elements.Add ( element );
                    }
                }
            }

        // ************************************ revise_element_content

        /// <summary>
        /// removes existing HTML-TOC generated content from <h?> and 
        /// <div> elements
        /// </summary>
        public void revise_element_content ( )
            {
            List < string > process = new List < string > ( );

            foreach ( ELEMENT element in DATA.Elements )
                {
                StringBuilder   sb;

                process.Clear ( );

                switch ( element.ElementType )
                    {
                    case ELEMENT_TYPE.HEADING2:
                    case ELEMENT_TYPE.HEADING3:
                    case ELEMENT_TYPE.HEADING4:
                    case ELEMENT_TYPE.HEADING5:
                    case ELEMENT_TYPE.HEADING6:
                        process.Add ( "A" );
                        process.Add ( "SPAN" );
                        break;

                    case ELEMENT_TYPE.TOCDIV:
                        process.Add ( "H2" );
                        process.Add ( "DIV" );
                        break;

                    default:
                        continue;
                    }

                DATA.Source = element.Content;
                sb = new StringBuilder ( DATA.Source );
                position = 0;

                while ( !eof ( ) )
                    {
                    ATTRIBUTES      attributes = null;
                    string          element_class = String.Empty;
                    int             end_of_element = position;
                    int             start_of_element = position;
                    string          tag_name = String.Empty;

                    skip_to_start_of_next_tag ( );      // <

                    if ( eof ( ) )
                        {
                        break;
                        }

                    start_of_element = position;
                    tag_name = retrieve_tag_name ( );   // ' ' or >
                    if ( String.IsNullOrEmpty ( tag_name ) )
                        {
                        break;
                        }

                    tag_name = tag_name.ToUpper ( ).Trim ( );
                    if ( !process.Contains ( tag_name ) )
                        {
                        skip_to_end_of_element ( tag_name );
                        advance ( );
                        continue;
                        }

                    attributes = retrieve_attributes ( );
                    element_class = 
                        attributes.attribute_value_by_name ( 
                            CONSTANTS.TOC_CLASS_ATTRIBUTE_NAME );
                    element_class = element_class.ToUpper ( ).Trim ( );

                    if ( !element_class.Equals ( 
                            CONSTANTS.TOC_GENERATED_CLASS.ToUpper ( ) ) )
                        {
                        skip_to_end_of_element ( tag_name );
                        advance ( );
                        continue;
                        }
                        
                    end_of_element = skip_to_end_of_element ( 
                                                tag_name );

                    sb.Remove ( start_of_element,
                                end_of_element - 
                                    start_of_element + 1 );
                    DATA.Source = sb.ToString ( );
                    
                    position = start_of_element + 1;
                    }

                DATA.Source.Replace ( CONSTANTS.CR, "" );
                DATA.Source.Replace ( CONSTANTS.NL, "" );
                DATA.Source.Replace ( "  ", " " );
                DATA.Source = DATA.Source.Trim ( );

                element.Content = DATA.Source;
                }
            }

        // ******************************* eliminate_unwanted_elements

        /// <summary>
        /// during collection, all headers were collected; at this 
        /// point in processing, the TOC-div properties have been 
        /// identified so those headers not of interest can be removed
        /// </summary>
        public void eliminate_unwanted_elements ( )
            {
                                        // there must be a better way 
                                        // than this
            if ( DATA.DesiredHeaders.Count > 0 )
                {
                List < ELEMENT > new_elements;

                new_elements = new List < ELEMENT > ( );

                foreach ( ELEMENT element in DATA.Elements )
                    {
                    string  uppercase = element.TagName.ToUpper ( );

                    if ( DATA.DesiredHeaders.Contains ( uppercase ) ||
                         ( element.ElementType == ELEMENT_TYPE.TOCDIV ) )
                        {
                        new_elements.Add ( element );
                        }
                    }

                DATA.Elements.Clear ( );
                DATA.Elements.AddRange ( new_elements );
                }
            }

        } // class HTMLParser

    } // namespace HTMLParser

