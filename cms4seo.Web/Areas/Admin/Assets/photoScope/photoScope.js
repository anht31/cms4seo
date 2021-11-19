/************************************************************
 * Template: photoScope
 * Type: angular
 * Version: 1.0.1
 * Modify: 
 * PowerBy: anht31@gmail.com
 * support max-width, max-height for insertEditor
 ************************************************************/


//var shouldTriggerOnBeforeUnload = true;


var photoScopeApp = angular.module('photoScope', []);

if (typeof window.entity === "undefined") {
    window.entity = "unknown";
}


photoScopeApp.controller('scopeController', function ($scope, $http) {


    // modal upload => upload file =================================================
    var modelId;

    if (typeof Id != "undefined") {
        modelId = document.getElementById('Id').value;
    } else {
        LobiboxMessage('warning', 'You must be create Id hidden field in order upload Image!!!');
        return;
    }


    // GET: api/PhotosApi
    // set avatar with Sort == 0
    $scope.selectAvatar = { Sort: 0 };
    //modelId = document.getElementById('Id').value;
    $scope.items = null;
    $scope.reloadData = function () {
        $http.get('/api/PhotosApi?entity=' + window.entity + '&modelId=' + modelId)
        .then(function (d) {

            $scope.items = d.data; // Success
        }, function () {
            LobiboxMessage('error', 'Error Occured !!!');
        });
    };
    $scope.reloadData();



    $scope.Delete = function (item) {


        $http.delete('/api/PhotosApi/' + item.Id)
        .then(function (d) {


            if (d.data.success === false) {
                LobiboxMessage('error', d.data.message);
            } else {
                LobiboxMessage('success', 'Delete success !!!');
            }

            $scope.reloadData();
        }, function () {
            LobiboxMessage('error', 'Error Occured !!!');
        });
    };



    $scope.ActiveAvatar = function (item) {
        if ($("input#Avatar").val() === item.SmPath)
            return "active";
        return "";
    };


    $scope.SetActive = function (item) {


        $("input#Avatar").val(item.SmPath);

        LobiboxMessage('success', 'Set Avatar Success!');

    };

    $scope.InsertEditor = function (item) {

        // for set focus 1
        var element;
        var editor;
        var range;

        function getDimension(url, callback) {
            var img = new Image();

            img.onload = function () {
                var width = this.width,
                    height = this.height;
                console.log(width, height);
                callback(width, height);
            }

            img.src = url;
        }

        getDimension(item.LgPath, insertCallback);

        function insertCallback(width, height) {

            // innerHtml ========================================================
            for (var instanceName in CKEDITOR.instances) {
                if (CKEDITOR.instances.hasOwnProperty(instanceName)) {
                    //var funcNum = window.CKEDITOR.instances[instanceName]._.filebrowserFn;
                    var oEditor = CKEDITOR.instances[instanceName];
                    var html = "<img alt=\"" + item.AltAttribute + "\" src=\"" + item.LgPath +
                        "\" style='width:100%; height:100%; max-width:" + width + "px; max-height:" + height + "px' />";
                    var newElement = CKEDITOR.dom.element.createFromHtml(html, oEditor.document);
                    oEditor.insertElement(newElement);
                    LobiboxMessage('success', 'Send Image to cursor success!');

                    // for set focus 2
                    element = newElement;
                    editor = oEditor;

                    break;
                }
            }

            // for set focus 3
            window.setTimeout(function () {

                // do whatever you want to do     
                if (editor != undefined && element) {

                    element.scrollIntoView();

                    // Thank you S/O
                    // http://stackoverflow.com/questions/16835365/set-cursor-to-specific-position-in-ckeditor
                    range = editor.createRange();
                    range.moveToPosition(element, CKEDITOR.POSITION_AFTER_END);
                    editor.getSelection().selectRanges([range]);
                }

            }, 100);
        }



    };



    $scope.InsertVideo = function (item) {

        // set-focus 1
        var element;
        var editor;
        var range;

        // innerHtml ========================================================
        for (var instanceName in CKEDITOR.instances) {
            if (CKEDITOR.instances.hasOwnProperty(instanceName)) {
                var oEditor = CKEDITOR.instances[instanceName];

                var videoType = item.MimeType.replace(".", "");

                var html =
                    //"<img alt=\"" + item.AltAttribute + "\" src=\"" + item.LgPath + "\" width='100%' height='100%'/>";
                    '<video width="100%" controls>' +
                        '<source src="' + item.LgPath + '" type="video/' + videoType + '">' +
                        'Your browser does not support the video tag.' +
                    '</video>';
                var newElement = CKEDITOR.dom.element.createFromHtml(html, oEditor.document);
                oEditor.insertElement(newElement);
                LobiboxMessage('success', 'Send Video to cursor success!');

                // set-focus 2
                element = newElement;
                editor = oEditor;

                break;
            }
        }

        // set-focus 3
        window.setTimeout(function () {
            if (editor != undefined && element) {
                element.scrollIntoView();
                range = editor.createRange();
                range.moveToPosition(element, CKEDITOR.POSITION_AFTER_END);
                editor.getSelection().selectRanges([range]);
            }

        }, 100);

    };


    //$scope.HideForDocument = function(item) {
    //    if (!item.MimeType.match(/.(jpg|jpeg|png|gif)$/i)) {
    //        return true;
    //    };

    //    return false;
    //};

    $scope.IsImage = function (item) {

        if (item.MimeType.match(/.(jpg|jpe|jpeg|bmp|png|gif|webp)$/i)) {
            return true;
        };

        return false;
    };

    $scope.IsDocument = function (item) {

        if (item.MimeType.match(/.(doc|docx|pdf|xls|xlsx|ppt|pptx|rar|zip)$/i)) {
            return true;
        };

        return false;
    };

    $scope.IsVideo = function (item) {

        if (item.MimeType.match(/.(mp4|webm)$/i)) {
            return true;
        };

        return false;
    };

    var readySort = true;

    $scope.Up = function (item) {

        if (readySort) {

            readySort = false;
            
            var url = '/api/PhotosApi/' + item.Id;

            // 0 is first
            item.Sort -= 15;

            $http.put(url, item).then(function (response) {

                // This function handles success
                $scope.reloadData();
            
                readySort = true;

            }, function (response) {
                
                readySort = true;
                
                // this function handles error
                LobiboxMessage('error', 'Error Occured !!!');
            });
        }
       

    };


    $scope.Down = function (item) {

        if (readySort) {

            readySort = false;
            
            var url = '/api/PhotosApi/' + item.Id;

            // 0 is first
            item.Sort += 15;

            $http.put(url, item).then(function (response) {

                // This function handles success
                $scope.reloadData();
            
                readySort = true;

            }, function (response) {
                
                readySort = true;
                
                // this function handles error
                LobiboxMessage('error', 'Error Occured !!!');
            });
        }
       

    };


    $('#drag-and-drop-zone').dmUploader({
        url: '/api/upload/?entity=' + window.entity + '&modelId=' + modelId,
        maxFileSize: 31457280, //20971520 in Byte - approximation 20MB
        maxFiles: 30, // maxium 1 file
        allowedTypes: '*',
        /*extFilter: 'jpg;png;gif',*/
        onInit: function () {

            $("#asideBoxStatus").hide();

            $.danidemo.addLog('#demo-debug', 'default', 'Plugin initialized correctly');
        },
        onBeforeUpload: function (id) {

            $("#asideBoxStatus").show();

            $.danidemo.addLog('#demo-debug', 'default', 'Starting the upload of #' + id);

            $.danidemo.updateFileStatus(id, 'default', 'Uploading...');
        },
        onNewFile: function (id, file) {
            $.danidemo.addFile('#demo-files', id, file);
        },
        onComplete: function () {
            $.danidemo.addLog('#demo-debug', 'default', 'All pending tranfers completed');

            LobiboxMessage('success', "Upload done");
        },
        onUploadProgress: function (id, percent) {
            var percentStr = percent + '%';

            $.danidemo.updateFileProgress(id, percentStr);
        },
        onUploadSuccess: function (id, data) {

            $("#asideBoxStatus").hide();

            $.danidemo.addLog('#demo-debug', 'success', 'Upload of file #' + id + ' completed');

            //$.danidemo.addLog('#demo-debug', 'info', 'Server Response for file #' + id + ': ' + JSON.stringify(data));

            $.danidemo.addLog('#demo-debug', 'info', 'Server Response for file #' + id + ': ' + data.Name);

            $.danidemo.updateFileStatus(id, 'success', 'Upload Complete');

            $.danidemo.updateFileProgress(id, '100%');

            $scope.reloadData();


            // Message from server
            if (data.success === false) {
                LobiboxMessage('error', data.message);
            }


            // some stuff, ex: display file list
            //angular.element(document.getElementById('panelScope')).scope().reloadData(); //with id is appController

        },
        onUploadError: function (id, message) {
            $.danidemo.updateFileStatus(id, 'error', message);

            $.danidemo.addLog('#demo-debug', 'error', 'Failed to Upload file #' + id + ': ' + message);

            LobiboxMessage('error', message);
        },
        onFileTypeError: function (file) {
            $.danidemo.addLog('#demo-debug', 'error', 'File \'' + file.name + '\' cannot be added: must be an image');
        },
        onFileSizeError: function (file) {
            $.danidemo.addLog('#demo-debug', 'error', 'File \'' + file.name + '\' cannot be added: size excess limit');
        },
        onFileExtError: function (file) {
            $.danidemo.addLog('#demo-debug', 'error', 'File \'' + file.name + '\' has a Not Allowed Extension');
        },
        onFallbackMode: function (message) {
            $.danidemo.addLog('#demo-debug', 'info', 'Browser not supported(do something else here!): ' + message);
        }
    });



});




// Function Add Photo to Entity Model - not test
//var AddItemToScope = function (id) {

//    var modelId = document.getElementById('Id').value;

//    var entity = window.entity;

//    $.ajax({
//        type: 'PUT',
//        async: false,
//        url: '/api/admin/PhotosApi/' + id + "/" + entity + "/" + modelId + "/AddScope",
//        success: function (data) {

//            LobiboxMessage('success', 'put scope success !!!');
//            angular.element(document.getElementById('panelScope')).scope().reloadData(); //with id is appController
//        },
//        error: function () {
//            LobiboxMessage('error', 'put scope fail !!!');
//        }
//    });

//}