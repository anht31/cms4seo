using System.Collections.Generic;

namespace HTMLTOCGenerator
    {

    // *********************************************** class Constants

    public class Constants
        {

        // ************************************ TOC defining constants
        /// <summary>
        /// 
        /// TOC defining constants specify what should be supplied in 
        /// the contents of HTML document TOC-div.
        /// 
        /// Assume the following TOC-div declaration:
        /// 
        /// <div class="toc" 
        ///      style="toc-headers:h2,h3;
        ///             toc-return:true;
        ///             toc-title:Table of Contents;
        ///             toc-image;
        ///             toc-image-width:16;
        ///             toc-image-height:16;
        ///             toc-header-level:h2;
        ///             toc-numbering;" >
        /// </div>
        /// 
        /// Item                Defined by
        /// 
        /// "class"             TOC_CLASS_ATTRIBUTE_NAME
        /// "toc"               TOC_CLASS_ATTRIBUTE_VALUE
        /// "style"             TOC_STYLE_ATTRIBUTE_NAME
        /// 
        /// The style property names and values should be supplied in 
        /// lowercase. However, because comparisons should be made 
        /// between uppercase values, the supplied property names are 
        /// converted to uppercase and then compared with the 
        /// following:
        /// 
        /// Item                Defined by
        /// 
        /// "toc-headers"       TOC_DIV_TOC_HEADERS
        /// "toc-return"        TOC_DIV_TOC_RETURN
        /// "toc-title          TOC_DIV_TOC_TITLE
        /// "toc-image"         TOC_DIV_TOC_IMAGE
        /// "toc-image-width"   TOC_DIV_TOC_IMAGE_WIDTH
        /// "toc-image-height"  TOC_DIV_TOC_IMAGE_HEIGHT
        /// "toc-header-level"  TOC_DIV_TOC_HEADER_LEVEL
        /// "toc-numbering"     TOC_DIV_TOC_NUMBERING
        /// 
        /// </summary>

        public const string     TOC_CLASS_ATTRIBUTE_NAME = "class";
        public const string     TOC_CLASS_ATTRIBUTE_VALUE = "toc";
        public const string     TOC_STYLE_ATTRIBUTE_NAME = "style";


        public const string     TOC_DIV_TOC_HEADERS = "TOC-HEADERS";
        public const string     TOC_DIV_TOC_RETURN = "TOC-RETURN";
        public const string     TOC_DIV_TOC_HEADER_LEVEL = 
                                    "TOC-HEADER-LEVEL";
        public const string     TOC_DIV_TOC_TITLE = "TOC-TITLE";
        
        public const string     TOC_DIV_TOC_IMAGE = 
                                    "TOC-RETURN-IMAGE";
        public const string     TOC_DIV_TOC_IMAGE_WIDTH = 
                                    "TOC-IMAGE-WIDTH";
        public const string     TOC_DIV_TOC_IMAGE_HEIGHT = 
                                    "TOC-IMAGE-HEIGHT";
                                    
        public const string     TOC_DIV_TOC_NUMBERING = 
                                    "TOC-NUMBERING";

        public const string     TOC_GENERATED_CLASS = "toc-generated";

        // *********************************************** enumertions

        public enum HTMLTOC_Status
            {
            INITIALIZING,
            NOSUCHFILE,
            INSUFFICIENPRIVLEGES,
            NOTOCDIV,
            NOHEADERS,
            NODESIREDHEADERS,
            SUCCESS
            }

        public enum Element_Type
            {
            NOTSPECIFIED,
            DIV,
            HEADING2,
            HEADING3,
            HEADING4,
            HEADING5,
            HEADING6,
            TOCDIV,
            UNRECOGNIZED,
            }

        // ************************************************* constants

        public static List < string >   ALLOWED_HEADERS = 
                                            new List < string > ( ) 
                                                {
                                                "H2", 
                                                "H3", 
                                                "H4", 
                                                "H5", 
                                                "H6" 
                                                };

        public static Dictionary < string, string > 
                                        TAG_END_TAG =
                                            new Dictionary < string, 
                                                             string > 
                                                {
                                                { "DIV", "/DIV" },
                                                { "H2",  "/H2" },
                                                { "H3",  "/H3" },
                                                { "H4",  "/H4" },
                                                { "H5",  "/H5" },
                                                { "H6",  "/H6" } 
                                                };

        public const int        INDENTATION = 2;
        public static string    INDENTATION_STRING = 
                                    new string ( ' ', INDENTATION );
        public const string     LOWERCASE_ALPHABETIC = 
                                    "abcdefghijklmnopqrstuvwxyz";
        public const string     NUMERIC = "0123456789";
        public const string     SPECIAL = "~!#$%^&*()_+`-=[]|;:>?,.";
        public const string     UPPERCASE_ALPHABETIC = 
                                    "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string     WHITESPACE = " \t\r\n\f";

        public static int       MINIMUM_LEVEL = 2;  // h1 and 0
        public static int       MAXIMUM_LEVELS = 
                                    ALLOWED_HEADERS.Count + 
                                    MINIMUM_LEVEL;

        public const string     CR = "\r";
        public const string     NL = "\n";
        public const char       NUL = ( char ) 0x00;

        public const string SIGN_ALPHABETIC =
            "àáãạảăắằẳẵặâấầẩẫậèéẹẻẽêềếểễệđìíĩỉịòóõọỏôốồổỗộơớờởỡợùúũụủưứừửữựỳỵỷỹýÀÁÃẠẢĂẮẰẲẴẶÂẤẦẨẪẬÈÉẸẺẼÊỀẾỂỄỆĐÌÍĨỈỊÒÓÕỌỎÔỐỒỔỖỘƠỚỜỞỠỢÙÚŨỤỦƯỨỪỬỮỰỲỴỶỸÝ";

    } // class Constants

    } // namespace HTMLTOCGenerator
