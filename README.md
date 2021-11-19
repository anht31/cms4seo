
## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Features](#features)
* [cms4seo Types](#cms4seo-types)
* [Run Project](#run-project)
* [Publish](#publish)
* [Requirements](#requirements)
* [Caution](#caution)
* [Release History](#release-history)
* [Acknowledgments](#Acknowledgments)
* [License](#License)

## General info
This is Powerful Opensource Website For SEO, write by asp.net MVC5 

## Features

* Setup step guide.
* Optimize Google pagespeed test above 95 point
* Owesome Comppress Image with mozjpeg
* 13 themes build-in
* 1 sample product build-in
* 9 language translation.
* Easy set permission with Group Roles
* Dynamic display for Header Title
* Redirect url list by csv file
* Url database driven
* New Photo store solution
* Aspect ratio Image without (display image same Card Size)
* New Upload image support drag & drop
* Support Photo Browser	
* Async controller with db provider in controller
* Update XML Setting productiviy
* fix hitcounter with signalR
* Support sitemap.xml & sitemap.html create
* Logs text support
* Custom throw hardcode 404, 400, 500 error
* Hitcounter with filter agent
* Display slider Adaptation with mobile
* Lazy loading
* Quick Edit Content
* Support Plugin Architecture


## Technologies
Project is created with:
* Net framework 4.5
* ASP.NET MVC5
* ASP.NET WebAPI
* Boostrap 4
* Generic Program
* mozjpeg compression
* Dynamic bundle compression
* dmuploader.js - Jquery File Uploader
* ckeditor 4 
* Genneric HtmlEditor

## cms4seo Types

* Default [IsComplexType] will not display
* use [ScaffoldColumn(false)] dataannotations to hidden field (etc: Id)
* use [DataType(cms4seo.CustomEditor)] dataannotations for custom design field (etc: Category Selector)
* other field will default for normaly display
* @Html.Partial("_UploadPhoto", cms4seoEntityType.Article) to binding Many-to-Many ArticlePhotoes


## Run Project

Right click on **cms4seo.Web** project and select **Set as Startup Project** in context menu. 
Right click on this Project and choose **Build**, take a time for Project restore Package automatic



> If Nuget Package not full load, enter command (Package Manager Console)

```
$ Update-Package -reinstall
```

> Note: in localhost, **ProjectId** will control which Project will be load. You can find ProjectId in **Web.config > appSettings**



## Publish

Right click on cms4seo.Web project and select Publish. After later, you see Release files on [SolutionFolder]\Publish
. In this Mode whenever you edit idividual files, you mustbe publish all whole solution folder
. If you want to select individual files and export you, you have to set a fixed foder
. This does not apply to the Assets and View in the cms4seo.Admin project, to export these idividual files
, you must publish the entire solution.



If you want fixed folder to publish, please edit cms4seo.pubxml 
 in **cms4seo.Web\PublishProfiles\Properties\cms4seo.pubxml**.
 On tag publishUrl (with comment out specify location), you enter one specify location.

 
 


##### ⚠️ Please dont't delete this file (cms4seo.pubxml), or remove Publish profiles, this profiles have some command to make sure Solution build in right way. ⚠️

<p>&nbsp;</p>

#### For Https, add this lines to Web.config

```
  <system.webServer>

    <!-- Rewriter HTTPS -->
    <rewrite>
      <rules>
        <!-- Redirect www to non-www -->
        <rule name="RedirectWwwToNonWww" stopProcessing="false">
          <match url="(.*)" />
          <conditions logicalGrouping="MatchAll" trackAllCaptures="false">
            <add input="{HTTP_HOST}" pattern="^(www\.)(.*)$" />
          </conditions>
          <action type="Redirect" url="https://{C:2}{REQUEST_URI}" redirectType="Permanent" />
        </rule>

        <!-- Redirect to HTTPS -->
        <rule name="RedirectToHTTPS" stopProcessing="true">
          <match url="(.*)" />
          <conditions>
            <add input="{HTTPS}" pattern="off" ignoreCase="true" />
          </conditions>
          <action type="Redirect" url="https://{SERVER_NAME}/{R:1}" redirectType="Permanent" />
        </rule>
      </rules>
    </rewrite>

  </system.webServer>
```


## Requirements

- MS VisualStudio 2015+
- MS SQL Server 2014

## Caution

- Set cms4seo.Web as Main Project
- For ReSharper User - Path Mapping: [Areas/Admin] to [cms4seo.Admin]
- If you want use Migration, please add Migration in Project cms4seo.Data

## Future Plan

- I18N support

## Release History

#### v1.4.1.3
* Migration support update theme key, value
* **⚠️ Migration database point 3 (v1331 to v1413) ⚠️**
* Update ProductProperties when add new Product Property
* This version resync Git

#### v1.4.1.2
* More Secure for Setup Step

#### v1.4.1.1
* Plugins Managers in Admin Page


#### v1.3.1.1
* Upload on shop with auto resize
* Migration sm, md image
* Remove full client resize mode
* fix upload video fail
* Detect mimeType in client & server
* support PNG resize
* Option auto convert Image to JPEG
* bocauorder themes support lazy video-player
* Display message when submit 301.csv
* **⚠️ Migration database point 2 ⚠️**
* Change Property IsAutoSeoMetaTag instead of IsLockSeoMetaTag
* Reserve Content for RichText of Category, Topic in design
* Support TOC for product, article
* display error message when create new user with insecure password
* fix product tag, article tag update when create new
* More dynamic OptionProperties for Product
* option Product view Counter
* support max-width, max-height for insertEditor
* Plugins Managers - fix assembly not load when soft reset
* Option sort position for photo in product (or article, slider)

#### v1.3.1.0
* FacebookPage Embed
* Client Resize Half Mode
* Client Resize Full Mode
* Improve color image when upload

#### v1.3.0.0
* Flat Site Architecture Dynamic 1 - 2 segment
* (/root-category, /root-category-second-category, /root-category/product)
* Helper for change image to Small, Medium, Large, AllSmall, AllMedium, AllLarge
* Some log for PluginManager
* Email Setting with save mode (private mode)
* Option Page for mix static page with dynamic page in info.cs model
* Auto select theme when avaiable
* override-href color
* Action FilterPrice for Product

#### v1.2.1.2

* ckeditor ACF off for page Info, for static design page
* fix col & col-lg display for ckeditor
* support upload Webm media type
* update ckeditor ACF support Video class & id

#### v1.2.1.1

* plugins architecture version 2
* plugins shopping cart version 2
* Partial _WidgetAction
* all theme bootstrap support Plugin v2

#### v1.2.0.1

* Shopping Card Plugin support
* Widget update
* fix plugin manager & assembly load

#### v1.1.0.1

* fix image source path when copy new theme
* support embed Styles and Scripts on foundation themes
* fix level 3 menu in sidebar link
* fix message when add new product
* friendly treeview for category dropdown list
* support 3 level category

#### 1.1.0.0

* **Plugin Architecture** Support Beta 1
* Display proper size of H1 - H6 title in ckeditor
* Fix fontawesome 5 for slick css next, pre arrow
* Fix Space error in 301.csv when redirect
* Fix label in Photo Upload Partile



#### 1.0.0.9 Upload config ProjectId

* Support tail .localhost in /setup/domain
* Support message in /setup/domain

#### 1.0.0.8 Update contact

* Update contact form post.
* fix dropdown menu lost focus.

#### 1.0.0.7 First publish

## Acknowledgments

* First thank for Quan Bro: anhquan75@gmail.com for guide me first to begin program.
* For solution Url routing by database: https://satvasolutions.com/url-routing-database-driven-url-asp-net-mvc-website/
* Sb-admin-2 boostrap 4 & other components: https://startbootstrap.com/themes/sb-admin-2/
* Thank JosePineiro for MozJpeg-wrapper: https://github.com/JosePineiro/MozJpeg-wrapper
* Thank daniel for dmuploader.js: http://www.daniel.com.uy/projects/jquery-file-uploader/
* Thank arboshiki for lobibox message: https://github.com/arboshiki/lobibox
* ckeditor 4 with codemirror
* Nadeem Afana for i18n StrongType & client side https://afana.me/
* John Atten for his Implementing Group-Based Permissions Management http://johnatten.com/2014/08/10/asp-net-identity-2-0-implementing-group-based-permissions-management
* Shannon Deminick for Developing a plugin framework in ASP.NET MVC with medium trust https://shazwazza.com/post/developing-a-plugin-framework-in-aspnet-with-medium-trust/

## License

Power by cms4seo.
License under the [GPLv3 License](LICENSE).