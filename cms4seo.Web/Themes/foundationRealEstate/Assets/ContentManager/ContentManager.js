/*-----------------------------------------------------------------------------------
/* CMS Switcher
-----------------------------------------------------------------------------------*/


jQuery(document).ready(function($) {
	
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

    $('#cms-switcher h2 a').click(function(e){
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
    })



    $("#theme-switcher").on('change', function() {
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

        if('.se-pre-con') {
            $('.se-pre-con').addClass('active');
        }
        

    });;




    
    
    if ($('#bootswatch-switcher').length > 0) {
        $("#bootswatch-switcher").on('change', function() {
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

            if('.se-pre-con') {
                $('.se-pre-con').addClass('active');
            }
        

        });;
    }



    SetStatusToggleSwitch();
    

    $("#switchEditContents").click( function(){

        if ($(this).is(':checked')) {
            setCookie("ActiveEditContent", "1", 1)
        } else {
            setCookie("ActiveEditContent", "0" , 1)
        }

        // reload page
        location.reload();


    });


});




/*-----------------------------------------------------------------------------------
/* CMS Switcher
-----------------------------------------------------------------------------------*/



function setCookie(name,value,days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days*24*60*60*1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "")  + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for(var i=0;i < ca.length;i++) {
        var c = ca[i];
        while (c.charAt(0)==' ') c = c.substring(1,c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
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

        document.querySelectorAll('[data-cms]').forEach(function(item) {

            var type = "text";

            if ($(item).closest('img').length) {
                type = "image";
            }

            var button =
                '<span class="editContentIcon editContentButton" onclick="BuildModalAndOpen(\'' +
                    $(item).attr('data-cms') +
                    '\',\'' + type +
                    '\', event' +
                    ')" target="_top">' +
                '</span>';

            $(item).addClass("activeEditContents");

            var attribute = 
                'BuildModalAndOpen(\'' +
                    $(item).attr('data-cms') +
                    '\',\'' + type + '\', event' + ')';
            $(item).attr("onclick", attribute);

            //$(item).after(button);

            //console.log($(item).attr('data-cms') + ":" + $(item).width() + "vs" + $(item).parent().width());

            //$(item).next().addClass("editContentButton");

            //if ($(item).closest('img').length) {
            //    $(item).next().addClass("editContentButton");
            //} else if ($(item).closest('span').length) {
            //    $(item).next().addClass("editContentButton");
            //} else if ($(item).width() + 35 >= $(item).parent().width()) {
            //    $(item).next().addClass("editContentButton with-parent-width");
            //} else {
            //    $(item).next().addClass("editContentButton");
            //}

  
            // prevent event fire
            //if ($(item).parents("a").length) {
            //    var link = $(item).parents("a");
            //    $(link).click(function(e){
            //        //e.stopPropagation();
            //        e.preventDefault();
            //        console.log("stop link: " + $(link).value);
            //    });
            //}


        });

    }
}


