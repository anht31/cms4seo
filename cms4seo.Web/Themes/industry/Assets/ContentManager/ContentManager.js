/*-----------------------------------------------------------------------------------
/* Content Manager
/* Version: v2.0.0.0
/* Update: after button with absolution position
/* support Alt Tag for Image
/* support Textarea Embed for Iframe
/* support Mock Image like .Image
/* resize and compress Base of ->  info-type, info-width, info-height,
/*      mobile-type (option), mobile-width, mobile-height
/* support jcrop, aspect ratio
/* support scope enclosed
/* modal key encode, pre loader animate
-----------------------------------------------------------------------------------*/

var cMgr = (function ($) {
    
    var dict = {};

    var isQueryDone = false;
    var isUploadDone = true;

    /* ==============================================
    Get MimeType from first 4 group 8 bit binary code
    =============================================== */

    // Add more from http://en.wikipedia.org/wiki/List_of_file_signatures
    function getMimeType(headerString) {
        switch (headerString) {
            case "89504e47":
                type = "image/png";
                break;
            case "47494638":
                type = "image/gif";
                break;
            case "ffd8ffe0":
            case "ffd8ffe1":
            case "ffd8ffe2":
                type = "image/jpeg";
                break;
            default:
                type = "unknown";
                break;
        }
        return type;
    }

    /* ==============================================
    Get Extension
    =============================================== */


    function getExtension(mimeType) {
        var extension;
        switch (mimeType) {
            case "image/png":
                extension = ".png";
                break;
            case "image/gif":
                extension = ".gif";
                break;
            case "image/jpeg":
                extension = ".jpeg";
                break;
            default:
                extension = "unknown";
                break;
        }
        return extension;
    }

    /* ==============================================
    convertDataURIToBinary - Begin
    =============================================== */

    function convertDataURIToBinary(dataUri) {
        const base64Marker = ';base64,';
        var base64Index = dataUri.indexOf(base64Marker) + base64Marker.length;
        var base64 = dataUri.substring(base64Index);
        var raw = window.atob(base64);
        var rawLength = raw.length;
        var array = new Uint8Array(rawLength);
        for (var i = 0; i < rawLength; i++) {
            array[i] = raw.charCodeAt(i);
        }
        return array;
    }

    /* ==============================================
    convertDataURIToBinary - End
    =============================================== */


    jQuery(document).ready(function ($) {

        // Color Changer
        //$(".green" ).click(function(){
        //    $("#colors" ).attr("href", "css/colors/green.css" );
        //    return false;
        //});



        // Layout Switcher
        //$(".boxed" ).click(function(){
        //    $("#layout" ).attr("href", "css/boxed.css" );
        //    return false;
        //});

        //$("#layout-switcher").on('change', function() {
        //    $('#layout').attr('href', $(this).val() + '.css');
        //});;



        // CMS Switcher	
        $('#cms-switcher').animate({
            left: '-240px'
        });

        $('#cms-switcher h2 a').click(function (e) {
            e.preventDefault();
            var div = $('#cms-switcher');
            console.log(div.css('left'));
            if (div.css('left') === '-240px') {
                $('#cms-switcher').animate({
                    left: '0px'
                });
            } else {
                $('#cms-switcher').animate({
                    left: '-240px'
                });
            }
        });



        $("#theme-switcher").on('change', function () {
            var optionSelected = $("option:selected", this);
            var valueSelected = this.value;

            //console.log(valueSelected);

            var xhttpBehaviour = new XMLHttpRequest();

            xhttpBehaviour.onreadystatechange = function () {
                if (this.readyState == 4 && this.status == 200) {
                    console.log(this.responseText);
                    // reload page
                    location.reload();
                }
            };

            if (window.location.port === "" || window.location.port === "80")
                xhttpBehaviour.open("POST", window.location.protocol +
                    "//" + window.location.hostname +
                    "/api/Contents/?theme=" + valueSelected + "&bootswatch=", true);
            else
                xhttpBehaviour.open("POST", window.location.protocol +
                    "//" + window.location.hostname + ":" + window.location.port +
                    "/api/Contents/?theme=" + valueSelected + "&bootswatch=", true);

            xhttpBehaviour.setRequestHeader("Content-type", "application/json");

            // for simple type
            xhttpBehaviour.send();

            if ('.se-pre-con') {
                $('.se-pre-con').addClass('active');
            }


        });;






        if ($('#bootswatch-switcher').length > 0) {
            $("#bootswatch-switcher").on('change', function () {
                var optionSelected = $("option:selected", this);

                var valueSelected = this.value;

                //console.log(valueSelected);

                var xhttpBehaviour = new XMLHttpRequest();

                xhttpBehaviour.onreadystatechange = function () {
                    if (this.readyState == 4 && this.status == 200) {
                        console.log(this.responseText);
                        // reload page
                        location.reload();
                    }
                };

                if (window.location.port === "" || window.location.port === "80")
                    xhttpBehaviour.open("POST", window.location.protocol +
                        "//" + window.location.hostname +
                        "/api/Contents/?theme=&bootswatch=" + valueSelected, true);
                else
                    xhttpBehaviour.open("POST", window.location.protocol +
                        "//" + window.location.hostname + ":" + window.location.port +
                        "/api/Contents/?theme=&bootswatch=" + valueSelected, true);

                xhttpBehaviour.setRequestHeader("Content-type", "application/json");

                // for simple type
                xhttpBehaviour.send();

                if ('.se-pre-con') {
                    $('.se-pre-con').addClass('active');
                }


            });;
        }



        SetStatusToggleSwitch();


        $("#switchEditContents").click(function () {

            if ($(this).is(':checked')) {
                setCookie("ActiveEditContent", "1", 1);
            } else {
                setCookie("ActiveEditContent", "0", 1);
            }

            // reload page
            location.reload();


        });


    });




    /*-----------------------------------------------------------------------------------
    /* CMS Switcher
    -----------------------------------------------------------------------------------*/



    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }

    function getCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }



    function SetStatusToggleSwitch() {

        var activeEditContent = getCookie("ActiveEditContent");

        if (activeEditContent == null) {
            $('#switchEditContents').attr('checked', false);
        }
        else if (activeEditContent == 1) {
            $('#switchEditContents').attr('checked', true);
        } else {
            $('#switchEditContents').attr('checked', false);
        }

        ActiveEditContents();
    }


    function ActiveEditContents() {

        var activeEditContent = getCookie("ActiveEditContent");

        if (activeEditContent == "1" && window.IsAuthorizeEditContent) {

            document.querySelectorAll('[data-cms]').forEach(function (item) {

                var type = "text";

                if (item.tagName == "IMG") {
                    type = "image";
                }
                //else if (item.tagName == "P") {
                //    type = "p";
                //}


                $(item).addClass("activeEditContents");

                var attribute =
                    'cMgr.buildModalAndOpen(\'' +
                        $(item).attr('data-cms') +
                        '\',\'' + type + '\', event' + ')';

                $(item).attr("onclick", attribute);



                if (type === "image") {

                    var span =
                        '<span class="activeEditContentSpan" onclick="cMgr.buildModalAndOpen(\'' +
                            $(item).attr('data-cms') +
                            '\',\'' + type +
                            '\', event' +
                            ')"' + 
                            //' id=' + GetButtonId() +
                            ' target="_top">' +
                            '</span>';

                    $(item).after(span);

                    //document.querySelector('#' + GetInputUploadId(id)).addEventListener('change',function() {
                    //    readURL(id);
                    //});
                }



            });

        }
    }

    function BuildModalAndOpen(id, type, event) {

        // prevent any unwant fire
        event.preventDefault();
        event.stopPropagation();

        var modalId = getModalId(id);

        var modal = document.getElementById(modalId);

        if (modal !== null) {
            modal.style.display = "block";
            return;
        }

        // get value & process is async
        GetContent(id);



        //console.log(id);
        var modalBuilder =
            '<div id="' + modalId + '" class="modal-cms">' +
                '<!-- Modal content -->' +
                '<div class="modal-content-cms">' +
                    '<span class="close">&times;</span>' +
                    '<h4>Edit</h4>' +
                    '<div class="modal-content-box" data-type=' + type + '>' +
                        // input render here
                    '</div>' +
                    '<div class="preloader-submit">' +
                        '<img class="preloader" src="/Assets/loader/ajax-loader.gif" alt="preloader">' +
                    '</div>' +
                    '<div class="progress active" style="display: none">'+
                        '<div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" style="width: 0%;">'+
                            '<span class="sr-only">0% Complete</span>'+
                        '</div>' +
                    '</div>'+
                    '<button type="submit" class="button submitButton">Save</button>' +
                        '<button class="button closeButton">Close</button>' +
                    '</div>' +
            '</div>';

        $("body").append(modalBuilder);

        // reload modal
        modal = document.getElementById(modalId);

        // Get the <span> element that closes the modal
        var spanCloseIcon = modal.getElementsByClassName("close")[0];

        var spanCloseButton = modal.getElementsByClassName("closeButton")[0];

        var submitButton = modal.getElementsByClassName("submitButton")[0];


        // When the user clicks on close Button the modal
        spanCloseButton.onclick = function () {
            modal.style.display = "none";
        }

        spanCloseIcon.onclick = function () {
            modal.style.display = "none";
        }

        // When the user clicks anywhere outside of the modal, close it
        window.onclick = function (event) {
            if (event.target == modal) {
                modal.style.display = "none";
            }
        }

        submitButton.onclick = function () {

            Submit(modal);

        }


        modal.style.display = "block";
    }


    function SubmitOnEnter(id) {

        // Get the modal
        var modal = document.getElementById(getModalId(id));

        Submit(modal);

    }


    function Submit(modal) {

        console.log('submit');
        $(modal.getElementsByClassName('submitButton')).prop('disabled', true);

        $(modal.getElementsByClassName('preloader-submit')).show();

        var modalInputBox = modal.getElementsByClassName("modal-content-box")[0];

        var inputs = $(modalInputBox).children();

        var query = 0;
        var response = 0;

        $.each(inputs, function (i, input) {

            //console.log(i);
            var key, value, editor;

            // can be DIV of ck_editor GUI
            if (input.tagName === "DIV") {
                return;
            }
                // input as child of LABEL
            else if (input.tagName === 'LABEL') {
                key = $(input).children().attr('name');
                value = $(input).children().val();

                if (key && key.endsWith("Textarea")) {
                    editor = CKEDITOR.instances[GetInputId(key)];
                    if (editor)
                        value = editor.getData();
                }

            } else {
                key = input.name;
                value = input.value;

                // update by editor
                if (key) {
                    editor = CKEDITOR.instances[GetInputId(key)];
                    if (editor)
                        value = editor.getData();
                }
            }


            if (key === "")
                return;


            

            // check upload
            if (key.endsWith(".Image")) {

                // crop
                // create new canvas
                // ctx.drawImage

                // get pos
                var pos = dict[key];
                var width = dict['width-' + key];
                var height = dict['height-' + key];

                var element = $('input#' + GetInputUploadId(key))[0];
                var require = getRequire(element);

                if (require && require.width == width && require.height == height) {

                    var inputUploads = document.getElementById(GetInputUploadId(key));
                    if (inputUploads.files && inputUploads.files[0]) {
                        $(modal.getElementsByClassName('preloader-submit')).hide();
                        $(modal.getElementsByClassName('progress')).show();
                        $(modal.getElementsByClassName('progress-bar')).width('5%');
                        UploadImage(inputUploads.files[0], key, modal);
                    }

                } else if (pos) {
                    console.log(pos);

                    //readURLToCrop(id);

                    var img = new Image();
                    img.onload = function() {

                        //query--;

                        //ctx.drawImage(img, 0, 0, 750, 400, 0, 0, 750, 500);

                        var hiddenCanvas = document.createElement('canvas');
                        hiddenCanvas.width = pos.w;
                        hiddenCanvas.height = pos.h;
                        hiddenCanvas.hidden = false;

                        var hiddenContext = hiddenCanvas.getContext("2d");

                        hiddenContext.drawImage(img, pos.x, pos.y, pos.w, pos.h, 0, 0, pos.w, pos.h);

                        //for test
                        //$('.modal-content-cms')[0].prepend(hiddenCanvas)

                        var fileName = dict['fileName-' + key];
                        var mimeType = dict['mimeType-' + key];
                        var extension = getExtension(mimeType);

                        console.log(fileName);

                        var dataUrl = hiddenCanvas.toDataURL(mimeType, 1.0);
                        var imageArray = convertDataURIToBinary(dataUrl);

                        var outputFile = new File([imageArray], fileName);

                        hiddenCanvas = null; // clear canvas

                        $(modal.getElementsByClassName('preloader-submit')).hide();
                        $(modal.getElementsByClassName('progress')).show();
                        $(modal.getElementsByClassName('progress-bar')).width('5%');
                        UploadImage(outputFile, key, modal);


                        console.log(dataUrl.length);

                    };
                    img.src = dict['img-' + key];


                }

            } else {
                query++;
                SetContent(key, value, reloadCallback);
            }

            

        
        });


        console.log(query);


        // callback
        function reloadCallback(id) {

            // input set 2 time for input image path

            // check done each inputs
            if (++response < query)
                return;

            isQueryDone = true;

            // reload
            if(isUploadDone)
                location.reload();
        }

    
    }


    function GetContent(id) {


        var xhttpBehaviour = new XMLHttpRequest();
        xhttpBehaviour.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {

                //console.log(this.responseText);

                var modal = document.getElementById(getModalId(id));

                // find target to fill
                if (modal !== null) {

                    var jsonResults = JSON.parse(this.responseText);

                    if (jsonResults.length == 0)
                        return;

                    // build more input object
                    var modalContentBox = modal.getElementsByClassName("modal-content-box")[0];


                    // clear children
                    modalContentBox.innerHTML = '';


                    // build uploadBox
                    var type = $(modalContentBox).attr("data-type");



                    if (jsonResults.length == 1) {
                        //input.val(jsonResults[0].Value);

                        // support embed with textarea
                        if (jsonResults[0].Key.endsWith("Embed")) {
                            BuildTextareaContent(modalContentBox, jsonResults[0].Key, jsonResults[0].Value);
                        }
                        else if (jsonResults[0].Key.endsWith("Textarea")) {

                            BuildTextareaContent(modalContentBox, jsonResults[0].Key, jsonResults[0].Value);

                            // build ckeditor
                            CKEDITOR.replace(GetInputId(jsonResults[0].Key),
                                {
                                    //height: '500px',
                                    language: 'vi',
                                    allowedContent: true // To get rid of ACF
                                });

                        } else {

                            if (jsonResults[0].Key.endsWith(".Image")) {
                                // upload support
                                BuildUploadBox(modalContentBox, type, jsonResults[0].Key);
                            }

                            BuildInputContent(modalContentBox, jsonResults[0].Key, jsonResults[0].Value);
                        }

                    }
                    else {

                        // Rebuild
                        $.each(jsonResults, function (i, item) {

                            // support embed with textarea
                            if (item.Key.endsWith("Embed")) {
                                BuildTextareaContent(modalContentBox, item.Key, item.Value);
                            }
                            else if (item.Key.endsWith("Textarea")) {

                                BuildTextareaContent(modalContentBox, item.Key, item.Value);

                                // build ck-editor
                                CKEDITOR.replace(GetInputId(item.Key),
                                    {
                                        //height: '500px',
                                        language: 'vi',
                                        allowedContent: true // To get rid of ACF
                                    });
                            } else {
                                if (item.Key.endsWith(".Image")) {
                                    // upload support
                                    BuildUploadBox(modalContentBox, type, item.Key);
                                }
                                BuildInputContent(modalContentBox, item.Key, item.Value);
                            }

                        });

                    }


                    $(".modal-content-box input").on('keyup', function (e) {
                        if (e.keyCode === 13) {
                            // Do something
                            SubmitOnEnter(id);
                        }
                    });
                }

            }
        };


        //xhttpBehaviour.open("GET", "http://localhost:3534/api/Contents?id=" + id, true);

        if (window.location.port === "" || window.location.port === "80")
            xhttpBehaviour.open("GET", window.location.protocol +
                "//" + window.location.hostname + "/api/Contents?id=" + id, true);
        else
            xhttpBehaviour.open("GET", window.location.protocol +
                "//" + window.location.hostname + ":" + window.location.port + "/api/Contents?id=" + id, true);

        xhttpBehaviour.setRequestHeader("Content-type", "application/json");

        xhttpBehaviour.send();

    }


    function SetContent(key, value, callback) {

        var xhttpBehaviour = new XMLHttpRequest();

        xhttpBehaviour.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                console.log(this.responseText);
            
                if(callback)
                    callback(key);
            }
        };

        if (window.location.port === "" || window.location.port === "80")
            xhttpBehaviour.open("PUT", window.location.protocol +
                "//" + window.location.hostname + "/api/Contents?id=" + key, true);
        else
            xhttpBehaviour.open("PUT", window.location.protocol +
                "//" + window.location.hostname + ":" + window.location.port + "/api/Contents?id=" + key, true);

        xhttpBehaviour.setRequestHeader("Content-type", "application/json");

        // for simple type
        xhttpBehaviour.send(JSON.stringify(value));
    }


    function BuildInputContent(boxContent, key, value) {
        var input =
            '<label>' +
                key +
                '<input class="form-control" name="' + key + '" id="' + GetInputId(key) + '" type="text" value="' + value + '">' +
            '</label>';

        $(boxContent).append(input);
    }


    function BuildTextareaContent(boxContent, key, value) {
        var textarea =
            '<label>' +
                    key +
                '<textarea class="form-control" cols="20" id="' + GetInputId(key) + '" name="' + key + '" rows="5" spellcheck="false">'
                    + value +
                '</textarea>' +
            '</label>';

        $(boxContent).append(textarea);
    }

    function BuildUploadBox(boxContent, type, id) {

        //var info = new ImageInfo();

        //if (type === "image") {

        //    return "";
        //}

    

        // safe code for first time reset with id contain dot
        var img;

        if (/[.]/.test(id)) {
            img = $(document).find("[data-cms*=" + CmsEncode(id) + "]")[0];
            console.log(id);
        }
        else {
            // use the *= selector: to find contain attribute value
            img = $(document).find("[data-cms*=" + id + "]")[0];
        }

    

        // detect image for Laptop or Mobile
        var isMobile = id.endsWith(".Mobile.Image");
        var infoItem = "";
        var infoRender = "";
        var previewScale = "";

        if (img) var info = getInfo(img);

        //if (parseInt(info.width) > parseInt(info.height)) {
        //    previewScale = 'width=400';
        //} else {
        //    previewScale = 'height=400';
        //}

        if (isMobile) {

            infoItem = "Mobile, type: " + info.mobileType + 
                ", resolution: " + info.mobileWidth + "x" + info.mobileHeight + " px";

            infoRender = ' info-type=' + info.mobileType + ' info-width=' + 
                info.mobileWidth + ' info-height=' + info.mobileHeight;

            previewScale = 'width=400'; // preview scale [1]


        } else {

            infoItem = "Laptop, type: " + info.type + 
                ", resolution: " + info.width + "x" + info.height + " px";

            infoRender = ' info-type=' + info.type + ' info-width=' + 
                info.width + ' info-height=' + info.height;

            previewScale = 'width=400'; // preview scale [1]

        }

        var uploadBox =
            '<div class="image-required">For ' + infoItem + '</div>' +
                '<input type="file" data-for=' + GetInputId(id) + ' id=' + GetInputUploadId(id) + 
                    ' preview-for=' + GetReviewId(id) + infoRender + ' />' +
            '<div class="box-preview">' +
                
                '<div id="' + GetReloaderImage(id) + '" class="preloader-image">' +
                    '<img class="preloader" src="/Assets/loader/ajax-loader.gif" alt="preloader">' +
                '</div>' +

                '<img id=' + GetReviewId(id) +  ' src="#" info="your image" ' + previewScale + ' class="margin-bottom-2">' +
            '<div>';


        $(boxContent).append(uploadBox);

        document.querySelector('#' + GetInputUploadId(id)).addEventListener('change',function() {
            $('#' + GetReloaderImage(id)).show();
            isUploadDone = false;
            checkName(this);
            readURL(id);
        });
    }

    function checkName(input) {
        if (input.files && input.files[0]) {

            var name = input.files[0].name;

            var target = input.getAttribute('data-for');

            // Ajax Submit
            $.ajax({
                url: '/api/UploadApi/?name=' + name,
                type: 'GET',
                dataType: 'json',
                cache: false,
                success: function (data) {

                    console.log(data);

                    // set preview input value
                    if ($('input#' + target)[0]) {
                        $('input#' + target)[0].value = data;
                    }

                },
                error: function (xhr, status, errMsg) {
                    if (xhr && xhr.responseJSON) {
                        console.error("status: " + status, "message: " + errMsg, ", note: " + xhr.responseJSON);
                    }
                },
                complete: function (xhr, textStatus) {
                    console.log('done');
                }
            });

        }
    }


    function readURL(id) {
        var img = document.getElementById(GetReviewId(id));
        var input = document.getElementById(GetInputUploadId(id));
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                console.log("changed");
                img.src = e.target.result;
                //drawimg(e.target.result);

                dict['img-' + id] = e.target.result;

                // get mimeType
                var arr = convertDataURIToBinary(img.src).subarray(0, 4);
                var header = "";
                for (var i = 0; i < arr.length; i++) {
                    header += arr[i].toString(16);
                }

                var mimeType = getMimeType(header);

                dict['mimeType-' + id] = mimeType;
                dict['fileName-' + id] = input.files[0].name;
                console.log(mimeType);
            }
            reader.onloadend = function (e) {
                console.log("loaded");
                var width = img.naturalWidth;
                var height = img.naturalHeight;

                dict['width-' + id] = width;
                dict['height-' + id] = height;

                // preview scale [2]
                if (width > height) {
                    img.width = 400;
                    img.height = height / width * 400;
                } else {
                    img.height = 400;
                    img.width = width / height * 400;
                }
                    
                $('#' + GetReloaderImage(id)).hide();

                previewCrop(id, width, height);
            }
            reader.readAsDataURL(input.files[0]);
        }
    }

    
    function getModalId(key) {
        return "modal-" + CmsEncode(key);
    }

    function GetInputId(key) {
        return "input-" + CmsEncode(key);
    }

    function GetInputUploadId(key) {
        return "upload-" + CmsEncode(key);
    }

    function GetReviewId(key) {
        return "preview-" + CmsEncode(key);
    }

    function GetReloaderImage(key) {
        return "preloader-image-" + CmsEncode(key);
    }


    function CmsEncode(key) {

        if (key == null)
            return;

        return key.replace(/\.|\,/g, '-');
    }

    function CmsDecode(key) {

        if (key == null)
            return null;

        return key.replace(/-/g, '.');
    }






    // uploader =======================================

    var queryUpload = 0;
    var responseUpload = 0;

    function UploadImage(image, key, modal) {

        queryUpload++;


        // constraint 
        var element = $('input#' + GetInputUploadId(key))[0];
        var require = getRequire(element);
        var constraint = '&type=' + require.type + '&width=' + require.width + '&height=' + require.height;

        var progressBar = $(modal.getElementsByClassName('progress-bar'));

        var target = GetInputId(key);
        //var preview = $(element).attr("preview-for");

        var formData = new FormData();
        formData.append(image.name, image);
        

        // Ajax Submit
        $.ajax({
            url: '/api/UploadApi' + '/?subFolder=Contents' + constraint ,
            type: "POST",
            dataType: null,
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            forceSync: false,
            xhr: function () {
                var xhrobj = $.ajaxSettings.xhr();
                if (xhrobj.upload) {
                    xhrobj.upload.addEventListener('progress', function (event) {

                        var percent = 0;
                        var position = event.loaded || event.position;
                        var total = event.total || event.totalSize;
                        if (event.lengthComputable) {
                            percent = Math.ceil(position / total * 100);
                        }

                        //console.log(percent);

                        progressBar.width(percent + '%');

                    }, false);
                }

                return xhrobj;
            },
            success: function (data, message, xhr) {
            

                // not need preview
                //if ($('#' + preview)[0]) {
                //    var img = $('#' + preview)[0];
                //    img.src = URL.createObjectURL(element.files[0]); // set src to blob url
                //    img.onload = imageIsLoaded;
                //}

            
                // set input value
                if ($('input#' + target)[0]) {
                    $('input#' + target)[0].value = data.path;
                    SetContent(key, data.path, reloadCallback);
                }


                // callback
                function reloadCallback(id) {

                    if (++responseUpload < queryUpload)
                        return;

                    isUploadDone = true;

                    // reload
                    if(isQueryDone)
                        location.reload();
                }

            },
            error: function (xhr, status, errMsg) {
                if (xhr && xhr.responseJSON) {
                    console.error("status: " + status, "message: " + errMsg, ", note: " + xhr.responseJSON);
                }
            },
            complete: function (xhr, textStatus) {

            }
        });
    }


    function imageIsLoaded() {
        console.log(this.src);  // blob url
        // update width and height ...
    }

    function CheckFileImage(file) {
        var extension = file.substr((file.lastIndexOf('.') + 1))
        if (extension === 'jpg' || extension === 'jpeg' || extension === 'gif' || extension === 'png' || extension === 'bmp') {
            return true;
        } else {
            return false;
        }
    }



    function getInfo(element) {
        var info = {
            type: element.getAttribute('info-type'),
            width: element.getAttribute('info-width'),
            height: element.getAttribute('info-height'),

            mobileType: element.getAttribute('mobile-type'),
            mobileWidth: element.getAttribute('mobile-width'),
            mobileHeight: element.getAttribute('mobile-height')
        }

        // mobileType as option
        if (info.mobileType === null || info.mobileType === undefined) {
            info.mobileType = info.type;
        }

        return info;
    }


    
    function getRequire(element) {
        var info = {
            type: element.getAttribute('info-type'),
            width: element.getAttribute('info-width'),
            height: element.getAttribute('info-height')
        }

        return info;
    }




    function previewCrop(id, width, height) {

        // default
        var scale = 1;
        var ratio = 4/3;

        var element = $('input#' + GetInputUploadId(id))[0];
        var require = getRequire(element);

        if (require && require.width && require.height)
            ratio = require.width / require.height;


        var isMobile = id.endsWith(".Mobile.Image");


        // destroy old jcrop
        var oldJcrop = dict['jcrop-' + id];
        if (oldJcrop)
            oldJcrop.destroy();


        // when true mode not active crop
        if (require && require.width == width && require.height == height) {
            $('#' + GetReviewId(id)).after("<span id='manual-" + CmsEncode(id)  + "' class='true-mode'>Manual Mode</span>");
            return;
        } else {
            $('#manual-' + CmsEncode(id)).remove();
        }


        var target = GetReviewId(id);
        
        const [w,h] = Jcrop.Rect.getMax(width,height,ratio);
        var x = Math.floor((width - w) / 2);
        var y = Math.floor((height - h) / 2);
        const rect = Jcrop.Rect.create(x,y,w,h);

        var jcrop = Jcrop.attach(target);

        dict['jcrop-' + id] = jcrop;

        const options = {aspectRatio: rect.aspect};
        var widget = jcrop.newWidget(rect,options);


        // rescale for preview [3]
        if (width > height)
            scale = width / 400;
        else
            scale = height / 400;


        var reCalculate = function(scale, width, height) {
            var realPos = widget.pos.scale(scale).round();
        
            if (realPos.w > width)
                realPos.w = width;

            if (realPos.h > height)
                realPos.h = height;

            return realPos;
        }

        // init
        dict[id] = reCalculate(scale, width, height);

        jcrop.listen('crop.change',function(widget,e){
            var pos = reCalculate(scale, width, height);
            dict[id] = pos;
            console.log(pos.x,pos.y,pos.w,pos.h);
        });

    }

    // way 1
    return {
        buildModalAndOpen: BuildModalAndOpen
    }

})(jQuery);
