$(".loop").click(function (e) {
    var img = $(e.target).siblings("img").first();
    $("#l_image:first").attr("src", img.attr("src"));
    $(".bg_l_image").fadeIn();
    $(".large_image_div").css("display","flex");
}); 

$(".large_image_div").click(function (e) {
    if (e.target.id != "l_image") {
        $("#l_image:first").attr("src", "");
        $(".bg_l_image").fadeOut();
        $(".large_image_div").fadeOut();
    }
});