function BuildModalAndOpen(id, type, event) {

    // prevent any unwant fire
    event.preventDefault();
    event.stopPropagation();


    if (document.getElementById(id) !== null) {
        var modal = document.getElementById(id);
        modal.style.display = "block";
        return;
    }

    // get value & process is async
    GetContent(id)

    //console.log(id);
    var modalBuilder =
        '<div id="' + id + '" class="modal-cms">' +
            '<!-- Modal content -->' +
            '<div class="modal-content-cms">' +
                '<span class="close">&times;</span>' +
                '<h4>Edit</h4>' +
                '<div class="modal-content-box" data-type=' + type + '>' +

                    //'<label>' + 
                    //    CmsDecode(id) +
                    //    '<input name="' + id + '" type="text" value="loading...">' +
                    //'</label>' +

                '</div>' +
                '<button type="submit" class="button submitButton">Save</button>' +
                '<button class="button closeButton">Close</button>' +
            '</div>' +
        '</div>';

    $("body").append(modalBuilder);

    // Get the modal
    var modal = document.getElementById(id);


    // Get the <span> element that closes the modal
    var spanCloseIcon = modal.getElementsByClassName("close")[0];

    var spanCloseButton = modal.getElementsByClassName("closeButton")[0];

    var submitButton = modal.getElementsByClassName("submitButton")[0];


    // When the user clicks on close Button the modal
    spanCloseButton.onclick = function() {
        modal.style.display = "none";
    }

    spanCloseIcon.onclick = function() {
        modal.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function(event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    }

    submitButton.onclick = function() {

        var modalInputBox = modal.getElementsByClassName("modal-content-box")[0];

        var inputs = $(modalInputBox).children();

        $.each(inputs, function(i, input) {

            //console.log(i);

            var key = $(input).children().attr('name');
            var value = $(input).children().val();

            SetContent(key, value);

        });

    }


    modal.style.display = "block";
}


function SubmitOnEnter(id) {

    // Get the modal
    var modal = document.getElementById(id);

    var modalInputBox = modal.getElementsByClassName("modal-content-box")[0];

    var inputs = $(modalInputBox).children();

    $.each(inputs, function(i, input) {

        //console.log(i);

        var key = $(input).children().attr('name');
        var value = $(input).children().val();

        SetContent(key, value);

    });

}


function GetContent(id) {

    var result;

    var xhttpBehaviour = new XMLHttpRequest();
    xhttpBehaviour.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {

            console.log(this.responseText);

            // find target to fill
            if (document.getElementById(id) !== null) {

                var modal = document.getElementById(id);

                var input = $('#' + id + ' input');

                var jsonResults = JSON.parse(this.responseText);

                if (jsonResults.length == 0)
                    return;

                // build more input object
                var modalContentBox = modal.getElementsByClassName("modal-content-box")[0];


                // clear childs
                modalContentBox.innerHTML = '';


                // build uploadBox
                var type = $(modalContentBox).attr("data-type");

                if (jsonResults.length == 1) {

                    BuildUploadBox(modalContentBox, type, jsonResults[0].Key)
                }
                else {

                    // Rebuild
                    $.each(jsonResults, function(i, item) {

                        if (item.Key.endsWith(".Image")) {
                            BuildUploadBox(modalContentBox, type, item.Key)
                        }

                    });

                }


                if (jsonResults.length == 1) {
                    //input.val(jsonResults[0].Value);
                    BuildInputContent(modalContentBox, jsonResults[0].Key, jsonResults[0].Value);
                }
                else {

                    // Rebuild
                    $.each(jsonResults, function(i, item) {

                        BuildInputContent(modalContentBox, item.Key, item.Value);


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


function SetContent(key, value) {

    var xhttpBehaviour = new XMLHttpRequest();

    xhttpBehaviour.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            console.log(this.responseText);
            // reload page
            location.reload();
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


function BuildUploadBox(boxContent, type, id) {

    if (type != "image")
        return "";

    // safe code for first time reset with id contain dot
    var img;
    var alt = "";
    if (/[.]/.test(id)) {
        img = $(document).find("img[data-cms=" + CmsEncode(id) + "]")[0];
        console.warn(id);
    }
    else {
        img = $(document).find("img[data-cms=" + id + "]")[0];
        if(img) alt = img.alt
    }
 
    var uploadBox =
        '<div class="image-required">For best display: ' + alt + '</div>' + 
        '<input type="file" onchange="UploadImage(this)" data-for='+ GetInputId(id) + ' />' +
        '<img id="previewImage" src="#" alt="your image" width=400 class="margin-bottom-2">';


    $(boxContent).append(uploadBox);
}


function GetInputId(key) {
    return "input-" + CmsEncode(key);
}

function CmsEncode(key) {

    if (key == null)
        return;

    return key.replace(/\./g, '-');
}

function CmsDecode(key) {

    if (key == null)
        return;

    return key.replace(/-/g, '.');
}






// uploader =======================================


function UploadImage(element) {
    console.log(element.value);

    if (!element.files || !element.files[0])
        return;

    var formData = new FormData();
    var file = element.files[0];
    formData.append('file',file);

    var target = $(element).attr("data-for");

    // Ajax Submit
    $.ajax({
        url: '/api/UploadApi' + '/?subFolder=Contents',
        type: "POST",
        dataType: null,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        forceSync: false,
        xhr: function(){
            var xhrobj = $.ajaxSettings.xhr();
            if(xhrobj.upload){
                xhrobj.upload.addEventListener('progress', function(event) {

                    var percent = 0;
                    var position = event.loaded || event.position;
                    var total = event.total || event.totalSize;
                    if(event.lengthComputable){
                        percent = Math.ceil(position / total * 100);
                    }

                    console.log(percent);

                }, false);
            }

            return xhrobj;
        },
        success: function (data, message, xhr){
            // display image
            var img = $("#previewImage")[0];
            img.src = URL.createObjectURL(element.files[0]); // set src to blob url
            img.onload = imageIsLoaded;

            if ($('input#' + target)[0]) {
                $('input#' + target)[0].value = data.path;
            }

        },
        error: function (xhr, status, errMsg){
            if (xhr && xhr.responseJSON) {
                console.error("status: " + status, "message: " +  errMsg, ", note: " + xhr.responseJSON);
            }
        },
        complete: function(xhr, textStatus){
            
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