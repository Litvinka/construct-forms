$("#anketa_form").submit(function (e) {
    var error = 0;
    $(".block_error").fadeOut()

    var text = $(".required_text");
    for (var i = 0; i < text.length; ++i) {
        var val = $(text[i]).children("textarea").first();
        if ($(val).val()=="") {
            error++;
            $(text[i]).parent().children(".block_error").first().fadeIn();
        }
    }

    var radio = $(".required_radio");
    for (var j = 0; j < radio.length; ++j) {
        var val = $(radio[j]).find("input[type='radio']:checked").first();
        if ($(val).length == 0) {
            error++;
            $(radio[j]).parent().children(".block_error").first().fadeIn();
        }
    }

    var checkbox = $(".required_checkbox");
    for (var j = 0; j < checkbox.length; ++j) {
        var val = $(checkbox[j]).find("input[type='checkbox']:checked").first();
        if ($(val).length == 0) {
            error++;
            $(checkbox[j]).parent().children(".block_error").first().fadeIn();
        }
    }

    var select = $(".required_select");
    for (var j = 0; j < select.length; ++j) {
        var val = $(select[j]).find("select").first();
        if ($(val).val() == "") {
            error++;
            $(select[j]).parent().children(".block_error").first().fadeIn();
        }
    }

    var table_radio = $(".required_table_radio");
    for (var j = 0; j < table_radio.length; ++j) {
        var tr = $(table_radio[j]).find("tr");
        for (var i = 1; i < tr.length; ++i) {
            var val = $(tr[i]).find("input[type='radio']:checked").first();
            if ($(val).length==0) {
                error++;
                $(table_radio[j]).parent().children(".block_error").first().fadeIn();
                break;
            }
        }
    }

    var table_checkbox = $(".required_table_checkbox");
    for (var j = 0; j < table_checkbox.length; ++j) {
        var tr = $(table_checkbox[j]).find("tr");
        for (var i = 1; i < tr.length; ++i) {
            var val = $(tr[i]).find("input[type='checkbox']:checked").first();
            if ($(val).length == 0) {
                error++;
                $(table_checkbox[j]).parent().children(".block_error").first().fadeIn();
                break;
            }
        }
    }

    var file = $(".required_file");
    for (var i = 0; i < file.length; ++i) {
        var val = $(file[i]).find("input[type=file]").first();
        if ($(val).val() == "") {
            error++;
            $(file[i]).parent().children(".block_error p").first().text("Загрузите файл");
            $(file[i]).parent().children(".block_error").first().fadeIn();
        }
    }
        
    if (error != 0) {
        e.preventDefault();
    }
});