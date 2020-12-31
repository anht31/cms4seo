/**
 * @license Copyright (c) 2020
 * cms4seo plugin for insert google map responsive
 * Note: Required .map-responsive class css in Theme css
 */

CKEDITOR.plugins.add('mapresponsive', {
    icons: 'mapresponsive',
    init: function (editor) {

        //Plugin logic goes here.
        editor.addCommand('mapresponsive', new CKEDITOR.dialogCommand('mapresponsiveDialog'));

        //editor.addCommand('InsertMapResponsive', {
        //    exec: function (editor) {
                
        //        editor.insertHtml('<div class="map-responsive">hi</div>');
        //    }
        //});

        editor.ui.addButton('Mapresponsive', {
            label: 'Insert Map Responsive',
            command: 'mapresponsive',
            toolbar: 'insert,90'
        });

        CKEDITOR.dialog.add('mapresponsiveDialog', this.path + 'dialogs/mapresponsive.js');

        editor.addContentsCss( this.path + 'styles/mapresponsive.css' );
    }
});