$("#type_quest").change(function (e) {
    var type = $("#type_quest :selected").val();
    var div = "<form method='post' id='form_add_quest' onsubmit='$('.else_variant').prop('disabled', false);' enctype='multipart/form-data' action='/Forms/AddQuestion'> <input type='hidden' name='type_q' id='type_q' value='" + type + "'/> <input type='hidden' name='form_id' id='form_id' value='" + $("#form_id").val() + "'/> <label for='text_quest'>Вопрос</label><div class='form-group'><input class='form-control' required id='text_quest' name='text_quest'></div> ";
    div += "<label for='number_ask'>Номер вопроса</label><div class='form-group'><input type='number' required class='form-control' id='number_ask' name='number_ask'></div> ";
    div += "<label for='file'>Картинка к вопросу</label><div class='form-group'><input class='form-control' type='file' id='file' name='file'><h6>Размер файла не должен превышать 3 мб</h6></div><div class='block_error'><p class='p_error'>Превышен размер файла</p></div> ";
    div += "<label for='video'>Ссылка на видео из Youtube</label><div class='form-group'><input class='form-control' id='video' name='video'></div>";
    div += "<label for='comment'>Комментарий</label><div class='form-group'><textarea class='form-control' id='comment' name='comment'></textarea></div> ";
    div += "<label for='is_required'>Обязательный вопрос?</label><div class='radio_required'><div class='form-group'><label><input type='radio' id='is_required' name='is_required' value='1'><span>Да</span></label></div><div class='form-group'><label><input type='radio' id='is_required' name='is_required' checked value='0'><span>Нет</span></label></div></div>";
    if (type == 2) {
        div += "<div class='radio'><div class='form-group'><input class='form-control' type='radio' disabled/><input class='form-control' name='variant' type='text' required placeholder='Введите название поля'/></div><p class='p_add'><a href='#' onclick='event.preventDefault(); AddVariant(" + type + ");' id='add_variant'>Добавить вариант</a> <span>или</span> <a href='#' class='else_variant' onclick='event.preventDefault(); AddElse(" + type +");'>Вариант \"ДРУГОЕ\"</a></p></div>";
    }
    else if (type == 3) {
        div += "<div class='checkbox'><div class='form-group'><input class='form-control' type='checkbox' disabled /><input class='form-control' name='variant' type='text' placeholder='Введите название поля'/></div><p class='p_add'><a href='#' onclick='event.preventDefault(); AddVariant(" + type + ");' id='add_variant'>Добавить вариант</a> <span>или</span> <a href='#' class='else_variant' onclick='event.preventDefault(); AddElse(" + type +");'>Вариант \"ДРУГОЕ\"</a></p></div>";
    }
    else if (type == 4) {
        div += "<div class='select'><div class='form-group'><input class='form-control' type='text' name='variant' placeholder='Введите название поля'/></div><p class='p_add'><a href='#' onclick='event.preventDefault(); AddVariant(" + type + ");' id='add_variant'>Добавить вариант</a> <span>или</span> <a href='#' class='else_variant' onclick='event.preventDefault(); AddElse(" + type +");'>Вариант \"ДРУГОЕ\"</a></p><div>";
    }
    else if (type == 5) {
        div += "<div class='tabl'> <div class='table_row col-md-6'><ul><label for='table_row'>Список строк:</label><li class='form-group'><input class='form-control' id='table_row' name='table_row' required/></li></ul><p class='p_add'><a href='#' onclick='event.preventDefault(); AddRow();'><img class='icon' title='Добавить строку' src='/Content/icons/plus-sign-in-a-black-circle.svg'></a></p></div> <div class='table_col col-md-6'><label>Список столбцов:</label><ul><li class='form-group'><input type='radio' disabled/><input class='form-control' name='table_col' required/></li></ul><p class='p_add'><a href='#' onclick='event.preventDefault(); AddCol(1);'><img class='icon' title='Добавить столбец' src='/Content/icons/plus-sign-in-a-black-circle.svg'></a></p></div> </div>";
    }
    else if (type == 6) {
        div += "<div class='table'> <div class='table_row col-md-6'><label for='table_row'>Список строк:</label><ul><li class='form-group'><input class='form-control' id='table_row' name='table_row' required/></li></ul><p class='p_add'><a href='#' onclick='event.preventDefault(); AddRow();'><img class='icon' title='Добавить строку' src='/Content/icons/plus-sign-in-a-black-circle.svg'></a></p></div> <div class='table_col col-md-6'><label>Список столбцов:</label><ul><li class='form-group'><input type='checkbox' disabled/><input class='form-control' name='table_col' required/></li></ul><p class='p_add'><a href='#' onclick='event.preventDefault(); AddCol(2);'><img class='icon' title='Добавить столбец' src='/Content/icons/plus-sign-in-a-black-circle.svg'></a></p></div> </div>";
    }
    else if (type == 7) {
        div += "<div class='for_file'> <div class='form-group'><label id='file_label' for='file'><span>Максимальный размер файла: </span><input class='form-control' value='3' type='number' min='1' max='20' id='file' name='file' required/> <span> мб</span><label></div></div>";
    }
    div += "<div class='form-group col-md-12'><input type='submit' class='btn btn-primary center-block' value='Сохранить вопрос'/></div></form>";
    $("#new_question").html(div);
});

