/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.extraPlugins = "lineutils,widget,codesnippet,prism,codemirror,youtube,btgrid,mapresponsive";
    config.extraAllowedContent = 'video[width,height,controls,autoplay,loop,muted,playsinline,id,class];source[src,type];iframe[allowfullscreen];i[class];span[class]';
    config.allowedContent = true;
    config.contentsCss = '/Themes/bocauorder/Assets/fontawesome-5/css/all.min.css';

    // config for youtube plugin
    // Make responsive (ignore width and height, fit to width):
    config.youtube_responsive = true;
    //Show related videos:
    config.youtube_related = false;

};
