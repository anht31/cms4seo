using System;

using ELEMENTTYPE = HTMLTOCGenerator.Constants.Element_Type;

namespace HTMLTOCGenerator
    {

    // ************************************************* class Element

    public class Element
        {

        // this class records information about an HTML element; the 
        // elements are headers and divs with a class of "toc"
        
        // <starttag>content<endtag>
        // ^         ^     ^       ^
        // |
        // element_starts_at
        //           |
        //           content_starts_at
        //                 |
        //                 content_end_at
        //                         |
        //                         element_ends_at

        string      bookmark = String.Empty;
        string      content = String.Empty;
        int         content_ends_at = 0;
        int         content_starts_at = 0;
        string      element = String.Empty;
        string      element_class = String.Empty;
        int         element_ends_at = 0;
        int         element_starts_at = 0;
        string      element_style = String.Empty;
        ELEMENTTYPE element_type = ELEMENTTYPE.NOTSPECIFIED;
        string      tag_name = String.Empty;
        string      toc_numeric_prefix = String.Empty;

        // **************************************** initialize_members

        void initialize_members ( )
            {

            bookmark = String.Empty;
            
            content = String.Empty;
            content_ends_at = 0;
            content_starts_at = 0;

            element = String.Empty;
            element_class = String.Empty;
            element_ends_at = 0;
            element_starts_at = 0;
            element_style = String.Empty;
            element_type = ELEMENTTYPE.NOTSPECIFIED;

            tag_name = String.Empty;
            
            toc_numeric_prefix = String.Empty;
            }

        // ************************************************** set_type

        void set_type ( string tag_name )
            {

            switch ( tag_name.ToUpper ( ).Trim ( ) )
                {
                case "H2":
                    element_type = ELEMENTTYPE.HEADING2;
                    break;

                case "H3":
                    element_type = ELEMENTTYPE.HEADING3;
                    break;

                case "H4":
                    element_type = ELEMENTTYPE.HEADING4;
                    break;

                case "H5":
                    element_type = ELEMENTTYPE.HEADING5;
                    break;

                case "H6":
                    element_type = ELEMENTTYPE.HEADING6;
                    break;

                case "DIV":
                    element_type = ELEMENTTYPE.DIV;
                    break;

                default:
                    element_type = ELEMENTTYPE.UNRECOGNIZED;
                    break;
                }
            }

        // *************************************************** Element

        public Element ( string     tag_name,
                         int        element_starts_at,
                         int        element_ends_at,
                         int        contents_starts_at,
                         int        content_ends_at )
            {

            initialize_members ( );

            TagName = tag_name;
            set_type ( TagName );

            ElementEndsAt = element_ends_at;
            ElementStartsAt = element_starts_at;

            ContentStartsAt = content_starts_at;
            ContentEndsAt = content_ends_at;
            }

        // *************************************************** Element

        public Element ( string  tag_name,
                         int     element_starts_at,
                         int     element_ends_at )
            {

            initialize_members ( );

            TagName = tag_name;
            set_type ( TagName );

            ElementEndsAt = element_ends_at;
            ElementStartsAt = element_starts_at;
            }

        // *************************************************** Element

        public Element ( )
            {

            initialize_members ( );
            }

        // ************************************************** Bookmark

        public string Bookmark 
            {

            get 
                {
                return ( bookmark );
                }

            set
                {
                bookmark = value;
                }
            }

        // ***************************************************** Class

        public string Class 
            {

            get 
                {
                return ( element_class );
                }

            set
                {
                element_class = value;
                }
            }

        // *************************************************** Content

        public string Content 
            {

            get 
                {
                return ( content );
                }

            set
                {
                content = value;
                }
            }

        // ********************************************* ContentEndsAt

        public int ContentEndsAt
            {

            get 
                {
                return ( content_ends_at );
                }

            set
                {
                content_ends_at = value;
                }
            }

        // ******************************************* ContentStartsAt

        public int ContentStartsAt
            {

            get 
                {
                return ( content_starts_at );
                }

            set
                {
                content_starts_at = value;
                }
            }

        // ********************************************* ElementEndsAt

        public int ElementEndsAt
            {

            get 
                {
                return ( element_ends_at );
                }

            set
                {
                element_ends_at = value;
                }
            }

        // ******************************************* ElementStartsAt

        public int ElementStartsAt
            {

            get 
                {
                return ( element_starts_at );
                }

            set
                {
                element_starts_at = value;
                }
            }

        // *********************************************** ElementType

        public ELEMENTTYPE ElementType 
            {

            get 
                {
                return ( element_type );
                }

            set
                {
                element_type = value;
                }
            }

        // ***************************************************** Style

        public string Style 
            {

            get 
                {
                return ( element_style );
                }

            set
                {
                element_style = value;
                }
            }

        // *************************************************** TagName

        public string TagName 
            {

            get 
                {
                return ( tag_name );
                }

            set
                {
                tag_name = value;
                }
            }

        // ****************************************** TocNumericPrefix

        public string TocNumericPrefix 
            {

            get 
                {
                return ( toc_numeric_prefix );
                }

            set
                {
                toc_numeric_prefix = value;
                }
            }

        } // class Element

    } // namespace HTMLTOCGenerator