function AddVariant(type) {
    if (type == 2) {
        $("#add_variant").parent().before("<div class='form-group'><input type='radio' disabled/><input type='text' class='form-control' name='variant' placeholder='Введите название поля'/><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode);' src='/Content/icons/minus.svg'></div>");
    }
    else if (type == 3) {
        $("#add_variant").parent().before("<div class='form-group'><input type='checkbox' disabled/><input type='text' name='variant' class='form-control' placeholder='Введите название поля'/><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode);' src='/Content/icons/minus.svg'></div>");
    }
    else if (type == 4) {
        $("#add_variant").parent().before("<div class='form-group'><input class='form-control' name='variant' type='text' placeholder='Введите название поля'/><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode);' src='/Content/icons/minus.svg'></div>");
    }
}

function AddRow() {
    $(".table_row>ul").append("<li class='form-group'><input class='form-control' name='table_row'/><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode);' src='/Content/icons/minus.svg'></li>");
}

function AddCol(t) {
    if (t == 1) {
        $(".table_col>ul").append("<li class='form-group'><input type='radio' disabled/><input class='form-control' name='table_col'/><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode);' src='/Content/icons/minus.svg'></li>");
    }
    else if (t == 2) {
        $(".table_col>ul").append("<li class='form-group'><input type='checkbox' disabled/><input class='form-control' name='table_col'/><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode);' src='/Content/icons/minus.svg'></li>");
    }
}

function AddElse(type) {
    $("#add_variant").parent().children("span").css('display', 'none');
    $("#add_variant").parent().children("a:last").css('display', 'none');
    if (type == 2) {
        $("#add_variant").parent().before("<div class='form-group'><input type='radio' disabled/><input type='text' value='Другое' class='form-control' name='else'/><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode); DelElse();' src='/Content/icons/minus.svg'></div>");
    }
    else if (type == 3) {
        $("#add_variant").parent().before("<div class='form-group'><input type='checkbox' disabled/><input value='Другое' type='text' name='else' class='form-control' /><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode); DelElse();' src='/Content/icons/minus.svg'></div>");
    }
    else if (type == 4) {
        $("#add_variant").parent().before("<div class='form-group'><input class='form-control' value='Другое' type='text' name='else'/><img class='icon del_variant' onclick='event.target.parentNode.parentNode.removeChild(event.target.parentNode); DelElse();' src='/Content/icons/minus.svg'></div>");
    }
}


function DelElse() {
    $("#add_variant").parent().children("span").css('display', 'block');
    $("#add_variant").parent().children("a:last").css('display', 'block');
}


$("#img_add_question").click(function () {
    if ($("#one_questions").css('display') == "none") {
        $("#one_questions").fadeIn();
        $("#img_add_question").attr("src", '/Content/icons/top.svg');
        $("#img_add_question").attr("title", 'Скрыть');
    }
    else {
        $("#one_questions").fadeOut();
        $("#img_add_question").attr("src", '/Content/icons/plus-sign-in-a-black-circle.svg');
        $("#img_add_question").attr("title", 'Добавить вопрос');
    }
});


//DATEPICKER
$(function () {
    $('#date_start').datepicker({
        dateFormat: 'dd.mm.yy'
    });
    $('#date_end').datepicker({
        dateFormat: 'dd.mm.yy'
    });
});
//DATEPICKER (END)


