using System;
using System.Collections.Generic;

using ELEMENT = HTMLTOCGenerator.Element;

namespace HTMLTOCGenerator
    {

    // this class contains the HTML source, the index into the source,
    // the List of elements processed and the List of desired headers
    
    // **************************************************** class Data

    public class Data
        {

        static List < string >  desired_headers = 
                                    new List < string > ( );
        static List < ELEMENT > elements = new List < ELEMENT > ( );
        static int              index = 0;
        static string           source = String.Empty;

        // ******************************************** DesiredHeaders

        public static List < string > DesiredHeaders
            {

            get
                {
                return ( desired_headers );
                }

            set
                {
                desired_headers = value;
                }
            }


        // ************************************************** Elements

        public static List < ELEMENT >  Elements
            {

            get
                {
                return ( elements );
                }

            set
                {
                elements = value;
                }
            }

        // ***************************************************** Index

        public static int Index
            {

            get
                {
                return ( index );
                }

            set
                {
                index = value;
                }
            }

        // **************************************************** Source

        public static string Source
            {

            get
                {
                return ( source );
                }

            set
                {
                source = value;
                }
            }

        } // class Data

    } // namespace Data
