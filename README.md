
## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Features](#features)
* [cms4seo Types](#cms4seo-types)
* [Setup](#setup)
* [Publish](#publish)
* [Requirements](#requirements)
* [Caution](#caution)
* [Release History](#release-history)
* [Acknowledgments](#Acknowledgments)
* [License](#License)

## General info
This is Powerful Opensource Website For SEO, write by asp.net MVC5 

## Features

* Optimize Google pagespeed test above 95 point
* Owesome Comppress Image with mozjpeg
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


## Setup

To run this project, restore **Nuget Package**, in visual studio, set cms4seo.Web as **StartUp Project**

#### 1. Set Startup Project

Right click on **cms4seo.Web** project and select **Set as Startup Project** in context menu.

#### 2. Make sure Package fully Restore


If Nuget Package not full load, enter command (Package Manager Console)

```
$ Update-Package -reinstall
```

> ##### Note: in localhost, ProjectId will control which Project will be load. You can find ProjectId in Web.config > appSettings 



## Publish

Right click on cms4seo.Web project and select Publish. After later, you see Release files on [SolutionFolder]\Publish


> Note: If you want fixed folder to publish, please edit cms4seo.pubxml 
> in **cms4seo.Web\PublishProfiles\Properties\cms4seo.pubxml**.
> On tag publishUrl, you enter one specify location.




```diff
! Please dont't delete this file (cms4seo.pubxml), or remove Publish profiles, 
! this profiles have some command to make sure Solution build in right way.
```

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
- Project cms4seo.Data must don't have any Migration Point, if you create new add-migration, you must delete it before publish. It cause of not create table on setup mode.

## Future Plan

- I18N support
- Shopping Cart
- Support Plugin

## Release History

### 1.0.0 Change to cms4seo



#### 2.0.0
* Using bootstrap 4
* upload photo by Create Product First
* Support Url database driven
* 301 redirect list
* Hardcode 400, 500 error

#### 1.0.0
* Using bootstrap 3
* photo upload by session variable

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

## License

(C) Power by cms4seo.
License under the [GPLv3 License](LICENSE.MD).