//REPLACE, IN ROWS AND COLS FOR TABLE_RADIO AND TABLE_CHECKBOX
$('#one_questions').on('submit', '#form_add_quest', function (e) {
        var type = $("#type_q").val();

    if (type == 5 || type == 6) {
        var table_row = $("input[name = table_row]");
        for (var i = 0; i < table_row.length; ++i){
            var txt = $(table_row[i]).val().replace(new RegExp(",", 'g'), "&#044");
            $(table_row[i]).val(txt);
        }
        var table_col = $("input[name = table_col]");
        for (var i = 0; i < table_col.length; ++i) {
            var txt = $(table_col[i]).val().replace(new RegExp(",", 'g'), "&#044");
            $(table_col[i]).val(txt);
        }
    }
    else if (type == 2 || type == 3) {
        var checkbox = $("input[name ='variant']");
        for (var i = 0; i < checkbox.length; ++i) {
            var txt = $(checkbox[i]).val().replace(new RegExp(",", 'g'), "&#044");
            $(checkbox[i]).val(txt);
        }
    }

});
$('.form_details').on('submit', '#edit_question', function (e) {
    var type = $("#type_q").val();
    if (type == 5 || type == 6) {
        var table_row = $("input[name = table_row]");
        for (var i = 0; i < table_row.length; ++i) {
            var txt = $(table_row[i]).val().replace(new RegExp(",", 'g'), "&#044");
            $(table_row[i]).val(txt);
        }
        var table_col = $("input[name = table_col]");
        for (var i = 0; i < table_col.length; ++i) {
            var txt = $(table_col[i]).val().replace(new RegExp(",", 'g'), "&#044");
            $(table_col[i]).val(txt);
        }
    }
    else if (type == 2 || type == 3) {
        var checkbox = $("input[name ='variant']");
        for (var i = 0; i < checkbox.length; ++i) {
            var txt = $(checkbox[i]).val().replace(new RegExp(",", 'g'), "&#044");
            $(checkbox[i]).val(txt);
        }
    }  
});
//REPLACE, IN ROWS AND COLS FOR TABLE_RADIO AND TABLE_CHECKBOX (END)


//SHOW VS HIDE RESET FILE BUTTON
$('.list_question').on('change', '.add_file', function (e) {
    var size = $(e.target).parent().find(".size_file").first().val();
    if (!validateSize(this, size)) {
        $(e.target).val("");
        $(e.target).parents('.block_question').first().children(".block_error").first().children(".p_error").text("Превышен размер файла");
        $(e.target).parents('.block_question').first().children(".block_error").first().fadeIn();
        $(e.target).siblings(".del_file").fadeOut();
    }
    else if ($(e.target).val()) {
        $(e.target).siblings(".del_file").fadeIn();
        $(e.target).parents('.block_question').first().children(".block_error").first().fadeOut();
    }
});
//SHOW VS HIDE RESET FILE BUTTON (END)


//LIMIT FILE SIZE
$('#one_questions').on('change', '#file', function (e) {
    if (!validateSize(this, 3)) {
        $(e.target).val("");
        $(e.target).parents('#new_question').find(".block_error").first().fadeIn();
    }
    else{
        $(e.target).parents('#new_question').find(".block_error").first().fadeOut();
    }
});
$('#edit_question').on('change', '#file', function (e) {
    if (!validateSize(this, 3)) {
        $(e.target).val("");
        $(e.target).parents('#edit_question').find(".block_error").first().fadeIn();
    }
    else {
        $(e.target).parents('#edit_question').find(".block_error").first().fadeOut();
    }
});
//LIMIT FILE SIZE (END)


//RESET FILE BUTTON
$('.list_question').on('click', '.del_file', function (e) { 
    e.preventDefault();
    $(e.target).siblings("input.add_file").first().val("");
    $(e.target).fadeOut();
});
//RESET FILE BUTTON (END)


//VALIDATE FILE SIZE
function validateSize(fileInput, size) {
    var fileObj, oSize;
    if (typeof ActiveXObject == "function") { // IE
        fileObj = (new ActiveXObject("Scripting.FileSystemObject")).getFile(fileInput.value);
    } else {
        fileObj = fileInput.files[0];
    }
    oSize = fileObj.size; // Size returned in bytes.
    if (oSize > size * 1024 * 1024) {
        return false
    }
    return true;
}
//VALIDATE FILE SIZE (END)


$("document").ready(function () {
    var el = $("#edit_question input[name='else']");
    if (el.length > 0){
        $("#edit_question #add_variant").parent().children("span").css('display', 'none');
        $("#edit_question #add_variant").parent().children("a:last").css('display', 'none');
    }
});

$("#del_form_button").click(function () {
    document.getElementById("del_form").showModal();
});

$("#del_form #close_form").click(function (e) {
    document.getElementById("del_form").close();
});