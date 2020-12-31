
/**
 * @license Copyright (c) 2020
 * cms4seo plugin for insert google map responsive
 * Note: Required .map-responsive class css in Theme css
 */

CKEDITOR.dialog.add('mapresponsiveDialog', function (editor) {
    return {
        title: 'Map Responsive Properties',
        minWidth: 400,
        minHeight: 200,

        contents: [
        {
            id: 'info',
            label: 'Basic Settings',
            elements: [
                {
                    type: 'textarea',
                    id: 'iframe',
                    label: 'Html Iframe',
                    validate: CKEDITOR.dialog.validate.notEmpty("Html Iframe field cannot be empty."),
                    //setup: function( element ) {
                    //    this.setValue( element.getText() );
                    //},
                    //commit: function( element ) {
                    //    element.setText( this.getValue() );
                    //}
                },
                //{
                //    type: 'text',
                //    id: 'abbr',
                //    label: 'Abbreviation',
                //    validate: CKEDITOR.dialog.validate.notEmpty("Abbreviation field cannot be empty."),
                //    //setup: function( element ) {
                //    //    this.setValue( element.getText() );
                //    //},
                //    //commit: function( element ) {
                //    //    element.setText( this.getValue() );
                //    //}
                //},
                //{
                //    type: 'text',
                //    id: 'title',
                //    label: 'Explanation',
                //    //validate: CKEDITOR.dialog.validate.notEmpty("Explanation field cannot be empty."),
                //    //setup: function( element ) {
                //    //    this.setValue( element.getAttribute( "title" ) );
                //    //},
                //    //commit: function( element ) {
                //    //    element.setAttribute( "title", this.getValue() );
                //    //}
                //}
            ]
        }
        ],

        onShow: function() {
            // The code that will be executed when a dialog window is loaded.
            //var selection = editor.getSelection();

            //var element = selection.getStartElement();

            //if ( element )
            //    element = element.getAscendant( 'abbr', true );

            //if ( !element || element.getName() != 'abbr' ) {
            //    element = editor.document.createElement( 'abbr' );
            //    this.insertMode = true;
            //}
            //else
            //    this.insertMode = false;

            //this.element = element;

            //if ( !this.insertMode )
            //    this.setupContent( element );

        },

        onOk: function() {

            //// v1 - error when insert in with another tag
            //var dialog = this;
            //var iframeMap = dialog.getValueOf( 'info', 'iframe' );
            //editor.insertHtml( '<div class="map-responsive">' + iframeMap + '</div>' );


            // v2
            var dialog = this;

            var div = editor.document.createElement('div');

            div.setAttribute('class', 'map-responsive');

            var iframe = dialog.getValueOf( 'info', 'iframe' )
            div.appendHtml(iframe);
            editor.insertElement(div);

        }
    };
});
