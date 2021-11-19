using System;
using System.Collections.Generic;

using ATTRIBUTE = HTMLTOCGenerator.Attribute;

namespace HTMLTOCGenerator
    {

    // ********************************************** class Attributes

    public class Attributes
        {

        Dictionary < string, ATTRIBUTE > attributes;

        // ************************************************ Attributes

        /// <summary>
        /// creates a new attributes dictionary
        /// </summary>
        public Attributes ( )
            {

            attributes = new Dictionary < string, ATTRIBUTE > ( );
            }

        // ******************************************************* Add

        /// <summary>
        /// given an attribute, adds it to the attributes dictionary
        /// </summary>
        /// <param name="attribute">
        /// attribute to be added to attributes
        /// </param>
        public void Add ( ATTRIBUTE attribute )
            {

            attributes.Add ( attribute.Name.Trim ( ).ToUpper ( ), 
                             attribute );
            }

        // ***************************************************** Clear

        /// <summary>
        /// clears the attributes dictionary
        /// </summary>
        public void Clear ( )
            {

            attributes.Clear ( );
            }

        // *************************************************** IsEmpty

        /// <summary>
        /// returns whether or not the the attributes dictionary 
        /// contains attribute instances
        /// </summary>
        /// <returns>
        /// true, if there the attributes dictionary contains no 
        /// attribute instances; otherwise, false
        /// </returns>
        public bool IsEmpty ( )
            {

            return ( attributes.Count <= 0 );
            }


        // ***************************************************** Count

        /// <summary>
        /// returns the number of attribute instances in the 
        /// attributes dictionary
        /// </summary>
        /// <returns>
        /// the number of attribute instances in the attributes 
        /// dictionary
        /// </returns>
        public int Count
            {

            get
                {
                return ( attributes.Count );
                }
            }

        // ***************************************** attribute_by_name

        /// <summary>
        /// Access an individual ATTRIBUTE by name
        /// </summary>
        /// <param name="name">
        /// name of the attribute to retrieve
        /// </param>
        /// <returns>
        /// if the attribute is found, the ATTRIBUTE for the given 
        /// attribute; otherwise, null
        /// </returns>
        public ATTRIBUTE attribute_by_name ( string name )
            {
            ATTRIBUTE attribute = null;

            name = name.ToUpper ( ).Trim ( );

            if ( !attributes.TryGetValue (     name, 
                                           out attribute ) )
                {
                attribute = null;
                }

            return ( attribute );
            }

        // *********************************** attribute_value_by_name

        /// <summary>
        /// Access an individual attribute value by name
        /// </summary>
        /// <param name="name">
        /// name of the attribute whose value is to be retrieved
        /// </param>
        /// <returns>
        /// if the attribute is found, the value of the specified 
        /// attribute; otherwise, the empty string
        /// </returns>
        public string attribute_value_by_name ( string name )
            {
            ATTRIBUTE attribute = null;

            if ( String.IsNullOrEmpty ( name ) )
                {
                throw new ArgumentNullException ( 
                    "name", 
                    "Expected an attribute name" );
                }

            name = name.ToUpper ( ).Trim ( );

            if ( attributes.TryGetValue (     name, 
                                          out attribute ) )
                {
                return ( attribute.Value );
                }
            else
                {
                return ( String.Empty );
                }
            }

        } // class Attributes

    } // namespace HTMLTOCGenerator
