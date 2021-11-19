

$(function () {

    // Set size per request
    var size = 6;

    if (window.loadSize)
        size = window.loadSize;

    var loadCount = 1;

    var loading = $("#loading");

    $("#loadMore").on("click", function (e) {
 
        e.preventDefault();
 
        $(document).on({
 
            ajaxStart: function () {
                loading.show();
            },
            ajaxStop: function () {
                loading.hide();
            }
        });

        var count = loadCount * size

        console.log(count);

        var url = '/Controller/Product/LoadMore?size=' + size + '&count=' + count;
        $.ajax({
            url: url,
            cache: false,
            type: "GET",
            success: function (data) {
 
                if (data.length !== 0) {
                    //$('#NewestProductsContainer').append(data.ModelString).fadeIn(2000);
                    $(data.ModelString).hide().appendTo('#NewestProductsContainer').fadeIn(1500);
                    //$(data.ModelString).insertBefore("#loadMore").hide().fadeIn(2000);
                }
 
                var ajaxModelCount = data.ModelCount - (loadCount * size);
                if (ajaxModelCount <= 0) {
                    $("#loadMore").hide().fadeOut(2000);
                }
 
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert("message : \n" + 
                    "An error occurred, for more info check the js console" +
                    "\n status : \n" + status + " \n error : \n" + error);
            }
        });
 
        loadCount = loadCount + 1; 
    }); 
}); 