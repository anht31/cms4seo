using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CONST = HTMLTOCGenerator.Constants;

namespace HTMLTOCGenerator
    {
    
    // ******************************************** class TOCNumbering
    
    public class TOCNumbering
        {
        
        static int [ ]  toc_numbering_levels = new int [ 
                                            CONST.MAXIMUM_LEVELS ];
        static bool     toc_numbering_levels_initialized = false;
        static string   toc_numbering_property_value = 
                            "h21,h31,h41,h51,h61";
        static bool     wants_toc_numbering = false;

        // ********************************* TocNumberingPropertyValue
        
        public static string TocNumberingPropertyValue
            {

            get
                {
                return ( toc_numbering_property_value );
                }

            set
                {
                toc_numbering_property_value = value;
                }
            }

        // ***************************************** WantsTocNumbering

        public static bool WantsTocNumbering
            {

            get
                {
                return ( wants_toc_numbering );
                }

            set
                {
                wants_toc_numbering = value;
                }
            }

        // The following methods interact with toc_numbering_levels 
        // array so that its values are accessible outside of this 
        // namespace
        
        // *************************** initialize_toc_numbering_levels
        
        public static void initialize_toc_numbering_levels ( )
            {
                                        // guard to insure that the 
                                        // method is invoked only once
            if ( !toc_numbering_levels_initialized )
                {
                                        // set all elements to 1
                for ( int i = 0; ( i < CONST.MAXIMUM_LEVELS ); i++ )
                    {
                    toc_numbering_levels [ i ] = 1;
                    }
                                        // zero out 'h0' and h1
                toc_numbering_levels [ 0 ] = 0;
                toc_numbering_levels [ 1 ] = 0;
                
                toc_numbering_levels_initialized = true;
                }
            }

        // *********************************** set_toc_numbering_level
        
        public static void set_toc_numbering_level ( int  index,
                                                     int  new_value )
            {
            
            if ( ( index < CONST.MINIMUM_LEVEL ) ||
                 ( index > CONST.MAXIMUM_LEVELS ) )
                {
                throw new ArgumentOutOfRangeException ( 
                    "index",
                    String.Format ( 
                        "Value for index must be in the range " +
                        "{0} to {1}, inclusive",
                        CONST.MINIMUM_LEVEL,
                        CONST.MAXIMUM_LEVELS ) );
                }
                
            if ( new_value < 0 ) 
                {
                throw new ArgumentOutOfRangeException ( 
                    "new_value",
                    "Value for new_value must be greater " +
                        "than zero" );
                }

            if ( !toc_numbering_levels_initialized )
                {
                initialize_toc_numbering_levels ( );
                }
                
            toc_numbering_levels [ index ] = new_value;
            }

        // *********************************** get_toc_numbering_level
        
        public static int get_toc_numbering_level ( int  index )
            {
            
            if ( ( index < CONST.MINIMUM_LEVEL ) ||
                 ( index > CONST.MAXIMUM_LEVELS ) )
                {
                throw new ArgumentOutOfRangeException ( 
                    "index",
                    String.Format ( 
                        "Value for index must be in the range " +
                        "{0} to {1}, inclusive",
                        CONST.MINIMUM_LEVEL,
                        CONST.MAXIMUM_LEVELS ) );
                }

            if ( !toc_numbering_levels_initialized )
                {
                initialize_toc_numbering_levels ( );
                }
                
            return ( toc_numbering_levels [ index ] );
            }

        // ***************************** increment_toc_numbering_level
        
        public static void increment_toc_numbering_level ( int index )
            {
            
            if ( ( index < CONST.MINIMUM_LEVEL ) ||
                 ( index > CONST.MAXIMUM_LEVELS ) )
                {
                throw new ArgumentOutOfRangeException ( 
                    "index",
                    String.Format ( 
                        "Value for index must be in the range " +
                        "{0} to {1}, inclusive",
                        CONST.MINIMUM_LEVEL,
                        CONST.MAXIMUM_LEVELS ) );
                }

            if ( !toc_numbering_levels_initialized )
                {
                initialize_toc_numbering_levels ( );
                }
                
            toc_numbering_levels [ index ]++;
            }

        // ***************************** decrement_toc_numbering_level
        
        public static void decrement_toc_numbering_level ( int index )
            {
            
            if ( ( index < CONST.MINIMUM_LEVEL ) ||
                 ( index > CONST.MAXIMUM_LEVELS ) )
                {
                throw new ArgumentOutOfRangeException ( 
                    "index",
                    String.Format ( 
                        "Value for index must be in the range " +
                        "{0} to {1}, inclusive",
                        CONST.MINIMUM_LEVEL,
                        CONST.MAXIMUM_LEVELS ) );
                }

            if ( !toc_numbering_levels_initialized )
                {
                initialize_toc_numbering_levels ( );
                }
                
            toc_numbering_levels [ index ]--;
            }

        } // class TOCNumbering
        
    } // namespace HTMLTOCGenerator
