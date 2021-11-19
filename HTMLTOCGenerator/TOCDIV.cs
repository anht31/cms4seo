using System;

using CONST = HTMLTOCGenerator.Constants;

namespace HTMLTOCGenerator
    {

    // TOCDIV records the contents of the TOC-div either defaulted or 
    // supplied by the user; for TOC numbering, see TOCNumbering.cs
    
    // ************************************************** class TOCDIV

    public class TOCDIV
        {

        static bool     have_TOCDIV = false;
        static string   toc_bookmark = "toc_return_to_toc";
        static string   toc_header_level = "h2";
        static string   toc_image = 
                          "/app_themes/codeproject/img/gototop16.png";
        static string   toc_image_height = "16";
        static string   toc_image_width = "16";
        static string   toc_title = "Table of Contents";
        static bool     wants_toc_return = true;
        static bool     wants_toc_title = true;

        // ************************************************ HaveTOCDIV

        public static bool HaveTOCDIV
            {

            get
                {
                return ( have_TOCDIV );
                }

            set
                {
                have_TOCDIV = value;
                }
            }

        // ******************************************** TocHeaderLevel

        public static string TocHeaderLevel
            {

            get
                {
                return ( toc_header_level );
                }

            set
                {
                toc_header_level = value;
                }
            }

        // *********************************************** TocBookmark

        public static string TocBookmark
            {

            get
                {
                return ( toc_bookmark );
                }
            }

        // ************************************************** TocImage

        public static string TocImage
            {

            get
                {
                return ( toc_image );
                }

            set
                {
                toc_image = value;
                }
            }

        // ******************************************** TocImageHeight
        
        public static string TocImageHeight
            {

            get
                {
                return ( toc_image_height );
                }

            set
                {
                toc_image_height = value;
                }
            }

        // ********************************************* TocImageWidth
        
        public static string TocImageWidth
            {

            get
                {
                return ( toc_image_width );
                }

            set
                {
                toc_image_width = value;
                }
            }

        // ************************************************** TocTitle

        public static string TocTitle
            {

            get
                {
                return ( toc_title );
                }

            set
                {
                toc_title = value;
                }
            }

        // ******************************************** WantsTocReturn

        public static bool WantsTocReturn
            {

            get
                {
                return ( wants_toc_return );
                }

            set
                {
                wants_toc_return = value;
                }
            }

        // ********************************************* WantsTocTitle

        public static bool WantsTocTitle
            {

            get
                {
                return ( wants_toc_title );
                }

            set
                {
                wants_toc_title = value;
                }
            }

        } // class TOCDIV

    } // namespace HTMLTOCGenerator
