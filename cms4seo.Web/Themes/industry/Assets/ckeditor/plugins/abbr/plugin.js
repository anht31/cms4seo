/**
 * Demo abbr part 1 & 2
 * https://ckeditor.com/docs/ckeditor4/latest/guide/plugin_sdk_sample_2.html
 */


CKEDITOR.plugins.add('abbr', {
    icons: 'abbr',
    init: function (editor) {

        // Plugin logic goes here...
        editor.addCommand('abbr', new CKEDITOR.dialogCommand('abbrDialog'));

        editor.ui.addButton('Abbr', {
            label: 'Insert Abbreviation',
            command: 'abbr',
            toolbar: 'insert,100'
        });

        if (editor.contextMenu) {
            editor.addMenuGroup('abbrGroup');
            editor.addMenuItem('abbrItem', {
                label: 'Edit Abbreviation',
                icon: this.path + 'icons/abbr.png',
                command: 'abbr',
                group: 'abbrGroup'
            });

            editor.contextMenu.addListener(function (element) {
                if (element.getAscendant('abbr', true)) {
                    return { abbrItem: CKEDITOR.TRISTATE_OFF };
                }
            });

        }

        CKEDITOR.dialog.add('abbrDialog', this.path + 'dialogs/abbr.js');

    }
});