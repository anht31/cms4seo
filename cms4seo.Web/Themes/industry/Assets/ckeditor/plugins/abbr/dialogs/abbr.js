/**
 * Demo abbr part 1 & 2
 * https://ckeditor.com/docs/ckeditor4/latest/guide/plugin_sdk_sample_2.html
 */


CKEDITOR.dialog.add('abbrDialog', function (editor) {
    return {
        title: 'Abbreviation Properties',
        minWidth: 400,
        minHeight: 200,

        contents: [
        {
            id: 'tab-basic',
            label: 'Basic Settings',
            elements: [
                {
                    type: 'text',
                    id: 'abbr',
                    label: 'Abbreviation',
                    validate: CKEDITOR.dialog.validate.notEmpty("Abbreviation field cannot be empty."),
                    setup: function( element ) {
                        this.setValue( element.getText() );
                    },
                    commit: function( element ) {
                        element.setText( this.getValue() );
                    }
                },
                {
                    type: 'text',
                    id: 'title',
                    label: 'Explanation',
                    validate: CKEDITOR.dialog.validate.notEmpty("Explanation field cannot be empty."),
                    setup: function( element ) {
                        this.setValue( element.getAttribute( "title" ) );
                    },
                    commit: function( element ) {
                        element.setAttribute( "title", this.getValue() );
                    }
                }
            ]
        },
        {
            id: 'tab-adv',
            label: 'Advanced Settings',
            elements: [
                {
                    type: 'text',
                    id: 'id',
                    label: 'Id',
                    setup: function( element ) {
                        this.setValue( element.getAttribute( "id" ) );
                    },
                    commit: function ( element ) {
                        var id = this.getValue();
                        if ( id )
                            element.setAttribute( 'id', id );
                        else if ( !this.insertMode )
                            element.removeAttribute( 'id' );
                    }

                }
            ]
        }
        ],

        onShow: function() {
            // The code that will be executed when a dialog window is loaded.
            var selection = editor.getSelection();

            var element = selection.getStartElement();

            if ( element )
                element = element.getAscendant( 'abbr', true );

            if ( !element || element.getName() != 'abbr' ) {
                element = editor.document.createElement( 'abbr' );
                this.insertMode = true;
            }
            else
                this.insertMode = false;

            this.element = element;

            if ( !this.insertMode )
                this.setupContent( element );

        },

        onOk: function() {
            var dialog = this,
                abbr = dialog.element;

            dialog.commitContent( abbr );

            if ( dialog.insertMode )
                editor.insertElement( abbr );


            //var dialog = this;
            //var abbr = editor.document.createElement( 'abbr' );

            //abbr.setAttribute( 'title', dialog.getValueOf( 'tab-basic', 'title' ) );
            //abbr.setText( dialog.getValueOf( 'tab-basic', 'abbr' ) );

            //var id = dialog.getValueOf( 'tab-adv', 'id' );
            //if ( id )
            //    abbr.setAttribute( 'id', id );

            //editor.insertElement( abbr );
        }
    };
});
