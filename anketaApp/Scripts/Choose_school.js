$(".school_questions #area").change(function () {
    if ($(".school_questions #area").val()) {
        $(".school_questions #district").attr("disabled", false);
        $.ajax({
            url: '/forms/AllDistrict',
            type: 'POST',
            data: JSON.stringify({ 'param': $("#area").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                data = data.replace(/\]/g, "");
                data = data.replace(/\[/g, "");
                var arr = data.toString().split('\",\"');
                arr[0] = arr[0].replace(/\"/, "");
                arr[arr.length - 1] = arr[arr.length - 1].replace(/\"/, "");
                document.getElementById('district').innerHTML = '';
                $('#district').append(new Option(arr[i + 1]));
                for (var i = 0; i < arr.length; ++i) {
                    $('#district').append(new Option(arr[i + 1], arr[i]));
                    ++i;
                }
            }
        });
    }
    else {
        document.getElementById('district').innerHTML = '';
        $(".school_questions #district").attr("disabled", true);
    }
    document.getElementById('school').innerHTML = '';
    $(".school_questions #school").attr("disabled", true);
});

$(".school_questions #district").change(function () {
    if ($(".school_questions #district").val()) {
        $(".school_questions #school").attr("disabled", false);
        $.ajax({
            url: '/forms/AllSchool',
            type: 'POST',
            data: JSON.stringify({ 'param': $("#district").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                data = data.replace(/\]/g, "");
                data = data.replace(/\[/g, "");
                data = data.replace(/\"/g, "");
                data = data.replace(/\\/g, "\"");
                var arr = data.toString().split(',');
                $('#school').attr("disabled", false);
                document.getElementById('school').innerHTML = '';
                $('#school').append(new Option(arr[i + 1]));
                for (var i = 0; i < arr.length && arr.length>1; ++i) {
                    $('#school').append(new Option(arr[i + 1], arr[i]));
                    ++i;
                }
            }
        });
    }
    else {
        document.getElementById('school').innerHTML = '';
        $(".school_questions #school").attr("disabled", true);
    }
});